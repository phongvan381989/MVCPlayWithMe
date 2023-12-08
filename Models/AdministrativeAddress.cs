using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    // Lưu thông tin địa giới hành chính đến cấp phường/xã
    public class AdministrativeAddress
    {
        public string province { get; set; }

        public List<string> districts { get; set; }

        public List<List<string>> subdistricts { get; set; }

        public AdministrativeAddress()
        {
            province = "";
            districts = new List<string>();
            subdistricts = new List<List<string>>();
        }

        public AdministrativeAddress(string pro, string dis, string subdis)
        {
            province = pro;
            districts = new List<string>();
            districts.Add(dis);
            subdistricts = new List<List<string>>();
            List<string> lsubdis = new List<string>();
            lsubdis.Add(subdis);
            subdistricts.Add(lsubdis);
        }
    }
}