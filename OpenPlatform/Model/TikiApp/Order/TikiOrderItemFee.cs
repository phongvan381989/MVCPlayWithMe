using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// For computing seller_fee on Order Item V2 and total_seller_fee on Order V2
    /// we only need to concern on final_amount.
    //    {
    //  "id": 208365438,
    //  "fee_type_key": "base_fee",
    //  "fee_type_name": "Phí cố định",
    //  "status": 1,
    //  "quantity": 1,
    //  "base_amount": 8000,
    //  "total_amount": -8000,
    //  "discount_amount": 0,
    //  "final_amount": -8000
    //}
    /// </summary>
    public class TikiOrderItemFee
    {
        /// <summary>
        /// 189434047	Unique Id of the fee entry
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// pickup_fee	Fee type
        /// </summary>
        public string fee_type_key { get; set; }

        /// <summary>
        /// Phí lấy hàng	Fee display name
        /// </summary>
        public string fee_type_name { get; set; }

        /// <summary>
        /// 1	If the fee is enabled or not
        /// </summary>
        public Int32 status { get; set; }

        /// <summary>
        /// 1	Multiply of the fee base_amount
        /// </summary>
        public Int32 quantity { get; set; }

        /// <summary>
        /// 5	Base fee amount: depends on the fee_type_key this can be used as   a fixed absolute amount percent of subtotal
        /// </summary>
        public Double base_amount { get; set; }

        /// <summary>
        /// -5500.00	Computed fee amount base on base_amount and quantity (before discount)
        /// </summary>
        public Double total_amount { get; set; }

        /// <summary>
        /// 0.00	Discount value for seller on this fee entry
        /// </summary>
        public Double discount_amount { get; set; }

        /// <summary>
        /// -5500.00	Actual fee amount in seller statement (after discount)
        /// </summary>
        public Double final_amount { get; set; }
    }
}
