using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiUpdate
    {
        public int product_id { get; set; }

        public TikiUpdate(int productId)
        {
            product_id = productId;
        }
    }
}
