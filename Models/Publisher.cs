using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Publisher
    {
        public int id { get; set; }

        public string publisherName { get; set; }

        public string detail { get; set; }

        public Publisher(int idInput, string nameInput, string detalInput)
        {
            id = idInput;
            publisherName = nameInput;
            detail = detalInput;
        }
    }
}