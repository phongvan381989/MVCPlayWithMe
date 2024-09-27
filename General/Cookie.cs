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
        public static void SetVisitorTypeCookie(HttpContextBase httpContext)
        {
            HttpCookie visitorType = new HttpCookie(Common.visitorType);
            visitorType.Value = "admin";
            visitorType.Expires = SetExpires(100);
            //uId.HttpOnly = true;

            httpContext.Response.Cookies.Add(visitorType);
        }

        /// <summary>
        /// Với khách vãng lai, khi mua thành công thêm id mã đơn hàng
        /// ex: 13#43#....#466
        /// </summary>
        /// <param name="httpContext"></param>
        public static void SetOrderListCookie(HttpContextBase httpContext, int id)
        {
            string value = "";
            if (httpContext.Request.Cookies[Common.orderIdList] != null)
            {
                value = httpContext.Request.Cookies[Common.orderIdList].Value;
                if(string.IsNullOrEmpty(value))
                {
                    value = id.ToString();
                }
                else
                {
                    value = value + "#" + id.ToString();
                }
            }
            HttpCookie visitorType = new HttpCookie(Common.orderIdList);
            visitorType.Value = value;
            visitorType.Expires = SetExpires(100);
            //uId.HttpOnly = true;

            httpContext.Response.Cookies.Add(visitorType);
        }

        public static CookieResultState GetUserIdCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if (httpContext.Request.Cookies[Common.userIdKey] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.userIdKey].Value;
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

        public static CookieResultState GetCartCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if(httpContext.Request.Cookies[Common.cartKey] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.cartKey].Value;
            }
            return cookie;
        }

        public static CookieResultState GetCustomerInforCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if (httpContext.Request.Cookies[Common.customerInforKey] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.customerInforKey].Value;
            }
            return cookie;
        }

        // cookie có dạng: cart=id=123#q=10#real=1$id=321#q=1#real=0$....$id=321#q=2#real=0
        public static List<Cart> GetListCartCookieFromCookieValue(string cartCookie)
        {
            List<Cart> listCartCookie = new List<Cart>();
            if (string.IsNullOrEmpty(cartCookie))
                return listCartCookie;

            string[] myArray = cartCookie.Split('$');
            for (int i = 0; i < myArray.Length; i++)
            {
                listCartCookie.Add(new Cart(myArray[i]));
            }

            return listCartCookie;
        }

        public static List<Cart> GetListCartCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = GetCartCookie(httpContext);
            return GetListCartCookieFromCookieValue(cookie.cookieValue);
        }

        // cookie có dạng: name=Hoàng Huệ#phone=0359127226#province=Hà Nội#district=Bắc Từ Liêm#subdistrict=Cổ Nhuế 2#detail=Số 24 , Ngõ Việt Hà 2, khu tập thể Việt Hà, tổ dân phố Phú Minh#defaultAdd=1
        public static List<Address> GetListCustomerInforCookieFromCookieValue(string customerInforCookie)
        {
            List<Address> listCustomerInforCookie = new List<Address>();
            if (string.IsNullOrEmpty(customerInforCookie))
                return listCustomerInforCookie;

            string[] myArray = customerInforCookie.Split('$');
            for (int i = 0; i < myArray.Length; i++)
            {
                listCustomerInforCookie.Add(new Address(myArray[i]));
            }
            return listCustomerInforCookie;
        }

        public static List<Address> GetListCustomerInforCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = GetCustomerInforCookie(httpContext);
            return GetListCustomerInforCookieFromCookieValue(cookie.cookieValue);
        }

        public static CookieResultState GetItemOnRowCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if (httpContext.Request.Cookies[Common.itemOnRowSearchPage] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.itemOnRowSearchPage].Value;
            }
            return cookie;
        }

        public static CookieResultState GetOrderListCookie(HttpContextBase httpContext)
        {
            CookieResultState cookie = new CookieResultState();
            if (httpContext.Request.Cookies[Common.orderIdList] != null)
            {
                cookie.cookieValue = httpContext.Request.Cookies[Common.orderIdList].Value;
            }
            return cookie;
        }
    }
}