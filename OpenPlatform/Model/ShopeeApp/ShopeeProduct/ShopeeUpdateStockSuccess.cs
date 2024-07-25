using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdateStockSuccess
    {
        // ID of model.
        public long model_id { get; set; }

        // location id; This field and the stock field are returned in pairs
        public string location_id { get; set; }

        // stock;This field is returned if seller stock is used in the request, and normal stock fields are not returned.
        public long stock { get; set; }
    }
}