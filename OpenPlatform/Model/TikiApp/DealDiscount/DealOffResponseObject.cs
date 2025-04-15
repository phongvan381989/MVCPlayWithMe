using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.DealDiscount
{
    public class DealOffResponseObject
    {
        public int code { get; set; }
        public int id { get; set; }
        public DealMessageEnVn message { get; set; }
        public string status { get; set; }
        public Boolean success { get; set; }
    }
}