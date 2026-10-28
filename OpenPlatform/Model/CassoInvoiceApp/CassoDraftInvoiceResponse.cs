using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp
{
    /// <summary>
    /// Response khi tạo hóa đơn thành công
    /// </summary>
    public class OpenApiInvoiceResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
        public OpenApiInvoiceResource data { get; set; }
    }

    /// <summary>
    /// Thông tin hóa đơn
    /// </summary>
    public class OpenApiInvoiceResource
    {
        public string id { get; set; }
        public string invoiceNumber { get; set; }
        public string lookupCode { get; set; }

        /// <summary>
        /// draft, processing, valid, invalid, failed
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// VAT, SALES
        /// </summary>
        public string invoiceType { get; set; }

        public string seriesId { get; set; }
        public string seriesSymbol { get; set; }
        public string seriesNotation { get; set; }
        public string taxAuthorityCode { get; set; }
        public string issuedAt { get; set; }
        public string serverIssuedAt { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }

        public OpenApiSellerInfo sellerInfo { get; set; }
        public OpenApiBuyerInfo buyerInfo { get; set; }
        public OpenApiPaymentInfo paymentInfo { get; set; }
        public List<OpenApiLineItem> items { get; set; }
        public OpenApiInvoiceSummary summary { get; set; }
        public Dictionary<string, object> metadata { get; set; }

        /// <summary>
        /// Thông tin điều chỉnh/thay thế (nếu hóa đơn này là hóa đơn điều chỉnh/thay thế)
        /// </summary>
        public OpenApiInvoiceCorrection correction { get; set; }

        /// <summary>
        /// Các hóa đơn điều chỉnh/thay thế đã phát hành cho hóa đơn này
        /// </summary>
        public List<OpenApiInvoiceCorrector> correctedBy { get; set; }

        /// <summary>
        /// Lý do cơ quan thuế từ chối (chỉ có khi status = invalid)
        /// </summary>
        public List<OpenApiCqtError> errors { get; set; }
    }

    /// <summary>
    /// Thông tin người bán (response)
    /// </summary>
    public class OpenApiSellerInfo
    {
        public string companyName { get; set; }
        public string taxCode { get; set; }
        public string address { get; set; }
        public string storeCode { get; set; }
        public string storeName { get; set; }
        public string businessLocationCode { get; set; }
        public string businessLocationName { get; set; }
        public string businessLocationAddress { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string bankAccount { get; set; }
        public string bankName { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string referenceCode { get; set; }
    }

    /// <summary>
    /// Thông tin người mua (response)
    /// </summary>
    public class OpenApiBuyerInfo
    {
        public string companyName { get; set; }
        public string taxCode { get; set; }
        public string budgetRelationCode { get; set; }
        public string address { get; set; }
        public string customerCode { get; set; }
        public string phone { get; set; }
        public string citizenId { get; set; }
        public string passportNumber { get; set; }
        public string email { get; set; }
        public string buyerName { get; set; }
        public string bankAccount { get; set; }
        public string bankName { get; set; }
    }

    /// <summary>
    /// Thông tin thanh toán (response)
    /// </summary>
    public class OpenApiPaymentInfo
    {
        public string referenceCode { get; set; }
    }

    /// <summary>
    /// Dòng hàng (response)
    /// </summary>
    public class OpenApiLineItem
    {
        public int? lineNumber { get; set; }
        public string goodsNature { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string unitOfMeasure { get; set; }
        public string quantity { get; set; }
        public string unitPrice { get; set; }
        public string discountRate { get; set; }
        public string discountAmount { get; set; }
        public string amount { get; set; }
        public string taxRate { get; set; }
        public string taxAmount { get; set; }
        public string catalogItemId { get; set; }
    }

    /// <summary>
    /// Tổng hóa đơn (response)
    /// </summary>
    public class OpenApiInvoiceSummary
    {
        public string totalTradeDiscount { get; set; }
        public string totalOtherDeduction { get; set; }
        public string totalPayment { get; set; }
        public string totalPaymentInWords { get; set; }
        public List<OpenApiFee> fees { get; set; }
        public string totalAmountBeforeTax { get; set; }
        public string totalTaxAmount { get; set; }
        public List<OpenApiTaxRateGroup> taxRateBreakdown { get; set; }
    }

    /// <summary>
    /// Khoản phí (response)
    /// </summary>
    public class OpenApiFee
    {
        public string feeName { get; set; }
        public string feeAmount { get; set; }
    }

    /// <summary>
    /// Nhóm thuế suất
    /// </summary>
    public class OpenApiTaxRateGroup
    {
        public string taxRate { get; set; }
        public string amount { get; set; }
        public string taxAmount { get; set; }
    }

    /// <summary>
    /// Thông tin điều chỉnh/thay thế
    /// </summary>
    public class OpenApiInvoiceCorrection
    {
        /// <summary>
        /// adjustment: hóa đơn điều chỉnh, replacement: hóa đơn thay thế
        /// </summary>
        public string kind { get; set; }

        /// <summary>
        /// Hóa đơn bị xử lý
        /// </summary>
        public string correctsInvoiceId { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public string reason { get; set; }
    }

    /// <summary>
    /// Hóa đơn điều chỉnh/thay thế
    /// </summary>
    public class OpenApiInvoiceCorrector
    {
        /// <summary>
        /// adjustment: hóa đơn điều chỉnh, replacement: hóa đơn thay thế
        /// </summary>
        public string kind { get; set; }

        /// <summary>
        /// ID hóa đơn điều chỉnh/thay thế
        /// </summary>
        public string invoiceId { get; set; }
    }

    /// <summary>
    /// Lỗi từ cơ quan thuế
    /// </summary>
    public class OpenApiCqtError
    {
        /// <summary>
        /// Mã lỗi (MLoi)
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Mô tả lỗi (MTLoi)
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Hướng xử lý (HDXLy)
        /// </summary>
        public string resolution { get; set; }
    }
}
