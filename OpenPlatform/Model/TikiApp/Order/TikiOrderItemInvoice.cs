using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    // All discounts count for the order item only. For computing row_total on Order Item V2,
    // we only need to concern on subtotal and discount_amount.
    /// </summary>
    public class TikiOrderItemInvoice
    {
        /// <summary>
        /// 409000.00	Product price at the time of the purchasing
        /// </summary>
        public Double price { get; set; }

        /// <summary>
        /// 1	Purchased quantity of this product
        /// </summary>
        public Int32 quantity { get; set; }

        /// <summary>
        /// 409000.00	Total value of this purchased product
        /// </summary>
        public Double subtotal { get; set; }

        /// <summary>
        /// 0.00	Total value of this order item the customer needs to pay (counted all customer discounts and customer fees)
        /// </summary>
        public Double row_total { get; set; }

        /// <summary>
        /// 428000.00	Total discount value for the customer
        /// </summary>
        public Double discount_amount { get; set; }

        /// <summary>
        /// 428000.00	Discount value due to the use of Tiki Xu
        /// </summary>
        public Double discount_tikixu { get; set; }

        /// <summary>
        /// 0.00	Discount value due to the active promotion at the time of the purchasing
        /// </summary>
        public Double discount_promotion { get; set; }

        /// <summary>
        /// 0.00	Discount value computed by percent on order item value
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

        public Double discount_tikier { get; set; }

        public Double discount_tiki_first { get; set; }

        public Object discount_data { get; set; }

        /// <summary>
        /// false	If the discount_coupon comes from seller coupon
        /// </summary>
        public bool is_seller_discount_coupon { get; set; }

        /// <summary>
        /// false	Can export VAT bill for this product
        /// </summary>
        public bool is_taxable { get; set; }

        public Double fob_price { get; set; }

        /// <summary>
        /// 0.00	Seller fee in seller statement (on this order item)
        /// </summary>
        public Double seller_fee { get; set; }

        /// <summary>
        /// 409000.00	Seller income in seller statement (from this order item)
        /// </summary>
        public Double seller_income { get; set; }

        /// <summary>
        /// Fee details
        /// </summary>
        public List<TikiOrderItemFee> fees { get; set; }
    }
}
