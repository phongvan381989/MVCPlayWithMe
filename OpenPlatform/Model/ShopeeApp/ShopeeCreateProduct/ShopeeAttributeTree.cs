using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct
{
    // Attribute extra info
    public class ShopeeAttributeInfo
    {
        //SINGLE_DROP_DOWN = 1
        //SINGLE_COMBO_BOX = 2
        //FREE_TEXT_FILED = 3
        //MULTI_DROP_DOWN   = 4
        //MULTI_COMBO_BOX   = 5 
        public int input_type { get; set; }

        //VALIDATOR_NO_VALIDATE_TYPE =  0
        //VALIDATOR_INT_TYPE = 1 
        //VALIDATOR_STRING_TYPE = 2
        //VALIDATOR_FLOAT_TYPE = 3 
        //VALIDATOR_DATE_TYPE = 4
        public int input_validation_type { get; set; }

        //FORMAT_NORMAL = 1
        //FORMAT_QUANTITATIVE_WITH_UNIT = 2
        public int format_type { get; set; }

        //YEAR_MONTH_DATE = 0 (DD/MM/YYYY)
        //YEAR_MONTH = 1 (MM/YYYY)
        public  int date_format_type { get; set; }

        // Attribute's available units list
        public List<string> attribute_unit_list { get; set; }

        //Max selected value count 
        public int max_value_count { get; set; }

        //Introduction for special Attribute
        public string introduction { get; set; }

        public Boolean is_oem { get; set; }

        //Indicates whether this attribute has searchable values.
        //If yes, please call v2.product.search_attribute_value_list to get the default values
        public Boolean support_search_value {get;set;}
    }

    public class ShoppAttributeValue
    {
        public int value_id { get; set; }
        public string name { get; set; }
        public string value_unit { get; set; }

        // Child attributes for the value of parent attribute
        // The structure content is the same as attribute_tree
        public List<object> child_attribute_list { get; set; }
    }

    public class ShopeeMulti_Lang
    {
        //Language
        public string language { get; set; }

        //Translate result
        public string value { get; set; }
    }

    public class ShopeeAtribteTreeCore
    {
        public int attribute_id { get; set; }

        // Is mandatory or not
        public Boolean mandatory { get; set; }

        public string name { get; set; }

        //All available values for this attribute
        public List<ShoppAttributeValue> attribute_value_list { get; set; }

        //Attribute extra info
        public ShopeeAttributeInfo attribute_info { get; set; }

        //Attribute translate info
        public List<ShopeeMulti_Lang> multi_lang { get; set; }
    }
    public class ShopeeAttributeTree
    {
        public List<ShopeeAtribteTreeCore> attribute_tree { get; set; }
        public int category_id { get; set; }
        public string warning { get; set; }
    }
}