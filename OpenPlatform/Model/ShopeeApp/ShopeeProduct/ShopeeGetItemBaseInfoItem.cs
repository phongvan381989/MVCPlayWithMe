using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItem
    {
        /// <summary>
        /// Shopee's unique identifier for an item.
        /// </summary>
        public long item_id { get; set; }

        /// <summary>
        /// Shopee's unique identifier for a category.
        /// </summary>
        public long category_id { get; set; }

        /// <summary>
        /// Name of the item in local language.
        /// </summary>
        public string item_name { get; set; }

        /// <summary>
        /// Description of the item in local language.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// An item SKU (stock keeping unit) is an identifier defined by a seller, sometimes called parent SKU. Item SKU can be assigned to an item in Shopee Listings.
        /// </summary>
        public string item_sku { get; set; }

        /// <summary>
        /// Timestamp that indicates the date and time that the item was created.
        /// </summary>
        public long create_time { get; set; }

        /// <summary>
        /// Timestamp that indicates the last time that there was a change in value of the item, such as price/stock change.
        /// </summary>
        public long update_time { get; set; }

        public List<ShopeeGetItemBaseInfoItemAttribute> attribute_list { get; set; }

        public List<ShopeeGetItemBaseInfoItemPriceInfo> price_info { get; set; }

        // Comment vì shopee update
        //public List<ShopeeGetItemBaseInfoItemStockInfo> stock_info { get; set; }

        public ShopeeGetItemBaseInfoItemImage image { get; set; }

        /// <summary>
        /// The net weight of this item, the unit is KG
        /// </summary>
        public float weight { get; set; }

        /// <summary>
        /// The dimension of this item.
        /// </summary>
        public ShopeeGetItemBaseInfoItemDimension dimension { get; set; }

        /// <summary>
        /// The logistics list.
        /// </summary>
        public List<ShopeeGetItemBaseInfoItemLogisticInfo> logistic_info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShopeeGetItemBaseInfoItemPreOrder pre_order { get; set; }

        /// <summary>
        /// The wholesales tier list
        /// </summary>
        public List<ShopeeGetItemBaseInfoItemWholeSales> wholesales { get; set; }

        /// <summary>
        /// Is it second-hand.
        /// </summary>
        public string condition { get; set; }

        /// <summary>
        /// Url of size chart image.
        /// </summary>
        public string size_chart { get; set; }

        /// <summary>
        /// Enumerated type that defines the current status of the item.
        /// Applicable values: NORMAL, DELETED, BANNED and UNLIST.
        /// </summary>
        public string item_status { get; set; }

        /// <summary>
        /// Does it contain model.
        /// </summary>
        public Boolean has_model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long promotion_id { get; set; }

        /// <summary>
        /// Info of video list.
        /// </summary>
        public List<ShopeeGetItemBaseInfoItemVideoInfo> video_info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShopeeGetItemBaseInfoItemBrand brand { get; set; }

        /// <summary>
        /// This field is only applicable for local sellers in Indonesia and Malaysia. Use this field to identify whether a product is a dangerous product. 0 for non-dangerous product and 1 for dangerous product. For more information, please visit the market's respective Seller Education Hub.
        /// </summary>
        public int item_dangerous { get; set; }

        /// <summary>
        /// Complaint policy.Only returned for local PL sellers, and need_complaint_policy in request is true.
        /// </summary>
        public ShopeeGetItemBaseInfoItemComplaintPolicy complaint_policy { get; set; }

        /// <summary>
        /// Tax information.Only returned if need_tax_info in request is true.
        /// </summary>
        public ShopeeGetItemBaseInfoItemTaxInfo tax_info { get; set; }

        /// <summary>
        /// new stock object.
        /// Please check this FAQ for more detail: https://open.shopee.com/faq?top=162&sub=166&page=1&faq=230
        /// </summary>
        public ShopeeGetModelList_Model_StockInfoV2 stock_info_v2 { get; set; }

        /// <summary>
        /// New description field. Only whitelist sellers can use it.
        /// </summary>
        public ShopeeGetItemBaseInfo_Item_DescriptionInfo description_info { get; set; }

        /// <summary>
        /// Type of description : values: See Data Definition- description_type (normal , extended)
        /// </summary>
        public string description_type { get; set; }

        /// <summary>
        /// (only for BR Local seller) This field will return when the item has no model and BR local seller have uploaded the gtin_code.
        /// </summary>
        public string gtin_code { get; set; }
    }
}
