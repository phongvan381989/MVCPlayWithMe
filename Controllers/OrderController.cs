using MVCPlayWithMe.Controllers.ProductSupporting;
using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MVCPlayWithMe.OpenPlatform.API.CassoInvoiceAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static MVCPlayWithMe.Controllers.AdministratorController;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers
{
    public class OrderController : BasicController
    {
        #region Order Management

        public class OrderDateRangeRequest
        {
            public string dateFrom { get; set; }
            public string dateTo { get; set; }
        }

        public class UpdateOrderStatusRequest
        {
            public int orderId { get; set; }
            public EOrderStatus newStatus { get; set; }
        }

        public class UpdatePaymentStatusRequest
        {
            public int orderId { get; set; }
            public EOrderPayStatus newPaymentStatus { get; set; }
        }
        #endregion

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
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    result = await OrderMySql.UpdateOrderStatusByIdAsync(request.orderId, request.newStatus, conn);

                    result = await OrderMySql.GetOrderByOrderId_CoreAsync(request.orderId, conn);
                    if (result.State != EMySqlResultState.OK)
                    {
                        return JsonConvert.SerializeObject(result);
                    }

                    Order order = result.myJson as Order;
                    await VBNProductSupporting.UpdateOrderWhenCreateOrCancelAsync(order, conn);

                    EECommerceType eECommerceType =
                    order.from == EOrderFrom.VOI_BE_NHO ? EECommerceType.PLAY_WITH_ME : EECommerceType.MANUAL; 
                    if (request.newStatus == EOrderStatus.RECEIVED)
                    {
                        // Xử lý xuât hóa đơn
                        // Insert trạng thái đơn vào danh sách xuất hóa đơn (chỉ insert nếu chưa tồn tại)
                        bool isNewOrder = await EInvoiceMySql.InsertOrderStatusBackgroundAsync(order.code,
                            eECommerceType, OrderStatus.GetOrderStatus(EOrderStatus.RECEIVED), conn);

                        // Xuất hóa đơn tự động chỉ cho đơn MỚI
                        if (isNewOrder)
                        {
                            CommonOrder commonOrder = new CommonOrder(order);
                            await OrderMySql.PlayWithMeGetMappingOfCommonOrderConnectOut(commonOrder, conn);
                            await CommonCassoInvoiceAPI.IssueInvoiceBackground(commonOrder, conn);
                        }
                    }
                    else if(request.newStatus == EOrderStatus.CANCELLED)
                    {
                        // Lưu vào bảng check đơn hủy nhưng đã đóng
                        await ECommerceOrderReturnShopMySql.InsertIfPackedAsync(order.code,
                            eECommerceType, EOrderType.NORMAL, conn);
                    }
                    else if (request.newStatus == EOrderStatus.RETURNED)
                    {
                        // Lưu/cập nhật trạng thái đơn vào danh sách cần điều chỉnh/hủy hóa đơn đã xuất do khách trả hàng hoàn tiền
                        await ECommerceOrderCheckInvoiceMySql.InsertOrIgnoreAsync(order.code,
                            eECommerceType);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
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
        public async Task<string> GetOrderFromId(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, "Chưa đăng nhập"));
            }

            MySqlResultState result = await OrderMySql.GetOrderByOrderIdAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> CreateOrderManually()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MVCPlayWithMe.Models.Order.Order order = null;
            MySqlResultState result = new MySqlResultState();
            try
            {
                // Đọc JSON body từ request
                string body;
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    body = await reader.ReadToEndAsync();
                }

                // Deserialize thành đối tượng Order
                order = JsonConvert.DeserializeObject<MVCPlayWithMe.Models.Order.Order>(body);

                if (order == null)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không thể parse dữ liệu đơn hàng.";
                    return JsonConvert.SerializeObject(result);
                }

                if (order.lsOrderDetail == null || order.lsOrderDetail.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Đơn hàng không có sản phẩm nào.";
                    return JsonConvert.SerializeObject(result);
                }

                // Tạo danh sách (sanPhamId, quantity) để trừ kho
                var sanPhamQuantities = order.lsOrderDetail
                    .Select(d => (d.sanPhamId, d.quantity))
                    .ToList();

                // Kiểm tra tồn kho trước khi tạo đơn
                result = await OrderMySql.CheckStockAvailabilityAsync(sanPhamQuantities);
                if (result.State != EMySqlResultState.OK)
                {
                    return JsonConvert.SerializeObject(result);
                }

                // Gọi repository để lưu đơn hàng
                int newOrderId = -1;
                string orderCode = string.Empty;

                // Sinh mã đơn hàng unique
                try
                {
                    orderCode = await OrderCodeSequenceMySql.GenerateUniqueOrderCodeAsync();
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    return JsonConvert.SerializeObject(result);
                }

                // Tạo Address object từ Order
                Address cusInfor = new Address
                {
                    name = order.name ?? "",
                    phone = order.phone ?? "",
                    province = order.province ?? "",
                    subdistrict = order.subdistrict ?? "",
                    detail = order.detail ?? ""
                };

                // PaymentDeadline: mặc định 48h nếu PENDING, null nếu đã thanh toán
                DateTime? paymentDeadline = order.OrderPayStatus == EOrderPayStatus.PENDING
                    ? DateTime.Now.AddHours(48)
                    : (DateTime?)null;


                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Start TRANSACTION
                    using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                    {
                        // 1. Insert Order
                        result = await OrderMySql.AddOrderTransactionAsync(conn, transaction,
                            order.customerId,
                            order.note ?? "",
                            (SByte)order.from,
                            orderCode,
                            (SByte)order.OrderStatus,
                            (SByte)order.OrderPayStatus,
                            (SByte)order.PaymentMethod,
                            paymentDeadline ?? DateTime.Now.AddHours(48),
                            cusInfor);

                        if (result.State != EMySqlResultState.OK)
                        {
                            await transaction.RollbackAsync();
                            return JsonConvert.SerializeObject(result);
                        }
                        newOrderId = result.myAnything;

                        // 2. Insert OrderDetail
                        result = await OrderMySql.AddDetailOrderTransactionAsync(conn, transaction, newOrderId, order.lsOrderDetail);
                        if (result.State != EMySqlResultState.OK)
                        {
                            await transaction.RollbackAsync();
                            return JsonConvert.SerializeObject(result);
                        }

                        // 3. Trừ số lượng sản phẩm trong kho tbproducts
                        result = await ProductMySql.UpdateQuantityAfterSaleVBNTransactionAsync(conn, transaction, sanPhamQuantities);
                        if (result.State != EMySqlResultState.OK)
                        {
                            await transaction.RollbackAsync();
                            return JsonConvert.SerializeObject(result);
                        }

                        // 4. Insert OrderPay
                        if (order.lsOrderPay != null && order.lsOrderPay.Count > 0)
                        {
                            result = await OrderMySql.AddPayOrderTransactionAsync(conn, transaction, newOrderId, order.lsOrderPay);
                            if (result.State != EMySqlResultState.OK)
                            {
                                await transaction.RollbackAsync();
                                return JsonConvert.SerializeObject(result);
                            }
                        }

                        // 5. Commit transaction
                        await transaction.CommitAsync();

                        result.State = EMySqlResultState.OK;
                        result.Message = $"Tạo đơn hàng thành công. Mã đơn: {orderCode}";
                        MyLogger.GetInstance().Info($"✅ CreateOrderManually DONE! OrderID={newOrderId}, OrderCode={orderCode}");
                    }

                    if (result.State != EMySqlResultState.OK)
                    {
                        return JsonConvert.SerializeObject(result);
                    }

                    result = await OrderMySql.GetOrderByOrderCode_CoreAsync(orderCode, conn);
                    if (result.State != EMySqlResultState.OK)
                    {
                        return JsonConvert.SerializeObject(result);
                    }

                    Order orderFromDb = result.myJson as Order;
                    await VBNProductSupporting.UpdateOrderWhenCreateOrCancelAsync(orderFromDb, conn);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

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
                var orders = await OrderMySql.GetOrdersByPayStatusAsync(EOrderPayStatus.PENDING);
                return View(orders);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"PendingOrders error: {ex.Message}");
                return View(new List<Order>());
            }
        }

        public async Task<ActionResult> OrderManually()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }
    }
}
