using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemWholeSales
    {
        /// <summary>
        /// The min count of this tier wholesale.
        /// </summary>
        public int min_count { get; set; }

        /// <summary>
        /// The max count of this tier wholesale.
        /// </summary>
        public int max_count { get; set; }

        /// <summary>
        /// The current price of the wholesale in the listing currency.If item is in promotion, this price is useless.
        /// </summary>
        public float unit_price { get; set; }

        /// <summary>
        /// The After-tax Price of the wholesale show to buyer.
        /// </summary>
        public float inflated_price_of_unit_price { get; set; }
    }
}
