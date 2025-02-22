using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification
{
    /// <summary>
    /// Get notified immediately on all order status updates. This includes order cancellations that occur before shipping,
    /// so that you can take the necessary steps in time.
    /// </summary>
    public class OrderStatusPush
    {
        /// <summary>
        /// Return by default. Shopee's unique identifier for an order.
        /// </summary>
        public string ordersn { get; set; }

        /// <summary>
        /// Return by default. Enumerated type that defines the current status of the order.
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// To indicate which COMPLETED status order is in.
        /// NORMAL: The order has been completed.
        /// RRAOC: The whole RRAOC(raise return&refund after order completed) progress has been completed.
        /// </summary>
        public string completed_scenario { get; set; }

        /// <summary>
        /// Return by default.
        /// Timestamp that indicates the last time that there was a change in value of order,
        /// such as order status changed from 'Paid' to 'Completed'.
        /// </summary>
        public long update_time { get; set; }

    }
}