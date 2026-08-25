using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public class SanPhamSearchParameter
    {
        public static int itemsPerPage = 30;

        /// <summary>
        /// Lọc theo nhà xuất bản (slug: "kim-dong", "tre")
        /// </summary>
        public string publisher { get; set; }

        public string name { get; set; }

        /// <summary>
        /// Lọc theo tác giả (tìm kiếm chính xác)
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// Lọc theo người dịch (tìm kiếm chính xác)
        /// </summary>
        public string translator { get; set; }

        /// <summary>
        /// Lọc theo danh mục (slug: "sach-thieu-nhi", "truyen-tranh")
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// Lọc theo công ty xuất bản (free-form text)
        /// </summary>
        public string publishingCompany { get; set; }

        // Index record trả về từ câu truy vấn
        // mặc định = -1; Lấy tất cả record
        public int start { get; set; }

        // Số lượng record trả về từ câu truy vấn
        // Chính là số đối tượng hiển thị trên 1 page khi tìm kiếm
        public int offset { get; set; }

        public int? lastId;
        public int? limit;
        public int? page;

        public SanPhamSearchParameter()
        {
            name = string.Empty;
            author = string.Empty;
            translator = string.Empty;
            publishingCompany = string.Empty;
            start = 0;
            offset = Common.offset;
        }
    }
}
