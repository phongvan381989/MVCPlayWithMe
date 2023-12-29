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
        static string[] arrayStatus = {
            "Chưa thanh toán",
            "Chờ đóng hàng",
            "Chờ lấy hàng",
            "Lấy hàng thất bại",
            "Chờ giao hàng",
            "Nhận hàng",
            "Đơn hủy",
            "Đã hủy đơn",
            "Đơn hoàn",
            "Hoàn thành",
            "Tất cả",
        };
        public int id { get; set; }

        public int orderId { get; set; }

        public EShopeeOrderStatus status { get; set; }

        public string strStatus { get; set; }

        public DateTime time { get; set; }

        public void SetStrStatus()
        {
            strStatus = arrayStatus[(int)status];
        }
    }
}