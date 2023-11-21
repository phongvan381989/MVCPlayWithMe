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
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

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

            return View("~/Views/Customer/Update.cshtml");
        }

        [HttpPost]
        public string Logout()
        {
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);
            sqler.CustomerLogout(cookieResult.cookieValue);
            Cookie.RecreateUserIdCookie(HttpContext);
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }


        [HttpPost]
        public string Login_Login(string userName, string passWord)
        {
            MySqlResultState result = sqler.LoginCustomer(userName, passWord);

            // Set cookie cho tài khoản
            if (result.State == EMySqlResultState.OK)
            {
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                // Lấy thông tin cutomer
                Customer customer = sqler.GetCustomerFromUserName(userName);

                // Lưu cookie vào bảng tbcookie_administrator
                MySqlResultState resultInsert = sqler.CookieCustomerLogin(cookieResult.cookieValue, customer.id);
                if (resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Update()
        {
            return View();
        }
    }
}