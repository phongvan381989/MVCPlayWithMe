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

        // Vì sản phẩm giá bìa thấp, để đạt % như mong muốn giá bán cần cao hơn cả giá bìa => Không ổn
        // Ta sẽ tính lại giá bán, bán dưới điểm hòa vốn. Ta sẽ chiết khấu 1 con số phần trăm cố định
        public const int constDiscount = 10;
    }
}
