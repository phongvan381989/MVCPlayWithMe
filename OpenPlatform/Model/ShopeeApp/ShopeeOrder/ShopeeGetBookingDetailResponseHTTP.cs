using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeGetBookingDetailResponseHTTP : CommonResponseHTTP
    {
        // Detail information you are querying.
        public ShopeeGetBookingDetailResponse response { get; set; }
    }

    public class ShopeeGetBookingDetailResponse
    {
        // The list of bookings.
        public List<ShopeeBookingDetail> booking_list { get; set; }
    }

    public class ShopeeBookingDetail
    {
        // Return by default. Shopee's unique identifier for a booking.
        public string booking_sn { get; set; }

        // Shopee's unique identifier for an order. Only return if booking_status is MATCHED.
        public string order_sn { get; set; }

        // Return by default. The two-digit code representing the region where the booking was made.
        public string region { get; set; }

        // Return by default. Enumerated type that defines the current status of the booking.
        // Available value: READY_TO_SHIP/PROCESSED/SHIPPED/CANCELLED/MATCHED
        public string booking_status { get; set; }

        // MATCH_PENDING/MATCH_SUCCESSFUL/MATCH_FAILED
        public string match_status { get; set; }

        // The logistics service provider that will deliver the booking
        public string shipping_carrier { get; set; }

        // Return by default. Timestamp that indicates the date and time that the booking was created
        public long create_time { get; set; }

        // Return by default. Timestamp that indicates the last time that there was a change in
        // value of booking, such as booking status changed from 'Processed' to 'Shipped'.
        public long update_time { get; set; }

        // Return by default. The deadline to ship out the parcel.
        public long ship_by_date { get; set; }

        // This object contains detailed breakdown for the recipient address.
        public ShopeeGetOrderDetailRecipientAddress recipient_address { get; set; }

        // This object contains the detailed breakdown for the result of this API call.
        public List<ShopeeGetOrderDetailItem> item_list { get; set; }

        // For Indonesia orders only. The name of the dropshipper.
        public string dropshipper { get; set; }

        // The phone number of dropshipper, could be empty.
        public string dropshipper_phone { get; set; }

        /// <summary>
        /// Could be one of buyer, seller, system or Ops.
        /// </summary>
        public string cancel_by { get; set; }

        /// <summary>
        /// Use this field to get reason for buyer, seller, and system cancellation.
        /// </summary>
        public string cancel_reason { get; set; }

        /// <summary>
        /// Use this field to indicate the order is fulfilled by shopee or seller. Applicable values: fulfilled_by_shopee, fulfilled_by_cb_seller, fulfilled_by_local_seller.
        /// </summary>
        public string fulfillment_flag { get; set; }

        /// <summary>
        /// The timestamp when pickup is done.
        /// </summary>
        public long pickup_done_time { get; set; }
    }
}