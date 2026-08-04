using System;

namespace MVCPlayWithMe.Models.Order
{
    /// <summary>
    /// Model cho bảng tborder_track - Lịch sử thay đổi trạng thái đơn hàng
    /// </summary>
    public class OrderStatusTrack
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Status { get; set; }         // INT: 1=PENDING_PAYMENT, 2=PAID, 3=PROCESSING, 4=SHIPPED, 5=COMPLETED, 6=CANCELLED
        public DateTime? Time { get; set; }

        // Helper properties (không map từ DB)
        public string StatusString => MVCPlayWithMe.General.OrderStatusMapper.ToString(Status);
        public string StatusDisplayName => MVCPlayWithMe.General.OrderStatusMapper.ToDisplayName(Status);
    }
}
