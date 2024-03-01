using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeUpdateStockFailure
    {
        /// <summary>
        /// ID of model.
        /// </summary>
        public long model_id { get; set; }

        /// <summary>
        /// Reason for failure.
        /// </summary>
        public string failed_reason { get; set; }
    }
}
