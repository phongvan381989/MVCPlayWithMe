using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdatePrice_Response
    {
        /// <summary>
        /// Fail model list.
        /// </summary>
        public List<ShopeeUpdateStockFailure> failure_list { get; set; }

        /// <summary>
        /// Success model list.
        /// </summary>
        public List<ShopeeUpdatePrice_Response_Success_List> success_list { get; set; }
    }
}
