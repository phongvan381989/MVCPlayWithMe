using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ProductModel
{
    // Đối tượng sản phẩm từ 1 dòng dữ liệu csv
    // Tạo mới nhanh, nhiều product từ csv.
    public class ProductFromCsv
    {
        public string comboName { get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }
        public int bookCoverPrice { get; set; }
        public float discount { get; set; }
        public string code { get; set; }
    }
}