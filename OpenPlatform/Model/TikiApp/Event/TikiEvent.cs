using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Event
{
    public class TikiEvent
    {
        // ID của thông báo
        public string id { get; set; }

        // Session ID hoặc token của thông báo
        public string sid { get; set; }

        // Thời điểm thông báo được tạo (timestamp)
        public long created_at { get; set; }

        // Payload chứa thông tin chi tiết về đơn hàng
        public Event_Payload payload { get; set; }

        // Loại thông báo
        public string type { get; set; }

        // Phiên bản của thông báo
        public string version { get; set; }
    }
}