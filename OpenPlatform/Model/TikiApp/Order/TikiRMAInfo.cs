using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// RMA stands for Return Merchandise Authorization – is a part of the process of
    /// returning a product to receive a refund, replacement, or repair during the product’s warranty period.
    /// Both parties can decide how to deal with it, which could be refund, replacement or repair.
    /// </summary>
    public class TikiRMAInfo
    {
        /// <summary>
        /// 603055	Unique Id of the RMA process
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// 16	The Tiki warehouse which handles this process
        /// </summary>
        public Int32 warehouse_id { get; set; }

        /// <summary>
        /// 789783969	The original order of which the product is returned or serviced
        /// Code of the other order
        /// </summary>
        public string original_order_code { get; set; }

        /// <summary>
        /// 745467462	The replacement order for the original order/product
        /// Code of this order
        /// </summary>
        public string exchanged_order_code { get; set; }

        /// <summary>
        /// Hoàn thành	Status of the RMA process
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 2020-06-09	When the RMA process is started
        /// </summary>
        public DateTime created_at { get; set; }
    }
}
