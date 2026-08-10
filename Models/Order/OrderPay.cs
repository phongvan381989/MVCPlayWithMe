using MVCPlayWithMe.General;
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
    public enum EOrderPayType
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
        static public string[] arrayOrderPayType = {
            "Tổng tiền hàng",
            "Phí vận chuyển",
            "Khuyến mãi",
             "OTHER1",
             "OTHER2",
             "OTHER3",
             "OTHER4",
             "OTHER5",
             "OTHER6",
             "OTHER7",
            "Tổng thanh toán"
        };

        public void SetstrType(EOrderPayType type)
        {
            if(type != EOrderPayType.PROMOTION)
                 strType = arrayOrderPayType[(int)type];
            else
                strType = orderSimplePromotion.StrType;
        }

        public int id { get; set; }

        public int orderId { get; set; }

        public int orderSimplePromoId { get; set; }

        // type = 2 thì mới có orderSimplePromoId, còn lại = null
        public OrderSimplePromotion orderSimplePromotion { get; set; }

        /// 0: Tổng tiền hàng
        /// 1: Phí ship
        /// 2: Khuyến mại khác
        /// 10: Tổng thanh toán = Tổng tiền hàng + Phí ship - Khuyến mại khác
        public EOrderPayType type { get; set; }

        public string strType { get; set; }

        public int value { get; set; }

        //public void SetStrType()
        //{
        //    if (type == EOrderPayType.TOTAL)
        //    {
        //        strType = "Tổng tiền hàng";
        //    }
        //    else if (type == EOrderPayType.SHIP)
        //    {
        //        strType = "Phí vận chuyển";
        //    }
        //    else if (type == EOrderPayType.FINAL)
        //    {
        //        strType = "Tổng thanh toán";
        //    }
        //}
    }
}
