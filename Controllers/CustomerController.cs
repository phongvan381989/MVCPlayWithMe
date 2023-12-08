using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class CustomerController : Controller
    {
        public CustomerMySql sqler;

        public CustomerController()
        {
            sqler = new CustomerMySql();
        }

        private Customer AuthentCustomer()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);
            if (string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                return null;
            }

            /// Check cookie đã được lưu trong db
            return sqler.GetCustomerFromCookie(cookieResult.cookieValue);
        }

        // GET: Customer
        public ActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public string CreateCustomer_Add(string userName, int userNameType, string passWord)
        {
            MySqlResultState result = sqler.AddNewCustomer(userName, userNameType, passWord);
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Login()
        {
            if (AuthentCustomer() == null)
                return View();
            // Quay về trang chủ
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        public string Logout()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);
            if (!string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                sqler.CustomerLogout(cookieResult.cookieValue);
                Cookie.RecreateUserIdCookie(HttpContext);
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
        public string Login_Login(string userName, string passWord, string customerInforCookie)
        {
            MySqlResultState result = sqler.LoginCustomer(userName, passWord);

            do
            {
                if (result.State != EMySqlResultState.OK)
                {
                    break;
                }

                // Set cookie cho tài khoản
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                // Lấy thông tin cutomer
                Customer customer = sqler.GetCustomerFromUserName(userName);
                if (customer == null || customer.id == -1)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Cant get customer from db";
                    break;
                }

                // Lưu cookie vào bảng tbcookie
                MySqlResultState resultInsert = sqler.CookieCustomerLogin(cookieResult.cookieValue, customer.id);
                if (resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                    break;
                }

                // Đang nhập thành công, lưu thông tin cookie khách vãng lai như: cart, customer information vào db
                // Lưu cart cookie
                List<Cart> lsCart = Cookie.GetListCartCookie(HttpContext);
                sqler.AddCartLogin(customer.id, lsCart);
                // Xóa cart cookie bên javascript

                // Lưu customer information
                List<CustomerInforCookie> lsCustomerInforCookie = Cookie.GetListCustomerInforCookieFromCookieValue(customerInforCookie);
                sqler.AddCustomerInforAddress(customer.id, lsCustomerInforCookie);
                // Xóa customer info cookie bên javascript
            }
            while (false);
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Update()
        {
            return View();
        }
    }
}