using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaParameterPriceUpdate
    {
        public long itemId { get; set; }
        public long skuId { get; set; }
        public string sellerSku { get; set; }
        public decimal special_price { get; set; } // giá bán
        public decimal price { get; set; } // Giá bìa
    }
}