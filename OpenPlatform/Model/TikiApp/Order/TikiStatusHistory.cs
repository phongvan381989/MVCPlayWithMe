using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiStatusHistory
    {
        /// <summary>
        /// 1105187480	Unique Id of the history entry
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// successful_delivery	Order status
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 2020-08-10 18:50:17	When the order arrives at this status
        /// </summary>
        public DateTime created_at { get; set; }
    }
}
