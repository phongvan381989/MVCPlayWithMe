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
        private Customer AuthentCustomer()
        {
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

            /// Check cookie đã được lưu trong db
            return Cookie.GetCustomerFromCookUId(cookieResult);
        }

        // GET: Customer
        public ActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public string CreateCustomer_Add(string userName, int userNameType, string passWord)
        {
            MySqlResultState result = MyMySql.AddNewCostomer(userName, userNameType, passWord);
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
            MyMySql.CustomerLogout(cookieResult.uId);
            Cookie.RecreateCookie(HttpContext);
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }


        [HttpPost]
        public string Login_Login(string userName, string passWord)
        {
            MySqlResultState result = MyMySql.LoginCustomer(userName, passWord);

            // Set cookie cho tài khoản
            if (result.State == EMySqlResultState.OK)
            {
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);
                //if (cookieResult.cooType != 1)
                //{
                //    Cookie.SetCookieType(HttpContext, 1);
                //}
                // Lấy thông tin cutomer
                Customer customer = MyMySql.GetCustomerFromUserName(userName);

                // Lưu cookie vào bảng tbcookie_administrator
                MySqlResultState resultInsert = MyMySql.CookieCustomerLogin(cookieResult.uId, customer.id);
                if (resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                }
            }
            return JsonConvert.SerializeObject(result);

            //StringBuilder sb = new StringBuilder();
            //sb.Append("{");

            //sb.Append("\"isValid\"");
            //sb.Append(":");
            //if (result.State == EMySqlResultState.OK)
            //{
            //    sb.Append("true");
            //}
            //else
            //{
            //    sb.Append("false");
            //}
            //sb.Append(",");
            //sb.Append("\"state\"");
            //sb.Append(":");
            //sb.Append((int)result.State);
            //sb.Append("}");
            //return sb.ToString();
        }

        public ActionResult Update()
        {
            return View();
        }
    }
}