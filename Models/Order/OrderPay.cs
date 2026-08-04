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
    /// 10: Tổng thanh toán = Tổng tiền hàng + Phí ship - Khuyến mại khác
    /// </summary>
    public enum EPayType
    {
        /// <summary>
        /// 0
        /// </summary>
        TOTAL,

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
        FINAL
    }

    /// <summary>
    /// Đối tượng thanh toán trong đơn hàng
    /// </summary>
    public class OrderPay
    {
        public int id { get; set; }

        public int orderId { get; set; }

        // type = 2 thì mới có orderSimplePromoId, còn lại = null
        public OrderSimplePromotion orderSimplePromotion { get; set; }

        /// 0: Tổng tiền hàng
        /// 1: Phí ship
        /// 2: Khuyến mại khác
        /// 10: Tổng thanh toán = Tổng tiền hàng + Phí ship - Khuyến mại khác
        public SByte type { get; set; }

        public string strType { get; set; }

        public int value { get; set; }

        public void SetStrType()
        {
            if (type == (SByte)EPayType.TOTAL)
            {
                strType = "Tổng tiền hàng";
            }
            else if (type == (SByte)EPayType.SHIP)
            {
                strType = "Phí vận chuyển";
            }
            else if (type == (SByte)EPayType.FINAL)
            {
                strType = "Tổng thanh toán";
            }
        }
    }
}
