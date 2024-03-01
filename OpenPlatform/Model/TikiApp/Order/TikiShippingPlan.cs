using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// 
    /// </summary>
    public class TikiShippingPlan
    {
        /// <summary>
        /// 1	The plan unique Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Giao hàng tiêu chuẩn	The plan name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// false	Is the shipping free
        /// </summary>
        public bool is_free_shipping { get; set; }

        /// <summary>
        /// 2020-08-24 23:59:59	The delivery deadline
        /// </summary>
        public DateTime promised_delivery_date { get; set; }

        /// <summary>
        /// Giao vào Thứ hai, 24/08	A short Vietnamese description about the delivery deadline
        /// </summary>
        public string description { get; set; }
    }
}
