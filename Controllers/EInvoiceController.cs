using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
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
        public async Task<string> GetList()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            List<EInvoice> list = await EInvoiceMySql.GetAllAsync();
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
    }
}
