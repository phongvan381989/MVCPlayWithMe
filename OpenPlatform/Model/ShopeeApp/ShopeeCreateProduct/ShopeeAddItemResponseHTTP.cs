using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct
{
    public class ShopeePreOrder
    {
        // The guaranteed days to ship orders.
        public int days_to_ship { get; set; }

        // Whether this item is pre order
        public Boolean is_pre_order { get; set; }
    }

    public class ShopeeImages
    {
        // ID of image
        public List<string> image_id_list { get; set; }

        // Display URL of image
        public List<string> image_url_list { get; set; }
    }

    public class ShopeePriceInfo
    {
        // Current price of item
        public float current_price { get; set; }

        // Original price of item
        public float original_price { get; set; }
    }

    public class ShopeeVideoInfo
    {
        // Video playback url
        public string video_url { get; set; }

        // Video preview image url
        public string thumbnail_url { get; set; }

        // Video duration
        public int duration { get; set; }
    }

    public class ShopeeAddItemResponse
    {
        // Description of item
        public string description { get; set; }

        // The weight of this item, the unit is KG.
        public float weight { get; set; }

        // Pre order setting
        ShopeePreOrder pre_order { get; set; }

        public string item_name { get; set; }

        // Item images
        public ShopeeImages images { get; set; }

        // Item status
        public string item_status { get; set; }

        // Item price info
        public ShopeePriceInfo price_info { get; set; }

        // Logistic setting
        public List<ShopeeLogisticInfo> logistic_info { get; set; }

        // Item ID
        public long item_id { get; set; }

        // Item attributes
        public List<ShopeeAttribute> attribute { get; set; }

        // Category ID
        public int category_id { get; set; }

        // The dimension of this item.
        public ShopeeDimension dimension { get; set; }

        // Item condition, could be NEW or USED
        public string condition { get; set; }

        // Item video
        public List<ShopeeVideoInfo> video_info { get; set; }

        // Wholesale setting
        public List<ShopeeWholesale> wholesale { get; set; }

        public ShopeeBrandRequestParameter brand { get; set; }

        // This field is only applicable for local sellers in Indonesia and Malaysia.
        // Use this field to identify whether a product is a dangerous product.
        // 0 for non-dangerous product and 1 for dangerous product. For more information,
        // please visit the market's respective Seller Education Hub.
        public int item_dangerous { get; set; }

        // New description field. Only whitelist sellers can use it. 
        // If item with extended_description this field will return, otherwise do not return.
        public ShopeeDescriptionInfo description_info { get; set; }

        // Values: See Data Definition- description_type (normal , extended).
        public string description_type { get; set; }

        // Complaint Policy for item. Only returned for local PL sellers.
        public ShopeeComplaintPolicy complaint_policy { get; set; }

        public List<ShopeeSellerStock> seller_stock { get; set; }
    }
    public class ShopeeAddItemResponseHTTP : CommonResponseHTTP
    {
        public ShopeeAddItemResponse response {get;set;}
    }
}