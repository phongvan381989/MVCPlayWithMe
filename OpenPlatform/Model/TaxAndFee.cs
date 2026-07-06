using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class TaxAndFee
    {
        public int Id { get; set; }

        // Tên sàn: SHOPEE, TIKI, LAZADA, web bán hàng chính chủ...
        public string name { get; set; }

        // thuế
        public float tax { get; set; }

        // phí sàn
        public float fee { get; set; }

        // Là lợi nhuận tuyệt đối tối thiểu trên một sản phẩm mà có thể tăng thêm mức 
        // chiết khấu (so với mức chiết khấu mặt bằng chung) bán ra. Mặc định 3000 VND
        public int minProfit { get; set; }

        // Chi phí đóng gói 1 đơn hàng trung bình. Mặc định là 2000
        public int packingCost { get; set; }

        // Lợi nhuận phần trăm mong muốn so với GIÁ BÁN (doanh thu)
        public float expectedPercentProfit { get; set; }

        // Lợi nhuận phần trăm nhỏ nhất có thể chấp nhận. Phục vụ những chương trình kiểu: rẻ vô địch,
        // cạnh tranh giá real time theo đối thủ,..
        public float minPercentProfit { get; set; }
    }
}
