using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    /// <summary>
    /// 0: Tổng tiền hàng
    /// 1: Phí ship
    /// 2: Khuyến mại khác
    /// 3: Tổng thanh toán = Tổng tiền hàng + Phí ship - Khuyến mại khác
    /// </summary>
    public enum EPayType
    {
        /// <summary>
        /// 0
        /// </summary>
        TOTAL_OF_COST_GOODS,

        /// <summary>
        /// 1
        /// </summary>
        SHIP,

        /// <summary>
        /// 2
        /// </summary>
        PROMOTION,

        /// <summary>
        /// 3
        /// </summary>
        SUM
    }

    /// <summary>
    /// Đối tượng thanh toán trong đơn hàng
    /// </summary>
    public class OrderPay
    {
        public int id { get; set; }

        public int orderId { get; set; }

        public int promotionOrderId { get; set; }

        public EPayType type { get; set; }

        public int value { get; set; }
    }
}