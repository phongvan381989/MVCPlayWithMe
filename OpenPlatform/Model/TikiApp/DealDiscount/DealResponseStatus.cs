using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount
{
    //{
    //    "statusCode": 400,
    //    "message": {
    //        "en": "Start time must be less than end time",
    //        "vi": "Thời gian bắt chạy giảm giá phải nhỏ hơn thời gian kết thúc"
    //    },
    //    "error": "Bad Request"
    //}
    public class DealResponseStatus
    {
        public int statusCode { get; set; }

        public DealMessageEnVn message { get; set; }

        public string error { get; set; }
    }
}