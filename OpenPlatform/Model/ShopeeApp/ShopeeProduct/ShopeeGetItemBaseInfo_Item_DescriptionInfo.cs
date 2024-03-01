using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfo_Item_DescriptionInfo
    {
        /// <summary>
        /// If description_type is extended , Description information will be returned through this field.
        /// </summary>
        public ShopeeGetItemBaseInfo_Item_DescriptionInfo_ExtendedDescription extended_description { get; set; }
    }
}
