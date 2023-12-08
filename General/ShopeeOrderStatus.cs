using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    /// 0: UNPAID, 1:  READY_TO_SHIP,
    /// 2: PROCESSED, // Đây là trạng thái sau khi in đơn 3:  SHIPPED, 4:  COMPLETED,
    /// 5: IN_CANCEL, 6:  CANCELLED, 7:  INVOICE_PENDING, 8: ALL
    public enum EShopeeOrderStatus
    {
        UNPAID,
        READY_TO_SHIP,
        PROCESSED,// Đây là trạng thái sau khi in đơn
        SHIPPED,
        COMPLETED,
        IN_CANCEL,
        CANCELLED,
        INVOICE_PENDING,
        ALL
    }
}