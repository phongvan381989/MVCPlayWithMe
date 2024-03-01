using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdateStockStock_SellerStock
    {
        public ShopeeUpdateStockStock_SellerStock(/*string loca, */long st)
        {
            //location_id = loca;
            stock = st;
        }
        /// <summary>
        /// location id, you can get the location id from v2.shop.get_warehouse_detail api, if seller don't have any warehouse, you don't need to upload this field.
        /// </summary>
        public string location_id { get; set; }

        /// <summary>
        /// stock
        /// </summary>
        public long stock { get; set; }
    }
}
