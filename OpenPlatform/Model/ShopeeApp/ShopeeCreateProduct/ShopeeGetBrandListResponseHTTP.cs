using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct
{
    public class ShopeeBrandObject
    {
        // Original name of brand
        public string original_brand_name { get; set; }

        public long brand_id { get; set; }

        public string display_brand_name { get; set; }

        public ShopeeBrandObject()
        {

        }

        public ShopeeBrandObject(long inbrand_id, string inoriginal_brand_name)
        {
            brand_id = inbrand_id;
            original_brand_name = inoriginal_brand_name;
        }
    }

    public class ShopeeGetBrandListResponse
    {
        public List<ShopeeBrandObject> brand_list { get; set; }
        // This is to indicate whether the item list is more than one page. If this value is true,
        // you may want to continue to check next page to retrieve the rest of items.
        public Boolean has_next_page { get; set; }

        // If has_next_page is true, this value need set to next request.offset
        public long next_offset { get; set; }

        // Whether is mandatory.
        public Boolean is_mandatory { get; set; }

        // Input type: DROP_DOWN
        public string input_type { get; set; }
    }

    public class ShopeeGetBrandListResponseHTTP : CommonResponseHTTP
    {
        public ShopeeGetBrandListResponse response { get; set; }
    }
}