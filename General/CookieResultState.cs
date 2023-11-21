using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    public class CookieResultState
    {
        //public int cooType; // 0: chưa set, 1: khách hàng; 2: người quản trị
        //public Boolean isFirst; // true. Cookie vừa set cho request mới
        public string cookieValue;

        public CookieResultState()
        {
        //    cooType = 0;
        //    isFirst = true;
            cookieValue = string.Empty;
        }
    }
}