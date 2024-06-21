using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ItemModel
{
    public class ItemModelSearchParameter
    {
        public int publisherId { get; set; }

        public string name { get; set; }

        // Index record trả về từ câu truy vấn
        // mặc định = -1; Lấy tất cả record
        public int start { get; set; }

        // Số lượng record trả về từ câu truy vấn
        // Chính là số đối tượng hiển thị trên 1 page khi tìm kiếm
        public int offset { get; set; }

        public ItemModelSearchParameter()
        {
            name = string.Empty;
            start = 0;
            offset = Common.offset;
        }
    }
}