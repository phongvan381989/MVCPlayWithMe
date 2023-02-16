using MVCPlayWithMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    public class Cookie
    {
        private static DateTime SetExpires()
        {
            return DateTime.Now.AddYears(1);
        }

        /// <summary>
        /// Set và get user id cookie.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static CookieResultState SetAndGetUserIdCookie(HttpContextBase httpContext)
        {
            CookieResultState cookieResut = new CookieResultState();
            if (httpContext.Request.Cookies["uId"] == null)
            {
                HttpCookie uId = new HttpCookie("uId");
                Guid guidVal = Guid.NewGuid();
                cookieResut.uId = guidVal.ToString("N");
                uId.Value = cookieResut.uId;
                uId.Expires = SetExpires();
                uId.HttpOnly = true;

                //HttpCookie cooType = new HttpCookie("cooType");
                //cooType.Value = "0";
                //cooType.Expires = SetExpires();

                httpContext.Response.Cookies.Add(uId);
                //httpContext.Response.Cookies.Add(cooType);

                // Mặc định lưu cookie vào bảng tbCookie trước, nếu sau đăng nhập như administrator thì xóa
                // cookie ở bảng tbCookie lưu sang bảng tbcookie_administrator
                MyMySql.AddNewCookie(cookieResut.uId, -1);
            }
            else
            {
                cookieResut.uId = httpContext.Request.Cookies["uId"].Value;
                //if (httpContext.Request.Cookies["cooType"] == null)// Bị người dùng xóa thủ công
                //{
                //    HttpCookie cooType = new HttpCookie("cooType");
                //    cooType.Value = "0";
                //    cooType.Expires = SetExpires();

                //    httpContext.Response.Cookies.Add(cooType);
                //}
                //else
                //{
                //    cookieResut.cooType = Int32.Parse(httpContext.Request.Cookies["cooType"].Value);
                //}
                //cookieResut.isFirst = false;
            }

            return cookieResut;
        }

        //public static void SetCookieType(HttpContextBase httpContext,int cookieType)
        //{
        //    if (httpContext.Request.Cookies["cooType"] == null)
        //    {
        //        HttpCookie cooType = new HttpCookie("cooType");
        //        cooType.Value = cookieType.ToString();
        //        cooType.Expires = SetExpires();

        //        httpContext.Response.Cookies.Add(cooType);
        //    }
        //    else
        //    {
        //        httpContext.Response.Cookies["cooType"].Value = cookieType.ToString();
        //        httpContext.Response.Cookies["cooType"].Expires = SetExpires();
        //    }
        //    return;
        //}

        /// <summary>
        /// Sau khi đăng xuất, tạo uid mới
        /// </summary>
        /// <param name="httpContext"></param>
        public static CookieResultState RecreateCookie(HttpContextBase httpContext)
        {
            CookieResultState cookieResut = new CookieResultState();

            HttpCookie uId = new HttpCookie("uId");
            Guid guidVal = Guid.NewGuid();
            cookieResut.uId = guidVal.ToString("N");
            uId.Value = cookieResut.uId;
            uId.Expires = SetExpires();
            uId.HttpOnly = true;
            httpContext.Response.Cookies.Add(uId);

            // Mặc định lưu cookie vào bảng tbCookie trước, nếu sau đăng nhập như administrator thì xóa
            // cookie ở bảng tbCookie lưu sang bảng tbcookie_administrator
            MyMySql.AddNewCookie(cookieResut.uId, -1);
            return cookieResut;
        }

        ///// <summary>
        ///// Sau khi đăng nhập, set cookie để phân biệt đây là tài khoản khách hàng hay người quản trị
        ///// </summary>
        ///// <param name="httpContext"></param>
        ///// <returns></returns>
        //public static string SetAndGetIsCustomerCookie(HttpContext httpContext, Boolean isCustomer)
        //{
        //    string strCooType = string.Empty;
        //    if (httpContext.Request.Cookies["cooType"] == null)
        //    {
        //        HttpCookie cooType = new HttpCookie("cooType");

        //        cooType.Value = isCustomer?"1":"0";
        //        cooType.Expires = DateTime.Now.AddYears(1);
        //        httpContext.Response.Cookies.Add(cooType);

        //        MyMySql.AddNewCookie(strCooType, -1);
        //    }
        //    else
        //    {
        //        HttpCookie cooType = httpContext.Request.Cookies["cooType"];
        //        strCooType = cooType["cooType"];
        //    }
        //    return strCooType;
        //}


        public static Administrator GetAdministratorFromCookieUId(CookieResultState cookieResult)
        {
            Administrator administrator = MyMySql.GetAdministratorFromCookie(cookieResult.uId);
            return administrator;
        }

        public static Customer GetCustomerFromCookUId(CookieResultState cookieResult)
        {
            Customer customer = MyMySql.GetCustomerFromCookie(cookieResult.uId);
            return customer;
        }

        /// <summary>
        /// Khi đăng nhập tài khoản administrator
        /// </summary>
        /// <returns></returns>
        public static Boolean MoveCookie()
        {
            return true;
        }
    }
}