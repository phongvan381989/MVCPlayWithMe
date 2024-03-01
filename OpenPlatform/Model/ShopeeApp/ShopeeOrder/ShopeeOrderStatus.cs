using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder
{
    public class ShopeeOrderStatus
    {
        // UNPAID:Order is created, buyer has not paid yet.
        // READY_TO_SHIP:Seller can arrange shipment.
        // PROCESSED:Seller has arranged shipment online and got tracking number from 3PL.
        // RETRY_SHIP:3PL pickup parcel fail. Need to re arrange shipment.
        // SHIPPED:The parcel has been drop to 3PL or picked up by 3PL.
        // TO_CONFIRM_RECEIVE:The order has been received by buyer.
        // IN_CANCEL:The order's cancelation is under processing.
        // CANCELLED:The order has been canceled.
        // TO_RETURN:The buyer requested to return the order and order's return is processing.
        // COMPLETED:The order has been completed.
        // ALL
        public enum EnumShopeeOrderStatus
        {
            UNPAID,
            READY_TO_SHIP,
            PROCESSED,// Đây là trạng thái sau khi in đơn
            RETRY_SHIP,
            SHIPPED,
            TO_CONFIRM_RECEIVE,
            IN_CANCEL,
            CANCELLED,
            TO_RETURN,
            COMPLETED,
            ALL
        }

        public ShopeeOrderStatus()
        {
            index = EnumShopeeOrderStatus.ALL;
        }
        public ShopeeOrderStatus(EnumShopeeOrderStatus input)
        {
            index = input;
        }

        public EnumShopeeOrderStatus index;
        public string GetString()
        {
            string str = null;
            if (index == EnumShopeeOrderStatus.UNPAID)
                str = "UNPAID";
            else if (index == EnumShopeeOrderStatus.READY_TO_SHIP)
                str = "READY_TO_SHIP";
            else if (index == EnumShopeeOrderStatus.PROCESSED)
                str = "PROCESSED";
            else if (index == EnumShopeeOrderStatus.RETRY_SHIP)
                str = "RETRY_SHIP";
            else if (index == EnumShopeeOrderStatus.SHIPPED)
                str = "SHIPPED";
            if (index == EnumShopeeOrderStatus.TO_CONFIRM_RECEIVE)
                str = "TO_CONFIRM_RECEIVE";
            else if (index == EnumShopeeOrderStatus.IN_CANCEL)
                str = "IN_CANCEL";
            else if (index == EnumShopeeOrderStatus.CANCELLED)
                str = "CANCELLED";
            else if (index == EnumShopeeOrderStatus.TO_RETURN)
                str = "TO_RETURN";
            else if (index == EnumShopeeOrderStatus.COMPLETED)
                str = "COMPLETED";
            else if (index == EnumShopeeOrderStatus.ALL)
                str = "ALL";
            return str;
        }
    }
}
