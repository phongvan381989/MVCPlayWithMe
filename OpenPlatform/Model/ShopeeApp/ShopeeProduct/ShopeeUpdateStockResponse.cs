using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdateStockResponse
    {
        /// <summary>
        /// Fail model list.
        /// </summary>
        public List<ShopeeUpdateStockFailure> failure_list { get; set; }

        /// <summary>
        /// Success model list.
        /// </summary>
        public List<ShopeeUpdateStockSuccess> success_list { get; set; }
    }
}
