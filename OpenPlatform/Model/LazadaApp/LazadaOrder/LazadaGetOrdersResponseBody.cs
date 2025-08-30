using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaOrder
{
    public class LazadaGetOrdersResponseBody : CommonLazadaResponseHTTP
    {
        public LazadaOrderData data { get; set; }
    }

    public class LazadaOrderData
    {
        // Displayed in the Head section, this number tells the complete number of all orders 
        // for the current filter set in the database.
        public int countTotal { get; set; }

        // Displayed in the Head section, this number tells the complete number of all orders 
        // for the current filter set in the database(included offset and limit).
        public int count { get; set; }
        public List<LazadaOrder> orders { get; set; }
    }

    public class LazadaOrder
    {
        public enum EnumLazadaOrderStatus
        {
            unpaid,
            pending,
            canceled,
            ready_to_ship,
            delivered,
            returned,
            shipped,
            failed,
            topack,
            toship,
            shipping,
            lost,
            all
        }

        static public string[] lazadaOrderStatusArray = {
            "unpaid",
            "pending",
            "canceled",
            "ready_to_ship",
            "delivered",
            "returned",
            "shipped",
            "failed",
            "topack",
            "toship",
            "shipping",
            "lost",
            "all"
        };

        // An array of unique status of the items in the order. You can find all of the different
        // status codes in the response example
        // Possible values are unpaid, pending, canceled, ready_to_ship, 
        // delivered, returned, shipped , failed, topack,toship,shipping and lost
        public List<string> statuses { get; set; }

        // Represents the order ID
        public long order_number { get; set; }

        // Represents the order ID
        public long order_id { get; set; }

        // Date and time when the order was placed.
        public string created_at { get; set; }

        // buyer note
        public string buyer_note { get; set; }

        // Trường không phải của Lazada, chứa item trong đơn hàng
        public List<LazadaOrderItem> orderItems { get; set; }
    }
}