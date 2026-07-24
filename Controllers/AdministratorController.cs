using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Index()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

             return View();
        }

        public async Task<ActionResult> New()
        {
            if ((await AuthentAdministratorAsync()) == null)
                return AuthenticationFail();

            return View();
        }

        [HttpPost]
        public async Task<string> New_AddAdministrator(string userName, int userNameType, string passWord, int privilege)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = await sqler.AddNewAdministratorAsync(userName, userNameType, passWord, privilege);
            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> Login()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return View();
            }

            return View("~/Views/Administrator/Index.cshtml");
        }

        [HttpPost]
        public async Task<string> Logout()
        {
            CookieResultState cookieResult = Cookie.GetVisitorTypeCookie(HttpContext);

            if (!string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                await sqler.AdministratorLogoutAsync(cookieResult.cookieValue);
                Cookie.DeleteVisitorTypeCookie(HttpContext);
            }
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }

        [HttpPost]
        public async Task<string> Login_Login(string userName, string passWord)
        {
            MySqlResultState result = await sqler.LoginAdministratorAsync(userName, passWord);

            // Set cookie cho tài khoản quản trị
            if (result.State == EMySqlResultState.OK)
            {
                CookieResultState cookieResult = Cookie.SetAndGetVisitorTypeCookie(HttpContext);

                // Lấy thông tin adminstrator
                Administrator administrator = await sqler.GetAdministratorFromUserNameAsync(userName);

                // Lưu cookie vào bảng tbcookie_administrator
                MySqlResultState resultInsert = await sqler.AddNewCookieAdministratorAsync(cookieResult.cookieValue, administrator.id);
                if (resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> ChangePassword(string oldPassWord,
            string newPassWord, string renewPassWord)
        {
            MySqlResultState result = new MySqlResultState();

            Administrator administrator = await AuthentAdministratorAsync();
            if (administrator != null)
            {
                result = await sqler.ChangePasswordAdministratorAsync(administrator.id,
                    oldPassWord, newPassWord, renewPassWord);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}
