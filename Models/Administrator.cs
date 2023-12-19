using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Administrator
    {
        public int id { get; set; }
        public string email { get; set; }
        //public string base64Salt { get; set; }
        //public string base64Hash { get; set; }
        public string sdt { get; set; }
        public string userName { get; set; }
        public int privilege { get; set; }

        public Administrator()
        {
            id = -1;// default. Ý chưa lấy được
        }
    }
}