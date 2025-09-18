using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetBookingListResponseHTTP : CommonResponseHTTP
    {
        // Detail information you are querying.
        public ShopeeGetBookingListResponse response { get; set; }
    }

    public class ShopeeGetBookingListResponse
    {
        // This is to indicate whether the booking list is more than one page.
        // If this value is true, you may want to continue to check next page to retrieve bookings.
        public Boolean more { get; set; }

        public List<ShopeeGetBookingListBaseInfo> booking_list { get; set; }

        // If more is true, you should pass the next_cursor in the next request as cursor.
        // The value of next_cursor will be empty string when more is false.
        public string next_cursor { get; set; }
    }

    public class ShopeeGetBookingListBaseInfo
    {
        // Shopee's unique identifier for a booking.
        public string booking_sn { get; set; }

        // Shopee's unique identifier for an order. Only return if booking_status is MATCHED.
        public string order_sn { get; set; }

        // The booking_status filter for retrieving booking and each one only every request.
        // Available value: READY_TO_SHIP/PROCESSED/SHIPPED/CANCELLED/MATCHED
        public string booking_status { get; set; }
    }
}