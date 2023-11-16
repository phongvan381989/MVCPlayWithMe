using MVCPlayWithMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    // Đăng nhập sẽ lưu uid quản trị, khách hàng tương ứng vào bảng tbcookie_administrator, tbcookie
    // Chưa đăng nhập uid chỉ được lưu ở client. Khi khách hàng đăng nhập đồng bộ dữ liệu client lên server
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

                httpContext.Response.Cookies.Add(uId);
            }
            else
            {
                cookieResut.uId = httpContext.Request.Cookies["uId"].Value;
            }

            return cookieResut;
        }

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
            return cookieResut;
        }
    }
}