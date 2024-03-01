using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiMultisellerConfirmation
    {
        public Int32 seller_id { get; set; }

        public bool need_other_sellers_confirm { get; set; }
    }
}
