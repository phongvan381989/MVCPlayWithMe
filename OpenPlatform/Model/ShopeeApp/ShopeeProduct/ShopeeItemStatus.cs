using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct
{
    public class ShopeeItemStatus
    {
        public enum EnumShopeeItemStatus
        {
            NORMAL,
            BANNED,
            DELETED,
            UNLIST
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
            else if (index == EnumShopeeItemStatus.DELETED)
                str = "DELETED";
            else if (index == EnumShopeeItemStatus.UNLIST)
                str = "UNLIST";

            return str;
        }
    }
}
