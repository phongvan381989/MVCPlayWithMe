using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdatePrice_Request
    {
        public ShopeeUpdatePrice_Request()
        {
            price_list = new List<ShopeeUpdatePrice_Request_Price_List>();
        }

        /// <summary>
        /// ID of item.
        /// </summary>
        public long item_id { get; set; }

        /// <summary>
        /// Required
        /// Length should be between 1 to 50.
        /// </summary>
        public List<ShopeeUpdatePrice_Request_Price_List> price_list { get; set; }
    }
}
