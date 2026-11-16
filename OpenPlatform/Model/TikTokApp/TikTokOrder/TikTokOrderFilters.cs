namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder
{
    /// <summary>
    /// Time range for filtering orders
    /// </summary>
    public enum TikTokOrderTimeRange
    {
        /// <summary>
        /// Orders from last 1 day
        /// </summary>
        OneDay = 1,

        /// <summary>
        /// Orders from last 7 days
        /// </summary>
        SevenDays = 7,

        /// <summary>
        /// Orders from last 30 days
        /// </summary>
        ThirtyDays = 30
    }

    /// <summary>
    /// Sort field for Get Order List API
    /// </summary>
    public static class TikTokOrderSortField
    {
        /// <summary>
        /// Sort by order creation time (default)
        /// </summary>
        public const string CREATE_TIME = "create_time";

        /// <summary>
        /// Sort by order update time
        /// </summary>
        public const string UPDATE_TIME = "update_time";
    }

    /// <summary>
    /// Sort order for Get Order List API
    /// </summary>
    public static class TikTokOrderSortOrder
    {
        /// <summary>
        /// Ascending order
        /// </summary>
        public const string ASC = "ASC";

        /// <summary>
        /// Descending order (default)
        /// </summary>
        public const string DESC = "DESC";
    }

    /// <summary>
    /// Shipping type filter for Get Order List API
    /// </summary>
    public static class TikTokShippingType
    {
        /// <summary>
        /// Shipping service provided by TikTok. The seller should obtain shipping label from TikTok.
        /// </summary>
        public const string TIKTOK = "TIKTOK";

        /// <summary>
        /// Seller provides shipping, including through 3rd party fulfillment providers on behalf of the seller.
        /// </summary>
        public const string SELLER = "SELLER";
    }

    /// <summary>
    /// Order type constants
    /// </summary>
    public static class TikTokOrderType
    {
        /// <summary>
        /// An item that is in stock and available for immediate purchase and fulfillment.
        /// </summary>
        public const string NORMAL = "NORMAL";

        /// <summary>
        /// An order placed during a lottery event in TikTok LIVE.
        /// </summary>
        public const string ZERO_LOTTERY = "ZERO_LOTTERY";

        /// <summary>
        /// An advance order for items that are not yet available or released.
        /// Fulfillment starts on a specific date in the future.
        /// US market only.
        /// </summary>
        public const string PRE_ORDER = "PRE_ORDER";

        /// <summary>
        /// An order for items that are produced only after the order is received.
        /// Fulfillment starts after the product is produced.
        /// US market only.
        /// </summary>
        public const string MADE_TO_ORDER = "MADE_TO_ORDER";

        /// <summary>
        /// An order for items that are out of stock but expected to be restocked.
        /// Fulfillment starts after the product is restocked.
        /// US market only.
        /// </summary>
        public const string BACK_ORDER = "BACK_ORDER";
    }

    /// <summary>
    /// Fulfillment type constants
    /// </summary>
    public static class TikTokFulfillmentType
    {
        /// <summary>
        /// A method where sellers fulfill orders directly from their own inventory,
        /// without using TikTok's fulfillment centers.
        /// </summary>
        public const string FULFILLMENT_BY_SELLER = "FULFILLMENT_BY_SELLER";

        /// <summary>
        /// A service offered by TikTok where sellers can send their products to TikTok's fulfillment centers.
        /// TikTok then takes care of storing, picking, packing, and shipping the products to customers.
        /// </summary>
        public const string FULFILLMENT_BY_TIKTOK = "FULFILLMENT_BY_TIKTOK";

        /// <summary>
        /// A method where Tokopedia GoTo Logistics provides warehousing and logistics services to sellers.
        /// Indonesia market only.
        /// </summary>
        public const string FULFILLMENT_BY_DILAYANI_TOKOPEDIA = "FULFILLMENT_BY_DILAYANI_TOKOPEDIA";
    }
}
