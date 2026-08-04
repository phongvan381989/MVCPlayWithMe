using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    // PROCESSED:Seller has arranged shipment online and got tracking number from 3PL.
    // SHIPPED:The parcel has been drop to 3PL or picked up by 3PL.
    // TO_CONFIRM_RECEIVE:The order has been received by buyer.
    // IN_CANCEL:The order's cancelation is under processing.
    // CANCELLED:The order has been canceled.
    // TO_RETURN:The buyer requested to return the order and order's return is processing.
    // COMPLETED:The order has been completed.
    public enum EOrderStatus
    {
        PROCESSED,
        SHIPPED,
        TO_CONFIRM_RECEIVE,
        IN_CANCEL,
        CANCELLED,
        TO_RETURN,
        COMPLETED,
    }

    // Phương thức thanh toán
    public enum EPaymentMethod
    {
        CASH_ON_DELIVERY,
        BANK_TRANSFER
    }

    public enum EOrderFrom
    {
        VOI_BE_NHO,
        FACEBOOK,
        TIKTOK,
        OTHER
    }

    public enum EOrderPayStatus
    {
        PENDING,
        PAID,
        REFUNDED
    }

    public class OrderStatus
    {
        static public string[] arrayOrderStatus = {
            "Shop chuẩn bị hàng",
            "Đã giao ĐVVC",
            "Khách đã nhận hàng",
            "Đơn hủy",
            "Đã hủy đơn",
            "Đơn hoàn",
            "Hoàn thành"
        };
    }
}
