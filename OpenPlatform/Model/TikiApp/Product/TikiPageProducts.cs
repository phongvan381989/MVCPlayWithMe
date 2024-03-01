using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    /// <summary>
    /// Deserialize tới đối tượng của lớp này khi lấy danh sách sản phẩm
    /// </summary>
    public class TikiPageProducts
    {
        public List<TikiProduct> data { get; set; }

        public TikiPagingOrder paging { get; set; }
    }
}
