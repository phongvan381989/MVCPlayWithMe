using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Order
{
    /// <summary>
    /// Lưu thông tin về đối tượng đơn hàng
    /// </summary>
    public class Order
    {
        public int id { get; set; }

        public int customerId { get; set; }

        public List<OrderPay> lsOrderPay { get; set; }

        public List<OrderTrack> lsOrderTrack { get; set; }

        public List<OrderDetail> lsOrderDetail { get; set; }

        // thông tin nhận hàng
        // không dùng class address vì người dùng có thể sửa thông tin này theo thời gian
        public string name { get; set; }
        public string phone { get; set; }
        public string province { get; set; }
        public string subdistrict { get; set; }
        public string detail { get; set; }
        public string code { get; set; }

        // 0: Nếu đơn phát sinh từ web, 1: Tạo thủ công đơn khi mua qua face, tiktok,...
        public int from { get; set; }

        public string note { get; set; }

        public DateTime? time { get; set; }

        // Bank transfer payment support
        public string OrderCode { get; set; }
        public EOrderStatus OrderStatus { get; set; }
        public EOrderPayStatus OrderPayStatus { get; set; }
        public EPaymentMethod PaymentMethod { get; set; }
        public DateTime? PaymentDeadline { get; set; }


        public Order()
        {
            id = -1;
            lsOrderPay = new List<OrderPay>();
            lsOrderTrack = new List<OrderTrack>();
            lsOrderDetail = new List<OrderDetail>();
        }
    }

}
