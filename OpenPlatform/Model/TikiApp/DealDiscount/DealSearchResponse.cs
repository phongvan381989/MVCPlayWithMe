using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount
{
    public class DealSearchResponse
    {
        public TikiPagingOrder page { get; set; }

        public Boolean use_es { get; set; }

        public List<DealCreatedResponseDetail> data { get; set; }
    }
}