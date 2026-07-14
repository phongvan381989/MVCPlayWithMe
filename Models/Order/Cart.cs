using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.SanPhamModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Order
{
    public class Cart
    {
        public int id { get; set; }

        public int customerId { get; set; }

        public int sanPhamId { get; set; }

        public int quantity { get; set; }

        // real: 1-thực sự chọn mua, 0-có thể mua sau này
        public int real { get; set; }

        /// <summary>
        /// Thời gian add/update item (Unix timestamp từ JS → DateTime)
        /// </summary>
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? time { get; set; }

        public SanPhamBasicInfo sanPhamBasicInfo { get; set; }

        public Cart()
        {

        }

        ////cookieValue: id=123&q=10&real=1
        //public Cart(string cookieValue)
        //{
        //    if(string.IsNullOrEmpty(cookieValue))
        //    {
        //        id = 0;
        //        q = 0;
        //        real = 0;
        //        itemId = 0;
        //        return;
        //    }

        //    string[] myArray = cookieValue.Split('#');
        //    id = Common.ConvertStringToInt32((myArray[0].Split('='))[1]);
        //    q = Common.ConvertStringToInt32((myArray[1].Split('='))[1]);
        //    real = Common.ConvertStringToInt32((myArray[2].Split('='))[1]);
        //    itemId = 0;
        //}

        //public Cart(int inId, int inQ, int inReal)
        //{
        //    id = inId;
        //    q = inQ;
        //    real = inReal;
        //    itemId = 0;
        //}

        //// Số lượng khác đã chọn, thời gian sau quay lại có thể đã không đủ, cần cập nhật lại
        //public void UpdateQ()
        //{
        //    if(q > quantity)
        //    {
        //        q = quantity;
        //    }
        //}
    }
}
