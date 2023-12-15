using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{

    /// <summary>
     /// status: Lấy theo shopee status:0: UNPAID, 1:  READY_TO_SHIP,
    /// 2: PROCESSED, // Đây là trạng thái sau khi in đơn 3:  SHIPPED, 4:  COMPLETED,
    /// 5: IN_CANCEL, 6:  CANCELLED, 7:  INVOICE_PENDING, 8: ALL
    /// </summary>
    public enum EOrderStaus
    {
        /// <summary>
        /// 0
        /// </summary>
        UNPAID,

        /// <summary>
        /// 1
        /// </summary>
        READY_TO_SHIP,

        /// <summary>
        /// 2
        /// </summary>
        PROCESSED,

        /// <summary>
        /// 3
        /// </summary>
        SHIPPED,

        /// <summary>
        /// 4
        /// </summary>
        COMPLETED,

        /// <summary>
        /// 5
        /// </summary>
        IN_CANCEL,

        /// <summary>
        /// 6
        /// </summary>
        CANCELLED,

        /// <summary>
        /// 7
        /// </summary>
        INVOICE_PENDING,

        /// <summary>
        /// 8
        /// </summary>
        ALL
    }

    /// <summary>
    /// Đối tượng trạng thái đơn hàng
    /// </summary>
    public class OrderTrack
    {
        public int id { get; set; }

        public int orderId { get; set; }

        public EOrderStaus status { get; set; }

        public DateTime time { get; set; }

    }
}