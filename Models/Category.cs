using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Category : BasicIdName
    {
        public Category(int idInput, string nameInput) : base(idInput, nameInput)
        {
        }
    }
}