using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// For operation models where product items are delivered to customer via Tiki warehouse,
    ///    these product items will first be transfered to Tiki warehouse.How these product items are
    ///    transfered to Tiki warehouse depends on the operation model and stated in pickup_method field.
    ///    And this process is described by this inventory requisition object.
    /// </summary>
    public class TikiInventoryRequisition
    {
        /// <summary>
        /// 14933766	Unique Id of the inventory requisition.
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        /// HN4/BOP/20/08/253202	The unique code on the label that describes the package of products delivered to Tiki warehouse
        /// See PO Label for how to print the label
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 2389	Id of the seller warehouse (aka. seller inventory)
        /// </summary>
        public Int32 seller_inventory_id { get; set; }
        /// <summary>
        /// done	Status of this process
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// false	If the PO Label is printed
        /// </summary>
        public bool is_printed { get; set; }
        /// <summary>
        /// cross_border	How product items are transfered to Tiki warehouse
        /// </summary>
        public string pickup_method { get; set; }
        /// <summary>
        /// hn4	Tiki warehouse code
        /// </summary>
        public string warehouse_code { get; set; }
        /// <summary>
        /// null	The error that happens during the process
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// 2020-08-10 18:50:18	When the this process starts
        /// </summary>
        public DateTime created_at { get; set; }
    }
}
