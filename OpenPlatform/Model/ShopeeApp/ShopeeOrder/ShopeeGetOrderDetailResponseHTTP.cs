using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetOrderDetailResponseHTTP
    {
        /// <summary>
        /// Indicate error type if hit error. Empty if no error happened.
        /// </summary>
        public string error { get; set; }

        /// <summary>
        /// Indicate error details if hit error. Empty if no error happened.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Indicate waring details if hit waring.Empty if no waring happened.
        /// </summary>
        public string warning { get; set; }

        /// <summary>
        /// The identifier for an API request for error tracking
        /// </summary>
        public string request_id { get; set; }

        public ShopeeGetOrderDetailResponse response { get; set; }
    }
}
