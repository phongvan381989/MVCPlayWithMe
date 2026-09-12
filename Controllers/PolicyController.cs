using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
using Newtonsoft.Json;

namespace MVCPlayWithMe.Controllers
{
    public class PolicyController : BasicController
    {

        public void ViewDataGetCommonInforOfVoiBeNho()
        {
            ViewData["webAddress"] = "voibenho.com";
            ViewData["httpsWebAddress"] = Common.httpsVoiBeNho;
            ViewData["hotline"] = "083 577 4489";
            ViewData["postAddress"] = "Số 28, Ngõ 3, Khu Tập Thể Đo Lường, Tổ Dân Phố 3A, phường Đông Ngạc, Hà Nội";
            ViewData["emailAddress"] = "playwithmebook@gmail.com";
            ViewData["ceoName"] = "HOÀNG THỊ HUỆ";
            ViewData["businessId"] = "01D-8014432";
            ViewData["inHaNoiFee"] = Common.ConvertIntToVNDFormat(Common.standardShipFeeInHaNoi);
            ViewData["outHaNoiFee"] = Common.ConvertIntToVNDFormat(Common.standardShipFeeOutHaNoi);
        }

        // GET: Policy
        public ActionResult InforCustomerPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách bảo mật thông tin";
            return View();
        }

        public ActionResult GuaranteePolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách bảo hành";
            return View();
        }

        public ActionResult ReturnRefundPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách đổi trả, hoàn tiền";
            return View();
        }

        public ActionResult CheckPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách kiểm hàng";
            return View();
        }

        public ActionResult ComplaintPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách xử lý khiếu nại";
            return View();
        }

        public ActionResult PayPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách thanh toán";
            return View();
        }

        public ActionResult OrderPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            //ViewData["title"] = "Chính sách bảo hành";
            return View();
        }

        public ActionResult ObligationPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Nghĩa vụ các bên trong giao dịch";
            return View();
        }

        public ActionResult TransportPolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Chính sách vận chuyển";
            return View();
        }

        public async Task<ActionResult> IntroducePolicy()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Giới thiệu tiệm sách và đồ chơi voi bé nhỏ";

            // Load bank account info để hiển thị tài khoản thanh toán
            var bankAccount = await MVCPlayWithMe.Models.BankAccount.BankAccountMySql.GetActiveBankAccountAsync();
            ViewBag.BankAccount = bankAccount;

            return View();
        }

        /// <summary>
        /// Trang sinh QR code thanh toán
        /// User có thể nhập mã đơn hoặc số tiền để sinh QR tương ứng
        /// </summary>
        public async Task<ActionResult> PaymentQR()
        {
            ViewDataGetCommonInforOfVoiBeNho();
            ViewData["title"] = "Tạo mã QR thanh toán";

            // Load bank account info
            var bankAccount = await MVCPlayWithMe.Models.BankAccount.BankAccountMySql.GetActiveBankAccountAsync();
            ViewBag.BankAccount = bankAccount;

            return View();
        }

        /// <summary>
        /// API sinh QR code thanh toán
        /// Logic:
        /// - Nếu có mã đơn: tìm order, lấy số tiền
        /// - Nếu chỉ có số tiền: sinh QR với số tiền, không có nội dung
        /// - Nếu có cả 2: sinh QR với số tiền + nội dung = mã đơn
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GeneratePaymentQR(string orderCode = null, int? amount = null)
        {
            // Đọc từ JSON body nếu không có parameter
            if (orderCode == null && amount == null)
            {
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    string body = await reader.ReadToEndAsync();
                    var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                    if (jsonData != null)
                    {
                        orderCode = jsonData.ContainsKey("orderCode") ? jsonData["orderCode"]?.ToString() : null;
                        if (jsonData.ContainsKey("amount") && jsonData["amount"] != null)
                        {
                            amount = Convert.ToInt32(jsonData["amount"]);
                        }
                    }
                }
            }

            try
            {
                // Lấy bank account
                var bankAccount = await MVCPlayWithMe.Models.BankAccount.BankAccountMySql.GetActiveBankAccountAsync();
                if (bankAccount == null)
                {
                    return Json(new
                    {
                        State = (int)EMySqlResultState.ERROR,
                        Message = "Không tìm thấy thông tin tài khoản ngân hàng"
                    }, JsonRequestBehavior.AllowGet);
                }

                // Lấy giá trị trực tiếp từ input, không query DB
                // Frontend đã chuyển orderCode sang không dấu trước khi gửi lên
                int finalAmount = amount ?? 0;
                string finalOrderCode = string.IsNullOrWhiteSpace(orderCode) ? "" : orderCode.Trim();

                // Generate QR code (cho phép QR trống nếu không có amount/orderCode)
                string qrCodeUrl = bankAccount.GenerateVietQR(
                    amount: finalAmount,
                    orderCode: finalOrderCode,
                    template: "compact2"
                );

                return Json(new
                {
                    State = (int)EMySqlResultState.OK,
                    Message = "Tạo QR code thành công",
                    QRCodeUrl = qrCodeUrl,
                    Amount = finalAmount,
                    OrderCode = finalOrderCode,
                    BankAccount = new
                    {
                        bankAccount.BankName,
                        bankAccount.AccountNumber,
                        bankAccount.AccountHolder,
                        bankAccount.Branch
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GeneratePaymentQR error: {ex}");
                return Json(new
                {
                    State = (int)EMySqlResultState.ERROR,
                    Message = $"Lỗi: {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
