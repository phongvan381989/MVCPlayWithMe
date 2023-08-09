using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Publisher : BasicIdName
    {

        public string detail { get; set; }

        public Publisher(int idInput, string nameInput, string detalInput) : base(idInput, nameInput)
        {
            detail = detalInput;
        }
    }
}