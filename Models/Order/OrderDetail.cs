using MVCPlayWithMe.General;
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

        public int sanPhamId { get; set; }

        public string name { get; set; }

        // Ảnh bìa từ tb_san_pham_media (DisplayOrder = nhỏ nhất) nếu đơn từ web voi bé nhỏ
        // url tới ảnh đầu tiên của sản phẩm trong tbProducts, ví dụ: /Media/SanPham/578/0.jpg
        public string CoverImageFileName { get; set; }

        public int quantity { get; set; }

        public int bookCoverPrice { get; set; }

        public int price { get; set; }
    }
}
