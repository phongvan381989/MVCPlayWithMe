using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp
{
    /// <summary>
    /// Lỗi 400 - Validation
    /// </summary>
    public class OpenApiError400
    {
        /// <summary>
        /// VALIDATION_ERROR, INVOICE_SERIES_INACTIVE, INVOICE_NO_LINE_ITEMS,
        /// INVOICE_SELLER_INCOMPLETE, BUYER_COMPANY_NAME_REQUIRED_WITH_TAX_CODE
        /// </summary>
        public string code { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
        public List<OpenApiFieldIssue> fields { get; set; }
    }

    /// <summary>
    /// Thông tin field bị lỗi
    /// </summary>
    public class OpenApiFieldIssue
    {
        public string path { get; set; }
        public string message { get; set; }
    }

    /// <summary>
    /// Lỗi 401 - Unauthorized (không có requestId)
    /// </summary>
    public class OpenApiUnauthorizedError
    {
        public string code { get; set; }  // UNAUTHORIZED
        public string message { get; set; }
    }

    /// <summary>
    /// Lỗi 403 - Forbidden
    /// </summary>
    public class OpenApiError403
    {
        /// <summary>
        /// INSUFFICIENT_SCOPE, ORG_NOT_FOUND, ORG_NOT_VERIFIED
        /// </summary>
        public string code { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
    }

    /// <summary>
    /// Lỗi 404 - Not Found
    /// </summary>
    public class OpenApiError404
    {
        /// <summary>
        /// ROUTE_NOT_FOUND, INVOICE_NOT_FOUND, SERIES_NOT_FOUND,
        /// SERIES_NOTATION_NOT_FOUND, TEMPLATE_NOT_FOUND
        /// </summary>
        public string code { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
    }

    /// <summary>
    /// Lỗi 405 - Method Not Allowed
    /// </summary>
    public class OpenApiError405
    {
        public string code { get; set; }  // METHOD_NOT_ALLOWED
        public string message { get; set; }
        public string requestId { get; set; }
    }

    /// <summary>
    /// Lỗi 413 - Payload Too Large
    /// </summary>
    public class OpenApiError413
    {
        public string code { get; set; }  // PAYLOAD_TOO_LARGE
        public string message { get; set; }
        public string requestId { get; set; }
    }

    /// <summary>
    /// Lỗi 422 - Unprocessable Entity
    /// </summary>
    public class OpenApiError422
    {
        /// <summary>
        /// DECIMAL_PLACES_EXCEEDED, TAX_RATE_REQUIRED,
        /// TAX_RATE_UNSUPPORTED, EMAIL_SENDER_NOT_CONFIGURED
        /// </summary>
        public string code { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
    }

    /// <summary>
    /// Lỗi 500 - Internal Server Error
    /// </summary>
    public class OpenApiError500
    {
        public string code { get; set; }  // INTERNAL_ERROR
        public string message { get; set; }
        public string requestId { get; set; }
    }

    /// <summary>
    /// Lỗi 503 - Identity Provider Unavailable (không có requestId)
    /// </summary>
    public class OpenApiIdentityProviderError
    {
        public string code { get; set; }  // IDENTITY_PROVIDER_UNAVAILABLE
        public string message { get; set; }
    }
}
