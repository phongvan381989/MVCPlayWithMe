using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder
{
    /// <summary>
    /// TikTok Shop Order Line Item (SKU)
    /// Field names match exactly with TikTok Shop JSON response
    /// </summary>
    public class TikTokOrderLineItem
    {
        // Line Item Basic Info
        public string id { get; set; }
        public string display_status { get; set; }

        // Product Info
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string sku_id { get; set; }
        public string sku_name { get; set; }
        public string sku_image { get; set; }
        public string seller_sku { get; set; }
        public string sku_type { get; set; }  // Deprecated, use order_type

        // Pricing
        public string currency { get; set; }
        public string original_price { get; set; }
        public string sale_price { get; set; }
        public string seller_discount { get; set; }
        public string platform_discount { get; set; }

        // Fees
        public string buyer_service_fee { get; set; }  // Indonesia market
        public string small_order_fee { get; set; }    // Thailand market
        public string retail_delivery_fee { get; set; } // US market
        public string pfand_fee { get; set; }          // Germany market - deposit fee
        public string distance_fee { get; set; }       // Indonesia Horizon+
        public string distance_shipping_fee { get; set; } // Indonesia Horizon+
        public string gift_retail_price { get; set; }

        // Tax
        public List<TikTokItemTax> item_tax { get; set; }

        // Package Info
        public string package_id { get; set; }
        public string package_status { get; set; }
        public long rts_time { get; set; }
        public string tracking_number { get; set; }

        // Shipping Provider
        public string shipping_provider_id { get; set; }
        public string shipping_provider_name { get; set; }

        // Cancellation
        public string cancel_reason { get; set; }
        public string cancel_user { get; set; }

        // Flags
        public bool is_gift { get; set; }
        public bool is_dangerous_good { get; set; }
        public bool needs_prescription { get; set; }  // ePharmacy
        public bool is_unboxing_item { get; set; }   // US market blind box

        // Unboxing (US market)
        public string unboxing_sku_code { get; set; }

        // Virtual Bundle
        public List<TikTokCombinedListingSku> combined_listing_skus { get; set; }

        // Deprecated
        public string handling_duration_days { get; set; }  // Deprecated
    }

    public class TikTokItemTax
    {
        public string tax_type { get; set; }    // SALES_TAX
        public string tax_rate { get; set; }
        public string tax_amount { get; set; }
    }

    public class TikTokCombinedListingSku
    {
        public string product_id { get; set; }
        public string sku_id { get; set; }
        public string seller_sku { get; set; }
        public int sku_count { get; set; }
    }
}
