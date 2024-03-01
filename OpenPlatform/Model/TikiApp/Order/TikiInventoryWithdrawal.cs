using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// Information about the withdrawal of product items from Tiki warehouse. This happens
    /// when product items have already been transfered into Tiki warehouse but the order is
    /// later canceled.
    /// </summary>
    public class TikiInventoryWithdrawal
    {
        /// <summary>
        /// 1453443	Unique Id of the withdrawal
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        /// waiting_for_picking	Status of the products withdrawal out of Tiki warehouse
        /// </summary>
        public string status { get; set; }
    }
}
