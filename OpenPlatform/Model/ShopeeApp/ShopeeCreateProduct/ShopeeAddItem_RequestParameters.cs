using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.ViewModel.Dev.ShopeeAPI.ShopeeCreateProduct
{
    public  class ShopeeDimension
    {
        // The height of package for this item, the unit is CM.
        public int package_height { get; set; }

        // The length of package for this item, the unit is CM.
        public int package_length { get; set; }

        // The width of package for this item, the unit is CM.
        public int package_width { get; set; }
    }

    public class ShopeeLogisticInfo
    {
        // False Size ID, If specify logistic fee_type is SIZE_SELECTION size_id is required.
        public int size_id { get; set; }

        // Tên hàm phải là ShouldSerialize + PropertyName
        public bool ShouldSerializesize_id()
        {
            //if(size_id <= 0)
            //    return false; // luôn không serialize
            //return true;
            return false;
        }

        // False Shipping fee, Only needed when logistics fee_type = CUSTOM_PRICE.
        public float shipping_fee { get; set; }
        // Tên hàm phải là ShouldSerialize + PropertyName
        public bool ShouldSerializeshipping_fee()
        {
            return false;
        }

        // True Whether channel is enabled for this item
        public Boolean enabled { get; set; }

        // True ID of the channel
        public int logistic_id { get; set; }

        // False Whether cover shipping fee for buyer
        public Boolean is_free { get; set; }

        public ShopeeLogisticInfo()
        {

        }

        public ShopeeLogisticInfo(int inlogistic_id)
        {
            enabled = true;
            is_free = false;
            logistic_id = inlogistic_id;
        }
    }

    public class ShopeeAttributeValue
    {
        // True
        // Value ID. In the following cases, the value id needs to be uploaded as 0,
        // and original_value_name is mandatory, needs to be filled in customized value.
        // (1) AttributeInputType is TEXT_FILED; (2) AttributeInputType is COMBO_BOX or 
        // MULTIPLE_SELECT_COMBO_BOX, and the seller want to fill in a customized value
        public int value_id { get; set; }

        // False
        // Value name. original_value_name from product.get_attributes api. If value id=0, 
        // this field is required. If AttributeType is DATE_TYPE or TIMESTAMP_TYPE,
        // you can upload timestamp(string type) as the original_value_name.
        public string original_value_name { get; set; }

        // False 
        // Unit of attribute value(quantitative attribute only).
        public string value_unit { get; set; }

        public ShopeeAttributeValue()
        {

        }

        public ShopeeAttributeValue(int invalue_id, string inoriginal_value_name, string invalue_unit)
        {
            value_id = invalue_id;
            original_value_name = inoriginal_value_name;
            value_unit = invalue_unit;
        }
    }

    public class ShopeeAttribute
    {
        // True  ID of attribute
        public int attribute_id { get; set; }

        // False
        public List<ShopeeAttributeValue> attribute_value_list { get; set; }

        public ShopeeAttribute()
        {

        }

        public ShopeeAttribute(int inattribute_id)
        {
            attribute_id = inattribute_id;
            attribute_value_list = new List<ShopeeAttributeValue>();
        }
    }

    public  class ShopeeImage
    {
        // True ID of image
        public List<string> image_id_list { get; set; }

        // False
        // Ratio of image,
        // OptionalAllowed ratios :
        //"1:1" (default) 
        //"3:4"
        //only applicable to whitelisted seller.
        public string image_ratio { get; set; }

        public ShopeeImage()
        {

        }

        public ShopeeImage(List<string> images)
        {
            image_id_list = images;
        }
    }

    public class ShopeeProOrder
    {
        // True Whether item is pre order
        public Boolean is_pre_order { get; set; }

        // False The guaranteed days to ship orders. Please get the days_to_ship range 
        // from get_dts_limit api
        public int days_to_ship { get; set; }
    }

    public class ShopeeWholesale
    {
        // True Minimum count of this tier
        public int min_count { get; set; }

        // Maximum count of this tier
        public int max_count { get; set; }

        // True Unit price of this tier
        public float unit_price { get; set; }
    }

    public class ShopeeBrandRequestParameter
    {
        // True Id of brand.
        public long brand_id { get; set; }

        // True Original name of brand( No Brand if not brand).
        public string original_brand_name { get; set; }

        public ShopeeBrandRequestParameter()
        {

        }

        public ShopeeBrandRequestParameter(long inbrand_id, string inoriginal_brand_name)
        {
            brand_id = inbrand_id;
            original_brand_name = inoriginal_brand_name;
        }
    }

    public class ShopeeSellerStock
    {
        // False location id
        public string location_id { get; set; }

        // True stock
        public int stock { get; set; }

        public ShopeeSellerStock()
        {

        }

        public ShopeeSellerStock(int instock)
        {
            stock = instock;
        }
    }

    public class ShopeeFieldListImageInfo
    {
        // Image id.
        public string image_id { get; set; }
    }

    public class ShopeeFieldList
    {
        // Type of extended description field ：values: See Data Definition- description_field_type (text , image).
        public string field_type { get; set; }

        // If field_type is text， text information will be set by this field.
        public string text { get; set; }

        // If field_type is image，image url will be set by this field.
        public ShopeeFieldListImageInfo image_info { get; set; }
    }

    public class ShopeeExtendedDescription
    {
        // Field of extended description.
        public List<ShopeeFieldList> field_list { get; set; }
    }

    public class ShopeeDescriptionInfo
    {
        // If description_type is extended , Description information should be set by this field.
        public ShopeeExtendedDescription extended_description { get; set; }
    }

    public class ShopeeComplaintPolicy
    {
        // Value should be in one of ONE_YEAR TWO_YEARS OVER_TWO_YEARS.
        public string warranty_time { get; set; }

        // Whether to exclude warranty complaints for entrepreneurs.If True means "I exclude warranty complaints for entrepreneur"
        public Boolean exclude_entrepreneur_warranty { get; set; }

        // Address for complaint. Fetch available addresses using v2.logistics.get_address_list, and use address_id returned from it.
        public long complaint_address_id { get; set; }

        // Additional information for warranty claim. Should be less than 1000 characters.
        public string additional_information { get; set; }
    }

    public class ShopeeAddItem_RequestParameters
    {
        // True Item price
        public float original_price { get; set; }

        // True if description_type is normal , Description information should be set by this field.
        public string description { get; set; }

        // True The weight of this item, the unit is KG.
        public float weight { get; set; }

        // True Item name
        public string item_name { get; set; }

        // False Item status, could be UNLIST or NORMAL
        public string item_status { get; set; }

        // False The dimension of this item.
        public ShopeeDimension dimension { get; set; }

        // True Logistic channel setting
        public List<ShopeeLogisticInfo> logistic_info { get; set; }

        // False
        // This field is optional(expect Indonesia) depending on the specific attribute under
        // different categories. Should call shopee.item.GetAttributes to get attribute first.
        // Must contain all all mandatory attribute.
        public List<ShopeeAttribute> attribute_list { get; set; }

        // True ID of category
        public int category_id { get; set; }

        // True Item images
        public ShopeeImage image { get; set; }

        // False Pre order setting
        public ShopeeProOrder pre_order { get; set; }

        // False SASKU tag of item
        public string item_sku { get; set; }

        // False Condition of item, could be USED or NEW
        public string condition { get; set; }

        // False Wholesale setting
        public List<ShopeeWholesale> wholesale { get; set; }

        // False Video upload ID returned from video uploading API.
        // Only accept one video_upload_id.
        public List<string> video_upload_id { get; set; }

        // False
        public ShopeeBrandRequestParameter brand { get; set; }

        // False This field is only applicable for local sellers in Indonesia and Malaysia.
        // Use this field to identify whether a product is a dangerous product.
        // 0 for non-dangerous product and 1 for dangerous product. For more information,
        // please visit the market's respective Seller Education Hub.
        public int item_dangerous { get; set; }
        // Tên hàm phải là ShouldSerialize + PropertyName
        public bool ShouldSerializeitem_dangerous()
        {
            return false;
        }

        // False
        // seller stock（Please notice that stock(including Seller Stock and Shopee Stock) 
        // should be larger than or equal to real-time reserved stock）
        public List<ShopeeSellerStock> seller_stock { get; set; }

        // Nhiều thuộc tính khác không cần thiết thêm sau nếu cần ....
    }
}
