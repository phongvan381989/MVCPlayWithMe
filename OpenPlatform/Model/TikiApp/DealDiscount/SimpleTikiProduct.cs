using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal
{
    public class SimpleTikiProduct
    {
        public int id { get; set; }
        public string imageSrc { get; set; }
        public string name { get; set; }
        public string sku { get; set; }

        // Sẽ chỉ tới models của CommonItem
        public List<CommonModel> models { get; set; }

        //public SimpleTikiProduct()
        //{
        //    models = new List<CommonModel>();
        //}
    }

}