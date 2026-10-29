using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Tương ứng dữ liệu tbecommerceorder_return_shop
    /// Theo dõi đơn đã đóng nhưng bị hủy, cần phải xác nhận hoàn hàng.
    /// Không được hoàn hàng nghĩa là thất lạc, mất.
    /// </summary>
    public class ECommerceOrderReturnShop
    {
        /// <summary>
        /// Id tự tăng.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mã đơn hàng.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Đơn hàng thuộc sàn nào?
        /// 0 - Play with me web.
        /// 1 - Tiki.
        /// 2 - Shopee.
        /// 3 - Lazada.
        /// </summary>
        public int ECommmerce { get; set; }

        /// <summary>
        /// Trạng thái hoàn hàng.
        /// 0 - Chưa hoàn hàng về shop.
        /// 1 - Đã hoàn về shop.
        /// 2 - Thất lạc.
        /// </summary>
        public int IsReturn { get; set; }
    }
}
