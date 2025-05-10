using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ProductModel
{
    // Đối tượng thống kê bán hàng
    public class ProductSellingStatistics
    {
        // id của sản phẩm trong kho
        public int id { get; set; }

        // tên sản phẩm tron kho
        public string name { get; set; }

        // url ảnh đại diện
        public string imageSrc { get; set; }

        // số lượng sản phẩm đã bán
        public int soldQuantity { get; set; }

        // id nhà phát hành
        public int publisherId { get; set; }

        // tồn kho
        public int quantityInWarehouse { get; set; }

        // Số lượng lấy từ nhà phát hành mới nhất
        public int newImportedQuantity { get; set; }

        // id của combo sản phẩm
        public int comboId { get; set; }
    }
}