using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp
{
    /// <summary>
    /// Request body để tạo hóa đơn nháp
    /// </summary>
    public class OpenApiCreateInvoiceBody
    {
        /// <summary>
        /// Ký hiệu hóa đơn đầy đủ, gồm 1 chữ số loại hóa đơn và 6 ký tự ký hiệu
        /// </summary>
        public string seriesNotation { get; set; }

        /// <summary>
        /// Ngày lập hóa đơn (yyyy-MM-dd hoặc ISO 8601 có offset)
        /// </summary>
        public string issuedAt { get; set; }

        /// <summary>
        /// Thông tin người bán
        /// </summary>
        public OpenApiSellerInput sellerInfo { get; set; }

        /// <summary>
        /// Thông tin người mua
        /// </summary>
        public OpenApiBuyerInput buyerInfo { get; set; }

        /// <summary>
        /// Thông tin thanh toán
        /// </summary>
        public OpenApiPaymentInput paymentInfo { get; set; }

        /// <summary>
        /// Thông tin tổng hóa đơn
        /// </summary>
        public OpenApiInvoiceSummaryInput summary { get; set; }

        /// <summary>
        /// Metadata tùy ý
        /// </summary>
        public Dictionary<string, object> metadata { get; set; }

        /// <summary>
        /// Danh sách dòng hàng (1-500 items)
        /// </summary>
        public List<OpenApiLineItemInput> items { get; set; }

        public static string RemoveCharactersInItemName(string input)
        {
            return Common.RemoveCharactersInList(input, "+?!");
        }

        public OpenApiCreateInvoiceBody(CommonOrder order)
        {
            // Hóa đơn bán hàng
            seriesNotation = "2C26TAA";
            issuedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

            sellerInfo = new OpenApiSellerInput
            {
                storeCode = "",
                storeName = "",
                phone = "0835774489",
                email = "playwithmebook@gmail.com",
                bankAccount = "19032646559022",
                bankName = "Ngân hàng TMCP Kỹ thương Việt Nam",
                fax = "",
                website = Common.httpsVoiBeNho,
                businessLocationCode = "00999",
                businessLocationName = "HỘ KINH DOANH HOÀNG THỊ HUỆ",
                businessLocationAddress = "Số 28, Ngõ 3, Khu tập thể Đo Lường, Tổ dân phố Phú Minh, Phường Đông Ngạc, TP Hà Nội",
                referenceCode = ""//,
                //companyName = "", // Lấy bên hệ thống
                //taxCode = "" // Lấy bên hệ thống
            };

            buyerInfo = new OpenApiBuyerInput
            {
                companyName = "",
                taxCode = "",
                budgetRelationCode = "",
                address = "",
                customerCode = "",
                phone = "",
                citizenId = "",
                passportNumber = "",
                email = "",
                buyerName = "Bán cho người tiêu dùng",
                bankAccount = "",
                bankName = ""
            };

            paymentInfo = new OpenApiPaymentInput
            {
                referenceCode = ""
            };

            List<OpenApiFeeInput> feesTemp = new List<OpenApiFeeInput>();

            OpenApiFeeInput openApiFeeInput = new OpenApiFeeInput();
            openApiFeeInput.feeName = "Phí";
            openApiFeeInput.feeAmount = "0";
            feesTemp.Add(openApiFeeInput);

            summary = new OpenApiInvoiceSummaryInput
            {
                totalOtherDeduction = "0",
                fees = feesTemp
            };

            metadata = new Dictionary<string, object>();
            metadata.Add("currency", "VND");
            metadata.Add("exchangeRate", "1");
            metadata.Add("paymentMethod", "TM/CK");
            metadata.Add("ecommerceName", order.ecommerceName);
            metadata.Add("orderCode", order.code);

            // Tạo danh sách items
            items = new List<OpenApiLineItemInput>();

            if (order.listItemName != null && order.listItemName.Count > 0)
            {
                for (int i = 0; i < order.listItemName.Count; i++)
                {
                    string itemName = order.listItemName[i];

                    // Nếu có model name, gộp vào item name
                    if (order.listModelName != null && i < order.listModelName.Count
                        && !string.IsNullOrEmpty(order.listModelName[i]))
                    {
                        itemName += " - " + order.listModelName[i];
                    }

                    itemName = RemoveCharactersInItemName(itemName);

                    int quantity = order.listQuantity != null && i < order.listQuantity.Count
                        ? (int)order.listQuantity[i] : 1;

                    float price = order.list_model_discounted_price != null && i < order.list_model_discounted_price.Count
                        ? order.list_model_discounted_price[i] : 0;

                    string unitOfMeasure = "Cuốn";
                    if (order.listMapping[i] != null && order.listMapping[i].Count > 1)
                    {
                        unitOfMeasure = "Bộ";
                    }

                    items.Add(new OpenApiLineItemInput
                    {
                        goodsNature = "Hàng hóa",
                        catalogItemId = Guid.NewGuid().ToString(),
                        itemCode = "",
                        itemName = itemName,
                        unitOfMeasure = unitOfMeasure,
                        quantity = quantity.ToString(),
                        unitPrice = price.ToString("F0"),
                        discountRate = "0",
                        discountAmount = "0"//,
                        //taxRate = "KKKNT"
                    });
                }
            }

            // Thêm dòng ghi chú mã đơn hàng
            items.Add(new OpenApiLineItemInput
            {
                goodsNature = "Ghi chú/diễn giải",
                catalogItemId = Guid.NewGuid().ToString(),
                itemCode = "",
                itemName = $"{order.ecommerceName} {order.code}",
                unitOfMeasure = "C",
                quantity = "0",
                unitPrice = "0",
                discountRate = "0",
                discountAmount = "0"//,
                //taxRate = "-9%"
            });
        }
    }

    /// <summary>
    /// Thông tin người bán
    /// </summary>
    public class OpenApiSellerInput
    {
        public string storeCode { get; set; }
        public string storeName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string bankAccount { get; set; }
        public string bankName { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string referenceCode { get; set; }
        public string businessLocationCode { get; set; }
        public string businessLocationName { get; set; }
        public string businessLocationAddress { get; set; }
        //public string companyName { get; set; }
        //public string taxCode { get; set; }
    }

    /// <summary>
    /// Thông tin người mua
    /// </summary>
    public class OpenApiBuyerInput
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
    /// Thông tin thanh toán
    /// </summary>
    public class OpenApiPaymentInput
    {
        public string referenceCode { get; set; }
    }

    /// <summary>
    /// Thông tin tổng hóa đơn
    /// </summary>
    public class OpenApiInvoiceSummaryInput
    {
        /// <summary>
        /// Khoản giảm trừ khác
        /// </summary>
        public string totalOtherDeduction { get; set; }

        /// <summary>
        /// Các khoản phí
        /// </summary>
        public List<OpenApiFeeInput> fees { get; set; }
    }

    /// <summary>
    /// Khoản phí
    /// </summary>
    public class OpenApiFeeInput
    {
        public string feeName { get; set; }
        public string feeAmount { get; set; }
    }

    /// <summary>
    /// Dòng hàng hóa/dịch vụ
    /// </summary>
    public class OpenApiLineItemInput
    {
        /// <summary>
        /// Tính chất dòng hàng: Hàng hóa, Khuyến mại, Chiết khấu, Ghi chú/diễn giải, Hàng hóa đặc trưng
        /// </summary>
        public string goodsNature { get; set; }

        /// <summary>
        /// Mã mục trong danh mục hàng hóa
        /// </summary>
        public string catalogItemId { get; set; }

        /// <summary>
        /// Mã hàng hóa, dịch vụ
        /// </summary>
        public string itemCode { get; set; }

        /// <summary>
        /// Tên hàng hóa, dịch vụ (bắt buộc)
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// Đơn vị tính (bắt buộc)
        /// </summary>
        public string unitOfMeasure { get; set; }

        /// <summary>
        /// Số lượng (bắt buộc)
        /// </summary>
        public string quantity { get; set; }

        /// <summary>
        /// Đơn giá (bắt buộc)
        /// </summary>
        public string unitPrice { get; set; }

        /// <summary>
        /// Tỷ lệ chiết khấu (%)
        /// </summary>
        public string discountRate { get; set; }

        /// <summary>
        /// Số tiền chiết khấu
        /// </summary>
        public string discountAmount { get; set; }

        ///// <summary>
        ///// Thuế suất (10%, 0%, KCT, KKKNT)
        ///// </summary>
        //public string taxRate { get; set; }
    }
}
