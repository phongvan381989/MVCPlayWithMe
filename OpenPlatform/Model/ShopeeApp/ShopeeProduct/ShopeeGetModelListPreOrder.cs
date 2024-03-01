using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetModelList_Model_PreOrder
    {
        /// <summary>
        /// Pre-order.
        /// </summary>
        public Boolean is_pre_order { get; set; }

        /// <summary>
        /// The days to ship.
        /// </summary>
        public int days_to_ship { get; set; }
    }
}
