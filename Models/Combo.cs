using System;
using System.Collections.Generic;
using MVCPlayWithMe.Models.ProductModel;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Combo : BasicIdName
    {
        public Combo(int idInput, string nameInput) : base(idInput, nameInput)
        {
            products = new List<Product>();
        }

        public Combo(int idInput, string nameInput, string codeInput, Byte statusInput, string detailInput) : base(idInput, nameInput)
        {
            products = new List<Product>();
            code = codeInput;
            status = statusInput;
            detail = detailInput;
        }

        public string code { get; set; }

        public Byte status { get; set; }

        public string detail { get; set; }

        // danh sách sản phẩm thuộc combo
        public List<Product> products { get; set; }
    }
}
