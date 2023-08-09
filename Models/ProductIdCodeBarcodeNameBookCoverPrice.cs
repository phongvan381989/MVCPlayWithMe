using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    /// <summary>
    /// Chỉ chứ các trường id, code, barcode và Name
    /// </summary>
    public class ProductIdCodeBarcodeNameBookCoverPrice
    {
        public int id { get; set; }
        public string code { get; set; }
        public string barcode { get; set; }
        public string name { get; set; }
        public int bookCoverPrice { get; set; }
        public ProductIdCodeBarcodeNameBookCoverPrice()
        {
            id = -1;
            code = string.Empty;
            barcode = string.Empty;
            name = string.Empty;
            bookCoverPrice = 0;
        }
    }
}