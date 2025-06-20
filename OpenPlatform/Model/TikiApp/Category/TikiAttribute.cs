using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category
{
    public class TikiAttribute
    {
        public string code { get; set; }
        public string data_type { get; set; }
        public string default_value { get; set; }
        public string description { get; set; }
        public string description_en { get; set; }
        public string display_name { get; set; }
        public string display_name_en { get; set; }
        public int id { get; set; }
        public string input_type { get; set; }
        public bool is_required { get; set; }
        public int category_id { get; set; }
    }
}