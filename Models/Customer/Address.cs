using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Customer
{
    // name=Hoàng Huệ#phone=0359127226#province=Hà Nội, Bắc Từ Liêm, Cổ Nhuế 2#detail=Số 24 , Ngõ Việt Hà 2, khu tập thể Việt Hà, tổ dân phố Phú Minh#defaultAdd=1
    public class Address
    {
        // id trong db
        public int id { get; set; }

        public string name { get; set; }

        public string phone { get; set; }

        // Hà Nội
        public string province { get; set; }

        // Bắc Từ Liêm
        public string district { get; set; }

        // Cổ Nhuế 2
        public string subdistrict { get; set; }

        // Số 24 , Ngõ Việt Hà 2, khu tập thể Việt Hà, tổ dân phố Phú Minh
        public string detail { get; set; }

        // defaultAdd:1 địa chỉ nhận hàng mặc định, ngược lại là 0
        public int defaultAdd { get;set;}

        public Address()
        {

        }

        public Address(string cookieValue)
        {
            if (string.IsNullOrEmpty(cookieValue))
            {
                name = "";
                phone = "";
                phone = "";
                province = "";
                district = "";
                subdistrict = "";
                detail = "";
                return;
            }

            string[] myArray = cookieValue.Split('#');
            name = (myArray[0].Split('='))[1];
            phone = (myArray[1].Split('='))[1];
            province = (myArray[2].Split('='))[1];
            district = (myArray[3].Split('='))[1];
            subdistrict = (myArray[4].Split('='))[1];
            detail = (myArray[5].Split('='))[1];

            defaultAdd = Common.ConvertStringToInt32((myArray[6].Split('='))[1]);
        }
    }
}