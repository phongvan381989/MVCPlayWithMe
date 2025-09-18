using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification
{
    /// <summary>
    /// Thông tin chung của một notification shopee
    /// </summary>
    public class ShopeeBaseNotification
    {
        /// <summary>
        /// Shopee's unique identifier for a shop. Required param for most APIs.
        /// </summary>
        public long shop_id { get; set; }

        /// <summary>
        /// Shopee's unique identifier for a push notification.
        /// 3: order_status_push
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// Timestamp that indicates the message was sent.
        /// </summary>
        public long timestamp { get; set; }

        public string msg_id { get; set; }
    }
}