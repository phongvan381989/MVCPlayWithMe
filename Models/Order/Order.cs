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

        public Address address { get; set; }

        public string note { get; set; }

        public DateTime time { get; set; }


        public Order()
        {
            id = -1;
            lsOrderPay = new List<OrderPay>();
            lsOrderTrack = new List<OrderTrack>();
            lsOrderDetail = new List<OrderDetail>();
            address = new Address();
        }
    }

}