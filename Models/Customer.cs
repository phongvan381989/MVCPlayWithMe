using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Customer
    {
        public int id { get; set; }
        public string email { get; set; }
        public string base64Salt { get; set; }
        public string base64Hash { get; set; }
        public string sdt { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
        public DateTime birthday { get; set; }

        /// <summary>
        /// 1: Nam, 2: Nu, 3: Khac, 4: Chua chon
        /// </summary>
        public int sex { get; set; }

        public Customer()
        {
            id = -1;// default. Ý chưa lấy được
        }
    }
}