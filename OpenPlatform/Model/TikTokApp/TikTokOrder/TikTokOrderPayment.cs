namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder
{
    /// <summary>
    /// TikTok Shop Order Payment Information
    /// Field names match exactly with TikTok Shop JSON response
    /// </summary>
    public class TikTokOrderPayment
    {
        public string currency { get; set; }

        // Product Pricing
        public string original_total_product_price { get; set; }
        public string sub_total { get; set; }
        public string seller_discount { get; set; }
        public string platform_discount { get; set; }

        // Shipping
        public string original_shipping_fee { get; set; }
        public string shipping_fee { get; set; }
        public string shipping_fee_seller_discount { get; set; }
        public string shipping_fee_platform_discount { get; set; }
        public string shipping_fee_cofunded_discount { get; set; }

        // Taxes
        public string tax { get; set; }
        public string product_tax { get; set; }
        public string shipping_fee_tax { get; set; }

        // Fees
        public string buyer_service_fee { get; set; }
        public string small_order_fee { get; set; }
        public string handling_fee { get; set; }
        public string retail_delivery_fee { get; set; }

        // Insurance (Indonesia market)
        public string item_insurance_fee { get; set; }
        public string item_insurance_tax { get; set; }
        public string shipping_insurance_fee { get; set; }

        // Distance Fee (Indonesia Horizon+ Program)
        public string distance_fee { get; set; }
        public string distance_shipping_fee { get; set; }

        // Payment Platform
        public string payment_platform_discount { get; set; }
        public string payment_discount_service_fee { get; set; }

        // Total
        public string total_amount { get; set; }
    }
}
