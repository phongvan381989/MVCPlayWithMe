using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Tương ứng dữ liệu tbecommerceorder_check_invoice
    /// Đơn có sự kiện hoàn hàng, hoàn một phần, theo dõi để thay đổi hóa đơn.
    /// </summary>
    public class ECommerceOrderCheckInvoice
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
        /// Trạng thái kiểm tra hóa đơn.
        /// 0 - Chưa xong, cần kiểm tra.
        /// 1 - Xong.
        /// </summary>
        public int IsDone { get; set; }

        /// <summary>
        /// Thời gian cập nhật (tự động sinh khi insert).
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}
