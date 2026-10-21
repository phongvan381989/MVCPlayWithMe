using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiUpdateQuantityBatch
    {
        public List<TikiUpdateQuantity> data { get; set; } = new List<TikiUpdateQuantity>();
    }
}
