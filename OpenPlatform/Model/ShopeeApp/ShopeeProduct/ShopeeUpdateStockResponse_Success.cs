using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdateStockResponse_Success
    {
        /// <summary>
        /// ID of model.
        /// </summary>
        public int model_id { get; set; }

        /// <summary>
        /// Stock of this model.
        /// Please use the stock field instead, we will deprecate this field on 2022/10/15.
        /// </summary>
        public int normal_stock { get; set; }

        /// <summary>
        /// location id; This field and the stock field are returned in pairs
        /// </summary>
        public string location_id { get; set; }

        /// <summary>
        /// stock;This field is returned if seller stock is used in the request, and normal stock fields are not returned.
        /// </summary>
        public int stock { get; set; }
    }
}
