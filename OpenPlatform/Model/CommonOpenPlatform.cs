using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class CommonOpenPlatform
    {
        // Theo SHOPEE: Enumerated type that defines the current status of the item. Applicable values:
        // NORMAL, BANNED, UNLIST, SELLER_DELETE, SHOPEE_DELETE, REVIEWING.
        public enum ShopeeProductStatus
        {
            NORMAL,
            BANNED,
            UNLIST,
            SELLER_DELETE,
            SHOPEE_DELETE,
            REVIEWING
        }

        public static string[] ShopeeProductStatusArray =
        {
            "NORMAL",
            "BANNED",
            "UNLIST",
            "SELLER_DELETE",
            "SHOPEE_DELETE",
            "REVIEWING"
        };

        public static int ShopeeGetEnumValueFromString(string status)
        {
            // Kiểm tra chuỗi có thể ánh xạ thành enum hay không
            if (Enum.TryParse<ShopeeProductStatus>(status, out var result))
            {
                return (int)result; // Trả về giá trị int của enum
            }

            return (int)ShopeeProductStatus.BANNED;
        }

        public enum CommonOrderStatus
        {
            ALL, // Tất cả trạng thái đơn hàng
            READY_TO_SHIP_PROCESSED, // Đơn hàng nhà bán cần gửi cho bên vận chuyển
            CANCELLED // Đơn đã h
        }
    }
}