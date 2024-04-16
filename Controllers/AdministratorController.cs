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
    public class AdministratorController : BasicController
    {
        AdministratorMySql sqler;
        public AdministratorController ()
        {
            //sqler = new ProductMySql();
            sqler = new AdministratorMySql();
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
            result = sqler.AddNewAdministrator(userName, userNameType, passWord, privilege);
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Login()
        {
            if (AuthentAdministrator() == null)
            {
                return View();
            }

            return View("~/Views/Administrator/Index.cshtml");
        }

        [HttpPost]
        public string Logout()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);

            if (!string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                sqler.AdministratorLogout(cookieResult.cookieValue);
                Cookie.DeleteUserIdCookie(HttpContext);
                Cookie.DeleteVistorTypeCookie(HttpContext);
            }
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }

        [HttpPost]
        public string Login_Login(string userName, string passWord)
        {
            MySqlResultState result = sqler.LoginAdministrator(userName, passWord);

            // Set cookie cho tài khoản quản trị
            if (result.State == EMySqlResultState.OK)
            {
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                Cookie.SetVistorTypeCookie(HttpContext);

                // Lấy thông tin adminstrator
                Administrator administrator = sqler.GetAdministratorFromUserName(userName);

                // Lưu cookie vào bảng tbcookie_administrator
                MySqlResultState resultInsert = sqler.AddNewCookieAdministrator(cookieResult.cookieValue, administrator.id);
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