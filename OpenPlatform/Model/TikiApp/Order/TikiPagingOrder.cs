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
    public class TikiPagingOrder
    {
        public int total { get; set; }
        public int current_page { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public int per_page { get; set; }
        public int last_page { get; set; }
    }
}
