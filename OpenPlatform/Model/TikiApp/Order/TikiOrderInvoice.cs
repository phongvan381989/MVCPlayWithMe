using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// This object contains total value, discounts and fees of customer and seller.
    /// </summary>
    public class TikiOrderInvoice
    {
        /// <summary>
        /// 	1	Number of Order Item V2s
        /// </summary>
        public Int32 items_count { get; set; }

        /// <summary>
        /// 1	Total quantity of products over all Order Item V2s
        /// </summary>
        public Int32 items_quantity { get; set; }

        /// <summary>
        /// 409000.00	Total value of products over all Order Item V2s
        /// </summary>
        public Double subtotal { get; set; }

        /// <summary>
        /// 0.00	Total value of the order the customer needs to pay (counted all customer discounts and customer fees)
        /// </summary>
        public Double grand_total { get; set; }

        /// <summary>
        /// 0.00	For Cash on Delivery: Total value to collect from the customer when deliver
        /// </summary>
        public Double collectible_amount { get; set; }

        /// <summary>
        /// 428000.00	Total discount value on the order for the customer
        /// </summary>
        public Double discount_amount { get; set; }

        /// <summary>
        /// 428000.00	Discount value due to the use of Tiki Xu
        /// </summary>
        public Double discount_tikixu { get; set; }

        /// <summary>
        /// 0.00	Discount due to promotion at the time of purchasing
        /// </summary>
        public Double discount_promotion { get; set; }

        /// <summary>
        /// 0.00	Discount due by percent of order value
        /// </summary>
        public Double discount_percent { get; set; }

        /// <summary>
        /// 0.00	Discount value due to the use of coupons
        /// </summary>
        public Double discount_coupon { get; set; }

        /// <summary>
        /// 0.00	Discount value due to other reasons (if any)
        /// </summary>
        public Double discount_other { get; set; }

        /// <summary>
        /// 0.00	Discount value due to the use gift cards
        /// </summary>
        public Double gift_card_amount { get; set; }

        /// <summary>
        /// 	The gift card code entered by the customer when purchases the order
        /// </summary>
        public string gift_card_code { get; set; }

        /// <summary>
        /// 	The coupon code entered by the customer when purchases the order
        /// </summary>
        public string coupon_code { get; set; }

        /// <summary>
        /// 19000.00	The shipping fee after discount the customer needs to pay
        /// </summary>
        public Double shipping_amount_after_discount { get; set; }

        /// <summary>
        /// 0.00	The discount of the shipping fee
        /// </summary>
        public Double shipping_discount_amount { get; set; }

        /// <summary>
        /// 0.00	The handling fee for oversized products the customer needs to pay
        /// </summary>
        public Double handling_fee { get; set; }

        /// <summary>
        /// 0.00	Other customer fee
        /// </summary>
        public Double other_fee { get; set; }

        /// <summary>
        /// 0.00	The final fee amount in seller statement
        /// </summary>
        public Double total_seller_fee { get; set; }

        /// <summary>
        /// 409000.00	The final income amount in seller statement
        /// </summary>
        public Double total_seller_income { get; set; }

        /// <summary>
        /// 2020-08-10 18:50:17	When the payment is made
        /// </summary>
        public DateTime purchased_at { get; set; }

        /// <summary>
        /// 	Customer tax information
        /// </summary>
        public TikiTaxInfo TaxInfo { get; set; }
    }
}
