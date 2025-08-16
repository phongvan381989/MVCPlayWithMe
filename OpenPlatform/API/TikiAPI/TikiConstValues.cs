using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI
{
    /// <summary>
    /// Chứa những giá trị hardcode dùng chung
    /// </summary>
    public class TikiConstValues
    {
        public const string cstrAuthenHTTPAddress = "https://api.tiki.vn/sc/oauth2/token";
        public const string cstrOrdersHTTPAddress = "https://api.tiki.vn/integration/v2/orders";
        public const string cstrProductsHTTPAddress = "https://api.tiki.vn/integration/v2.1/products";
        public const string cstrProductUpdate = "https://api.tiki.vn/integration/v2.1/products/updateSku";

        // limit must not be greater than 100, nhưng chỗ khác thực tế lại là 50, ảo ma canada
        public const string cstrPerPage = "50";
        //public const int intIdKho28Ngo3TTDL = 159368; Id kho hàng 28 ngõ 3, tập thể Đo Lường. Đã tắt kho hàng này
        public const int intIdKho28Ngo3TTDL = 387453;
        public const string cstrHomeAdress = "https://tiki.vn/cua-hang/play-with-me";

        public const string cstrPullEvent = "https://api.tiki.vn/integration/v1/queues/fe4d3d23-9a2a-4e7e-8fc3-9016e18c78f5/events/pull";

        public const string cstrCreateDeal = "https://api-sellercenter.tiki.vn/campdeal/openapi/v1/deals";
        public const string cstrSearchDeal = "https://api-sellercenter.tiki.vn/campdeal/openapi/v1/deals";
        public const string cstrOffDeal = "https://api-sellercenter.tiki.vn/campdeal/openapi/v1/deals/off-many";
        public const string cstrCreatedBy = "HUEHOANG1293@GMAIL.COM";
        public const string cstrCategory = "https://api.tiki.vn/integration/v2/categories";
        public const string cstrCreateProduct = "https://api.tiki.vn/integration/v2.1/requests";
        public const string cstrTrackingRequestCreateProduct = "https://api.tiki.vn/integration/v2/tracking/";

        public const string inventory_type = "dropship";

        public static string GenerateRandomSKUString()
        {
            string prefix = "VBN"; // Phần bắt đầu của SKU

            // Thời gian hiện tại theo định dạng yyyyMMddHHmmss
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            int randomLength = 5; // Độ dài phần ngẫu nhiên

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] result = new char[randomLength];

            for (int i = 0; i < randomLength; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            string sku = prefix + timestamp + new string(result);
            return sku;
        }

        public static long GenerateRandomMincodeLong()
        {
            // Bước 1: Lấy thời gian hiện tại theo định dạng yymmddHHmmss
            string timestamp = DateTime.Now.ToString("yyMMddHHmmss");

            // Bước 2: Sinh số ngẫu nhiên 5 chữ số
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999); // Sinh số ngẫu nhiên từ 10000 đến 99999

            // Bước 3: Kết hợp thời gian và số ngẫu nhiên
            string randomCode = $"{timestamp}{randomNumber}";

            // Bước 4: Chuyển sang kiểu long
            long finalCode = long.Parse(randomCode);

            return finalCode;
        }
    }
}
