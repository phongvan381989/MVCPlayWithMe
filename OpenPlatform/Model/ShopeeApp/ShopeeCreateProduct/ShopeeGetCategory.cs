using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct
{
    public class ShopeeGetCategory
    {
        public long category_id { get; set; }
        public long parent_category_id { get; set; }
        public string original_category_name { get; set; }
        public string display_category_name { get; set; }
        public Boolean has_children { get; set; }
    }
}