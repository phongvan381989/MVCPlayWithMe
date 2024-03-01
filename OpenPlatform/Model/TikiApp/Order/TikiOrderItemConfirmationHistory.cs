using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiOrderItemConfirmationHistory
    {
        /// <summary>
        /// null	When the item changes to this confirmation status
        /// </summary>
        public DateTime confirmed_at { get; set; }

        /// <summary>
        /// 156453540	The Id of the Order Item V2
        /// </summary>
        public long order_item_id { get; set; }

        /// <summary>
        /// 2020-08-11 12:00:00.0	Available confirmation deadline
        /// </summary>
        public DateTime sla_confirmed_at { get; set; }

        /// <summary>
        /// waiting	The confirmation status
        /// </summary>
        public string status { get; set; }
    }
}
