using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ProductModel
{
    public class ProductIdName : BasicIdName
    {
        public ProductIdName(int idInput, string nameInput) : base(idInput, nameInput)
        {
        }
    }
}