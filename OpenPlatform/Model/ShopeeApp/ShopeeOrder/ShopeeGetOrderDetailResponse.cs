using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailResponse
    {
        /// <summary>
        /// The list of orders.
        /// </summary>
        public List<ShopeeOrderDetail> order_list { get; set; }
    }
}
