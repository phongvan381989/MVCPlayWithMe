using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemStockInfo
    {
        /// <summary>
        /// The stock type. Applicable values: See Data Definition- StockType.
        /// </summary>
        public long stock_type { get; set; }

        /// <summary>
        /// location_id of the stock.
        /// </summary>
        public string stock_location_id { get; set; }

        /// <summary>
        /// Current available inventory, if item under promotion, it will be promotion stock, if not, it will be normal_stock
        /// </summary>
        public long current_stock { get; set; }

        /// <summary>
        /// The stock set by the seller.
        /// </summary>
        public long normal_stock { get; set; }

        /// <summary>
        /// Promotion stock. Sellers can set Promotion stock for some promotion, which can only be used during the event. If the item with multiple promotion, this value is the total number of locked stocks for multiple promotions
        /// </summary>
        public long reserved_stock { get; set; }
    }
}
