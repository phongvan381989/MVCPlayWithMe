using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiUpdateQuantityWarehouseQuantities
    {
        public TikiUpdateQuantityWarehouseQuantities(int id, int quantity)
        {
            warehouse_id = id;
            qty_available = quantity;
        }
        public int warehouse_id { get; set; }

        public int qty_available { get; set; }
    }
}
