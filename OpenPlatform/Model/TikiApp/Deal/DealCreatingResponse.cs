using MVCPlayWithMe.OpenPlatform.API.TikiAPI.DealDiscount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount
{
    public class DealCreatingResponse
    {
        // Thông tin trạng thái nếu có
        public DealResponseStatus dealResponseStatus { get; set; }

        public List<DealCreatedResponseDetail> ls { get; set; }
    }
}