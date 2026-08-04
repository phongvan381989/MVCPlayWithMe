using System;

namespace MVCPlayWithMe.Models.BankAccount
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolder { get; set; }
        public string Branch { get; set; }
        public string QRCodeTemplate { get; set; }
        public int IsActive { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public BankAccount()
        {
            IsActive = 1;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Generate VietQR URL cho đơn hàng cụ thể
        /// </summary>
        /// <param name="amount">Số tiền (VNĐ)</param>
        /// <param name="orderCode">Mã đơn hàng (VD: DH000123)</param>
        /// <param name="template">Template: compact, compact2, qr_only, print (default: compact)</param>
        /// <returns>VietQR image URL</returns>
        public string GenerateVietQR(int amount, string orderCode, string template = "compact2")
        {
            // Nếu có QRCodeTemplate custom (từ DB) với placeholder <AMOUNT> và <DESCRIPTION>
            if (!string.IsNullOrEmpty(QRCodeTemplate) && QRCodeTemplate.Contains("<"))
            {
                return QRCodeTemplate
                    .Replace("<AMOUNT>", amount.ToString())
                    .Replace("<DESCRIPTION>", orderCode);
            }

            // Default: VietQR.io API
            // VietQR chỉ chấp nhận chữ cái, số và khoảng trắng trong accountName
            // Thay dấu "-" và "_" thành space, xóa các ký tự đặc biệt khác
            string cleanAccountName = (AccountHolder ?? "")
                .Replace("-", " ")  // VND-TGTT-HKD → VND TGTT HKD
                .Replace("_", " ");

            // Loại bỏ các ký tự đặc biệt (giữ chữ, số, space)
            cleanAccountName = System.Text.RegularExpressions.Regex.Replace(
                cleanAccountName,
                @"[^a-zA-Z0-9\s]",
                ""
            );

            // Xóa nhiều space liên tiếp thành 1 space
            cleanAccountName = System.Text.RegularExpressions.Regex.Replace(
                cleanAccountName,
                @"\s+",
                " "
            ).Trim();

            // Doc: https://vietqr.io/danh-sach-api
            return $"https://img.vietqr.io/image/{GetVietQRBankCode()}-{AccountNumber}-{template}.jpg?amount={amount}&addInfo={orderCode}&accountName={Uri.EscapeDataString(cleanAccountName)}";
        }

        /// <summary>
        /// Map BankCode (VCB, TCB, ...) sang VietQR bank code (970436, 970407, ...)
        /// </summary>
        private string GetVietQRBankCode()
        {
            switch (BankCode?.ToUpper())
            {
                case "VCB": return "970436";      // Vietcombank
                case "TCB": return "970407";      // Techcombank
                case "BIDV": return "970418";     // BIDV
                case "VTB": return "970415";      // VietinBank
                case "AGB": return "970405";      // Agribank
                case "MB": return "970422";       // MBBank
                case "ACB": return "970416";      // ACB
                case "VPB": return "970432";      // VPBank
                case "STB": return "970403";      // Sacombank
                case "TPB": return "970423";      // TPBank
                case "SCB": return "970429";      // SCB
                case "SHB": return "970443";      // SHB
                case "MSB": return "970426";      // MSB
                case "OCB": return "970448";      // OCB
                case "SEA": return "970440";      // SeABank
                case "VAB": return "970427";      // VietABank
                default: return BankCode;         // Fallback
            }
        }
    }
}
