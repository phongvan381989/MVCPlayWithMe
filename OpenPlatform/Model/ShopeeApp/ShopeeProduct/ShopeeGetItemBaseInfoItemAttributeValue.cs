using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeGetItemBaseInfoItemAttributeValue
    {
        /// <summary>
        /// Unique identifier for value of this item attribute.
        /// </summary>
        public long value_id { get; set; }

        /// <summary>
        /// original_value_name
        /// </summary>
        public string original_value_name { get; set; }

        /// <summary>
        /// Value unit of this item attribute.
        /// </summary>
        public string value_unit { get; set; }
    }
}
