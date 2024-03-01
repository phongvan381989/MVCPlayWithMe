using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailPackageItem
    {
        /// <summary>
        /// Shopee's unique identifier for an item.s
        /// </summary>
        public long item_id { get; set; }

        /// <summary>
        /// Shopee's unique identifier for a model.
        /// </summary>
        public long model_id { get; set; }
    }
}
