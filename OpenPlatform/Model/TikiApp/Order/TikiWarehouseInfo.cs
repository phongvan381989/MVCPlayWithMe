using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// The Tiki warehouse that fulfills the order. This Tiki warehouse information
    /// together with appropriate seller inventories must be used to confirm the order.
    /// </summary>
    public class TikiWarehouseInfo
    {
        /// <summary>
        /// 17	Warehouse unique id
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// Ha Noi 4	Warehouse friendly name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// hn4	Warehouse unique code
        /// </summary>
        public string code { get; set; }
    }
}
