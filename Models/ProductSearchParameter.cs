using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    // Chứa thông tin phục vụ tìm kiếm sản phẩm trong kho
    public class ProductSearchParameter
    {
        public string codeOrBarcode { get; set; }
        public string name { get; set; }
        public string combo { get; set; }
        public string publisher { get; set; }

        // Trạng thái sản phẩm. 0: Đang kinh doanh bình thường,
        // 1: Nhà phát hành tạm thời hết hàng, 2: Ngừng kinh doanh
        // 10: lấy tất cả các trạng thái
        public int status { get; set; }

        // Index record trả về từ câu truy vấn
        // mặc định = -1; Lấy tất cả record
        public int start { get; set; }

        // Số lượng record trả về từ câu truy vấn
        // Chính là số đối tượng hiển thị trên 1 page khi tìm kiếm
        public int offset { get; set; }

        public ProductSearchParameter()
        {
            codeOrBarcode = string.Empty;
            name = string.Empty;
            combo = string.Empty;
            publisher = string.Empty;
            start = -1;

            status = 0;
        }
    }
}