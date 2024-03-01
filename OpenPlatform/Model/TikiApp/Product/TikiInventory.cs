using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiInventory
    {
        public string fulfillment_type { get; set; }

        public string inventory_type { get; set; }

        public List<TikiIventoryWarehouseStocks> warehouse_stocks { get; set; }
    }
}
