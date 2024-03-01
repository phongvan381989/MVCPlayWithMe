using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeLogistic
{
    public class ShopeeGetTrackingNumberResponse
    {
        /// <summary>
        /// The tracking number of this order.
        /// </summary>
        public string tracking_number { get; set; }

        /// <summary>
        /// The unique identifier for package of BR correios
        /// </summary>
        public string plp_number { get; set; }

        /// <summary>
        /// The first mile tracking number of the order. Only for Cross Border Seller
        /// </summary>
        public string first_mile_tracking_number { get; set; }

        /// <summary>
        /// The last mile tracking number of the order. Only for Cross Border BR seller
        /// </summary>
        public string last_mile_tracking_number { get; set; }

        /// <summary>
        /// Indicate hint information if cannot get some fields under special scenarios. For example, cannot get tracking_number when cvs store is closed.
        /// </summary>
        public string hint { get; set; }
    }
}
