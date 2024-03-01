using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdatePrice_Request_Price_List
    {
        public ShopeeUpdatePrice_Request_Price_List(long model, float price)
        {
            model_id = model;
            original_price = price;
        }
        /// <summary>
        /// 0 for no model item.
        /// </summary>
        public long model_id { get; set; }

        /// <summary>
        /// Original price for this model.
        /// </summary>
        public float original_price { get; set; }
    }
}
