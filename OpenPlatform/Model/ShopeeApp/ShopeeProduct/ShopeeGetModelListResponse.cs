using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetModelListResponse
    {
        /// <summary>
        /// Variation config of item.
        /// </summary>
        public List<ShopeeGetModelList_TierVariation> tier_variation { get; set; }

        /// <summary>
        /// Model list.
        /// </summary>
        public List<ShopeeGetModelList_Model> model { get; set; }
    }
}
