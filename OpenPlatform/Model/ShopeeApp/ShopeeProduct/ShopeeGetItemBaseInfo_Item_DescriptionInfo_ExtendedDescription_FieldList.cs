using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfo_Item_DescriptionInfo_ExtendedDescription_FieldList
    {
        /// <summary>
        /// description_field_type (text , image).
        /// </summary>
        public string field_type { get; set; }

        /// <summary>
        /// If field_type is text, text information will be returned through this field.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// If field_type is image, image url will be returned through this field.
        /// </summary>
        public ShopeeGetModelList_TierVariation_Option_Image image_info { get; set; }
    }
}
