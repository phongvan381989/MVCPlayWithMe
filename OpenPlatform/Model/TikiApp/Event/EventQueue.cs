using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Event
{
    public class EventQueue
    {
        // Mã của hàng đợi
        public string code { get; set; }

        // Tên hàng đợi
        public string name { get; set; }

        // Trạng thái hàng đợi: ACTIVE, DE-ACTIVE
        public string status { get; set; }

        // Mã của yêu cầu lấy dữ liệu gần nhất
        public string ack_id { get; set; }
    }
}