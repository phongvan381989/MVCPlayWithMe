using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order
{
    /// <summary>
    /// Deserialize tới đối tượng của lớp này khi lấy danh sách đơn hàng
    /// </summary>
    public class TikiPageOrders
    {
        public List<TikiOrder> data { get; set; }
        public TikiPagingOrder paging { get; set; }
    }
}
