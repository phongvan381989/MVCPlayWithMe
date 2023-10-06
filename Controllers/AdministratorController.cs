using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class AdministratorController : Controller
    {
        public AdministratorController ()
        {
            //sqler = new ProductMySql();
        }
        private Administrator AuthentAdministrator()
        {
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

            /// Check cookie đã được lưu trong db
            return Cookie.GetAdministratorFromCookieUId(cookieResult);
        }

        //ProductMySql sqler;
        /// <summary>
        /// Nơi đến khi xác thực thất bại hoặc logout
        /// </summary>
        /// <returns></returns>
        private ActionResult AuthenticationFail()
        {
            return View("~/Views/Administrator/Login.cshtml");
        }

        // GET: Administrator
        public ActionResult Index()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

             return View();
        }

        public ActionResult New()
        {
            if (AuthentAdministrator() == null)
                return AuthenticationFail();

            return View();
        }

        [HttpPost]
        public string New_AddAdministrator(string userName, int userNameType, string passWord, int privilege)
        {
            MySqlResultState result = null;
            if (AuthentAdministrator() == null)
            {
                result = new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage);
            }
            result = MyMySql.AddNewAdministrator(userName, userNameType, passWord, privilege);
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Login()
        {
            if (AuthentAdministrator() == null)
                return View();

            return View("~/Views/Administrator/Index.cshtml");
        }

        [HttpPost]
        public string Logout()
        {
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

            //if(string.IsNullOrEmpty(cookieResult.uId))
                //return "{\"state\": 4}";

            MyMySql.AdministratorLogout(cookieResult.uId);
            Cookie.RecreateCookie(HttpContext);
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }

        [HttpPost]
        public string Login_Login(string userName, string passWord)
        {
            MySqlResultState result = MyMySql.LoginAdministrator(userName, passWord);

            // Set cookie cho tài khoản quản trị
            if (result.State == EMySqlResultState.OK)
            {
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                // Lấy thông tin adminstrator
                Administrator administrator = MyMySql.GetAdministratorFromUserName(userName);

                // Lưu cookie vào bảng tbcookie_administrator
                MySqlResultState resultInsert = MyMySql.AddNewCookieAdministrator(cookieResult.uId, administrator.id);
                if(resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                }
            }
            return JsonConvert.SerializeObject(result);
        }

    }
}