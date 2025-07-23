using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform
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
            CANCELLED // Đơn hủy
        }

        // Hàm này lấy sản phẩm mới đăng / mới cập nhật có trạng thái NORMAL trên sàn Tiki, Shopee, Lazada,...
        // trong những ngày gần đây. Kiểm tra nếu sản phẩm chưa tồn tại thì insert vào db tương ứng
        public static void GetNewItemAndInsertIfDontExist(int intervalDay)
        {
            MyLogger.GetInstance().Info("START get item and insert if dont exist");
            try
            {
                // Tiki
                DateTime dtNow = DateTime.Now;
                List<TikiProduct> lsTikiItem = GetListProductTiki.TikiProductGetNormal_ItemList(dtNow.AddDays(intervalDay * -1), dtNow);

                // Shopee
                long update_time_to = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                List<ShopeeItem> lsShopeeItem = ShopeeGetItemList.ShopeeProductGetNormal_ItemList(
                    update_time_to - intervalDay * 24 * 3600,
                    update_time_to);

                List<ShopeeGetItemBaseInfoItem> lsShopeeBaseInfoItem =
                    ShopeeGetItemBaseInfo.ShopeeProductGetListItemBaseInforFromListShopeeItem(lsShopeeItem);

                 ShopeeMySql shopeeSqler = new ShopeeMySql();
                TikiMySql tikiSqler = new TikiMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    foreach (var pro in lsTikiItem)
                    {
                        CommonItem item = new CommonItem(pro);
                        // Không tồn tại ta thêm vào danh sách
                        tikiSqler.TikiInsertIfDontExistConnectOut(item, conn);
                    }

                    foreach (var pro in lsShopeeBaseInfoItem)
                    {
                        CommonItem item = new CommonItem(pro);
                        // Không tồn tại trong DB ta insert
                        shopeeSqler.ShopeeInsertIfDontExistConnectOut(item, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            MyLogger.GetInstance().Info("END get item and insert if dont exist");
        }

        // Kiểm tra trạng thái đơn Shopee có phải là hỏa tốc
        public static Boolean IsShopeeExpress(string checkout_shipping_carrier)
        {
            if((checkout_shipping_carrier.IndexOf("Hỏa Tốc", StringComparison.OrdinalIgnoreCase) >= 0)||
                (checkout_shipping_carrier.IndexOf("Siêu Tốc", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return true;
            }
            return false;
        }
    }
}