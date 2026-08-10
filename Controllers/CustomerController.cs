using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class CustomerController : BasicController
    {
        // GET: Customer
        public async Task<ActionResult> CreateCustomer()
        {
            if ((await AuthentCustomerAsync()) == null)
            {
                ViewData["title"] = "Tạo tài khoản khách hàng";
                return View();
            }
            // Quay về trang chủ
            return View("~/Views/Home/Search.cshtml");
        }

        [HttpPost]
        public async Task<string> CreateCustomer_Add(string userName, string passWord)
        {
            MySqlResultState result = await CustomerMySql.AddNewCustomerAsync(userName, passWord);
            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> Login()
        {
            if ((await AuthentCustomerAsync()) == null)
            {
                ViewData["title"] = "Đăng nhập tài khoản khách hàng";
                return View();
            }
            // Quay về trang chủ
            return View("~/Views/Home/Search.cshtml");
        }

        [HttpPost]
        public async Task<string> Logout()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);
            if (!string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                await CustomerMySql.CustomerLogoutAsync(cookieResult.cookieValue);
                Cookie.DeleteUserIdCookie(HttpContext);
            }
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }


        /// <summary>
        /// Vì customerInforCookie chứa unicode, bị lỗi khi lấy như cookie do chưa encode,
        /// nên ta gửi như tham số
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="customerInforCookie"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Login_Login(string userName, string passWord)
        {
            MySqlResultState result = await CustomerMySql.LoginCustomerAsync(userName, passWord);

            do
            {
                if (result.State != EMySqlResultState.OK) break;

                // Set cookie cho tài khoản
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                // Lấy thông tin customer
                Customer customer = await CustomerMySql.GetCustomerFromUserNameAsync(userName);
                if (customer == null || customer.id == -1)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Cant get customer from db";
                    break;
                }

                // Lưu cookie vào bảng tbcookie
                MySqlResultState resultInsert = await CustomerMySql.CookieCustomerLoginAsync(cookieResult.cookieValue, customer.id);
                if (resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                    break;
                }

                // ✅ Guest cart + addresses sẽ được sync riêng qua API /Customer/SyncGuestData
            }
            while (false);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Sync guest cart + addresses từ localStorage lên DB sau khi login
        /// Sử dụng 1 connection + transaction để đảm bảo all-or-nothing
        /// Nếu 1 phần fail → rollback toàn bộ → giữ localStorage để user retry
        /// </summary>
        [HttpPost]
        public async Task<string> SyncGuestData(string guestCartJson = null, string guestAddressesJson = null)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                // 1. Authenticate customer (đã login)
                Customer cus = await AuthentCustomerAsync();
                if (cus == null)
                {
                    result.State = EMySqlResultState.AUTHEN_FAIL;
                    result.Message = "Chưa đăng nhập";
                    return JsonConvert.SerializeObject(result);
                }

                // 2. Parse JSON
                List<Cart> guestCarts = null;
                if (!string.IsNullOrEmpty(guestCartJson))
                {
                    guestCarts = JsonConvert.DeserializeObject<List<Cart>>(guestCartJson);
                }

                List<Address> guestAddresses = null;
                if (!string.IsNullOrEmpty(guestAddressesJson))
                {
                    guestAddresses = JsonConvert.DeserializeObject<List<Address>>(guestAddressesJson);
                }

                // 3. ✅ Sync trong 1 transaction (1 connection, all-or-nothing)
                result = await CustomerMySql.SyncGuestDataInTransactionAsync(cus.id, guestCarts, guestAddresses);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"SyncGuestData error: {ex.Message}");
                result.State = EMySqlResultState.ERROR;
                result.Message = "Lỗi sync dữ liệu";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateAddress(string address)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                Address add = JsonConvert.DeserializeObject<Address>(address);
                if (add.defaultAdd == 1)
                {
                    await CustomerMySql.DeleteDefaultAddressAsync(cus.id);
                }
                result = await CustomerMySql.UpdateAddressAsync(add);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> DeleteAddress(string address)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                Address add = JsonConvert.DeserializeObject<Address>(address);
                if (add.defaultAdd != 1)
                {
                    result = await CustomerMySql.DeleteAddressAsync(add);
                }
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> InsertAddress(string address)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                Address add = JsonConvert.DeserializeObject<Address>(address);
                if (add.defaultAdd == 1)
                {
                    await CustomerMySql.DeleteDefaultAddressAsync(cus.id);
                }
                result = await CustomerMySql.InsertAddressAsync(cus.id, add);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateInfor(string email, string sdt, string fullName,
            int day, int month, int year,
            int sex)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                cus.email = email;
                cus.sdt = sdt;
                cus.fullName = fullName;
                cus.birthday = new DateTime(year, month, day);
                cus.sex = sex;

                result = await CustomerMySql.UpdateInforAsync(cus);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ChangePassword(string oldPassWord,
            string newPassWord, string renewPassWord)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                result = await CustomerMySql.ChangePasswordCustomerAsync(cus.id, oldPassWord, newPassWord, renewPassWord);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> CheckUidCookieValid()
        {
            MySqlResultState result = new MySqlResultState();
            if ((await AuthentCustomerAsync()) == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Xác thực khách hàng không thành công.";
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> GetListAddress()
        {
            List<Address> lsAddress = new List<Address>();
            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                lsAddress = await CustomerMySql.GetListAddressAsync(cus.id);
            }
            else
            {
                lsAddress = null;
            }
            return JsonConvert.SerializeObject(lsAddress);
        }

        [HttpPost]
        public async Task<string> GetCartCount()
        {
            Customer cus = await AuthentCustomerAsync();
            MySqlResultState result = new MySqlResultState();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
            }
            else
            {
                result = await OrderMySql.GetCartCountAsync(cus.id);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> GetCustomer()
        {
            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                cus = await CustomerMySql.GetCustomerAsync(cus.id);
            }
            return JsonConvert.SerializeObject(cus);
        }

        public async Task<ActionResult> AccountInfor()
        {
            if ((await AuthentCustomerAsync()) == null)
                return View("~/Views/Customer/Login.cshtml");

            ViewData["title"] = "Thông tin tài khoản khách hàng";
            return View();
        }

        // Đơn hàng của khách đăng nhập hoặc vãng lai
        // Nếu orderCode null, lấy tất cả đơn hàng
        public ActionResult Order(int? id)
        {
            //if (AuthentCustomer() == null)
            //    return View("~/Views/Customer/Login.cshtml");

            ViewData["title"] = "Đơn mua";
            return View();
        }

        ///// <summary>
        ///// Không dùng hàm này nữa
        ///// /// 0: UNPAID, 1:  READY_TO_SHIP,
        ///// 2: PROCESSED, // Đây là trạng thái sau khi in đơn 3:  SHIPPED, 4:  COMPLETED,
        ///// 5: IN_CANCEL, 6:  CANCELLED, 7:  INVOICE_PENDING, 8: ALL
        ///// </summary>
        ///// <param name="status"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public string SearchOrderCount(int statusOrder)
        //{
        //    Customer cus = await AuthentCustomerAsync();
        //    MySqlResultState result = new MySqlResultState();
        //    if (cus == null)
        //    {
        //        result.State = EMySqlResultState.AUTHEN_FAIL;
        //    }
        //    else
        //    {
        //        result = orderCustomerMySql.SearchOrderCount(cus.id, statusOrder);
        //    }
        //    return JsonConvert.SerializeObject(result);
        //}

        //// Không dùng hàm này nữa
        //[HttpGet]
        //public string ChangePage(int statusOrder, int start, int offset)
        //{
        //    Customer cus = await AuthentCustomerAsync();

        //    MySqlResultState result = new MySqlResultState();
        //    if (cus == null)
        //    {
        //        result.State = EMySqlResultState.AUTHEN_FAIL;
        //        return JsonConvert.SerializeObject(result);
        //    }
        //    else
        //    {
        //        result = ordersqler.SearchOrderChangePage(cus.id, statusOrder, start, offset);
        //    }

        //    return JsonConvert.SerializeObject(result);
        //}

        // Lấy tất cả đơn của khách đăng nhập hoặc vãng lai
        [HttpPost]
        public async Task<string> GetAllOrder()
        {
            Customer cus = await AuthentCustomerAsync();

            MySqlResultState result = new MySqlResultState();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Chưa đăng nhập";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                result = await OrderMySql.GetOrderListByCustomerIdAsync(cus.id);
            }

            return JsonConvert.SerializeObject(result);
        }

        // Lấy 1 đơn
        [HttpPost]
        public async Task<string> GetOrderFromId(int id)
        {
            MySqlResultState result = new MySqlResultState();
            Customer cus = await AuthentCustomerAsync();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Chưa đăng nhập";
                return JsonConvert.SerializeObject(result);
            }
            result = await OrderMySql.GetOrderByOrderIdAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        // Tìm kiếm đơn theo tên hoặc 5 số cuối SDT người nhận đối với khách vãng lai
        [HttpPost]
        public async Task<string> SearchOrderForAnonymous(string sdtNameForSearch)
        {
            MySqlResultState result = await OrderMySql.SearchOrderForAnonymousAsync(sdtNameForSearch);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng từ OrderCodes (localStorage) - dùng cho khách vãng lai
        /// </summary>
        /// <param name="orderCodes">Danh sách OrderCode từ localStorage (comma-separated hoặc JSON array)</param>
        [HttpPost]
        public async Task<string> GetOrdersByOrderCodes(string orderCodes)
        {
            MySqlResultState result = new MySqlResultState();

            if (string.IsNullOrWhiteSpace(orderCodes))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Danh sách OrderCode trống";
                result.myJson = new List<Order>();
                return JsonConvert.SerializeObject(result);
            }

            try
            {
                // Parse orderCodes (có thể là JSON array hoặc comma-separated string)
                List<string> orderCodeList;

                if (orderCodes.Trim().StartsWith("["))
                {
                    // JSON array format: ["ORD-001", "ORD-002", "ORD-003"]
                    orderCodeList = JsonConvert.DeserializeObject<List<string>>(orderCodes);
                }
                else
                {
                    // Comma-separated format: "ORD-001,ORD-002,ORD-003"
                    orderCodeList = orderCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(code => code.Trim())
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .ToList();
                }

                if (orderCodeList == null || orderCodeList.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Danh sách OrderCode trống sau khi parse";
                    result.myJson = new List<Order>();
                    return JsonConvert.SerializeObject(result);
                }

                result = await OrderMySql.GetOrderListByOrderCodesAsync(orderCodeList);
            }
            catch (Exception ex)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = $"Lỗi parse OrderCodes: {ex.Message}";
                result.myJson = new List<Order>();
                MyLogger.GetInstance().Error($"GetOrdersByOrderCodes error: {ex}");
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}
