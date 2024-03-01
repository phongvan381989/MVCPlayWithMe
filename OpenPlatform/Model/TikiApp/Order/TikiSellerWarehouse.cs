using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// Seller warehouse (also call seller inventory) is a warehouse of seller from which Tiki can pickup products or to which Tiki can return products.
    /// In order to operate their stores, seller needs to register their active warehouses to Tiki.These warehouses will be used when they want to fulfill their orders.
    /// Example:
    /// {
    /// "id": 558,
    /// "seller_id": 871,
    /// "is_primary": false,
    /// "name": "Seller Alice Shop Hà Nội",
    /// "status": 1,
    /// "address": {
    ///     "street": "01 đường số 2",
    ///     "ward": "Xã Đông Xuân",
    ///     "district": "Huyện Sóc Sơn",
    ///     "region": "Hà Nội",
    ///     "country": "Viet Nam"
    /// },
    /// "type": "requisition"
    /// }
    /// 
    /// Warehouse status:
    /// 1 means active
    /// 0 means inactive
    /// Warehouse types:
    /// requisition means this inventory supplies products to Tiki warehouse
    /// withdrawal means this inventory receives returned products from Tiki
    /// </summary>
    public class TikiSellerWarehouse
    {
        /// <summary>
        /// 558	The unique id of the seller inventory
        /// To be used in order confirmation and dropship confirmation
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// 871	The seller who own the inventory
        /// </summary>
        public Int32 seller_id { get; set; }

        /// <summary>
        /// true	Is this the main inventory of the seller
        /// </summary>
        public bool is_primary { get; set; }

        /// <summary>
        /// Alice Shop Hà Nội	The friendly name of the seller inventory
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 1	Is this inventory active or not
        /// </summary>
        public Int32 status { get; set; }

        /// <summary>
        /// N/A	Detail address of the inventory
        /// </summary>
        public TikiSellerAdress address { get; set; }

        /// <summary>
        /// requisition	Type of the inventory
        /// </summary>
        public string type { get; set; }
    }
}
