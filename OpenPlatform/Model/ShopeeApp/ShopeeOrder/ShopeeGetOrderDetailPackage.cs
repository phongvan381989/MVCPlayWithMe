using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailPackage
    {
        /// <summary>
        /// Shopee's unique identifier for the package under an order.
        /// </summary>
        public string package_number { get; set; }

        /// <summary>
        /// The Shopee logistics status for the order. Applicable values: See Data Definition-LogisticsStatus.
        /// </summary>
        public string logistics_status { get; set; }

        /// <summary>
        /// The logistics service provider that the buyer selected for the order to deliver items.
        /// </summary>
        public string shipping_carrier { get; set; }

        /// <summary>
        /// The lis of items.
        /// </summary>
        public List<ShopeeGetOrderDetailPackageItem> item_list;
    }
}
