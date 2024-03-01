using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeItemId
    {
        public ShopeeItemId(long item, long model)
        {
            item_id = item;
            model_id = model;
        }

        public ShopeeItemId(long item, long model, long quan)
        {
            item_id = item;
            model_id = model;
            quantity = quan;
        }

        /// <summary>
        /// Shopee's unique identifier for an item.
        /// </summary>
        public long item_id { get; set; }

        /// <summary>
        /// Shopee's unique identifier for an model.
        /// </summary>
        public long model_id { get; set; }

        /// <summary>
        /// Số lượng sản phẩm trên sàn
        /// </summary>
        public long quantity { get; set; }
    }
}
