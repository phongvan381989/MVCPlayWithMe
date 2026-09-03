using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.PaymentProof;
using MVCPlayWithMe.Models.SanPhamModel;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class AdministratorController : BasicController
    {
        //AdministratorMySql sqler;
        //public AdministratorController ()
        //{
        //    //sqler = new ProductMySql();
        //    sqler = new AdministratorMySql();
        //}

        // GET: Administrator
        public async Task<ActionResult> Index()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

             return View();
        }

        public async Task<ActionResult> New()
        {
            if ((await AuthentAdministratorAsync()) == null)
                return AuthenticationFail();

            return View();
        }

        [HttpPost]
        public async Task<string> New_AddAdministrator(string userName, int userNameType, string passWord, int privilege)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = await AdministratorMySql.AddNewAdministratorAsync(userName, userNameType, passWord, privilege);
            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> Login()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return View();
            }

            return View("~/Views/Administrator/Index.cshtml");
        }

        [HttpPost]
        public async Task<string> Logout()
        {
            CookieResultState cookieResult = Cookie.GetVisitorTypeCookie(HttpContext);

            if (!string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                await AdministratorMySql.AdministratorLogoutAsync(cookieResult.cookieValue);
                Cookie.DeleteVisitorTypeCookie(HttpContext);
            }
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }

        [HttpPost]
        public async Task<string> Login_Login(string userName, string passWord)
        {
            MySqlResultState result = await AdministratorMySql.LoginAdministratorAsync(userName, passWord);

            // Set cookie cho tài khoản quản trị
            if (result.State == EMySqlResultState.OK)
            {
                CookieResultState cookieResult = Cookie.SetAndGetVisitorTypeCookie(HttpContext);

                // Lấy thông tin adminstrator
                Administrator administrator = await AdministratorMySql.GetAdministratorFromUserNameAsync(userName);

                // Lưu cookie vào bảng tbcookie_administrator
                MySqlResultState resultInsert = await AdministratorMySql.AddNewCookieAdministratorAsync(cookieResult.cookieValue, administrator.id);
                if (resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ChangePassword(string oldPassWord,
            string newPassWord, string renewPassWord)
        {
            MySqlResultState result = new MySqlResultState();

            Administrator administrator = await AuthentAdministratorAsync();
            if (administrator != null)
            {
                result = await AdministratorMySql.ChangePasswordAdministratorAsync(administrator.id,
                    oldPassWord, newPassWord, renewPassWord);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        #region Bank Transfer Payment Management

        /// <summary>
        /// Hiển thị danh sách đơn hàng chờ thanh toán (chuyển khoản)
        /// </summary>
        public async Task<ActionResult> PendingOrders()
        {
            // Check admin login
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            try
            {
                // Lấy tất cả đơn hàng PENDING_PAYMENT (chuyển khoản chưa xác nhận)
                var orders = await OrderMySql.GetOrdersByStatusAsync("PENDING_PAYMENT");
                return View(orders);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"PendingOrders error: {ex.Message}");
                return View(new List<Order>());
            }
        }

        /// <summary>
        /// Xác nhận đã nhận tiền chuyển khoản
        /// Admin check sao kê ngân hàng → Nhập mã GD → Xác nhận
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ConfirmBankTransferPayment(
            int orderId,
            string transactionCode,
            string transferNote,
            int transferAmount)
        {
            try
            {
                // Check admin login
                Administrator admin = await AuthentAdministratorAsync();
                if (admin == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                int adminId = admin.id;

                // Lấy thông tin order
                var order = await OrderMySql.GetCommonOrderFromCodeIdAsync(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra order status
                if (order.OrderPayStatus !=  (int)EOrderPayStatus.PENDING)
                {
                    return Json(new { success = false, message = "Đơn hàng không ở trạng thái chờ thanh toán" });
                }

                // Transaction: Create proof + Update order + Deduct inventory
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                    {
                        try
                        {
                            // 1. Tạo PaymentProof record (Admin tự tạo, không phải khách upload)
                            var proof = new PaymentProof
                            {
                                OrderId = orderId,
                                TransactionCode = transactionCode,
                                TransferAmount = transferAmount,
                                TransferNote = transferNote,
                                Status = "verified",  // Admin tạo = verified luôn
                                VerifiedBy = adminId,
                                VerifiedAt = DateTime.Now
                            };

                            // Insert proof vào DB
                            await PaymentProofMySql.InsertVerifiedProofAsync(proof);

                            // 2. Update order status to PAID
                            await OrderMySql.UpdateOrderStatusTransactionAsync(
                                conn, transaction, orderId, "PAID"
                            );

                            // 3. Trừ tồn kho
                            var productQuantities = order.lsOrderDetail
                                .Select(d => (d.sanPhamId, d.quantity))
                                .ToList();

                            await SanPhamMySql.UpdateQuantityAfterSaleTransactionAsync(
                                conn, transaction, productQuantities
                            );

                            await transaction.CommitAsync();

                            // 4. Log
                            MyLogger.GetInstance().Info(
                                $"Admin #{adminId} confirmed bank transfer for order #{orderId} (OrderCode: {order.OrderCode}). " +
                                $"TransactionCode: {transactionCode}, Amount: {transferAmount}"
                            );

                            return Json(new { success = true, message = "Đã xác nhận thanh toán" });
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            MyLogger.GetInstance().Error($"ConfirmBankTransferPayment transaction error: {ex.Message}");
                            return Json(new { success = false, message = ex.Message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"ConfirmBankTransferPayment error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Hủy đơn hàng (quá hạn thanh toán hoặc lý do khác)
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CancelOrder(int orderId)
        {
            try
            {
                // Check admin login
                Administrator admin = await AuthentAdministratorAsync();
                if (admin == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                int adminId = admin.id;

                // Lấy thông tin order
                var order = await OrderMySql.GetCommonOrderFromCodeIdAsync(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Update order status to CANCELLED
                var result = await OrderMySql.UpdateOrderStatusAsync(orderId, "CANCELLED");

                if (result.State == EMySqlResultState.OK)
                {
                    // Log
                    MyLogger.GetInstance().Info($"Admin #{adminId} cancelled order #{orderId} (OrderCode: {order.OrderCode})");

                    return Json(new { success = true, message = "Đã hủy đơn hàng" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"CancelOrder error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Order Management

        /// <summary>
        /// Trang quản lý tất cả đơn hàng (Admin)
        /// </summary>
        public async Task<ActionResult> OrderList()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            ViewData["title"] = "Quản lý đơn hàng Voi bé nhỏ";
            return View();
        }

        /// <summary>
        /// Lấy tất cả đơn hàng (Admin xem được tất cả)
        /// </summary>
        [HttpPost]
        public async Task<string> GetAllOrdersAdmin(OrderDateRangeRequest request = null)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, "Chưa đăng nhập"));
            }

            // Parse dates
            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.dateFrom))
                {
                    if (DateTime.TryParse(request.dateFrom, out DateTime from))
                    {
                        fromDate = from;
                    }
                }

                if (!string.IsNullOrEmpty(request.dateTo))
                {
                    if (DateTime.TryParse(request.dateTo, out DateTime to))
                    {
                        // Set to end of day
                        toDate = to.Date.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            MySqlResultState result = await OrderMySql.GetAllOrdersAsync(fromDate, toDate);
            return JsonConvert.SerializeObject(result);
        }

        public class OrderDateRangeRequest
        {
            public string dateFrom { get; set; }
            public string dateTo { get; set; }
        }

        /// <summary>
        /// Admin cập nhật trạng thái đơn hàng
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateOrderStatus(UpdateOrderStatusRequest request)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                Administrator admin = await AuthentAdministratorAsync();
                if (admin == null)
                {
                    result.State = EMySqlResultState.AUTHEN_FAIL;
                    result.Message = "Chưa đăng nhập";
                    return JsonConvert.SerializeObject(result);
                }

                // Validate status
                if (!Enum.IsDefined(typeof(EOrderStatus), request.newStatus))
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Trạng thái không hợp lệ";
                    return JsonConvert.SerializeObject(result);
                }

                // Update status
                result = await OrderMySql.UpdateOrderStatusByIdAsync(request.orderId, request.newStatus);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        public class UpdateOrderStatusRequest
        {
            public int orderId { get; set; }
            public int newStatus { get; set; }
        }

        /// <summary>
        /// Admin cập nhật trạng thái thanh toán
        /// </summary>
        [HttpPost]
        public async Task<string> UpdatePaymentStatus(UpdatePaymentStatusRequest request)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                Administrator admin = await AuthentAdministratorAsync();
                if (admin == null)
                {
                    result.State = EMySqlResultState.AUTHEN_FAIL;
                    result.Message = "Chưa đăng nhập";
                    return JsonConvert.SerializeObject(result);
                }

                // Validate payment status
                if (!Enum.IsDefined(typeof(EOrderPayStatus), request.newPaymentStatus))
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Trạng thái thanh toán không hợp lệ";
                    return JsonConvert.SerializeObject(result);
                }

                // Update payment status
                result = await OrderMySql.UpdatePaymentStatusByIdAsync(request.orderId, request.newPaymentStatus);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        public class UpdatePaymentStatusRequest
        {
            public int orderId { get; set; }
            public int newPaymentStatus { get; set; }
        }

        /// <summary>
        /// Trang chi tiết đơn hàng (Admin)
        /// </summary>
        public async Task<ActionResult> OrderDetail(int? id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            ViewData["title"] = "Chi tiết đơn hàng";
            ViewData["orderId"] = id;
            return View();
        }

        /// <summary>
        /// Lấy chi tiết 1 đơn hàng (Admin)
        /// </summary>
        [HttpPost]
        public async Task<string> GetOrderDetailAdmin(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, "Chưa đăng nhập"));
            }

            MySqlResultState result = await OrderMySql.GetOrderByOrderIdAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region Output Management

        /// <summary>
        /// Trang quản lý Output (xuất/nhập kho)
        /// </summary>
        public async Task<ActionResult> Output()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        /// <summary>
        /// Tìm kiếm Output theo Code và ECommerce
        /// Accepts both form data and JSON
        /// </summary>
        [HttpPost]
        public async Task<string> SearchOutput(string code = null, int? ecommerce = null)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, "Chưa đăng nhập"));
            }


            if (string.IsNullOrWhiteSpace(code) || !ecommerce.HasValue)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.EXCEPTION, "Code hoặc ECommerce không hợp lệ"));
            }

            List<Output> outputs = new List<Output>();

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    outputs = await OutputMySql.GetOutputsByCodeAndECommerceAsync(conn, code, ecommerce.Value);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn($"SearchOutput failed: {ex.Message}");
                    return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.EXCEPTION, ex.Message));
                }
            }

            MySqlResultState result = new MySqlResultState();
            result.myJson = outputs;
            return JsonConvert.SerializeObject(result);
        }

        #endregion
    }
}
