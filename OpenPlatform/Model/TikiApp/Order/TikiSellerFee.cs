using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiSellerFee
    {
        public Int32 id { get; set; }

        public string fee_type_key { get; set; }

        public Int32 status { get; set; }

        public Int32 quantity { get; set; }

        public Double base_amount { get; set; }

        public Double total_amount { get; set; }

        public Double discount_amount { get; set; }

        public Double final_amount { get; set; }
    }
}
