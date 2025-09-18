using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification
{
    // Get notified immediately on all booking status updates.
    // This includes booking cancellations that occur before shipping, so that you can take the necessary steps in time.
    public class ShopeeBookingSatusPush : ShopeeBaseNotification
    {
        // Main Push message data
        public ShopeeBookingSatusPushData data { get; set; }
    }

    public class ShopeeBookingSatusPushData
    {
        // Return by default. Shopee's unique identifier for a booking.
        public string booking_sn { get; set; }

        // Return by default. Enumerated type that defines the current status
        // of the booking.
        // READY_TO_SHIP/PROCESSED/SHIPPED/CANCELLED/MATCHED
        public string booking_status { get; set; }

        // Return by default. Timestamp that indicates the last time that there was a
        // change in value of booking, such as booking status changed from 
        // 'Processed' to 'Shipped'.
        public long update_time { get; set; }
    }
}