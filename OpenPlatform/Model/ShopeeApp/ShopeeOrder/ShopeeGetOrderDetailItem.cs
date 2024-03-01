using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailItem
    {
        /// <summary>
        /// Shopee's unique identifier for an item.
        /// </summary>
        public long item_id { get; set; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        public string item_name { get; set; }

        /// <summary>
        /// A item SKU (stock keeping unit) is an identifier defined by a seller, sometimes called parent SKU. Item SKU can be assigned to an item in Shopee Listings.
        /// </summary>
        public string item_sku { get; set; }

        /// <summary>
        /// ID of the model that belongs to the same item.
        /// </summary>
        public long model_id { get; set; }

        /// <summary>
        /// Name of the model that belongs to the same item. A seller can offer models of the same item. For example, the seller could create a fixed-priced listing for a t-shirt design and offer the shirt in different colors and sizes. In this case, each color and size combination is a separate model. Each model can have a different quantity and price.
        /// </summary>
        public string model_name { get; set; }

        /// <summary>
        /// A model SKU (stock keeping unit) is an identifier defined by a seller. It is only intended for the seller's use. Many sellers assign a SKU to an item of a specific type, size, and color, which are models of one item in Shopee Listings.
        /// </summary>
        public string model_sku { get; set; }

        /// <summary>
        /// The number of identical items purchased at the same time by the same buyer from one listing/item.
        /// </summary>
        public int model_quantity_purchased { get; set; }

        /// <summary>
        /// The original price of the item in the listing currency.
        /// </summary>
        public float model_original_price { get; set; }

        /// <summary>
        /// The after-discount price of the item in the listing currency. If there is no discount, this value will be same as that of model_original_price. In case of bundle deal item, this value will return 0 as by design bundle deal discount will not be breakdown to item/model level. Due to technical restriction, the value will return the price before bundle deal if we don't configure it to 0. Please call GetEscrowDetails if you want to calculate item-level discounted price for bundle deal item.
        /// </summary>
        public float model_discounted_price { get; set; }

        /// <summary>
        /// This value indicates whether buyer buy the order item in wholesale price.
        /// </summary>
        public Boolean wholesale { get; set; }

        /// <summary>
        /// The weight of the item
        /// </summary>
        public float weight { get; set; }

        /// <summary>
        /// To indicate if this item belongs to an addon deal.
        /// </summary>
        public Boolean add_on_deal { get; set; }

        /// <summary>
        /// To indicate if this item is main item or sub item. True means main item, false means sub item.
        /// </summary>
        public Boolean main_item { get; set; }

        /// <summary>
        /// A unique ID to distinguish groups of items in Cart, and Order. (e.g. AddOnDeal)
        /// </summary>
        public long add_on_deal_id { get; set; }

        /// <summary>
        /// Available type：product_promotion, flash_sale, group_by, bundle_deal, add_on_deal_main, add_on_deal_sub
        /// </summary>
        public string promotion_type { get; set; }

        /// <summary>
        /// The ID of the promotion.
        /// </summary>
        public long promotion_id { get; set; }

        /// <summary>
        /// The identify of order item.
        /// </summary>
        public long order_item_id { get; set; }

        /// <summary>
        /// The identify of product promotion.
        /// </summary>
        public long promotion_group_id { get; set; }

        /// <summary>
        /// Image info of the product.
        /// </summary>
        public ShopeeGetOrderDetailImageInfocs image_info { get; set; }

        /// <summary>
        /// The list of warehouse IDs of the item.
        /// </summary>
        public List<string> product_location_id { get; set; }
    }
}
