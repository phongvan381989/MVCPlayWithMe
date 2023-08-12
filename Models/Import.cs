﻿using System;
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
        public int priceImport { get; set; }
        public int quantity { get; set; }
        public int bookCoverPrice { get; set; }
        public string dateImport { get; set; } // định dạng yyyy-MM-dd

        public Import()
        {
            id = -1;
            //productId = -1
        }
    }
}