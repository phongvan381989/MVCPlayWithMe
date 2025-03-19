using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeItemStatus
    {
        // NORMAL/BANNED/UNLIST/REVIEWING/SELLER_DELETE/SHOPEE_DELETE
        public enum EnumShopeeItemStatus
        {
            NORMAL,
            BANNED,
            UNLIST,
            REVIEWING,
            SELLER_DELETE,
            SHOPEE_DELETE
        }

        public ShopeeItemStatus()
        {
            index = EnumShopeeItemStatus.NORMAL;
        }
        public ShopeeItemStatus(EnumShopeeItemStatus input)
        {
            index = input;
        }

        public EnumShopeeItemStatus index;
        public string GetString()
        {
            string str = null;
            if (index == EnumShopeeItemStatus.NORMAL)
                str = "NORMAL";
            else if (index == EnumShopeeItemStatus.BANNED)
                str = "BANNED";
            else if (index == EnumShopeeItemStatus.UNLIST)
                str = "UNLIST";
            else if (index == EnumShopeeItemStatus.REVIEWING)
                str = "DELETED";
            else if (index == EnumShopeeItemStatus.SELLER_DELETE)
                str = "DELETED";
            else if (index == EnumShopeeItemStatus.SHOPEE_DELETE)
                str = "DELETED";

            return str;
        }
    }
}
