using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    //    {
    //  "product_id": 159772197,
    //  "seller_warehouse": "159368",
    //  "warehouse_quantities": [
    //         {
    //           "warehouse_id": 159368,
    //           "qty_available": 10
    //         }
    //  ]
    //}
    public class TikiUpdateQuantity : TikiUpdate
    {
        //public TikiUpdateQuantity()
        //{
        //    warehouse_quantities = new List<TikiUpdateQuantityWarehouseQuantities>();
        //}

        public TikiUpdateQuantity(int productId, int sellerWarehouseId) : base ( productId)
        {
            warehouse_quantities = new List<TikiUpdateQuantityWarehouseQuantities>();
            warehouse_quantities.Add(new TikiUpdateQuantityWarehouseQuantities(sellerWarehouseId, 0));
            GenerateSellerWarehouse();
        }

        public void GenerateSellerWarehouse()
        {
            seller_warehouse = string.Empty;
            foreach (var e in warehouse_quantities)
            {
                seller_warehouse = seller_warehouse + e.warehouse_id.ToString() + ",";
            }
            // Bỏ "," cuối cùng
            seller_warehouse = seller_warehouse.TrimEnd(',');
        }

        public void UpdateQuantity(int qty)
        {
            //if(warehouse_quantities.Count() == 0)
            warehouse_quantities[0].qty_available = qty;
        }
        //public int product_id { get; set; }

        public string seller_warehouse { get; set; }

        public List<TikiUpdateQuantityWarehouseQuantities> warehouse_quantities { get; set; }
    }
}
