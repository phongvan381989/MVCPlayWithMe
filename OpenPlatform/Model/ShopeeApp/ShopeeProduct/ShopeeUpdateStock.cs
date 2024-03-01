using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdateStock
    {
        public ShopeeUpdateStock()
        {
            stock_list = new List<ShopeeUpdateStockStock>();
        }

        /// <summary>
        /// ID of item.
        /// </summary>
        public long item_id { get; set; }

        /// <summary>
        /// Required
        /// Length should be between 1 to 50.
        /// </summary>
        public List<ShopeeUpdateStockStock> stock_list { get; set; }
    }
}
