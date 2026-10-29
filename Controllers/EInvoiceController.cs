using MVCPlayWithMe.Controllers.ProductSupporting;
using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.OpenPlatform.API.CassoInvoiceAPI;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class EInvoiceController : BasicController
    {
        /// <summary>
        /// Trang quản lý hóa đơn điện tử
        /// </summary>
        public async Task<ActionResult> Index()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            return View();
        }

        /// <summary>
        /// Lấy danh sách hóa đơn
        /// </summary>
        [HttpPost]
        public async Task<string> GetList(string fromDate = null)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            DateTime? fromDateTime = null;
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime temp;
                if (DateTime.TryParse(fromDate, out temp))
                {
                    fromDateTime = temp;
                }
            }

            List<EInvoice> list = await EInvoiceMySql.GetAllAsync(fromDateTime);
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Thêm hóa đơn mới (hoặc cập nhật nếu đã tồn tại)
        /// </summary>
        [HttpPost]
        public async Task<string> Insert(string code, int ecommerce, string orderStatus, float moneySum, int invoiceStatus, string note)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            EInvoice invoice = new EInvoice
            {
                Code = code,
                ECommmerce = ecommerce,
                OrderStatus = orderStatus,
                MoneySum = moneySum,
                InvoiceStatus = invoiceStatus,
                Note = note,
                InvoiceDate = DateTime.Now
            };

            MySqlResultState result = await EInvoiceMySql.InsertAsync(invoice);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Cập nhật trạng thái hóa đơn
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateStatus(int id, int invoiceStatus, string invoiceCode, string note)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await EInvoiceMySql.UpdateInvoiceStatusAsync(id, invoiceStatus, invoiceCode ?? string.Empty, note ?? string.Empty);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Xóa hóa đơn
        /// </summary>
        [HttpPost]
        public async Task<string> Delete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await EInvoiceMySql.DeleteAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Trang chi tiết đơn hàng
        /// </summary>
        public async Task<ActionResult> Order(string ecommerce, string orderCode)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            ViewBag.EType = ecommerce;
            ViewBag.OrderId = orderCode;
            return View();
        }

        /// <summary>
        /// Lấy danh sách đơn hàng cần xuất hóa đơn theo ngày (cho Export CSV)
        /// </summary>
        [HttpPost]
        public async Task<string> GetOrdersForExport(string date = null)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (string.IsNullOrEmpty(date))
            {
                return JsonConvert.SerializeObject(new List<object>());
            }

            DateTime selectedDate;
            if (!DateTime.TryParse(date, out selectedDate))
            {
                return JsonConvert.SerializeObject(new List<object>());
            }

            // Lấy danh sách einvoice cần xuất
            List<EInvoice> einvoices = await EInvoiceMySql.GetOrdersForExportAsync(selectedDate);

            // Lấy thông tin chi tiết đơn hàng
            List<CommonOrder> commonOrders = new List<CommonOrder>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                {
                    foreach (var einvoice in einvoices)
                    {
                        
                        var commonOrder = await CommonProductSupporting.GetOrderDetail(
                            Common.GetStringFromEECommerceType((Common.EECommerceType)einvoice.ECommmerce), einvoice.Code, conn);
                        if(commonOrder != null)
                        {
                            commonOrders.Add(commonOrder);
                        }    
                    }
                }
            }

            return JsonConvert.SerializeObject(commonOrders);
        }

        /// <summary>
        /// Tạo hóa đơn nháp từ đơn hàng
        /// </summary>
        [HttpPost]
        public async Task<string> CreateDraftInvoice()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                // Đọc JSON body
                string body;
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    body = await reader.ReadToEndAsync();
                }

                // Deserialize trực tiếp thành CommonOrder
                CommonOrder order = JsonConvert.DeserializeObject<CommonOrder>(body);

                if (order == null)
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Không parse được dữ liệu đơn hàng";
                    return JsonConvert.SerializeObject(result);
                }

                Boolean hasCasso = await EInvoiceMySql.HasCassoInvoiceLookUpInfoAsync(order.code,
                    (int)Common.GetEECommerceTypeFromString(order.ecommerceName));
                if(hasCasso)
                {
                    result.State = EMySqlResultState.EXIST;
                    result.Message = "Đã có hóa đơn nháp. Đơn đã có mã tìm kiếm, id hóa đơn trên hệ thống Casso. KHông được tạo mới";
                    return JsonConvert.SerializeObject(result);
                }

                // Tạo request body
                OpenApiCreateInvoiceBody request = new OpenApiCreateInvoiceBody(order);

                // Gọi API Casso
                CassoResponse<OpenApiInvoiceResource> response =
                    await CommonCassoInvoiceAPI.CreateDraftInvoiceAsync(request);

                if (response.IsSucceed)
                {
                    result = await EInvoiceMySql.UpdateCassoInvoiceLookUpInfoAsync(order.code,
                        (int)Common.GetEECommerceTypeFromString(order.ecommerceName),
                        response.data.id, response.data.lookupCode, 0);
                    result.myJson = new
                    {
                        success = true,
                        message = "Tạo hóa đơn nháp thành công",
                        invoiceId = response.data.id,
                        lookupCode = response.data.lookupCode,
                        invoiceNumber = response.data.invoiceNumber,
                        status = response.data.status
                    };
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = $"Lỗi từ Casso: {response.message}";
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                return JsonConvert.SerializeObject(result);
            }
        }

        /// <summary>
        /// Xuất hóa đơn (phát hành lên cơ quan thuế)
        /// </summary>
        [HttpPost]
        public async Task<string> IssueInvoice()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                // Đọc JSON body
                string body;
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    body = await reader.ReadToEndAsync();
                }

                // Deserialize trực tiếp thành CommonOrder
                CommonOrder order = JsonConvert.DeserializeObject<CommonOrder>(body);

                if (order == null)
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Không parse được dữ liệu đơn hàng";
                    return JsonConvert.SerializeObject(result);
                }

                Boolean hasCasso = await EInvoiceMySql.HasCassoInvoiceLookUpInfoAsync(order.code,
                    (int)Common.GetEECommerceTypeFromString(order.ecommerceName));
                if (hasCasso)
                {
                    result.State = EMySqlResultState.EXIST;
                    result.Message = "Đã có hóa đơn nháp. Đơn đã có mã tìm kiếm, id hóa đơn trên hệ thống Casso. KHông được tạo mới";
                    return JsonConvert.SerializeObject(result);
                }

                // Tạo request body
                OpenApiCreateInvoiceBody request = new OpenApiCreateInvoiceBody(order);

                // Gọi API Casso để xuất hóa đơn
                CassoResponse<OpenApiInvoiceResource> response =
                    await CommonCassoInvoiceAPI.IssueInvoiceAsync(request);

                if (response.IsSucceed)
                {
                    // Cập nhật InvoiceId và LookupCode vào DB
                    result = await EInvoiceMySql.UpdateCassoInvoiceLookUpInfoAsync(
                        order.code,
                        (int)Common.GetEECommerceTypeFromString(order.ecommerceName),
                        response.data.id,
                        response.data.lookupCode, 1);

                    result.myJson = new
                    {
                        success = true,
                        message = "Xuất hóa đơn thành công!",
                        invoiceId = response.data.id,
                        lookupCode = response.data.lookupCode,
                        invoiceNumber = response.data.invoiceNumber,
                        status = response.data.status,
                        taxAuthorityCode = response.data.taxAuthorityCode
                    };
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = $"Lỗi từ Casso: {response.message}";
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                return JsonConvert.SerializeObject(result);
            }
        }

        /// <summary>
        /// Trang kiểm tra hóa đơn cần sửa đổi
        /// </summary>
        public async Task<ActionResult> Check()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            return View();
        }

        /// <summary>
        /// Lấy danh sách đơn cần kiểm tra hóa đơn
        /// </summary>
        [HttpPost]
        public async Task<string> GetCheckInvoiceList(int? isDone = null)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            List<ECommerceOrderCheckInvoice> list;

            if (isDone.HasValue)
            {
                list = await ECommerceOrderCheckInvoiceMySql.GetByIsDoneAsync(isDone.Value);
            }
            else
            {
                list = await ECommerceOrderCheckInvoiceMySql.GetAllAsync();
            }

            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Cập nhật trạng thái đã kiểm tra xong (IsDone = 1)
        /// </summary>
        [HttpPost]
        public async Task<string> UpdateCheckInvoiceStatus(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await ECommerceOrderCheckInvoiceMySql.UpdateIsDoneByIdAsync(id);
            return JsonConvert.SerializeObject(result);
        }
    }
}
