using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderListBaseInfo
    {
        /// <summary>
        /// Shopee's unique identifier for an order.
        /// </summary>
        public string order_sn { get; set; }

        /// <summary>
        /// The order_status filter for retriveing orders and each one only every request. 
        /// Available value: UNPAID/READY_TO_SHIP/PROCESSED/SHIPPED/COMPLETED/IN_CANCEL/CANCELLED
        /// </summary>
        public string order_status { get; set; }
    }
}
