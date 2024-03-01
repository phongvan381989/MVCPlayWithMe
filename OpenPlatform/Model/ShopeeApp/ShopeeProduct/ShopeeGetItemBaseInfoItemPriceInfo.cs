using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemPriceInfo
    {
        /// <summary>
        /// The three-digit code representing the currency unit used for the item in Shopee Listings
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// The original price of the item in the listing currency
        /// </summary>
        public float original_price { get; set; }

        /// <summary>
        /// The current price of the item in the listing currency. If product under a onging promotion, current_price will be the promotion price
        /// </summary>
        public float current_price { get; set; }

        /// <summary>
        /// The After-tax original price of the item in the listing currency.
        /// </summary>
        public float inflated_price_of_original_price { get; set; }

        /// <summary>
        /// The After-tax current price of the item in the listing currency.
        /// </summary>
        public float inflated_price_of_current_price { get; set; }

        /// <summary>
        /// The price of the item in sip.If item is for CNSC primary shop, this field will not be returned
        /// </summary>
        public float sip_item_price { get; set; }

        /// <summary>
        /// source of sip' price. ( auto or manual).If item is for CNSC SIP primary shop, this field will not be returned
        /// </summary>
        public string sip_item_price_source { get; set; }
    }
}
