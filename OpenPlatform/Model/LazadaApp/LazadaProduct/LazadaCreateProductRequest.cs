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
    }

    public class LazadaProductImageRequest
    {
        public List<string> Image { get; set; }
    }
}