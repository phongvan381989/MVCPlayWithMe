using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetModelList_Model_StockInfo
    {
        /// <summary>
        /// Normal stock.
        /// </summary>
        public int normal_stock { get; set; }

        /// <summary>
        /// Stock type.
        /// </summary>
        public int stock_type { get; set; }

        /// <summary>
        /// Current stock.
        /// </summary>
        public int current_stock { get; set; }

        /// <summary>
        /// Stock reserved for upcoming promotion.
        /// </summary>
        public int reserved_stock { get; set; }

        /// <summary>
        /// location_id of the stock.
        /// </summary>
        public string stock_location_id { get; set; }
    }
}
