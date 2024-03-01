using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// Some basic seller information
    /// </summary>
    public class TikiOrderItemSeller
    {
        /// <summary>
        /// 33	Unique seller Id
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// Zero Shop	Seller or Store name
        /// </summary>
        public string name { get; set; }
    }
}
