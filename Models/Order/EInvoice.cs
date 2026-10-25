using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.Order
{
    public class EInvoice
    {
        public int Id { get; set; }

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Sàn TMĐT: 0=PlayWithMe, 1=Tiki, 2=Shopee, 3=Lazada
        /// </summary>
        public int ECommmerce { get; set; }

        /// <summary>
        /// Trạng thái đơn hàng các sàn trả về
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Tổng tiền đơn hàng
        /// </summary>
        public float MoneySum { get; set; }

        /// <summary>
        /// Trạng thái hóa đơn: 0=Chưa xuất, 1=Đã xuất, 2=Lỗi
        /// </summary>
        public int InvoiceStatus { get; set; }

        /// <summary>
        /// Ngày xuất hóa đơn
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// Mã hóa đơn điện tử
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        public EInvoice()
        {
            Id = -1;
            Code = string.Empty;
            ECommmerce = -1;
            OrderStatus = string.Empty;
            MoneySum = 0;
            InvoiceStatus = 0; // Chưa xuất
            InvoiceDate = DateTime.Now;
            InvoiceCode = string.Empty;
            Note = string.Empty;
        }
    }
}
