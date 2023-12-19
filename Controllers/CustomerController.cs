﻿using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class CustomerController : BasicController
    {
        public CustomerMySql sqler;
        public OrderMySql ordersqler;

        public CustomerController()
        {
            sqler = new CustomerMySql();
            ordersqler = new OrderMySql();
        }

        // GET: Customer
        public ActionResult CreateCustomer()
        {
            if (AuthentCustomer() == null)
                return View();
            // Quay về trang chủ
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        public string CreateCustomer_Add(string userName, string passWord)
        {
            MySqlResultState result = sqler.AddNewCustomer(userName, passWord);
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Login()
        {
            if (AuthentCustomer() == null)
                return View();
            // Quay về trang chủ
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        public string Logout()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);
            if (!string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                sqler.CustomerLogout(cookieResult.cookieValue);
                Cookie.DeleteUserIdCookie(HttpContext);
            }
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }


        /// <summary>
        /// Vì customerInforCookie chứa unicode, bị lỗi khi lấy như cookie do chưa encode,
        /// nên ta gửi như tham số
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="customerInforCookie"></param>
        /// <returns></returns>
        [HttpPost]
        public string Login_Login(string userName, string passWord, string customerInforCookie)
        {
            MySqlResultState result = sqler.LoginCustomer(userName, passWord);

            do
            {
                if (result.State != EMySqlResultState.OK)
                {
                    break;
                }

                // Set cookie cho tài khoản
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                // Lấy thông tin cutomer
                Customer customer = sqler.GetCustomerFromUserName(userName);
                if (customer == null || customer.id == -1)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Cant get customer from db";
                    break;
                }

                // Lưu cookie vào bảng tbcookie
                MySqlResultState resultInsert = sqler.CookieCustomerLogin(cookieResult.cookieValue, customer.id);
                if (resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                    break;
                }
                // Đăng nhập thành công, không xóa cookie của khách vãng lai
                //// Đang nhập thành công, lưu thông tin cookie khách vãng lai như: cart, customer information vào db
                //// Lưu cart cookie
                //List<Cart> lsCart = Cookie.GetListCartCookie(HttpContext);
                //sqler.AddCartLogin(customer.id, lsCart);
                //// Xóa cart cookie bên javascript

                //// Lưu customer information
                //List<Address> lsAddress = Cookie.GetListCustomerInforCookieFromCookieValue(customerInforCookie);
                //sqler.AddCustomerInforAddress(customer.id, lsAddress);
                //// Xóa customer info cookie bên javascript
            }
            while (false);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string UpdateAddress(string address)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = AuthentCustomer();
            if (cus != null)
            {
                Address add = JsonConvert.DeserializeObject<Address>(address);
                if(add.defaultAdd == 1)
                {
                    // Xóa default cũ nếu có
                    sqler.DeleteDefaultAddress(cus.id);
                }
                result = sqler.UpdateAddress(add);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string DeleteAddress(string address)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = AuthentCustomer();
            if (cus != null)
            {
                Address add = JsonConvert.DeserializeObject<Address>(address);
                // Chỉ xóa địa chỉ không phải mặc định
                if (add.defaultAdd != 1)
                {
                    result = sqler.DeleteAddress(add);
                }

            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string InsertAddress(string address)
        {
            MySqlResultState result = new MySqlResultState();

            Customer cus = AuthentCustomer();
            if (cus != null)
            {
                Address add = JsonConvert.DeserializeObject<Address>(address);
                if (add.defaultAdd == 1)
                {
                    // Xóa default cũ nếu có
                    sqler.DeleteDefaultAddress(cus.id);
                }
                result = sqler.InsertAddress(cus.id, add);
            }
            else
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng";
            }

            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Update()
        {
            return View();
        }

        [HttpPost]
        public string CheckUidCookieValid()
        {
            MySqlResultState result = new MySqlResultState();
            if (AuthentCustomer() == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Xác thực khách hàng không thành công.";
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string GetListAddress()
        {
            List<Address> lsAddress = new List<Address>();
            Customer cus = AuthentCustomer();
            if (cus != null)
            {
                lsAddress = sqler.GetListAddress(cus.id);
            }
            else
            {
                lsAddress = null; // Không lấy được customer trả về null
            }
            // Authent thất bại, trả về null
            return JsonConvert.SerializeObject(lsAddress);
        }

        [HttpPost]
        public string CreateCustomer_CheckValidUserName(string userName)
        {
            MySqlResultState result;
            result = sqler.CheckValidUserName(userName);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string GetCartCount()
        {
            Customer cus = AuthentCustomer();
            MySqlResultState result = new MySqlResultState();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
            }
            else
            {
                result = ordersqler.GetCartCount(cus.id);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string GetCustomer()
        {
            Customer cus = AuthentCustomer();
            if (cus == null)
            {
            }
            else
            {
                cus = sqler.GetCustomer(cus.id);
            }
            return JsonConvert.SerializeObject(cus);
        }

        public ActionResult AccountInfor()
        {
            if (AuthentCustomer() == null)
                return View("~/Views/Customer/Login.cshtml");

            return View();
        }

        public ActionResult MyOrder()
        {
            if (AuthentCustomer() == null)
                return View("~/Views/Customer/Login.cshtml");

            return View();
        }
    }
}