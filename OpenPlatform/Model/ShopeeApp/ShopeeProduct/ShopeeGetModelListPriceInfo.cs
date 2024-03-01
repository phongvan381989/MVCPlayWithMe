using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetModelList_Model_PriceInfo
    {
        /// <summary>
        /// Current price of item.
        /// </summary>
        public float current_price { get; set; }

        /// <summary>
        /// original_price
        /// </summary>
        public float original_price { get; set; }

        /// <summary>
        /// Original price of item after tax.
        /// </summary>
        public float inflated_price_of_original_price { get; set; }

        /// <summary>
        /// Current price of item after tax.
        /// </summary>
        public float inflated_price_of_current_price { get; set; }

        /// <summary>
        /// SIP item price.If item is for CNSC primary shop, this field will not be returned
        /// </summary>
        public float sip_item_price { get; set; }

        /// <summary>
        /// SIP item price source, could be manual or auto.If item is for CNSC primary shop, this field will not be returned
        /// </summary>
        public float sip_item_price_source { get; set; }

        /// <summary>
        /// Currency for the item price.
        /// </summary>
        public string currency { get; set; }
    }
}
