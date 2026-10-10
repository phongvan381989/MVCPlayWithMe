using System;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Models.ProductModel
{
    /// <summary>
    /// Model cho bảng tb_track_original_sku
    /// Tracking SKU gốc từ sàn TMĐT (Tiki, Shopee, Lazada)
    /// </summary>
    public class TrackOriginalSku
    {
        public int Id { get; set; }

        /// <summary>
        /// Sàn TMĐT: 0 = PlayWithMe, 1 = Tiki, 2 = Shopee, 3 = Lazada
        /// </summary>
        public EECommerceType ECommmerce { get; set; }

        /// <summary>
        /// SKU gốc từ sàn TMĐT
        /// </summary>
        public string OriginalSku { get; set; }

        /// <summary>
        /// ID sản phẩm trong kho (tbProducts) phục vụ mapping
        /// Sản phẩm combo sẽ có nhiều dòng dữ liệu trong db, chỉ khác productId
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Ngày tạo record
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 0 = Simple product, 1 = Variant
        /// </summary>
        public byte IsVariant { get; set; }

        /// <summary>
        /// Track ID từ sàn TMĐT (ví dụ: track_id của Tiki khi tạo sản phẩm)
        /// </summary>
        public string TrackId { get; set; }

        /// <summary>
        /// Product ID trên sàn TMĐT
        /// </summary>
        public long ProOnEcoId { get; set; }

        /// <summary>
        /// Đánh dấu sản phẩm trên sàn đã được lưu vào DB chưa
        /// 0 = chưa lưu, > 0 = đã lưu
        /// </summary>
        public byte IsSaveProOnEcoToDb { get; set; }

        public TrackOriginalSku()
        {
            Id = -1;
            ECommmerce = EECommerceType.TIKI;
            OriginalSku = string.Empty;
            ProductId = -1;
            Quantity = 0;
            CreatedDate = DateTime.Now;
            IsVariant = 0;
            TrackId = string.Empty;
            ProOnEcoId = -1;
            IsSaveProOnEcoToDb = 0;
        }
    }
}
