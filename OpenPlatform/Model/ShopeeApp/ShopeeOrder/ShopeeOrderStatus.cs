using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeOrderStatus
    {
        // UNPAID:Order is created, buyer has not paid yet.
        // READY_TO_SHIP:Seller can arrange shipment.
        // PROCESSED:Seller has arranged shipment online and got tracking number from 3PL.
        // RETRY_SHIP:3PL pickup parcel fail. Need to re arrange shipment.
        // SHIPPED:The parcel has been drop to 3PL or picked up by 3PL.
        // TO_CONFIRM_RECEIVE:The order has been received by buyer.
        // IN_CANCEL:The order's cancelation is under processing.
        // CANCELLED:The order has been canceled.
        // TO_RETURN:The buyer requested to return the order and order's return is processing.
        // COMPLETED:The order has been completed.
        // ALL
        public enum EnumShopeeOrderStatus
        {
            UNPAID,
            READY_TO_SHIP,
            PROCESSED,// Đây là trạng thái sau khi in đơn
            RETRY_SHIP,
            SHIPPED,
            TO_CONFIRM_RECEIVE,
            IN_CANCEL,
            CANCELLED,
            TO_RETURN,
            COMPLETED,
            ALL
        }

        static public string[] shopeeOrderStatusArray = {
            "UNPAID",
            "READY_TO_SHIP",
            "PROCESSED",// Đây là trạng thái sau khi in đơn
            "RETRY_SHIP",
            "SHIPPED",
            "TO_CONFIRM_RECEIVE",
            "IN_CANCEL",
            "CANCELLED",
            "TO_RETURN",
            "COMPLETED",
            "ALL"
        };


        // READY_TO_SHIP/PROCESSED/SHIPPED/CANCELLED/MATCHED
        public enum EnumShopeeBookingStatus
        {
            READY_TO_SHIP,
            PROCESSED,
            SHIPPED,
            CANCELLED,
            MATCHED,
            ALL
        }

        static public string[] shopeeBookingStatusArray = {
            "READY_TO_SHIP",
            "PROCESSED",
            "SHIPPED",
            "CANCELLED",
            "MATCHED",
            "ALL"
        };
    }
}
