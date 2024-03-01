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
        public const string cstrPerPage = "20";
        //public const int intIdKho28Ngo3TTDL = 159368; Id kho hàng 28 ngõ 3, tập thể Đo Lường. Đã tắt kho hàng này
        public const int intIdKho28Ngo3TTDL = 387453;
        public const string cstrHomeAdress = "https://tiki.vn/cua-hang/play-with-me";
    }
}
