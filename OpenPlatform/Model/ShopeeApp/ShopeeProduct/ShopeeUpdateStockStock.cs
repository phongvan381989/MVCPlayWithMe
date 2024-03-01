using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdateStockStock
    {
        public ShopeeUpdateStockStock(long id, long stock)
        {
            model_id = id;
            normal_stock = stock;
            seller_stock = new List<ShopeeUpdateStockStock_SellerStock>();
            seller_stock.Add(new ShopeeUpdateStockStock_SellerStock(stock));
        }
        /// <summary>
        /// 0 for no model item.
        /// </summary>
        public long model_id { get; set; }

        /// <summary>
        /// Required
        /// Normal stock.
        /// </summary>
        public long normal_stock { get; set; }

        /// <summary>
        /// new stock info（Please notice that stock(including Seller Stock and Shopee Stock) should be larger than or equal to real-time reserved stock）
        /// </summary>
        public List<ShopeeUpdateStockStock_SellerStock> seller_stock { get; set; }
    }
}
