using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiDiscountCoupon
    {
        public Double seller_discount { get; set; }

        public Double platform_discount { get; set; }

        public Double total_discount { get; set; }
    }
}
