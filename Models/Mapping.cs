using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Mapping
    {
        public int quantity { set; get; }
        public Product product { get; set; }

        public Mapping()
        {

        }

        public Mapping(int id, int quan)
        {
            product = new Product(id);
            quantity = quan;
        }

        public Mapping(Product pro, int quan)
        {
            product = pro;
            quantity = quan;
        }
    }
}