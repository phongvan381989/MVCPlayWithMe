using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class DetailItem
    {
        public string code { get; set; }
        public string field { get; set; }
        public long item_id { get; set; }
        public string message { get; set; }
        public string seller_sku { get; set; }
        public long sku_id { get; set; }
    }

    public class LazadaUpdatePriceQuantityReponseBody : CommonLazadaResponseHTTP
    {
        public string _trace_id_ { get; set; }
        public List<DetailItem> detail { get; set; }
    }
}