using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaOrder
{
    public class LazadaGetOrderResponseBody : CommonLazadaResponseHTTP
    {
        public LazadaOrder data { get; set; }
    }
}