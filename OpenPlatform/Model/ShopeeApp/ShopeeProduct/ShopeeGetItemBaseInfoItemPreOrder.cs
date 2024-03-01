using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemPreOrder
    {
        /// <summary>
        /// Pre-order will be set true.
        /// </summary>
        public Boolean is_pre_order { get; set; }

        /// <summary>
        /// The days to ship. Only work for pre-order, it means this value should be bigger than 7.
        /// </summary>
        public int days_to_ship { get; set; }
    }
}
