using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category
{
    public class TikiCategory
    {
        public string description { get; set; }
        public int id { get; set; }
        public bool is_primary { get; set; }
        public bool is_product_listing_enabled { get; set; }
        public string name { get; set; }
        public bool no_license_seller_enabled { get; set; }
        public int parent_id { get; set; }
    }
}