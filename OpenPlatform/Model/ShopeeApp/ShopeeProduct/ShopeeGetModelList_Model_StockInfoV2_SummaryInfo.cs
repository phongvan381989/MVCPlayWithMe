using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    /// <summary>
    /// stock summary Info
    /// </summary>
    public class ShopeeGetModelList_Model_StockInfoV2_SummaryInfo
    {
        /// <summary>
        /// total reserved stock
        /// </summary>
        public int total_reserved_stock { get; set; }

        /// <summary>
        /// total available stock
        /// </summary>
        public int total_available_stock { get; set; }
    }
}
