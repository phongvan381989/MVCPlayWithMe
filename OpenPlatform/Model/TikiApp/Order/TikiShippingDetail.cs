using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiShippingDetail
    {
        public string tracking_id { get; set; }

        public string client_order_id { get; set; }

        public Double shipping_fee { get; set; }

        public string partner_code { get; set; }

        public string service_code { get; set; }

        public DateTime timestamp { get; set; }

        public string status { get; set; }

        public string reason_code { get; set; }

        public TikiShippingDetailDriver driver { get; set; }

        public string tracking_url { get; set; }
    }
}
