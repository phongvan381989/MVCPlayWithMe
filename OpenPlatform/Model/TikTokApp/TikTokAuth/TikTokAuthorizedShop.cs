namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp
{
    /// <summary>
    /// TikTok Shop Get Authorized Shops API Response
    /// API: GET /authorization/202309/shops
    /// Field names match exactly with TikTok Shop JSON response
    /// </summary>
    public class TikTokAuthorizedShopsResponse : TikTokCommonResponse
    {
        public TikTokAuthorizedShopsData data { get; set; }

        public bool IsSuccess()
        {
            return code == 0 && data != null && data.shops != null;
        }
    }

    public class TikTokAuthorizedShopsData
    {
        /// <summary>
        /// List of shops that seller has authorized for the app
        /// </summary>
        public TikTokShopInfo[] shops { get; set; }
    }

    /// <summary>
    /// TikTok Shop Information
    /// Field names match exactly with TikTok Shop JSON response
    /// </summary>
    public class TikTokShopInfo
    {
        /// <summary>
        /// Internal shop identifier
        /// Example: "7000714532876273420"
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Encrypted shop token for API requests
        /// Use this as shop_cipher parameter in shop-specific APIs
        /// Example: "GCP_XF90igAAAABh00qsWgtvOiGFNqyubMt3"
        /// </summary>
        public string cipher { get; set; }

        /// <summary>
        /// Shop code displayed on Seller Center
        /// Example: "CNGBCBA4LLU8"
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Shop name
        /// Example: "Maomao beauty shop"
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Shop region code
        /// Example: "GB", "US", "TH", "ID"
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// Seller type
        /// Possible values:
        /// - "CROSS_BORDER": Cross border sellers with multiple shops in different countries
        /// - "LOCAL": Local sellers with only 1 shop
        /// </summary>
        public string seller_type { get; set; }
    }
}
