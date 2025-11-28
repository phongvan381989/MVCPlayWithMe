using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeLogistic
{
    public class ShopeeGetBookingTrackingNumberResponseHTTP : CommonResponseHTTP
    {
        public ShopeeGetBookingTrackingNumberResponse response { get; set; }
    }

    public class ShopeeGetBookingTrackingNumberResponse
    {
        public string tracking_number { get; set; }
    }
}