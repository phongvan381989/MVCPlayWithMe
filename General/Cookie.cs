using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    // Đăng nhập sẽ lưu uid quản trị, khách hàng tương ứng vào bảng tbcookie_administrator, tbcookie
    // Chưa đăng nhập uid chỉ được lưu ở client. Khi khách hàng đăng nhập đồng bộ dữ liệu client lên server
    // Khách chưa đăng nhập, sản phẩm ở giỏ hàng mặc địnhcó real = 0, chỉ real = 1 khi click mua ngay bên page Item
    // hoặc checkout( ở page checkout trạng thái real không được lưu vào cookie)
    public class Cookie
    {
        private static DateTime SetExpires(int year)
        {
            return DateTime.Now.AddYears(year);
        }

        /// <summary>
        /// Set và get user id cookie.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static CookieResultState SetAndGetUserIdCookie(HttpContextBase httpContext)
        {
            CookieResultState cookieResut = new CookieResultState();

            HttpCookie uId = new HttpCookie(Common.userIdKey);
            Guid guidVal = Guid.NewGuid();
            cookieResut.cookieValue = guidVal.ToString("N");
            uId.Value = cookieResut.cookieValue;
            uId.Expires = SetExpires(100);
            //uId.HttpOnly = true;

            httpContext.Response.Cookies.Add(uId);

            return cookieResut;
        }

        /// <summary>
        /// Chỉ có cookie này khi đăng nhập như người quản trị
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static CookieResultState SetAndGetVisitorTypeCookie(HttpContextBase httpContext)
        {

            CookieResultState cookieResut = new CookieResultState();

            HttpCookie uId = new HttpCookie(Common.visitorType);
            Guid guidVal = Guid.NewGuid();
            cookieResut.cookieValue = guidVal.ToString("N");
            uId.Value = cookieResut.cookieValue;
            uId.Expires = SetExpires(100);
            //uId.HttpOnly = true;

            httpContext.Response.Cookies.Add(uId);

            return cookieResut;
        }

        /// <summary>
        /// Với khách vãng lai, khi mua thành công thêm id mã đơn hàng
        /// ex: 13#43#....#466
        /// </summary>
        /// <param name="httpContext"></param>
        //public static void SetOrderListCookie(HttpContextBase httpContext, int id)
        //{
        //    string value = "";
        //    if (httpContext.Request.Cookies[Common.orderIdList] != null)
        //    {
        //        value = httpContext.Request.Cookies[Common.orderIdList].Value;
        //        if(string.IsNullOrEmpty(value))
        //        {
        //            value = id.ToString();
        //        }
        //        else
        //        {
        //            value = value + "#" + id.ToString();
        //        }
        //    }
        //    HttpCookie orderIdList = new HttpCookie(Common.orderIdList);
        //    orderIdList.Value = value;
        //    orderIdList.Expires = SetExpires(100);
        //    //uId.HttpOnly = true;

        //    httpContext.Response.Cookies.Add(orderIdList);
        //}

        public static CookieResultState GetUserIdCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if (httpContext.Request.Cookies[Common.userIdKey] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.userIdKey].Value;
            }
            return cookie;
        }

        public static CookieResultState GetVisitorTypeCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if (httpContext.Request.Cookies[Common.visitorType] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.visitorType].Value;
            }
            return cookie;
        }

        /// <summary>
        /// Sau khi đăng xuất, xóa uid
        /// </summary>
        /// <param name="httpContext"></param>
        public static void DeleteUserIdCookie(HttpContextBase httpContext)
        {
            HttpCookie uId = new HttpCookie(Common.userIdKey);
            uId.Value = "";
            uId.Expires = SetExpires(-1);
            //uId.HttpOnly = true;
            httpContext.Response.Cookies.Add(uId);
            return;
        }

        /// <summary>
        /// Sau khi đăng xuất, xóa VisitorType cookie
        /// </summary>
        /// <param name="httpContext"></param>
        public static void DeleteVisitorTypeCookie(HttpContextBase httpContext)
        {
            HttpCookie visitorType = new HttpCookie(Common.visitorType);
            visitorType.Value = "";
            visitorType.Expires = SetExpires(-1);
            //uId.HttpOnly = true;
            httpContext.Response.Cookies.Add(visitorType);
            return;
        }
    }
}
