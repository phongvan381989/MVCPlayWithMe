using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfo_Item_DescriptionInfo_ExtendedDescription
    {
        /// <summary>
        /// If description_type is extended , Description information will be returned through this field.
        /// </summary>
        public List<ShopeeGetItemBaseInfo_Item_DescriptionInfo_ExtendedDescription_FieldList> field_list { get; set; }
    }
}
