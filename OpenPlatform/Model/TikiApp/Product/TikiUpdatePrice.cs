using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    //    {
    //  "original_sku" : "xxx-yyy-123",
    //  "product_id": 2166152,
    //  "price": 100000,
    //  "active": 1,
    //  "seller_warehouse": "1034, 1015",
    //  "warehouse_quantities": [
    //         {
    //           "warehouse_id": 1034,
    //           "qty_available": 17
    //         },
    //         {
    //           "warehouse_id": 1015,
    //            "qty_available": 18
    //         }
    //  ]
    //}
    public class TikiUpdatePrice : TikiUpdate
    {
        public int price { get; set; }
        public TikiUpdatePrice(int productId ) : base(productId)
        {

        }

        public void UpdatePrice(int intPrice)
        {
            price = intPrice;
        }
    }
}
