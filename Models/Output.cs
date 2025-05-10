using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    // Tương ứng với tbOutput
    public class Output
    {
        public int id { get; set; }
        public string code { get; set; }
        public int eCommmerce { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public DateTime time { get; set; }

        // true: nếu đơn đã bị hủy
        // dữ liệu này không có trong bảng tbOutput
        public Boolean isCancel { get; set; }
    }
}