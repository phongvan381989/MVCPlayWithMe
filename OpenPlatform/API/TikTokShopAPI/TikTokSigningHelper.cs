using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MVCPlayWithMe.OpenPlatform.API.TikTokShopAPI
{
    /// <summary>
    /// TikTok Shop API Request Signing Helper
    /// Based on: https://partner.tiktokshop.com/docv2/page/sign-your-api-request
    ///
    /// Signing Algorithm: HMAC-SHA256
    /// Sign String Format: app_secret + path + sorted_query_params + request_body + app_secret
    /// </summary>
    public class TikTokSigningHelper
    {
        /// <summary>
        /// TikTok Shop OpenAPI Base URL
        /// </summary>
        public const string BASE_URL = "https://open-api.tiktokglobalshop.com";

        /// <summary>
        /// Generate HMAC-SHA256 signature for TikTok Shop API request
        /// </summary>
        /// <param name="appSecret">App Secret (used as HMAC key)</param>
        /// <param name="path">API path (e.g., "/product/202309/products")</param>
        /// <param name="queryParams">Query parameters (excluding "sign" and "access_token")</param>
        /// <param name="requestBody">Request body as JSON string (empty string for GET requests)</param>
        /// <returns>Hex string signature</returns>
        public static string GenerateSignature(
            string appSecret,
            string path,
            Dictionary<string, string> queryParams,
            string requestBody = "")
        {
            // Step 1: Build sign string
            // Format: app_secret + path + sorted_params + body + app_secret
            StringBuilder signString = new StringBuilder();

            // 1.1: Start with app_secret
            signString.Append(appSecret);

            // 1.2: Append path
            signString.Append(path);

            // 1.3: Append sorted query parameters (exclude "sign" and "access_token")
            if (queryParams != null && queryParams.Count > 0)
            {
                var sortedParams = queryParams
                    .Where(kv => kv.Key != "sign" && kv.Key != "access_token")
                    .OrderBy(kv => kv.Key)
                    .ToList();

                foreach (var param in sortedParams)
                {
                    signString.Append(param.Key);
                    signString.Append(param.Value);
                }
            }

            // 1.4: Append request body (if any)
            if (!string.IsNullOrEmpty(requestBody))
            {
                signString.Append(requestBody);
            }

            // 1.5: End with app_secret
            signString.Append(appSecret);

            // Step 2: Compute HMAC-SHA256
            byte[] keyBytes = Encoding.UTF8.GetBytes(appSecret);
            byte[] signBytes = Encoding.UTF8.GetBytes(signString.ToString());

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(signBytes);

                // Step 3: Convert to hex string (lowercase)
                string signature = BitConverter.ToString(hashBytes)
                    .Replace("-", "")
                    .ToLower();

                return signature;
            }
        }

        /// <summary>
        /// Generate current Unix timestamp in seconds (GMT/UTC+00:00)
        /// </summary>
        public static long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Build complete API URL with signature
        /// </summary>
        /// <param name="baseUrl">Base URL (e.g., "https://open-api.tiktokglobalshop.com")</param>
        /// <param name="path">API path</param>
        /// <param name="appKey">App Key</param>
        /// <param name="appSecret">App Secret</param>
        /// <param name="accessToken">Access Token (optional, null for public APIs)</param>
        /// <param name="additionalParams">Additional query parameters</param>
        /// <returns>Complete URL with timestamp and signature</returns>
        public static string BuildSignedUrl(
            string baseUrl,
            string path,
            string appKey,
            string appSecret,
            string accessToken = null,
            Dictionary<string, string> additionalParams = null)
        {
            // Prepare query parameters
            var queryParams = new Dictionary<string, string>();
            queryParams["app_key"] = appKey;
            queryParams["timestamp"] = GetTimestamp().ToString();

            // Add additional parameters
            if (additionalParams != null)
            {
                foreach (var param in additionalParams)
                {
                    queryParams[param.Key] = param.Value;
                }
            }

            // Generate signature
            string signature = GenerateSignature(appSecret, path, queryParams, "");

            // Build URL
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append(baseUrl);
            urlBuilder.Append(path);
            urlBuilder.Append("?");

            // Add all query parameters
            var allParams = new List<string>();
            foreach (var param in queryParams.OrderBy(kv => kv.Key))
            {
                allParams.Add($"{param.Key}={Uri.EscapeDataString(param.Value)}");
            }

            // Add signature
            allParams.Add($"sign={signature}");

            // Add access token if provided (not included in signing)
            if (!string.IsNullOrEmpty(accessToken))
            {
                allParams.Add($"access_token={Uri.EscapeDataString(accessToken)}");
            }

            urlBuilder.Append(string.Join("&", allParams));

            return urlBuilder.ToString();
        }
    }
}
