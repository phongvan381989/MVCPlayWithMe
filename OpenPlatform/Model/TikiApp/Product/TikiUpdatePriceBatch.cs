using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiUpdatePriceBatch
    {
        public List<TikiUpdatePrice> data { get; set; } = new List<TikiUpdatePrice>();
    }
}
