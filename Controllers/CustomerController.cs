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
        public CustomerMySql sqler;
        public OrderMySql ordersqler;

        public CustomerController()
        {
            sqler = new CustomerMySql();
            ordersqler = new OrderMySql();
        }

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
            MySqlResultState result = await sqler.AddNewCustomerAsync(userName, passWord);
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
                await sqler.CustomerLogoutAsync(cookieResult.cookieValue);
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
            MySqlResultState result = await sqler.LoginCustomerAsync(userName, passWord);

            do
            {
                if (result.State != EMySqlResultState.OK) break;

                // Set cookie cho tài khoản
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                // Lấy thông tin customer
                Customer customer = await sqler.GetCustomerFromUserNameAsync(userName);
                if (customer == null || customer.id == -1)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Cant get customer from db";
                    break;
                }

                // Lưu cookie vào bảng tbcookie
                MySqlResultState resultInsert = await sqler.CookieCustomerLoginAsync(cookieResult.cookieValue, customer.id);
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
                result = await sqler.SyncGuestDataInTransactionAsync(cus.id, guestCarts, guestAddresses);
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
                    await sqler.DeleteDefaultAddressAsync(cus.id);
                }
                result = await sqler.UpdateAddressAsync(add);
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
                    result = await sqler.DeleteAddressAsync(add);
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
                    await sqler.DeleteDefaultAddressAsync(cus.id);
                }
                result = await sqler.InsertAddressAsync(cus.id, add);
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

                result = await sqler.UpdateInforAsync(cus);
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
                result = await sqler.ChangePasswordCustomerAsync(cus.id, oldPassWord, newPassWord, renewPassWord);
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
                lsAddress = await sqler.GetListAddressAsync(cus.id);
            }
            else
            {
                lsAddress = null;
            }
            return JsonConvert.SerializeObject(lsAddress);
        }

        //[HttpPost]
        //public string CreateCustomer_CheckValidUserName(string userName)
        //{
        //    MySqlResultState result;
        //    result = sqler.CheckValidUserName(userName);
        //    return JsonConvert.SerializeObject(result);
        //}

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
                result = await ordersqler.GetCartCountAsync(cus.id);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> GetCustomer()
        {
            Customer cus = await AuthentCustomerAsync();
            if (cus != null)
            {
                cus = await sqler.GetCustomerAsync(cus.id);
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

            ViewData["title"] = "Danh sách đơn hàng";
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
        //        result = ordersqler.SearchOrderCount(cus.id, statusOrder);
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
                CookieResultState cookie = Cookie.GetOrderListCookie(HttpContext);
                List<int> lsId = new List<int>();
                string[] ids = cookie.cookieValue.Split('#');
                foreach (var id in ids)
                {
                    lsId.Add(Common.ConvertStringToInt32(id));
                }
                result = await ordersqler.GetAllOrderFromListIdAsync(lsId);
            }
            else
            {
                result = await ordersqler.GetAllOrderAsync(cus.id);
            }

            return JsonConvert.SerializeObject(result);
        }

        // Lấy 1 đơn
        [HttpPost]
        public async Task<string> GetOrderFromId(int id)
        {
            MySqlResultState result = await ordersqler.GetOrderFromIdAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        // Tìm kiếm đơn theo tên hoặc 4 số cuối SDT người nhận đối với khách vãng lai
        [HttpPost]
        public async Task<string> SearchOrderForAnonymous(string sdtNameForSearch)
        {
            MySqlResultState result = await ordersqler.SearchOrderForAnonymousAsync(sdtNameForSearch);
            return JsonConvert.SerializeObject(result);
        }
    }
}
