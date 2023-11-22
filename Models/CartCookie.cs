using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class CartCookie : PriceQuantity
    {
        // item name
        public string itemName { get; set; }

        // model name
        public string modelName { get; set; }

        // item id sản phẩm
        public int itemId { get; set; }

        // model id sản phẩm
        public int id { get; set; }

        // Số lượng đặt mua
        public int q { get; set; }

        // real: 1-thực sự chọn mua, 0-có thể mua sau này
        public int real { get; set; }

        // src ảnh
        public string imageSrc { get; set; }

        //cookieValue: id=123&q=10&real=1
        public CartCookie(string cookieValue)
        {
            if(string.IsNullOrEmpty(cookieValue))
            {
                id = 0;
                q = 0;
                real = 0;
                return;
            }

            string[] myArray = cookieValue.Split('#');
            id = Common.ConvertStringToInt32((myArray[0].Split('='))[1]);
            q = Common.ConvertStringToInt32((myArray[1].Split('='))[1]);
            real = Common.ConvertStringToInt32((myArray[2].Split('='))[1]);
        }
    }
}