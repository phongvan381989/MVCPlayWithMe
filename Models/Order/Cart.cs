using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Order
{
    public class Cart : PriceQuantity
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

        public Cart()
        {

        }

        //cookieValue: id=123&q=10&real=1
        public Cart(string cookieValue)
        {
            if(string.IsNullOrEmpty(cookieValue))
            {
                id = 0;
                q = 0;
                real = 0;
                itemId = 0;
                return;
            }

            string[] myArray = cookieValue.Split('#');
            id = Common.ConvertStringToInt32((myArray[0].Split('='))[1]);
            q = Common.ConvertStringToInt32((myArray[1].Split('='))[1]);
            real = Common.ConvertStringToInt32((myArray[2].Split('='))[1]);
            itemId = 0;
        }

        public Cart(int inId, int inQ, int inReal)
        {
            id = inId;
            q = inQ;
            real = inReal;
            itemId = 0;
        }

        // Số lượng khác đã chọn, thời gian sau quay lại có thể đã không đủ, cần cập nhật lại
        public void UpdateQ()
        {
            if(q > quantity)
            {
                q = quantity;
            }
        }
    }
}