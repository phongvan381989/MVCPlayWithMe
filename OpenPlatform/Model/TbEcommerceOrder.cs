using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    // Tương ứng dữ liệu tbecommerceorder
    public class TbEcommerceOrder
    {
        /// <summary>
        /// Id tự tăng của đơn hàng.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Mã đơn hàng.
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Mã vận chuyển.
        /// </summary>
        public string shipCode { get; set; }

        /// <summary>
        /// Trạng thái thực tế đã thực hiện.
        /// 0 - PACKED: đã đóng hàng.
        /// 1 - RETURNED: đã hoàn hàng nhập kho.
        /// 2 - BOOKED: Đã trừ số lượng trong kho theo đơn phát sinh trên sàn, nhưng chưa PACKED.
        /// 3 - UNBOOKED: Đã cộng số lượng trong kho khi đơn trên sàn hủy và trạng thái là BOOKED.
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Thời gian insert dữ liệu.
        /// </summary>
        public DateTime? time { get; set; }

        /// <summary>
        /// Đơn hàng thuộc sàn nào?
        /// 0 - Play with me web.
        /// 1 - Tiki.
        /// 2 - Shopee.
        /// 3 - Lazada.
        /// </summary>
        public int eCommerce { get; set; }

        /// <summary>
        /// Timestamp xảy ra thay đổi trạng thái đơn hàng sàn TMDT ghi nhận.
        /// </summary>
        public long updateTime { get; set; }

        /// <summary>
        /// Id duy nhất định danh tin nhắn.
        /// </summary>
        public string msgId { get; set; }
    }
}