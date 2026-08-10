using System;

namespace MVCPlayWithMe.Models
{
    /// <summary>
    /// 0 = Giảm phí ship khi tổng tiền hàng >= MinOrderValue
    /// 1 = Mỗi 100k giảm thêm Discount (tính từ MinOrderValue)
    /// </summary>
    public enum EOrderSimplePromotionType
    {
        /// <summary>
        /// 0
        /// </summary>
        SHIP_DISCOUNT,

        /// <summary>
        /// 1
        /// </summary>
        TOTAL_DISCOUNT
    }

    public enum eDiscountType
    {
        /// <summary>
        /// 0 = Discount là số tiền tuyệt đối (VNĐ)
        /// </summary>
        ABSOLUTE,
        /// <summary>
        /// 1 = Discount là % chiết khấu (không có số lẻ vì INT)
        /// </summary>
        PERCENTAGE
    }

    /// <summary>
    /// Entity cho bảng OrderSimplePromotion
    /// Chương trình giảm giá đơn giản áp dụng cho tất cả đơn hàng
    /// </summary>
    public class OrderSimplePromotion
    {
        static public string[] arrayOrderSimplePromotionType = {
            "Giảm phí ship",
            "Mỗi 100k giảm thêm"
        };

        /// <summary>
        /// ID tự tăng
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên chương trình giảm giá (duy nhất)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Giá trị đơn hàng nhỏ nhất được áp dụng (VNĐ)
        /// </summary>
        public int MinOrderValue { get; set; }

        /// <summary>
        /// Trạng thái: 0 = Bật, 1 = Tắt
        /// </summary>
        public SByte Status { get; set; }

        /// <summary>
        /// Loại giảm giá:
        /// 0 = Miễn phí ship khi tổng tiền hàng >= MinOrderValue
        /// 1 = Mỗi 100k giảm thêm Discount (tính từ MinOrderValue)
        /// </summary>
        public EOrderSimplePromotionType Type { get; set; }

        public void SetStrType()
        {
            StrType = arrayOrderSimplePromotionType[(int)Type];
        }
        public string StrType { get; set; }
       

        /// <summary>
        /// Số tiền giảm hoặc % giảm (tùy DiscountType)
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Kiểu giảm giá:
        /// 0 = Discount là số tiền tuyệt đối (VNĐ)
        /// 1 = Discount là % chiết khấu (không có số lẻ vì INT)
        /// </summary>
        public eDiscountType DiscountType { get; set; }

        /// <summary>
        /// Thời gian tạo chương trình
        /// </summary>
        public DateTime? Time { get; set; }

        /// <summary>
        /// Mô tả hiển thị cho khách hàng
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public OrderSimplePromotion()
        {
            Status = 1; // Mặc định: Tắt
            Name = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Constructor đầy đủ
        /// </summary>
        public OrderSimplePromotion(int id, string name, int minOrderValue, SByte status,
            EOrderSimplePromotionType type, int discount, eDiscountType discountType, DateTime? time, string description)
        {
            Id = id;
            Name = name;
            MinOrderValue = minOrderValue;
            Status = status;
            Type = type;
            Discount = discount;
            DiscountType = discountType;
            Time = time;
            Description = description;
        }
    }
}
