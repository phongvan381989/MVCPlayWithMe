using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Order
{
    /// <summary>
    /// Đối tượng sản phẩm trong đơn hàng
    /// </summary>
    public class OrderDetail
    {
        public int id { get; set; }

        public int orderId { get; set; }

        public int modelId { get; set; }

        public int quantity { get; set; }

        public int bookCoverPrice { get; set; }

        public int price { get; set; }
    }
}