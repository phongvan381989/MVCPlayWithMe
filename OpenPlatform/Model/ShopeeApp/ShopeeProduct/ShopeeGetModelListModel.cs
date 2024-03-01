using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetModelList_Model
    {
        /// <summary>
        /// Price info.
        /// </summary>
        public List<ShopeeGetModelList_Model_PriceInfo> price_info { get; set; }

        /// <summary>
        /// Model ID.
        /// </summary>
        public long model_id { get; set; }

        /// <summary>
        /// Stock info.
        /// Comment vì shopee cập nhật dùng stock_info_v2
        /// </summary>
        //public List<ShopeeGetModelList_Model_StockInfo> stock_info { get; set; }

        /// <summary>
        /// Tier index of this model.
        /// </summary>
        public List<int> tier_index { get; set; }

        /// <summary>
        /// Current promotion ID of this model.
        /// </summary>
        public long promotion_id { get; set; }

        /// <summary>
        /// SKU of this model. the length should be under 100.
        /// </summary>
        public string model_sku { get; set; }

        /// <summary>
        /// The model status. Should be MODEL_NORMAL or MODEL_UNAVAILABLE.
        /// MODEL_NORMAL models can be sold on the buyer's side,
        /// and MODEL_UNAVAILABLE models cannot be sold on the buyer's side.
        /// </summary>
        public string model_status { get; set; }

        /// <summary>
        /// (Only whitelisted users can use)
        /// </summary>
        public ShopeeGetModelList_Model_PreOrder pre_order { get; set; }

        /// <summary>
        /// new stock info.
        /// Please check this FAQ for more detail: https://open.shopee.com/faq?top=162&sub=166&page=1&faq=230
        /// </summary>
        public ShopeeGetModelList_Model_StockInfoV2 stock_info_v2 { get; set; }

        /// <summary>
        /// (Only BR local seller available) gtin code.
        /// </summary>
        public string gtin_code { get; set; }
    }
}
