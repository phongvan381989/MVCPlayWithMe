using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderListResponse
    {
        /// <summary>
        /// This is to indicate whether the order list is more than one page. 
        /// If this value is true, you may want to continue to check next page to retrieve orders.
        /// </summary>
        public Boolean more { get; set; }

        public List<ShopeeGetOrderListBaseInfo> order_list { get; set; }

        /// <summary>
        /// If more is true, you should pass the next_cursor in the next request as cursor.
        /// The value of next_cursor will be empty string when more is false.
        /// </summary>
        public string next_cursor { get; set; }
    }
}
