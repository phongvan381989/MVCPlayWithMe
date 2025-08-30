using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaNotification
{
    public class LazadaOrderStatusPush
    {
        // #Order status
        public string order_status { get; set; }

        // Is trade_order_id mapping to order_id ?     Yes
        public long trade_order_id { get; set; }

        // #timestamp of the order status update
        //update time (seconds)
        public long status_update_time { get; set; }

        // What is a sub-order id ?  Use trade order id to query the api.
        public string trade_order_line_id { get; set; }
    }
}