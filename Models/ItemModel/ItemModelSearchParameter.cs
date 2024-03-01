using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ItemModel
{
    public class ItemModelSearchParameter
    {
        public string name { get; set; }

        // Index record trả về từ câu truy vấn
        // mặc định = -1; Lấy tất cả record
        public int start { get; set; }

        // Số lượng record trả về từ câu truy vấn
        // Chính là số đối tượng hiển thị trên 1 page khi tìm kiếm
        public int offset { get; set; }

        // 0: Model chưa mapping, 1: Có mapping, 2: Lấy tất
        public int hasMapping { get; set; }

        public ItemModelSearchParameter()
        {
            name = string.Empty;
            start = 0;
            offset = Common.offset;
            hasMapping = 2;
        }
    }
}