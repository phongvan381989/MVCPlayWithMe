using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetModelList_TierVariation
    {
        /// <summary>
        /// Variation name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// option list
        /// </summary>
        public List<ShopeeGetModelList_TierVariation_Option> option_list { get; set; }
    }
}
