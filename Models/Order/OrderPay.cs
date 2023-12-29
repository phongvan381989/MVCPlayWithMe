using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Order
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

        OTHER1,
        OTHER2,
        OTHER3,
        OTHER4,
        OTHER5,
        OTHER6,
        OTHER7,

        /// <summary>
        /// 10
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

        public string strType { get; set; }

        public int value { get; set; }

        public void SetStrType()
        {
            if(type == EPayType.TOTAL_OF_COST_GOODS)
            {
                strType = "Tổng tiền hàng";
            }
            else if(type == EPayType.SHIP)
            {
                strType = "Phí vận chuyển";
            }
            else if( type == EPayType.SUM)
            {
                strType = "Tổng thanh toán";
            }
        }
    }
}