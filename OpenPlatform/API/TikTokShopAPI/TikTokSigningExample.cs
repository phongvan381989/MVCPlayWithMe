using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikTokApp;
using MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokAuth;
using MySqlConnector;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.TikTokShopAPI
{
    /// <summary>
    /// Example: How to use TikTokSigningHelper to call TikTok Shop APIs
    /// </summary>
    public class TikTokSigningExample
    {
        /// <summary>
        /// Example 1: Call GET API with signature
        /// API: Get Product List
        /// </summary>
        public static async Task<IRestResponse> ExampleGetProductList(string shopCipher)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Get auth credentials from DB
                    var auth = await TikTokAuthMySql.GetAuthFromDBAsync(conn);
                    if (auth == null || !auth.IsValid())
                    {
                        MyLogger.GetInstance().Error("TikTok auth not found or invalid");
                        return null;
                    }

                    // API endpoint
                    string baseUrl = "https://open-api.tiktokglobalshop.com";
                    string path = "/product/202309/products";

                    // Additional query parameters
                    var additionalParams = new Dictionary<string, string>
                    {
                        { "shop_cipher", shopCipher },
                        { "page_size", "20" }
                    };

                    // Build signed URL
                    string signedUrl = TikTokSigningHelper.BuildSignedUrl(
                        baseUrl,
                        path,
                        auth.AppKey,
                        auth.AppSecret,
                        auth.AccessToken,
                        additionalParams
                    );

                    // Execute request
                    RestRequest request = new RestRequest(signedUrl, Method.GET);
                    request.AddHeader("Content-Type", "application/json");

                    IRestResponse response = Common.client.Execute(request);
                    MyLogger.InfoRestLog(Common.client, request, response);

                    return response;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok GetProductList error: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Example 2: Call POST API with signature and body
        /// API: Update Product Stock
        /// </summary>
        public static async Task<IRestResponse> ExampleUpdateProductStock(
            string shopCipher,
            string productId,
            int stock)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Get auth credentials
                    var auth = await TikTokAuthMySql.GetAuthFromDBAsync(conn);
                    if (auth == null || !auth.IsValid())
                    {
                        MyLogger.GetInstance().Error("TikTok auth not found or invalid");
                        return null;
                    }

                    // API endpoint
                    string baseUrl = "https://open-api.tiktokglobalshop.com";
                    string path = "/product/202309/stocks/update";

                    // Request body
                    string requestBody = $@"{{
                        ""product_id"": ""{productId}"",
                        ""skus"": [
                            {{
                                ""id"": ""sku_id_1"",
                                ""stock_infos"": [
                                    {{
                                        ""available_stock"": {stock}
                                    }}
                                ]
                            }}
                        ]
                    }}";

                    // Query parameters (for POST, usually just shop_cipher)
                    var queryParams = new Dictionary<string, string>
                    {
                        { "app_key", auth.AppKey },
                        { "timestamp", TikTokSigningHelper.GetTimestamp().ToString() },
                        { "shop_cipher", shopCipher }
                    };

                    // Generate signature with body
                    string signature = TikTokSigningHelper.GenerateSignature(
                        auth.AppSecret,
                        path,
                        queryParams,
                        requestBody
                    );

                    // Build URL with signature
                    string url = $"{baseUrl}{path}?" +
                        $"app_key={Uri.EscapeDataString(auth.AppKey)}&" +
                        $"timestamp={queryParams["timestamp"]}&" +
                        $"shop_cipher={Uri.EscapeDataString(shopCipher)}&" +
                        $"sign={signature}&" +
                        $"access_token={Uri.EscapeDataString(auth.AccessToken)}";

                    // Execute request
                    RestRequest request = new RestRequest(url, Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                    IRestResponse response = Common.client.Execute(request);
                    MyLogger.InfoRestLog(Common.client, request, response);

                    return response;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok UpdateProductStock error: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Example 3: Manual signature verification
        /// Use this to debug signing issues
        /// </summary>
        public static void VerifySignature()
        {
            // Test data
            string appSecret = "test_secret";
            string path = "/product/202309/products";
            var queryParams = new Dictionary<string, string>
            {
                { "app_key", "test_key" },
                { "timestamp", "1234567890" },
                { "shop_cipher", "GCP_test" }
            };
            string requestBody = "";

            // Generate signature
            string signature = TikTokSigningHelper.GenerateSignature(
                appSecret,
                path,
                queryParams,
                requestBody
            );

            // Log for verification
            MyLogger.GetInstance().Info($"Test Signature: {signature}");

            // Expected sign string construction:
            // test_secret + /product/202309/products + app_keytest_keyshop_cipherGCP_testtimestamp1234567890 + test_secret
            // (parameters sorted alphabetically)
        }
    }
}
