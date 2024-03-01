using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemAttribute
    {
        /// <summary>
        /// The Identify of each category.
        /// </summary>
        public int attribute_id { get; set; }

        /// <summary>
        /// The name of each attribute.
        /// </summary>
        public string original_attribute_name { get; set; }

        /// <summary>
        /// This is to indicate whether this attribute is mandantory.
        /// </summary>
        public Boolean is_mandatory { get; set; }

        public List<ShopeeGetItemBaseInfoItemAttributeValue> attribute_value_list { get; set; }
    }
}
