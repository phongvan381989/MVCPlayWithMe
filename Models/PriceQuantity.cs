using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    // Giá bìa, giá thực tế nhập, số lượng tồn kho, số lượng đã bán, % chiết khấu...
    public class PriceQuantity
    {
        /// <summary>
        /// Giá trên bao bì
        /// </summary>
        public int bookCoverPrice { get; set; }

        /// <summary>
        /// Giá bán thực tế, chưa bao gồm các khuyến mại
        /// </summary>
        public int price { get; set; }

        /// <summary>
        /// Số lượng sản phẩm đã được bán
        /// </summary>
        public int soldQuantity { get; set; }

        /// <summary>
        /// Số lượng sản phẩm khách có thể mua
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// % giảm giá so với giá bìa.
        /// % này được xét cho từng model, không phải check theo từng chương trình khuyến mãi
        /// Chương trình khuyến mãi ví dụ: Miễn phí vận chuyển,
        /// hỗ trợ phí vận chuyển, giảm % hoặc tiền khi tổng giá trị đơn hàng lớn hơn ngưỡng,
        /// giảm % hoặc tiền cho khách thân quen, đơn sau,....
        /// </summary>
        public int discount { get; set; }

        public PriceQuantity()
        {

        }

        public PriceQuantity(
            int isoldQuantity,
            int iBookCoverPrice,
            int iPrice
            )
        {
            soldQuantity = isoldQuantity;
            bookCoverPrice = iBookCoverPrice;
            price = iPrice;
        }

        public void Copy(PriceQuantity from)
        {
            this.bookCoverPrice = from.bookCoverPrice;
            this.price = from.price;
            this.soldQuantity = from.soldQuantity;
            this.quantity = from.quantity;
            this.discount = from.discount;
        }
    }
}