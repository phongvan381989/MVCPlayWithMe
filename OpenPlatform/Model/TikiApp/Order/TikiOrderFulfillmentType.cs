using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// https://open.tiki.vn/docs/docs/current/guides/tiki-theory-v2/order-v2/#order-fulfillment-type
    /// </summary>
    public class TikiOrderFulfillmentType
    {
        public enum EnumOrderFulfillmentType
        {
            //Fulfillment Type    Description
            tiki_delivery,//   Supplied by either Tiki or Seller
                          //Delivered by Tiki
            seller_delivery,// Supplied by Seller
            //Delivered by Seller
            cross_border,//    Supplied oversea by Seller
            dropship,//    Supplied by Seller
            //Delivered by Tiki directly to Customer
            instant_delivery,//    Supplied by either Tiki or Seller
            //Delivered instantly online: Ebooks, eVoucher, scratch cards…
        }
        static public string[] ArrayStringOrderFulfillmentType =
        {
            "tiki_delivery",
            "seller_delivery",
            "cross_border",
            "dropship",
            "instant_delivery"
        };
    }
}
