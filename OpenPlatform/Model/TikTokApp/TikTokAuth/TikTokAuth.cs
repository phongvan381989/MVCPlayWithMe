using System;

namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokAuth
{
    /// <summary>
    /// TikTok Shop authentication credentials
    /// Maps to table: tb_tiktok_authen
    /// </summary>
    public class TikTokAuth
    {
        public int Id { get; set; }

        /// <summary>
        /// Service ID from TikTok Shop (returned in authorization response)
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// App Key (app_key)
        /// Your TikTok Shop App application key that was generated when you created your TikTok Shop App.
        /// Used in authorization URL and API signing.
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// App Secret (app_secret)
        /// Your TikTok Shop App secret that was generated when you created your TikTok Shop App.
        /// IMPORTANT: Keep this value server-side only. Never expose to client/frontend.
        /// Used for API request signing and token exchange.
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// Access Token (access_token)
        /// The access token necessary to make API calls to TikTok Shop OpenAPI.
        /// Default validity period: 7 days.
        /// Must be refreshed before expiration using refresh_token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Access Token Expiration (access_token_expire_in)
        /// The expiration date and time for the access_token in Unix epoch time (seconds).
        /// Default: 7 days from token generation.
        /// When current time > this value, must refresh the access token.
        /// </summary>
        public long AccessTokenExpireIn { get; set; }

        /// <summary>
        /// Refresh Token (refresh_token)
        /// Used to generate a new access_token once the current date is greater than access_token_expire_in.
        /// Exchange this token for a new access_token + refresh_token pair before it expires.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Refresh Token Expiration (refresh_token_expire_in)
        /// The expiration date and time of the refresh token in Unix epoch format (seconds).
        /// Once this date and time has been reached, you must re-authorize your TikTok Shop App
        /// by having the seller go through the authorization flow again.
        /// </summary>
        public long RefreshTokenExpireIn { get; set; }

        /// <summary>
        /// Shop ID (from Get Authorized Shops API)
        /// Internal identifier for the TikTok Shop
        /// </summary>
        public string ShopId { get; set; }

        /// <summary>
        /// Shop Cipher (from Get Authorized Shops API)
        /// Encrypted token used to securely identify a shop in API requests
        /// Required for cross-border shops and shop-specific API calls
        /// </summary>
        public string ShopCipher { get; set; }

        /// <summary>
        /// Last refresh timestamp (calculated field, not in DB)
        /// </summary>
        public DateTime? LastRefreshTime { get; set; }

        public TikTokAuth()
        {
            ServiceId = string.Empty;
            AppKey = string.Empty;
            AppSecret = string.Empty;
            AccessToken = string.Empty;
            AccessTokenExpireIn = 0;
            RefreshToken = string.Empty;
            RefreshTokenExpireIn = 0;
            ShopId = string.Empty;
            ShopCipher = string.Empty;
        }

        /// <summary>
        /// Check if tokens are valid
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AppKey)
                && !string.IsNullOrEmpty(AppSecret)
                && !string.IsNullOrEmpty(AccessToken)
                && !string.IsNullOrEmpty(RefreshToken);
        }

        /// <summary>
        /// Check if access token needs refresh (within 5 minutes of expiration)
        /// </summary>
        public bool NeedsRefresh()
        {
            if (LastRefreshTime == null || AccessTokenExpireIn <= 0)
                return true;

            DateTime expiryTime = LastRefreshTime.Value.AddSeconds(AccessTokenExpireIn);
            return DateTime.Now >= expiryTime.AddMinutes(-5);
        }
    }

    /// <summary>
    /// TikTok Shop token API response
    /// Returned from /api/v2/token/get and /api/v2/token/refresh
    /// </summary>
    public class TikTokTokenResponse : TikTokCommonResponse
    {
        public TikTokTokenData data { get; set; }

        public bool IsSuccess()
        {
            return code == 0 && data != null && !string.IsNullOrEmpty(data.access_token);
        }
    }

    public class TikTokTokenData
    {
        /// <summary>
        /// Access token for API calls
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Unix timestamp when access token expires
        /// </summary>
        public long access_token_expire_in { get; set; }

        /// <summary>
        /// Refresh token to get new access token
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// Unix timestamp when refresh token expires
        /// </summary>
        public long refresh_token_expire_in { get; set; }

        /// <summary>
        /// Open ID (optional, returned in some regions)
        /// </summary>
        public string open_id { get; set; }

        /// <summary>
        /// Seller name (optional)
        /// </summary>
        public string seller_name { get; set; }

        /// <summary>
        /// Seller base region (optional)
        /// </summary>
        public string seller_base_region { get; set; }
    }
}
