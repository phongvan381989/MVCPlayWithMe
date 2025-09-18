using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
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
        // Vì sản phẩm giá bìa thấp, để đạt % như mong muốn giá bán cần cao hơn cả giá bìa => Không ổn
        // Ta sẽ tính lại giá bán, bán dưới điểm hòa vốn. Ta sẽ chiết khấu 1 con số phần trăm cố định
        public const int constDiscount = 1;

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

        // Kiểm tra trạng thái đơn Shopee có phải là Hoả Tốc - Trong Ngày, Siêu Tốc - 4 Giờ
        public static Boolean IsShopeeExpress(string checkout_shipping_carrier)
        {
            if((checkout_shipping_carrier.IndexOf("Hỏa Tốc", StringComparison.OrdinalIgnoreCase) >= 0)||
                (checkout_shipping_carrier.IndexOf("Siêu Tốc", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return true;
            }
            return false;
        }

        // Xây dựng công thức tính giá bán
        // p: giá bìa, 
        // dI: chiết khấu nhập, VD: 0.4
        // dO: chiết khấu bán,
        // x: % lợi nhuận mong muốn so với GIÁ NHẬP SÁCH
        // t: phần trăm phí trả sản + thuế nộp nhà nước so với giá bán trên sàn
        // c: chi phí đóng gói cố đinh
        // m: lợi nhuận tuyệt đối. Nếu giá bìa lớn ta có thể chiết khấu sâu hơn khi bán để lợi nhuận tối thiểu bằng m
        // Giá bán  - giá nhập  - thuế phí   - phí đóng hàng = lợi nhuận còn lại
        // p(1 - dO) - p(1 -dI) - p(1 - dO)t - c             = p(1 - dI) x
        //
        // p(1 - dO)(1 - t) = p(1 -dI) + c + p(1 - dI) x
        // =>p(1 - dO) = (p(1 -dI) + c + p(1 - dI)x) / (1 - t)
        // Làm tròn xuống thành số nguyên
        private static int CaculateSalePriceCore(int p, float dI, float x, float t, int c, int m)
        {
            if (p * (1 - dI) * x > m)
            {
                x = m / (p * (1 - dI));
            }

            int salePrice = (int)Math.Floor((p * (1 - dI) + c + p * (1 - dI) * x) / (1 - t));

            // Làm tròn salePrice là bội của 100 VND
            if (salePrice % 100 != 0)
            {
                salePrice = salePrice - salePrice % 100;
            }

            // Vì sản phẩm giá bìa thấp, để đạt % như mong muốn giá bán cần cao hơn cả giá bìa
            // Ta tính lại giá bán, bán dưới điểm hòa vốn
            if (salePrice >= p)
            {
                salePrice = p * (100 - constDiscount) / 100;
                // Làm tròn salePrice là bội của 100 VND
                if (salePrice % 100 != 0)
                {
                    salePrice = salePrice - salePrice % 100;
                }
            }

            return salePrice;
        }

        // Xây dựng công thức tính giá bán
        // p: giá bìa, 
        // dI: chiết khấu nhập, VD: 0.4
        // dO: chiết khấu bán,
        // x: % lợi nhuận mong muốn so với GIÁ BÁN
        // t: phần trăm phí trả sản + thuế nộp nhà nước so với giá bán trên sàn
        // c: chi phí đóng gói cố đinh
        // NOTE: Bỏ qua lợi nhuận tuyệt đối m
        // m: lợi nhuận tuyệt đối. Nếu giá bìa lớn ta có thể chiết khấu sâu hơn khi bán để lợi nhuận tối thiểu bằng m
        // Giá bán  - giá nhập  - thuế phí   - phí đóng hàng = lợi nhuận còn lại
        // p(1 - dO) - p(1 -dI) - p(1 - dO)t - c             = p(1 - dO) x
        //
        // p(1 - dO)(1 - t -x) = p(1 -dI) + c
        // =>p(1 - dO) = (p(1 -dI) + c) / (1 - t - x)
        // Làm tròn xuống thành số nguyên
        private static int CaculateSalePriceCore_Ver2(int p, float dI, float x, float t, int c, int m)
        {
            //if (p * (1 - dO) * x > m)
            //{
            //    x = m / (p * (1 - dI));
            //}

            int salePrice = 0;
            if (p == 0)
            {
                return salePrice;
            }

            salePrice = (int)Math.Floor((p * (1 - dI) + c) / (1 - t - x));

            // Vì sản phẩm giá bìa thấp, để đạt % như mong muốn giá bán cần cao hơn cả giá bìa
            // Ta tính lại giá bán, bán dưới điểm hòa vốn => Chấp nhận lỗ
            if (salePrice >= p)
            {
                salePrice = p * (100 - constDiscount) / 100;
                // Làm tròn salePrice là bội của 100 VND
                if (salePrice % 100 != 0)
                {
                    salePrice = salePrice - salePrice % 100;
                }
            }

            // Làm tròn salePrice là bội của 100 VND
            if (salePrice % 100 != 0)
            {
                salePrice = salePrice - salePrice % 100;
            }

            return salePrice;
        }


        public static int CaculateSalePriceCoreFromCommonModel(CommonModel commonModel,
        List<Publisher> listPublisher,
        TaxAndFee taxAndFee
        )
        {
            int p = commonModel.GetBookCoverPrice();
            int salePrice = 0;
            if (p == 0)
            {
                return salePrice;
            }

            // Lấy chiết khấu với 1 chữ số sau dấu phảy
            float dI = commonModel.GetDiscount(listPublisher) / 100;

            salePrice = CaculateSalePriceCore_Ver2(p, dI,
                taxAndFee.expectedPercentProfit / 100,
                (taxAndFee.tax + taxAndFee.fee) / 100,
                taxAndFee.packingCost,
                taxAndFee.minProfit);

            return salePrice;
        }

        public static int CaculateSalePriceCoreFromBookCoverPrice(
            int bookCoverPrice,
            float discountOfProduct, // chiết khấu của sản phẩm nếu có, không có thì sẽ là 0
            float discountOfPublisher, // Chiết khấu chung của nhà phát hành
            TaxAndFee taxAndFee
        )
        {
            int p = bookCoverPrice;
            int salePrice = 0;
            if (p == 0)
            {
                return salePrice;
            }

            // Lấy chiết khấu với 1 chữ số sau dấu phảy
            float dI = discountOfProduct < discountOfPublisher? discountOfPublisher: discountOfProduct;
            dI = dI / 100;

            salePrice = CaculateSalePriceCore_Ver2(p, dI,
                taxAndFee.expectedPercentProfit / 100,
                (taxAndFee.tax + taxAndFee.fee) / 100,
                taxAndFee.packingCost,
                taxAndFee.minProfit);

            return salePrice;
        }
    }
}