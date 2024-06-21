using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Import
    {
        public int id { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public int bookCoverPrice { get; set; }
        public int discount { get; set; }
        public string dateImport { get; set; } // định dạng yyyy-MM-dd tên date vi phạm kiểu Date

        public Import()
        {
            id = -1;
            //productId = -1
        }
    }
}