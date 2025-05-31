using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Event
{
    public class Event_Payload
    {
        //"payload": {
        //"orderCode": "747364141",
        //"order_code": "747364141",
        //"status": "canceled"
        //},
        //"type": "ORDER_STATUS_UPDATED",
        // Mã đơn hàng (order code)

        //"payload": {
        //    "order_code": "766671826"
        //},
        //"type": "ORDER_CREATED_SUCCESSFULLY",
        public string order_code { get; set; }

        // Mã đơn hàng (viết lại)
        public string orderCode { get; set; }

        // Trạng thái đơn hàng
        public string status { get; set; }


        //"payload": {
        //        "original_sku": "",
        //        "product_id": 71925545,
        //        "quantity_available_from": 167,
        //        "quantity_available_to": 167,
        //        "quantity_from": 0,
        //        "quantity_reserved_from": 0,
        //        "quantity_reserved_to": 2,
        //        "quantity_sellable_from": 167,
        //        "quantity_sellable_to": 165,
        //        "quantity_to": 0,
        //        "warehouse_id": 387453
        //},
        //"type": "PRODUCT_INVENTORY_UPDATED",

        public string original_sku { get; set; }
        public int product_id { get; set; }
        public int quantity_available_from { get; set; }
        public int quantity_available_to { get; set; }
        public int quantity_from { get; set; }
        public int quantity_reserved_from { get; set; }
        public int quantity_reserved_to { get; set; }
        public int quantity_sellable_from { get; set; }
        public int quantity_sellable_to { get; set; }
        public int quantity_to { get; set; }
        public int warehouse_id { get; set; }
    }
}