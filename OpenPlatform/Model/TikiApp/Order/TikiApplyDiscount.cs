using System;
namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiApplyDiscount
    {
        public string rule_id { get; set; }

        public string type { get; set; }

        public Double amount { get; set; }

        public Double seller_sponsor { get; set; }

        public Double tiki_sponsor { get; set; }
    }
}
