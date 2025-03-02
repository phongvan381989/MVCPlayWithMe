using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Event
{
    public class Event_Payload
    {
        // Mã đơn hàng (order code)
        public string order_code { get; set; }

        // Mã đơn hàng (viết lại)
        public string orderCode { get; set; }

        // Trạng thái đơn hàng
        public string status { get; set; }
    }
}