using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class BasicIdName
    {
        public int id { get; set; }

        public string name { get; set; }

        public BasicIdName(int idInput, string nameInput)
        {
            id = idInput;
            name = nameInput;
        }
    }
}