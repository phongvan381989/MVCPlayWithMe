using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikTokApp;
using MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokAuth;
using MySqlConnector;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.TikTokShopAPI
{
    /// <summary>
    /// TikTok Shop OAuth2 Authentication API
    ///
    /// IMPORTANT NOTES:
    /// 1. grant_type MUST be "authorized_code" (NOT standard OAuth2 "authorization_code")
    /// 2. Using "authorization_code" will cause the request to FAIL
    /// 3. Access token default validity: 7 days
    /// 4. Refresh token before access_token_expire_in to avoid re-authorization
    /// 5. App Secret must be kept server-side only, never expose to client
    /// </summary>
    public class TikTokAuthAPI
    {
        // Grant type for TikTok Shop token exchange
        // WARNING: Must use "authorized_code", NOT "authorization_code"
        private const string GRANT_TYPE_AUTH_CODE = "authorized_code";
        private const string GRANT_TYPE_REFRESH_TOKEN = "refresh_token";

        public static TikTokAuth tikTokAuth;


        /// <summary>
        /// Load authentication config at application startup
        /// </summary>
        public static async Task InitializeAsync()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    tikTokAuth = await TikTokAuthMySql.GetAuthFromDBAsync(conn);

                    if (tikTokAuth != null && tikTokAuth.IsValid())
                    {
                        MyLogger.GetInstance().Info($"TikTok Auth initialized. AppKey: {tikTokAuth.AppKey}");
                    }
                    else
                    {
                        MyLogger.GetInstance().Warn("TikTok Auth not configured or invalid");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok Auth initialization error: {ex}");
            }
        }

        /// <summary>
        /// Check if access token is expiring soon and auto-refresh if needed
        /// Uses global tikTokAuth variable for performance (no DB query unless null)
        /// Should be called before making any API request
        /// </summary>
        /// <param name="refreshThresholdMinutes">Refresh if token expires within this many minutes (default: 5)</param>
        /// <returns>True if token is valid and not expired, False otherwise</returns>
        public static async Task<Boolean> CheckAndRefreshTokenIfNeededAsync(int refreshThresholdMinutes = 5)
        {
            try
            {
                // Load from DB only if null or invalid
                if (tikTokAuth == null || !tikTokAuth.IsValid())
                {
                    using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                    {
                        await conn.OpenAsync();
                        tikTokAuth = await TikTokAuthMySql.GetAuthFromDBAsync(conn);
                    }

                    if (tikTokAuth == null || !tikTokAuth.IsValid())
                    {
                        MyLogger.GetInstance().Error("TikTok auth not found or invalid");
                        return false;
                    }
                }

                // Check if access token needs refresh
                // AccessTokenExpireIn is Unix timestamp in seconds
                long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long timeUntilExpiry = tikTokAuth.AccessTokenExpireIn - currentTimestamp;

                // Refresh if token expires within threshold
                long thresholdSeconds = refreshThresholdMinutes * 60;
                if (timeUntilExpiry <= thresholdSeconds)
                {
                    MyLogger.GetInstance().Info($"Access token expiring in {timeUntilExpiry} seconds. Refreshing...");

                    bool refreshSuccess = await TikTokAuthTokenRefreshAsync();
                    if (!refreshSuccess)
                    {
                        MyLogger.GetInstance().Error("Failed to refresh access token");
                        return false;
                    }

                    MyLogger.GetInstance().Info("Access token refreshed successfully");
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"CheckAndRefreshTokenIfNeeded error: {ex}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Exchange authorization code for access token and refresh token
        /// Called after seller authorizes the app
        /// </summary>
        /// <param name="code">Authorization code from callback URL</param>
        /// <returns>True if successful</returns>
        public static async Task<bool> TikTokAuthTokenCreateAndSaveDBAsync(string code)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Get app credentials from DB
                    var auth = await TikTokAuthMySql.GetAuthFromDBAsync(conn);
                    if (auth == null || string.IsNullOrEmpty(auth.AppKey) || string.IsNullOrEmpty(auth.AppSecret))
                    {
                        MyLogger.GetInstance().Error("TikTok app credentials not found in database");
                        return false;
                    }

                    // TODO: Verify correct endpoint URL for your region
                    // US: https://auth.tiktok-shops.com/api/v2/token/get
                    // SEA: https://auth.tiktokglobalshop.com/api/v2/token/get
                    // Check TikTok Partner Center docs for your region
                    string tokenEndpoint = "https://auth.tiktok-shops.com/api/v2/token/get";

                    // Prepare GET request with query parameters
                    // Format: /api/v2/token/get?app_key=xxx&app_secret=xxx&auth_code=xxx&grant_type=authorized_code
                    RestRequest request = new RestRequest(tokenEndpoint, Method.GET);
                    request.AddQueryParameter("app_key", auth.AppKey);
                    request.AddQueryParameter("app_secret", auth.AppSecret);
                    request.AddQueryParameter("auth_code", code);
                    request.AddQueryParameter("grant_type", GRANT_TYPE_AUTH_CODE); // "authorized_code"

                    // Execute request
                    IRestResponse response = Common.client.Execute(request);
                    MyLogger.InfoRestLog(Common.client, request, response);

                    if (!response.IsSuccessful)
                    {
                        MyLogger.GetInstance().Error($"TikTok token exchange failed: {response.StatusCode} - {response.Content}");
                        return false;
                    }

                    // Parse response
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    TikTokTokenResponse tokenResponse = JsonConvert.DeserializeObject<TikTokTokenResponse>(response.Content, settings);

                    if (!tokenResponse.IsSuccess())
                    {
                        MyLogger.GetInstance().Error($"TikTok token exchange error: {tokenResponse.message}");
                        return false;
                    }

                    // Save tokens to DB
                    await TikTokAuthMySql.SaveTokenAsync(
                        tokenResponse.data.access_token,
                        tokenResponse.data.access_token_expire_in,
                        tokenResponse.data.refresh_token,
                        tokenResponse.data.refresh_token_expire_in,
                        conn);

                    // Reload global auth config
                    tikTokAuth = await TikTokAuthMySql.GetAuthFromDBAsync(conn);

                    MyLogger.GetInstance().Info("TikTok tokens successfully exchanged and saved");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok token exchange exception: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// Should be called before access token expires
        /// </summary>
        /// <returns>True if successful</returns>
        public static async Task<bool> TikTokAuthTokenRefreshAsync()
        {
            try
            {
                // TODO: Verify correct endpoint URL for your region
                // US: https://auth.tiktok-shops.com/api/v2/token/refresh
                // SEA: https://auth.tiktokglobalshop.com/api/v2/token/refresh
                string refreshEndpoint = "https://auth.tiktok-shops.com/api/v2/token/refresh";

                // Prepare GET request with query parameters
                // Format: /api/v2/token/refresh?app_key=xxx&app_secret=xxx&refresh_token=xxx&grant_type=refresh_token
                RestRequest request = new RestRequest(refreshEndpoint, Method.GET);
                request.AddQueryParameter("app_key", tikTokAuth.AppKey);
                request.AddQueryParameter("app_secret", tikTokAuth.AppSecret);
                request.AddQueryParameter("refresh_token", tikTokAuth.RefreshToken);
                request.AddQueryParameter("grant_type", GRANT_TYPE_REFRESH_TOKEN); // "refresh_token"

                // Execute request
                IRestResponse response = Common.client.Execute(request);
                MyLogger.InfoRestLog(Common.client, request, response);

                if (!response.IsSuccessful)
                {
                    MyLogger.GetInstance().Error($"TikTok token refresh failed: {response.StatusCode} - {response.Content}");
                    return false;
                }

                // Parse response
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                TikTokTokenResponse tokenResponse = JsonConvert.DeserializeObject<TikTokTokenResponse>(response.Content, settings);

                if (!tokenResponse.IsSuccess())
                {
                    MyLogger.GetInstance().Error($"TikTok token refresh error: {tokenResponse.message}");
                    return false;
                }

                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    // Save new tokens to DB
                    bool saveResult = await TikTokAuthMySql.SaveTokenAsync(
                        tokenResponse.data.access_token,
                        tokenResponse.data.access_token_expire_in,
                        tokenResponse.data.refresh_token,
                        tokenResponse.data.refresh_token_expire_in,
                        conn);

                    if(!saveResult)
                    {
                        MyLogger.GetInstance().Error("Failed to save refreshed tokens to database");
                        return false;
                    }

                    // Update auth config
                    TikTokAuthAPI.tikTokAuth.AccessToken = tokenResponse.data.access_token;
                    TikTokAuthAPI.tikTokAuth.AccessTokenExpireIn = tokenResponse.data.access_token_expire_in;
                    TikTokAuthAPI.tikTokAuth.RefreshToken = tokenResponse.data.refresh_token;
                    TikTokAuthAPI.tikTokAuth.RefreshTokenExpireIn = tokenResponse.data.refresh_token_expire_in;

                    MyLogger.GetInstance().Info("TikTok tokens successfully refreshed");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok token refresh exception: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Get list of shops that seller has authorized for the app
        /// API: GET /authorization/202309/shops
        /// </summary>
        /// <returns>TikTokAuthorizedShopsResponse with list of shops</returns>
        public static async Task<TikTokAuthorizedShopsResponse> GetAuthorizedShopsAsync()
        {
            try
            {
                // Check and refresh token if needed
                bool isTokenValid = await CheckAndRefreshTokenIfNeededAsync();
                if (!isTokenValid)
                {
                    return null;
                }

                // Build API request
                string path = "/authorization/202309/shops";
                var queryParams = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "app_key", tikTokAuth.AppKey },
                    { "timestamp", TikTokSigningHelper.GetTimestamp().ToString() }
                };

                // Generate signature
                string signature = TikTokSigningHelper.GenerateSignature(
                    tikTokAuth.AppSecret,
                    path,
                    queryParams,
                    ""  // Empty body for GET request
                );

                // Build URL
                var urlParams = new System.Collections.Generic.List<string>();
                foreach (var param in queryParams)
                {
                    urlParams.Add($"{param.Key}={Uri.EscapeDataString(param.Value)}");
                }
                urlParams.Add($"sign={signature}");

                string url = $"{TikTokSigningHelper.BASE_URL}{path}?{string.Join("&", urlParams)}";

                // Execute GET request
                RestRequest request = new RestRequest(url, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("x-tts-access-token", tikTokAuth.AccessToken);

                IRestResponse response = Common.client.Execute(request);
                MyLogger.InfoRestLog(Common.client, request, response);

                if (!response.IsSuccessful)
                {
                    MyLogger.GetInstance().Error($"TikTok GetAuthorizedShops failed: {response.StatusCode} - {response.Content}");
                    return null;
                }

                // Parse response
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                TikTokAuthorizedShopsResponse shopsResponse =
                    JsonConvert.DeserializeObject<TikTokAuthorizedShopsResponse>(response.Content, settings);

                return shopsResponse;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok GetAuthorizedShops exception: {ex}");
                return null;
            }
        }
    }
}
