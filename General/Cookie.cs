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
            if (httpContext.Request.Cookies[Common.userIdKey] == null)
            {
                HttpCookie uId = new HttpCookie(Common.userIdKey);
                Guid guidVal = Guid.NewGuid();
                cookieResut.cookieValue = guidVal.ToString("N");
                uId.Value = cookieResut.cookieValue;
                uId.Expires = SetExpires();
                uId.HttpOnly = true;

                httpContext.Response.Cookies.Add(uId);
            }
            else
            {
                cookieResut.cookieValue = httpContext.Request.Cookies[Common.userIdKey].Value;
            }

            return cookieResut;
        }

        /// <summary>
        /// Sau khi đăng xuất, tạo uid mới
        /// </summary>
        /// <param name="httpContext"></param>
        public static CookieResultState RecreateUserIdCookie(HttpContextBase httpContext)
        {
            CookieResultState cookieResut = new CookieResultState();

            HttpCookie uId = new HttpCookie(Common.userIdKey);
            Guid guidVal = Guid.NewGuid();
            cookieResut.cookieValue = guidVal.ToString("N");
            uId.Value = cookieResut.cookieValue;
            uId.Expires = SetExpires();
            uId.HttpOnly = true;
            httpContext.Response.Cookies.Add(uId);
            return cookieResut;
        }

        public static CookieResultState GetCartCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if(httpContext.Request.Cookies[Common.cartKey] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.cartKey].Value;
            }
            return cookie;
        }

        // cookie có dạng: cart=id=123#q=10#real=1$id=321#q=1#real=0$....$id=321#q=2#real=0
        public static List<CartCookie> GetListCartCookieFromCartCookie(string cartCookie)
        {
            List<CartCookie> listCartCookie = new List<CartCookie>();
            if (string.IsNullOrEmpty(cartCookie))
                return listCartCookie;

            string[] myArray = cartCookie.Split('$');
            for (int i = 0; i < myArray.Length; i++)
            {
                listCartCookie.Add(new CartCookie(myArray[i]));
            }
            return listCartCookie;
        }

        public static List<CartCookie> GetListCartCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = GetCartCookie(httpContext);
            return GetListCartCookieFromCartCookie(cookie.cookieValue);
        }
    }
}