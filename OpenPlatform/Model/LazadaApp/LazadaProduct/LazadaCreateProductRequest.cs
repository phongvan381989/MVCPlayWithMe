using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaCreateProductRequest
    {
        public LazadaCreateProductRequest_Core Request { get; set; }
    }

    public class LazadaCreateProductRequest_Core
    {
        public LazadaProductRequest Product { get; set; }
    }

    public class LazadaProductRequest
    {
        // Tối đa 8 ảnh
        public LazadaProductImageRequest Images { get; set; }
        public LazadaSkusRequest Skus { get; set; }
        public string PrimaryCategory { get; set; }
        public AttributesRequest Attributes { get; set; }
        public LazadaProductSkuVariationRequest variation { get; set; }
    }

    public class LazadaSkusRequest
    {
        public List<LazadaSkuRequest> Sku { get; set; }
    }

    public class LazadaSkuRequest
    {
        // Tối đa 8 ảnh cho mỗi biến thể
        public LazadaProductImageRequest Images { get; set; }
        public string SellerSku { get; set; }
        // public string coming_soon { get; set; }
        // public int delay_delivery_days { get; set; }
        // public string package_content { get; set; }

        // cm
        public string package_height { get; set; }

        // cm
        public string package_length { get; set; }

        // kg
        // Khối lượng kiện hàng nên nằm trong khoảng 0.001 và 300.0
        public string package_weight { get; set; }

        // cm
        public string package_width { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string special_price { get; set; }
        // public string special_from_date { get; set; }
        // public string special_to_date { get; set; }

        //// Trường giá trị của variation, cần thay lại bằng tiếng Việt trước khi gửi về server Lazada
        //public string valueOfVariation { get; set; }
        public LazadaProductSkuSalePro saleProp { get; set; }
    }

    public class AttributesRequest
    {
        public string author { get; set; }
        public string language { get; set; }
        public string short_description { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string name_engravement { get; set; }
        public string warranty_type { get; set; }
        public string gift_wrapping { get; set; }
        public int preorder_days { get; set; }
        public string brand { get; set; }
        public string brand_id { get; set; }
        public string preorder { get; set; }
        public string number_of_pages { get; set; }

        //"options": [{
        //            "en_name": "Unabridged",
        //            "id": 52489,
        //            "name": "Đầy đủ"
        //        }, {
        //            "en_name": "Abridged",
        //            "id": 52499,
        //            "name": "Rút gọn"
        //        }
        //    ]
        public string version { get; set; }

        public string isbn_issn { get; set; }


        // Độ tuổi phù hợp
        public string recommended_age { get; set; }

        public string battery_required { get; set; }

        public string video { get; set; }
    }

    public class LazadaProductImageRequest
    {
        public List<string> Image { get; set; }
    }

    public class LazadaProductSkuVariationRequest
    {
        public LazadaProductSkuVariationCoreRequest Variation1 { get; set; }
    }

    public class LazadaProductSkuVariationCoreRequest
    {
        public Boolean customize { get; set; }
        public Boolean hasImage { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public LazadaProductSkuVariationOptionRequest options { get; set; }
    }

    public class LazadaProductSkuVariationOptionRequest
    {
        public List<string> option { get; set; }
    }

    public class LazadaProductSkuSalePro
    {
        // Trường giá trị của variation, cần thay lại bằng tiếng Việt trước khi gửi về server Lazada
        public string valueOfVariation { get; set; }
    }
}