using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiIventoryWarehouseStocks
    {
        public int quantity { get; set; }

        public int quantity_available { get; set; }

        public int quantity_reserved { get; set; }

        public int quantity_sellable { get; set; }

        public int warehouse_id { get; set; }
    }
}
