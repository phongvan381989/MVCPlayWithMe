using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.CassoInvoiceAPI
{
    public class CommonCassoInvoiceAPI
    {
        private const string CASSO_API_BASE_URL = "https://openapi.casso.vn";

        /// <summary>
        /// Lấy access_token từ Casso API và lưu vào DB
        /// Có check exp
        /// </summary>
        /// 
        public static async Task<CassoAuthen> GetAndSaveAccessTokenAsync(Boolean mustRefresh)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                CassoAuthen authen = await CassoAuthenMySql.GetAsync();

                if (authen == null)
                {
                    return null;
                }

                // Kiểm tra token đã hết hạn chưa (trừ 5 phút để đảm bảo)
                long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long expiryWithBuffer = authen.exp - 300; // Trừ 5 phút

                if (currentTimestamp >= expiryWithBuffer || mustRefresh)
                {
                    // Gọi API Casso để lấy access_token
                    var client = new RestClient(CASSO_API_BASE_URL);
                    var request = new RestRequest("/oauth/token", Method.POST);

                    // Set header
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                    // Add parameters
                    request.AddParameter("app_id", authen.app_id);
                    request.AddParameter("app_secret", authen.app_secret);

                    // Execute request
                    IRestResponse response = await client.ExecuteAsync(request);

                    if (!response.IsSuccessful)
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = $"Lỗi gọi API Casso: {response.StatusCode} - {response.Content}";
                        MyLogger.GetInstance().Warn($"Casso API error: {response.ErrorMessage}");
                        return null;
                    }

                    // Parse response trực tiếp vào CassoAuthen
                    var tokenResponse = JsonConvert.DeserializeObject<CassoAuthen>(response.Content);

                    if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Response không chứa access_token";
                        return null;
                    }

                    // Lưu vào DB
                    result = await CassoAuthenMySql.SaveOrUpdateAsync(tokenResponse);

                    authen.access_token = tokenResponse.access_token;
                    authen.exp = tokenResponse.exp;
                    authen.expires_in = tokenResponse.expires_in;
                    authen.issued_at = tokenResponse.issued_at;
                }

                return authen;
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                MyLogger.GetInstance().Warn($"GetAndSaveAccessTokenAsync error: {ex.ToString()}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra response có lỗi "token expired" không
        /// </summary>
        private static bool IsTokenExpiredError(string responseContent, int statusCode)
        {
            if (string.IsNullOrEmpty(responseContent))
            {
                return false;
            }

            try
            {
                // Case 1: Lỗi 401/403 với format {"exp": "token expired"}
                if (statusCode == 401 || statusCode == 403)
                {
                    var errorResponse = JsonConvert.DeserializeObject<TokenExpiredResponse>(responseContent);
                    if (errorResponse != null && errorResponse.exp == "token expired")
                    {
                        return true;
                    }
                }

                // Case 2: Response chuẩn với code error
                var cassoResponse = JsonConvert.DeserializeObject<CassoResponse<object>>(responseContent);
                if (cassoResponse != null &&
                    (cassoResponse.code == "UNAUTHORIZED" || cassoResponse.code == "TOKEN_EXPIRED"))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Execute Casso API request với auto token refresh
        /// </summary>
        public static async Task<IRestResponse> ExecuteCassoApiAsync(string endpoint, Method method = Method.GET, object body = null, int retryCount = 0)
        {
            // Lấy access token
            CassoAuthen authen = await GetAndSaveAccessTokenAsync(false);

            if (authen == null)
            {
                throw new Exception("Cannot get valid access_token from Casso");
            }

            // Log request
            string requestBodyJson = body != null ? JsonConvert.SerializeObject(body, Common.jsonSerializersettings) : "null";
            MyLogger.GetInstance().Info($"Casso API Request: {method} {endpoint}\nBody: {requestBodyJson}");

            // Tạo request
            var client = new RestClient(CASSO_API_BASE_URL);
            var request = new RestRequest(endpoint, method);

            // Add Authorization header
            request.AddHeader("Authorization", $"Bearer {authen.access_token}");
            request.AddHeader("Content-Type", "application/json");

            // Add body nếu có
            if (body != null)
            {
                request.AddJsonBody(body);
            }

            // Execute request
            IRestResponse response = await client.ExecuteAsync(request);

            // Log response
            MyLogger.GetInstance().Info($"Casso API Response: {(int)response.StatusCode} {response.StatusCode}\nContent: {response.Content}");

            // Kiểm tra "token expired" error
            if (IsTokenExpiredError(response.Content, (int)response.StatusCode))
            {
                if (retryCount < 1)
                {
                    // Refresh token và retry 1 lần
                    MyLogger.GetInstance().Info("Casso token expired, refreshing...");
                    authen = await GetAndSaveAccessTokenAsync(true);

                    if (authen != null)
                    {
                        // Retry với token mới
                        return await ExecuteCassoApiAsync(endpoint, method, body, retryCount + 1);
                    }
                }

                MyLogger.GetInstance().Warn("Casso token expired and refresh failed");
            }

            return response;
        }

        /// <summary>
        /// Execute Casso API và parse response thành CassoResponse<T>
        /// </summary>
        public static async Task<CassoResponse<T>> ExecuteCassoApiAsync<T>(string endpoint, Method method = Method.GET, object body = null)
        {
            try
            {
                IRestResponse response = await ExecuteCassoApiAsync(endpoint, method, body, 0);

                if (!response.IsSuccessful)
                {
                    MyLogger.GetInstance().Warn($"Casso API error: {response.StatusCode} - {response.Content}");
                    return new CassoResponse<T>
                    {
                        code = "ERROR",
                        message = $"HTTP {response.StatusCode}: {response.ErrorMessage}",
                        requestId = null,
                        data = default(T)
                    };
                }

                // Parse response
                var cassoResponse = JsonConvert.DeserializeObject<CassoResponse<T>>(response.Content);
                return cassoResponse;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ExecuteCassoApiAsync error: {ex.ToString()}");
                return new CassoResponse<T>
                {
                    code = "EXCEPTION",
                    message = ex.Message,
                    requestId = null,
                    data = default(T)
                };
            }
        }

        /// <summary>
        /// Tạo hóa đơn nháp
        /// </summary>
        public static async Task<CassoResponse<OpenApiInvoiceResource>> CreateDraftInvoiceAsync(OpenApiCreateInvoiceBody request)
        {
            CassoResponse<OpenApiInvoiceResource> cassoResponse = await ExecuteCassoApiAsync<OpenApiInvoiceResource>("/xinvoice/v1/invoices/draft", Method.POST, request);
            return cassoResponse;
        }

        /// <summary>
        /// Lấy chi tiết hóa đơn theo ID
        /// </summary>
        public static async Task<CassoResponse<OpenApiInvoiceResource>> GetInvoiceByIdAsync(string invoiceId)
        {
            return await ExecuteCassoApiAsync<OpenApiInvoiceResource>($"/xinvoice/v1/invoices/{invoiceId}", Method.GET);
        }

        /// <summary>
        /// Tạo và phát hành hóa đơn (gửi lên cơ quan thuế)
        /// </summary>
        public static async Task<CassoResponse<OpenApiInvoiceResource>> IssueInvoiceAsync(OpenApiCreateInvoiceBody request)
        {
            return await ExecuteCassoApiAsync<OpenApiInvoiceResource>("/xinvoice/v1/invoices/issue", Method.POST, request);
        }
    }

    /// <summary>
    /// Response lỗi token expired từ Casso (401/403)
    /// </summary>
    public class TokenExpiredResponse
    {
        public string exp { get; set; }
    }

    /// <summary>
    /// Response chuẩn từ Casso API
    /// </summary>
    public class CassoResponse<T>
    {
        /// <summary>
        /// Mã code cố định (SUCCEED, UNAUTHORIZED, TOKEN_EXPIRED, ...)
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Message mô tả (tiếng Việt, có thể thay đổi)
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Request ID để trace (null nếu lỗi 401/403)
        /// </summary>
        public string requestId { get; set; }

        /// <summary>
        /// Data trả về (chỉ có khi code == "SUCCEED")
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// Kiểm tra response có thành công không
        /// </summary>
        public bool IsSucceed => code == "SUCCEED";
    }
}
