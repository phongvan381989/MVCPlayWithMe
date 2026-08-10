using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Order
{
    /// <summary>
    /// Đối tượng trạng thái đơn hàng
    /// </summary>
    public class OrderTrack
    {
        public int id { get; set; }

        public int orderId { get; set; }

        public EOrderStatus status { get; set; }

        public string strStatus { get; set; }

        public DateTime? time { get; set; }

        public void SetStrStatus()
        {
            strStatus = OrderStatus.arrayOrderStatus[(int)status];
        }

        //public static string GetString(int index)
        //{
        //    string str = null;
        //    if (index == (int)EOrderStatus.PROCESSING)
        //        str = "PROCESSING";
        //    else if (index == (int)EOrderStatus.SHIPPED)
        //        str = "SHIPPED";
        //    if (index == (int)EOrderStatus.TO_CONFIRM_RECEIVE)
        //        str = "TO_CONFIRM_RECEIVE";
        //    else if (index == (int)EOrderStatus.IN_CANCEL)
        //        str = "IN_CANCEL";
        //    else if (index == (int)EOrderStatus.CANCELLED)
        //        str = "CANCELLED";
        //    else if (index == (int)EOrderStatus.TO_RETURN)
        //        str = "TO_RETURN";
        //    else if (index == (int)EOrderStatus.COMPLETED)
        //        str = "COMPLETED";

        //    return str;
        //}
    }
}
