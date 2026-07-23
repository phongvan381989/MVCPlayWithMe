using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    // Lưu thông tin địa giới hành chính đến cấp phường/xã
    public class AdministrativeAddress
    {
        public int provinceId { get; set; }

        public string province { get; set; }

        public List<string> subdistricts { get; set; }

        public AdministrativeAddress()
        {
            province = "";
            subdistricts = new List<string>();
        }

        public AdministrativeAddress(int id, string pro, string subdis)
        {
            provinceId = id;
            province = pro;
            subdistricts = new List<string>();
            subdistricts.Add(subdis);
        }
    }
}
