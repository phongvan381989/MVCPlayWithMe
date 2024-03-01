using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetModelList_Model_StockInfoV2
    {
        /// <summary>
        /// stock summary Info
        /// </summary>
        public ShopeeGetModelList_Model_StockInfoV2_SummaryInfo summary_info { get; set; }

        /// <summary>
        /// seller stock
        /// </summary>
        public List<ShopeeGetModelList_Model_StockInfoV2_SellerStock> seller_stock { get; set; }

        /// <summary>
        /// shopee stock
        /// </summary>
        public List<ShopeeGetModelList_Model_StockInfoV2_ShopeeStock> shopee_stock { get; set; }
    }
}
