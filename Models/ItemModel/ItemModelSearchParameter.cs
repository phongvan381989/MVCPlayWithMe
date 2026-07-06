using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ItemModel
{
    public class ItemModelSearchParameter
    {
        public int? publisherId { get; set; }

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
        /// Lọc theo danh mục (CategoryId)
        /// </summary>
        public int? categoryId { get; set; }

        /// <summary>
        /// Lọc theo nhà xuất bản (tìm kiếm chính xác)
        /// </summary>
        public string publishingCompany { get; set; }

        // Index record trả về từ câu truy vấn
        // mặc định = -1; Lấy tất cả record
        public int start { get; set; }

        // Số lượng record trả về từ câu truy vấn
        // Chính là số đối tượng hiển thị trên 1 page khi tìm kiếm
        public int offset { get; set; }

        public ItemModelSearchParameter()
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
