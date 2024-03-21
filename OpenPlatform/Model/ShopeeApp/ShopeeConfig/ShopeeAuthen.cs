using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeConfig
{
    public class ShopeeAuthen
    {
        public string shopId { get; set; }
        public ShopeeToken shopeeToken { get; set; }
        public string partnerId { get; set; }
        public string partnerKey { get; set; }
        public string code { get; set; }

        public ShopeeAuthen()
        {
            shopeeToken = new ShopeeToken();
        }
    }
}