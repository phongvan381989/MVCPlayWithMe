using System;

namespace MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp
{
    /// <summary>
    /// Thông tin xác thực Casso API
    /// </summary>
    public class CassoAuthen
    {
        public int Id { get; set; }

        /// <summary>
        /// App ID từ Casso
        /// </summary>
        public string app_id { get; set; }

        /// <summary>
        /// App Secret từ Casso
        /// </summary>
        public string app_secret { get; set; }

        /// <summary>
        /// Access Token để gọi API (JWT)
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Loại token: "Bearer"
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// Thời gian sống của token (giây)
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// Thời gian cấp token (Unix timestamp)
        /// </summary>
        public long issued_at { get; set; }

        /// <summary>
        /// Thời gian hết hạn (Unix timestamp)
        /// </summary>
        public long exp { get; set; }

        /// <summary>
        /// Các ký tự cần bỏ qua trong tên sản phẩm khi xuất hóa đơn
        /// </summary>
        public string SkippingCharacters { get; set; }

        public CassoAuthen()
        {
            Id = -1;
            app_id = string.Empty;
            app_secret = string.Empty;
            access_token = string.Empty;
            token_type = string.Empty;
            expires_in = 0;
            issued_at = 0;
            exp = 0;
            SkippingCharacters = string.Empty;
        }
    }
}
