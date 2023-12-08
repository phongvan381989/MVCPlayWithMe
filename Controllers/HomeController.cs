using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class HomeController : Controller
    {
        public ItemModelMySql itemModelsqler;
        public OrderMySql ordersqler;
        public CustomerMySql customersqler;
        public AdministrativeAddressMySql admiAddsqler;

        public HomeController()
        {
            itemModelsqler = new ItemModelMySql();
            ordersqler = new OrderMySql();
            customersqler = new CustomerMySql();
            admiAddsqler = new AdministrativeAddressMySql();
        }
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //// Xóa bỏ item có 1 model bất kỳ chưa được mapping sản phẩm trong kho
        //private void RemoveItemHasNoMapping(List<Item>  listItem)
        //{
        //    bool isNoMapping = false;
        //    int length = listItem.Count();

        //    for(int i = length -1; i >= 0; i--)
        //    {
        //        Item item = listItem[i];
        //        isNoMapping = false;
        //        foreach(var model in item.models)
        //        {
        //            if(model.mapping.Count() == 0)
        //            {
        //                isNoMapping = true;
        //                break;
        //            }
        //        }

        //        if(isNoMapping)
        //        {
        //            listItem.RemoveAt(i);
        //        }
        //    }
        //}

        // GET: AllProduts
        public ActionResult Index()
        {
            //ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            //searchParameter.hasMapping = 1;
            // Lấy 20 sản phẩm
            //List<Item> listItem = sqler.SearchItemChangePage(searchParameter);
            //RemoveItemHasNoMapping(listItem);
            return View(/*listItem*/);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm trên sàn
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public string SearchItemCount(string namePara, int hasMapping)
        {
            // Đếm số sản phẩm trong kết quả tìm kiếm
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = namePara;
            searchParameter.hasMapping = hasMapping;
            int count = 0;
            count = itemModelsqler.SearchItemCount(searchParameter);
            return count.ToString();
        }

        [HttpGet]
        public string ChangePage(string namePara, int hasMapping, int start, int offset)
        {
            List<Item> lsSearchResult;
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = namePara;
            searchParameter.start = start;
            searchParameter.offset = offset;
            searchParameter.hasMapping = hasMapping;
            lsSearchResult = itemModelsqler.SearchItemChangePage(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public ActionResult Item(int id)
        {
            ViewData["itemObject"] = JsonConvert.SerializeObject(itemModelsqler.GetItemFromId(id));
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uidCookie"></param>
        /// <param name="modelId"></param>
        /// <param name="quantity">số lượng khách muốn chọn mua</param>
        /// <param name="maxQuantity">số lượng max khách có thể chọn</param>
        /// <returns></returns>
        [HttpPost]
        public string Item_AddModelToCart(string cartObj, int maxQuantity)
        {
            MySqlResultState result = new MySqlResultState();
            // Lấy thông tin cutomer
            Customer customer = customersqler.GetCustomerFromCookie(Cookie.GetUserIdCookie(HttpContext).cookieValue);
            if(customer == null)
            {
                result.State = EMySqlResultState.DONT_EXIST;
                result.Message = "Không lấy được thông tin khách hàng.";
                return JsonConvert.SerializeObject(result);
            }
            Cart cart = JsonConvert.DeserializeObject<Cart>(cartObj);
            result = customersqler.AddCart(customer.id, cart, maxQuantity);
            return JsonConvert.SerializeObject(result);
        }


        public ActionResult Cart()
        {
            // Check khách vãng lai hay đã đăng nhập

            // Xử lý nếu là khách vãng lai
            if (ViewData["listCartCookieObject"] == null)
            {
                // Lấy cart cookie
                List<Cart> ls = Cookie.GetListCartCookie(HttpContext);
                // Không được gọi từ click mua ngay thì mặc định tất cả giỏ hàng có real = 0;

                foreach (var cart in ls)
                {
                    cart.real = 0;
                }

                ordersqler.GetCart(ls);
                ViewData["listCartCookieObject"] = JsonConvert.SerializeObject(ls);
            }
            return View();
        }

        public ActionResult CartBuyNow()
        {
            // Check khách vãng lai hay đã đăng nhập

            // Xử lý nếu là khách vãng lai
            // Lấy cart cookie
            List<Cart> ls = Cookie.GetListCartCookie(HttpContext);
            ordersqler.GetCart(ls);
            ViewData["listCartCookieObject"] = JsonConvert.SerializeObject(ls);
            return View("~/Views/Home/Cart.cshtml");
            //return View("~/Views/Administrator/Index.cshtml");
        }

        //// Lấy sản phẩm khách chọn mua, real = 1;
        //private List<CartCookie> GetRealCartCookie(List<CartCookie> ls)
        //{
        //    List<CartCookie> lsRealCartCookie = new List<CartCookie>();
        //    foreach (var cart in ls)
        //    {
        //        if (cart.real == 1)
        //        {
        //            lsRealCartCookie.Add(cart);
        //        }
        //    }
        //    return lsRealCartCookie;
        //}

        // Danh sách sản phẩm đã chọn mua, phí vận chuyển,
        // giảm giá thêm: giảm giá cho khách quen, giảm giá cho đơn lơn hơn 500k,...
        // cart: encode base64
        public ActionResult Checkout(string cart)
        {
            // Check khách vãng lai hay đã đăng nhập

            // Xử lý nếu là khách vãng lai
            List<Cart> lsRealCartCookie = null;
            if (!string.IsNullOrEmpty(cart))
            {
                // Decode base64
                var base64EncodedBytes = System.Convert.FromBase64String(cart);
                string decodeCart = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                lsRealCartCookie = Cookie.GetListCartCookieFromCookieValue(decodeCart);
            }

            ordersqler.GetCart(lsRealCartCookie);
            ViewData["listCartCookieObject"] = JsonConvert.SerializeObject(lsRealCartCookie);

            // Lấy customer infor cookie cho khách đăng nhập
            //List<CustomerInforCookie> lsCustomerInforCookie = Cookie.GetListCustomerInforCookie(HttpContext);
            //ViewData["listCustomerInfotCookieObject"] = JsonConvert.SerializeObject(lsCustomerInforCookie);
            //ViewData["listCustomerInfotCookieObject"] = "";

            return View();
        }

        [HttpPost]
        public string GetAdministrativeAddress()
        {
            List<AdministrativeAddress> ls = admiAddsqler.GetListAdministrativeAddress();
            return JsonConvert.SerializeObject(ls);
        }

        // Check id model đúng, check số lượng cần mua có đủ, check giá bìa, giá bán thực tế có chính xác
        private CommonResult CheckCart(List<Cart> ls)
        {
            CommonResult result = new CommonResult();
            int length = ls.Count();
            if(length == 0)
            {
                result.status = false;
                result.message = "Giỏ hàng trống.";
                MyLogger.GetInstance().Warn("result: " + JsonConvert.SerializeObject(result));
                return result;
            }

            List<Cart> lsTemp = new List<Cart>();
            for(int i = 0; i < length; i++)
            {
                var cart = ls[i];
                lsTemp.Add(new Cart(cart.id, cart.q, cart.real));
            }

            // Lấy dữ liệu mới nhất
            ordersqler.GetCart(lsTemp);
            int indexWarning = 0;
            for (int i = 0; i < length; i++)
            {
                var cart = ls[i];
                var cartTemp = lsTemp[i];

                // Check id model đúng
                if (cartTemp.itemId == 0)
                {
                    result.status = false;
                    result.message = "Sản phẩm: " + cart.itemName + " - " + cart.modelName + 
                        " không lấy được thông tin. Vui lòng tải lại trang và kiểm tra";
                    indexWarning = i;
                    break;
                }

                // Check số lượng cần mua có đủ
                if (cartTemp.q < cart.q)
                {
                    result.status = false;
                    result.message = "Sản phẩm: " + cart.itemName + " - " + cart.modelName +
                        " số lượng tồn kho không đủ. Vui lòng chọn lại";
                    indexWarning = i;
                    break;
                }

                // Check giá thực tế có đúng
                if (cartTemp.price != cart.price)
                {
                    result.status = false;
                    result.message = "Sản phẩm: " + cart.itemName + " - " + cart.modelName +
                        " giá không đúng. Vui lòng tải lại trang và kiểm tra";
                    indexWarning = i;
                    break;
                }
            }

            if(!result.status)
            {
                MyLogger.GetInstance().Warn("cart web sent to server: " + JsonConvert.SerializeObject(ls[indexWarning]));
                MyLogger.GetInstance().Warn("cart get from db to check: " + JsonConvert.SerializeObject(lsTemp[indexWarning]));
                MyLogger.GetInstance().Warn("result: " + JsonConvert.SerializeObject(result));

            }
            return result;
        }

        // Cần kiểm tra vì khách có thể f12 trên web, sửa javascipt, html
        public string CheckOrderOnSever(string cart, string customerInfor, string shipFee, string noteToShop)
        {
            CommonResult result = null;
            // Có thể lấy qua get cookie
            List<Cart> lsCart = JsonConvert.DeserializeObject <List<Cart>>(cart);
            CustomerInforCookie cusInfor = JsonConvert.DeserializeObject<CustomerInforCookie>(customerInfor);
            int ship = Common.ConvertStringToInt32(shipFee);
            string note = noteToShop;
            // Kiểm tra cart
            result = CheckCart(lsCart);
            if(!result.status)
            {
                return JsonConvert.SerializeObject(result);
            }

            // Với khách đăng nhập
            {
                // cập nhật lại cart trên db
            }

            // insert order
            int orderId = ordersqler.AddOrder(-1, ship, (int)EShopeeOrderStatus.UNPAID, noteToShop, cusInfor);
            if(orderId == -1)
            {
                result.status = false;
                result.message = "Không tạo được đơn hàng.";
                return JsonConvert.SerializeObject(result);
            }

            // insert track order
            ordersqler.AddTrackOrder(orderId, 0);

            // insert detail order
            ordersqler.AddDetailOrder(orderId, lsCart);

            return JsonConvert.SerializeObject(result);
        }
    }
}