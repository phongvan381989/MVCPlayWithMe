using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailImageInfocs
    {
        /// <summary>
        /// The image url of the product. Default to be variation image, if the model does not have a variation image, will use an item main image instead.
        /// </summary>
        public string image_url { get; set; }
    }
}
