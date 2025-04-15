using MVCPlayWithMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class CommonModel
    {
        /// <summary>
        /// Shopee: Không có phân loại mặc định -1, nếu có phân loại là id sản phẩm trên shop TMDT
        /// Tiki: giá trị mặc định là -1
        /// </summary>
        public long modelId { get; set; }

        // model id tương ứng trong tbShopeeModelId
        public int dbModelId { get; set; }

        public string name { get; set; }

        /// <summary>
        /// Đường dẫn chứa ảnh đại diện
        /// </summary>
        public string imageSrc { get; set; }

        ///// <summary>
        ///// Số lượng sản phẩm trong đơn hàng
        ///// </summary>
        //public int amount { get; set; }

        /// <summary>
        /// the sell price of a product
        /// </summary>
        public int price { get; set; }

        /// <summary>
        /// the price before discount of a product
        /// </summary>
        public int market_price { get; set; }

        public int quantity_sellable { get; set; }

        // Nếu sản phẩm là shopee, đây là id sản phẩm trên web voibenho đã được sinh ra nếu có
        public int pWMMappingModelId { get; set; }

        /// <summary>
        /// product is active (1) or inactive
        /// </summary>
        public Boolean bActive { get; set; }

        /// <summary>
        /// Lý do cập nhật số lượng lỗi
        /// </summary>
        public string whyUpdateFail { get; set; }

        public List<Mapping> mapping { set; get; }

        public CommonModel()
        {
            mapping = new List<Mapping>();
        }

        // Từ list mapping tính được tồn kho sản phẩm
        public int GetQuatityFromListMapping()
        {
            int qty = Int32.MaxValue;
            if (mapping.Count == 0)
                qty = 0;
            foreach (var m in mapping)
            {
                if (qty > m.product.quantity / m.quantity)
                    qty = m.product.quantity / m.quantity;
            }
            if (qty < 0)
                qty = 0;

            return qty;
        }

        // Từ list mapping tính được giá bìa
        public int GetBookCoverPrice()
        {
            int p = 0;
            foreach (var map in mapping)
            {
                p = p + map.quantity * map.product.bookCoverPrice;
            }
            return p;
        }

        // Tính chiết khấu khi nhập hàng.
        // Tính theo chiết khấu sản phẩm nếu có (lớn hơn chiết khấu của nhà phát hành), ngược lại tính
        // theo chiết khấu chung của nhà phát hành.
        // Vì có thể có nhiều giá trị chiết khấu ta tính chiết khấu trung bình theo công thức:
        // lấy tổng giá thực nhập chia cho tổng giá bìa
        // Giá trị trả về theo %, lấy sau dấu ',' một chữ số: VD: 40%, 50.5%
        public float GetDiscount(List<Publisher> listPublisher)
        {
            int bookPriceSum = 0;
            float dIPercent = 0;

            for (int i = 0; i < mapping.Count; i++)
            {
                dIPercent = 0;
                for (int j = 0; j < listPublisher.Count; j++)
                {
                    // Lấy discount của nhà phát hành
                    if (mapping[i].product.publisherId == listPublisher[j].id)
                    {
                        dIPercent = listPublisher[j].discount;
                        break;
                    }
                }

                // So sánh với discount của sản phẩm, chọn giá trị lớn nhất
                if (mapping[i].product.discount > dIPercent)
                {
                    dIPercent = mapping[i].product.discount;
                }
                bookPriceSum = (int)(bookPriceSum + mapping[i].product.bookCoverPrice *
                    mapping[i].quantity * dIPercent / 100);
            }

            // Lấy chiết khấu với 1 chữ số sau dấu phảy
           dIPercent = (1000 * bookPriceSum / GetBookCoverPrice()) / (float)10;

            return dIPercent;
        }
    }
}