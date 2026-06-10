using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.Order;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class HomeController : BasicController
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

        [HttpGet]
        public async Task<ActionResult> Search(string keyword, int? page)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();

                ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
                searchParameter.name = keyword;
                int count = await itemModelsqler.SearchItemCountConnectOutAsync(searchParameter, conn);
                ViewData["dataCountResult"] = count.ToString();

                List<Item> lsSearchResult;
                int intPage = 1;
                if (page != null)
                    intPage = page.Value;

                int itemOnRow = Common.ConvertStringToInt32(Cookie.GetItemOnRowCookie(HttpContext).cookieValue);
                if (itemOnRow == System.Int32.MinValue)
                {
                    MyLogger.GetInstance().Info("Search call keyword: " + keyword + ", page" + page);
                    itemOnRow = Common.itemOnRowDefault;
                }
                searchParameter.offset = itemOnRow * Common.rowOnPage;
                searchParameter.start = (intPage - 1) * searchParameter.offset;
                lsSearchResult = await itemModelsqler.SearchItemPageConnectOutAsync(searchParameter, conn);
                ViewData["dataListItem"] = JsonConvert.SerializeObject(lsSearchResult);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }

        // Trả về khi click button tìm kiếm item
        // Object trả về gồm cả số lượng kết quả
        [HttpGet]
        public async Task<string> HomeSearch(string keyword, int? page)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();

                ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
                searchParameter.name = keyword;
                int count = await itemModelsqler.SearchItemCountConnectOutAsync(searchParameter, conn);

                List<Item> lsSearchResult;
                int intPage = 1;
                if (page != null)
                    intPage = page.Value;

                int itemOnRow = Common.ConvertStringToInt32(Cookie.GetItemOnRowCookie(HttpContext).cookieValue);
                searchParameter.offset = itemOnRow * Common.rowOnPage;
                searchParameter.start = (intPage - 1) * searchParameter.offset;
                lsSearchResult = await itemModelsqler.SearchItemPageConnectOutAsync(searchParameter, conn);

                StringBuilder sb = new StringBuilder();
                sb.Append("{\"countResult\":" + count.ToString() + ",\"listItem\":" + JsonConvert.SerializeObject(lsSearchResult) + @"}");
                return sb.ToString();
            }
        }

        [HttpGet]
        public async Task<string> SearchPage(string keyword, int page)
        {
            ItemModelSearchParameter searchParameter = new ItemModelSearchParameter();
            searchParameter.name = keyword;

            int itemOnRow = Common.ConvertStringToInt32(Cookie.GetItemOnRowCookie(HttpContext).cookieValue);
            searchParameter.offset = itemOnRow * Common.rowOnPage;
            searchParameter.start = (page - 1) * searchParameter.offset;
            List<Item> lsSearchResult = await itemModelsqler.SearchItemPageAsync(searchParameter);
            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public ActionResult Item(int id)
        {
            // Cập nhật title bên javascript
            return View();
        }

        [HttpPost]
        public async Task<string> GetItemFromId(int id)
        {
            Item item = await itemModelsqler.GetItemFromIdAsync(id);
            if (item != null)
            {
                item.SetShopeeItemId();
            }
            return JsonConvert.SerializeObject(item);
        }

        [HttpPost]
        public async Task<string> Item_AddModelToCart(string cartObj, int maxQuantity)
        {
            MySqlResultState result = new MySqlResultState();
            Customer customer = await AuthentCustomerAsync();
            if (customer == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng.";
                return JsonConvert.SerializeObject(result);
            }
            Cart cart = JsonConvert.DeserializeObject<Cart>(cartObj);
            result = await customersqler.AddCartAsync(customer.id, cart, maxQuantity);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> CartPageLoadCart()
        {
            Customer cus = await AuthentCustomerAsync();
            List<Cart> ls = null;
            if (cus == null)
            {
                // Khách vãng lai
                ls = Cookie.GetListCartCookie(HttpContext);
                await ordersqler.GetCartAsync(ls);
            }
            else
            {
                // Khách đăng nhập
                ls = await ordersqler.GetListCartAsync(cus.id);
                await ordersqler.GetCartAsync(ls);
                // Làm mới real = 0
                await ordersqler.RefreshRealOfCartAsync(cus.id);
            }
            return JsonConvert.SerializeObject(ls);
        }

        public ActionResult Cart()
        {
            ViewData["title"] = "Giỏ hàng";
            return View();
        }

        // Danh sách sản phẩm đã chọn mua, phí vận chuyển,
        // giảm giá thêm: giảm giá cho khách quen, giảm giá cho đơn lơn hơn 500k,...
        // cart: encode base64
        [HttpPost]
        public async Task<string> CheckoutPageLoadCart(string cart)
        {
            List<Cart> lsRealCartCookie = new List<Cart>();
            if (!Common.ParameterOfURLQueryIsNullOrEmpty(cart))
            {
                var base64EncodedBytes = System.Convert.FromBase64String(cart);
                string decodeCart = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                lsRealCartCookie = Cookie.GetListCartCookieFromCookieValue(decodeCart);
                await ordersqler.GetCartAsync(lsRealCartCookie);
            }
            return JsonConvert.SerializeObject(lsRealCartCookie);
        }

        // Danh sách sản phẩm đã chọn mua, phí vận chuyển,
        // giảm giá thêm: giảm giá cho khách quen, giảm giá cho đơn lơn hơn 500k,...
        // cart: encode base64
        public ActionResult Checkout(string cart)
        {
            ViewData["title"] = "Thông tin đơn hàng trước khi mua";
            return View();
        }

        [HttpPost]
        public async Task<string> GetAdministrativeAddress()
        {
            List<AdministrativeAddress> ls = await admiAddsqler.GetListAdministrativeAddressAsync();
            return JsonConvert.SerializeObject(ls);
        }

        // Check id model đúng, check số lượng cần mua có đủ, check giá bìa, giá bán thực tế có chính xác
        private async Task<MySqlResultState> CheckCartAsync(List<Cart> ls)
        {
            MySqlResultState result = new MySqlResultState();
            int length = ls.Count();
            if (length == 0)
            {
                result.State = EMySqlResultState.EMPTY;
                result.Message = "Giỏ hàng trống.";
                MyLogger.GetInstance().Warn("result: " + JsonConvert.SerializeObject(result));
                return result;
            }

            List<Cart> lsTemp = new List<Cart>();
            for (int i = 0; i < length; i++)
            {
                var cart = ls[i];
                lsTemp.Add(new Cart(cart.id, cart.q, cart.real));
            }

            // Lấy dữ liệu mới nhất
            await ordersqler.GetCartAsync(lsTemp);
            int indexWarning = 0;
            for (int i = 0; i < length; i++)
            {
                var cart = ls[i];
                var cartTemp = lsTemp[i];

                // Check id model đúng
                if (cartTemp.itemId == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Sản phẩm: " + cart.itemName + " - " + cart.modelName +
                        " không lấy được thông tin. Vui lòng tải lại trang và kiểm tra";
                    indexWarning = i;
                    break;
                }

                // Check số lượng cần mua có đủ
                if (cartTemp.q < cart.q)
                {
                    result.State = EMySqlResultState.OVER_MAX;
                    result.Message = "Sản phẩm: " + cart.itemName + " - " + cart.modelName +
                        " số lượng tồn kho không đủ. Vui lòng chọn lại";
                    indexWarning = i;
                    break;
                }

                // Check giá thực tế có đúng
                if (cartTemp.price != cart.price)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Sản phẩm: " + cart.itemName + " - " + cart.modelName +
                        " giá không đúng. Vui lòng tải lại trang và kiểm tra";
                    indexWarning = i;
                    break;
                }
            }

            if (result.State != EMySqlResultState.OK)
            {
                MyLogger.GetInstance().Warn("cart web sent to server: " + JsonConvert.SerializeObject(ls[indexWarning]));
                MyLogger.GetInstance().Warn("cart get from db to check: " + JsonConvert.SerializeObject(lsTemp[indexWarning]));
                MyLogger.GetInstance().Warn("result: " + JsonConvert.SerializeObject(result));
            }
            return result;
        }

        // Cần kiểm tra vì khách có thể f12 trên web, sửa javascipt, html
        [HttpPost]
        public async Task<string> CheckOrderOnSever(string cart, string customerInfor,
            string listOrderPay, string noteToShop)
        {
            MySqlResultState result = null;
            List<Cart> lsBuyedCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
            Address cusInfor = JsonConvert.DeserializeObject<Address>(customerInfor);
            List<OrderPay> lsOrderPay = JsonConvert.DeserializeObject<List<OrderPay>>(listOrderPay);

            // Kiểm tra cart
            result = await CheckCartAsync(lsBuyedCart);
            if (result.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(result);
            }

            // Với khách đăng nhập
            Customer cus = await AuthentCustomerAsync();

            // insert order
            int orderId;
            if (cus != null)
                orderId = await ordersqler.AddOrderAsync(cus.id, noteToShop, 0, cusInfor);
            else
                orderId = await ordersqler.AddOrderAsync(-1, noteToShop, 0, cusInfor);

            if (orderId == -1)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Không tạo được đơn hàng.";
                return JsonConvert.SerializeObject(result);
            }

            // khách vãng lai ta thêm order id vào cookie
            if (cus == null)
            {
                Cookie.SetOrderListCookie(HttpContext, orderId);
            }

            if (cus != null)
            {
                // Xóa sản phẩm trong đơn hàng khỏi cart
                result = await ordersqler.DeleteListCartAsync(cus.id, lsBuyedCart);
                if (result.State != EMySqlResultState.OK)
                {
                    return JsonConvert.SerializeObject(result);
                }
            }

            // insert track order
            await ordersqler.AddTrackOrderAsync(orderId, 0);

            // insert detail order
            await ordersqler.AddDetailOrderAsync(orderId, lsBuyedCart);

            // insert pay order
            await ordersqler.AddPayOrderAsync(orderId, lsOrderPay);

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> DeleteOneCart(int modelId)
        {
            Customer cus = await AuthentCustomerAsync();
            MySqlResultState result = new MySqlResultState();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
            }
            else
            {
                result = await ordersqler.DeleteOneCartAsync(cus.id, modelId);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateCartQuantity(int modelId, int quantity)
        {
            Customer cus = await AuthentCustomerAsync();
            MySqlResultState result = new MySqlResultState();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
            }
            else
            {
                result = await ordersqler.UpdateCartQuantityAsync(cus.id, modelId, quantity);
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}
