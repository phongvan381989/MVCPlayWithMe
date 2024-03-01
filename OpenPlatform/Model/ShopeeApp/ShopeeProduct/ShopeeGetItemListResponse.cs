using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemListResponse
    {
        /// <summary>
        /// list of item info with item_id/ item_status/ update_time
        /// </summary>
        public List <ShopeeItem> item { get; set; }

        /// <summary>
        /// This is to indicate whether the item list is more than one page. If this value is true, you may want to continue to check next page to retrieve the rest of items.
        /// </summary>
        public Boolean has_next_page { get; set; }

        /// <summary>
        /// if has_next_page is true, this value need set to next request.offset
        /// </summary>
        public int next_offset { get; set; }

        /// <summary>
        /// total count of all items
        /// </summary>
        public int total_count { get; set; }
    }
}
