using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{

    public enum EOrderStatus
    {
        PROCESSING,
        SHIPPING,
        RECEIVED,
        CANCELLED,
        RETURNED,
        COMPLETED,
    }

    public enum EOrderType
    {
        NORMAL,
        BOOKING
    }

    // Trạng thái hoàn hàng
    public enum EOrderReturnStatus
    {
        NOT_RETURNED,
        RETURNED,
        LOST
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
        ZALO,
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
            "Chuẩn bị hàng",
            "Đang giao",
            "Đã nhận",
            "Đã hủy",
            "Đơn hoàn",
            "Hoàn thành"
        };

        static public string GetOrderStatus(EOrderStatus eStatus)
        {
            return arrayOrderStatus[(int)eStatus];
        }
    }
}
