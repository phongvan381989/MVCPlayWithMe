using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Order
{
    /// <summary>
    /// Đối tượng trạng thái đơn hàng
    /// </summary>
    public class OrderTrack
    {
        static public string[] arrayStatus = {
            "Chưa thanh toán",
            "Chuẩn bị hàng",
            "Chờ giao ĐVVC",
            "Lấy hàng thất bại.",
            "Chờ giao khách",
            "Nhận hàng",
            "Đơn hủy",
            "Đã hủy đơn",
            "Đơn hoàn",
            "Hoàn thành",
            "Tất cả",
        };
        public int id { get; set; }

        public int orderId { get; set; }

        public EOrderStatus status { get; set; }

        public string strStatus { get; set; }

        public DateTime time { get; set; }

        public void SetStrStatus()
        {
            strStatus = arrayStatus[(int)status];
        }

        public static string GetString(int index)
        {
            string str = null;
            if (index == (int)EOrderStatus.UNPAID)
                str = "UNPAID";
            else if (index == (int)EOrderStatus.READY_TO_SHIP)
                str = "READY_TO_SHIP";
            else if (index == (int)EOrderStatus.PROCESSED)
                str = "PROCESSED";
            else if (index == (int)EOrderStatus.RETRY_SHIP)
                str = "RETRY_SHIP";
            else if (index == (int)EOrderStatus.SHIPPED)
                str = "SHIPPED";
            if (index == (int)EOrderStatus.TO_CONFIRM_RECEIVE)
                str = "TO_CONFIRM_RECEIVE";
            else if (index == (int)EOrderStatus.IN_CANCEL)
                str = "IN_CANCEL";
            else if (index == (int)EOrderStatus.CANCELLED)
                str = "CANCELLED";
            else if (index == (int)EOrderStatus.TO_RETURN)
                str = "TO_RETURN";
            else if (index == (int)EOrderStatus.COMPLETED)
                str = "COMPLETED";
            else if (index == (int)EOrderStatus.ALL)
                str = "ALL";
            return str;
        }
    }
}