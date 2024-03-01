using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    public class TikiOrderItemFilterByDate
    {
        public enum EnumOrderItemFilterByDate
        {
            today,
            last7days,
            last30days
        }
        static public string[] ArrayStringOrderItemFilterByDate =
        {
            "today",
            "last7days",
            "last30days"
        };
    }
}
