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

        /// <summary>
        /// product is active (1) or inactive
        /// </summary>
        public Boolean bActive { get; set; }

        /// <summary>
        /// Lý do cập nhật số lượng lỗi
        /// </summary>
        public string pwhyUpdateFail { get; set; }

        public List<Mapping> mapping { set; get; }

        public CommonModel()
        {
            mapping = new List<Mapping>();
        }
    }
}