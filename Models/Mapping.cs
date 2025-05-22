using System;
using System.Collections.Generic;
using MVCPlayWithMe.Models.ProductModel;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Mapping
    {
        public int quantity { set; get; }
        public Product product { get; set; }

        // id tương ứng trong bảng
        public int id { get; set; }

        public Mapping()
        {
            id = -1;
        }

        public Mapping(int proId, int quan)
        {
            id = -1;
            product = new Product(proId);
            quantity = quan;
        }

        public Mapping(Product pro, int quan)
        {
            id = -1;
            product = pro;
            quantity = quan;
        }
    }
}