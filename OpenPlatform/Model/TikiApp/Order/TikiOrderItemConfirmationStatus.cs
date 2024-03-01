using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// https://open.tiki.vn/docs/docs/current/guides/tiki-theory-v2/order-item-v2/#order-item-confirmation-status
    /// </summary>
    public class TikiOrderItemConfirmationStatus
    {
        public enum EnumOrderItemConfirmationStatus
        {
            //Statuses    Description
            waiting,// This order item needs availability confirmation
            seller_confirmed,//    This order item is confirmed to be available for fulfillment – enough stock – by seller
                             //For drop shipping orders, this item needs pickup confirmation
            seller_canceled,// This order item is canceled – not enough stock – by seller
            confirmed,//   Internal status
            ready_to_pick,//   This order item is in a drop shipping order, and is ready for picking by Tiki
        }
        static public string[] ArrayStringOrderItemConfirmationStatus =
        {
            "waiting",
            "seller_confirmed",
            "seller_canceled",
            "confirmed",
            "ready_to_pick"
        };
    }
}
