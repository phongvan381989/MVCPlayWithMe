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
        public int priceImport { get; set; }
        public int quantity { get; set; }
        public DateTime dateImport { get; set; }

        public Import()
        {
            id = -1;
        }
    }
}