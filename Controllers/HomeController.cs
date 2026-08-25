using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MySqlConnector;
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
        [HttpGet]
        public async Task<ActionResult> Search()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }


        // Trả về khi click button tìm kiếm item
        // Object trả về gồm cả số lượng kết quả
        /// <summary>
        /// Load More API - Keyset pagination với cursor (lastId)
        /// Initial load: lastId = 0, limit = 30
        /// Load more: lastId = Id của item cuối, limit = 30
        /// page: Trang hiện tại (optional, dùng để track/log)
        /// </summary>
        [HttpGet]
        public async Task<string> HomeSearch(string keyword,
            string author,
            string translator,
            string category,
            string publishingCompany,
            string publisher,
            int? lastId,
            int? limit,
            int? page)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    SanPhamSearchParameter searchParameter = new SanPhamSearchParameter();
                    searchParameter.name = keyword;
                    searchParameter.author = author;
                    searchParameter.translator = translator;
                    searchParameter.category = category;
                    searchParameter.publishingCompany = publishingCompany;
                    searchParameter.publisher = publisher;
                    searchParameter.lastId = lastId;
                    searchParameter.page = page;
                    searchParameter.limit = limit;

                    var (lsSearchResult, hasMore) = await SanPhamMySql.SearchSanPhamWithCursorAsync(
                        searchParameter,
                        conn);

                    // Return: items, hasMore
                    result.State = EMySqlResultState.OK;
                    result.myJson = new
                    {
                        hasMore = hasMore,
                        loadedCount = lsSearchResult.Count,
                        lsSearch = lsSearchResult
                    };
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public async Task<ActionResult> Item(string slugId)
        {
            //// Parse ID từ slugId (format: slug-123)
            //int id = ParseIdFromSlugId(slugId);
            //if (id <= 0)
            //{
            //    return RedirectToAction("Error");
            //}

            //// Lấy item để kiểm tra tồn tại
            //Item item = await ItemModelMySql.GetItemFromIdAsync(id);
            //if (item == null)
            //{
            //    return RedirectToAction("Error");
            //}

            //// Tạo slug chuẩn từ tên item
            //string correctSlug = Common.GenerateSlug(item.name);
            //string correctSlugId = correctSlug + "-" + id;

            //// Nếu slug không đúng, redirect về URL chuẩn (SEO 301)
            //if (!string.Equals(slugId, correctSlugId, StringComparison.OrdinalIgnoreCase))
            //{
            //    return RedirectToActionPermanent("Item", new { slugId = correctSlugId });
            //}

            // Cập nhật title bên javascript
            //ViewBag.ItemId = id;
            return View();
        }

        /// <summary>
        /// Redirect từ URL cũ /Home/Item/123 sang URL mới /item/slug-123
        /// </summary>
        //[HttpGet]
        //public async Task<ActionResult> ItemRedirect(int id)
        //{
        //    try
        //    {
        //        Item item = await ItemModelMySql.GetItemFromIdAsync(id);
        //        if (item == null)
        //        {
        //            return RedirectToAction("Error");
        //        }

        //        string slug = Common.GenerateSlug(item.name);
        //        string slugId = slug + "-" + id;

        //        return RedirectToActionPermanent("Item", new { slugId = slugId });
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Error");
        //    }
        //}

        /// <summary>
        /// Parse ID từ slugId
        /// VD: "doraemon-tap-1-123" -> 123
        /// </summary>
        private int ParseIdFromSlugId(string slugId)
        {
            if (string.IsNullOrWhiteSpace(slugId))
                return -1;

            int id;
            // Tìm dấu - cuối cùng
            int lastDashIndex = slugId.LastIndexOf('-');
            if (lastDashIndex < 0 || lastDashIndex == slugId.Length - 1) {
                if (int.TryParse(slugId, out id))
                    return id;
                return -1;
            }


            // Lấy phần sau dấu - cuối
            string idString = slugId.Substring(lastDashIndex + 1);


            if (int.TryParse(idString, out id))
                return id;

            return -1;
        }

        [HttpPost]
        public async Task<string> GetItemFromId(int id)
        {
            Item item = await ItemModelMySql.GetItemFromIdAsync(id);
            if (item != null)
            {
                item.SetShopeeItemId();
            }
            return JsonConvert.SerializeObject(item);
        }

        /// <summary>
        /// Trang chi tiết sản phẩm cho người mua (tb_san_pham)
        /// URL format: /Home/SanPham/ten-sach-123
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> SanPham(string slugId)
        {
            // Parse ID từ slugId (format: slug-123)
            int id = ParseIdFromSlugId(slugId);
            if (id <= 0)
            {
                return RedirectToAction("Error");
            }

            //// Lấy sản phẩm để kiểm tra tồn tại
            //SanPham sanPham = await SanPhamMySql.GetByIdAsync(id);
            //if (sanPham == null)
            //{
            //    return RedirectToAction("Error");
            //}

            //// Tạo slug chuẩn từ tên sản phẩm
            //string correctSlug = Common.GenerateSlug(sanPham.Name);
            //string correctSlugId = correctSlug + "-" + id;

            //// Nếu slug không đúng, redirect về URL chuẩn (SEO 301)
            //if (!string.Equals(slugId, correctSlugId, StringComparison.OrdinalIgnoreCase))
            //{
            //    return RedirectToActionPermanent("SanPham", new { slugId = correctSlugId });
            //}

            return View();
        }

        /// <summary>
        /// API lấy danh sách sản phẩm cùng ComboId (bao gồm cả sản phẩm với ID được query)
        /// Tối ưu: Chỉ 1 stored procedure call, return list
        /// JavaScript sẽ tự tìm sản phẩm chính theo ID
        /// </summary>
        [HttpPost]
        public async Task<string> GetSanPhamWithVariants(int id)
        {
            // Gọi 1 stored procedure duy nhất để lấy danh sách variants (bao gồm sản phẩm chính)
            List<SanPham> variants = await SanPhamMySql.GetSanPhamWithVariantsAsync(id);

            if (variants == null || variants.Count == 0)
            {
                return "null";
            }

            // Return list, JavaScript sẽ tự tìm sản phẩm chính
            return JsonConvert.SerializeObject(variants);
        }

        // Nguyên tắc: real luôn luôn = 0 trong db, sản phẩm nào được chọn trên giao diện sẽ gửi riêng
        [HttpPost]
        public async Task<string> AddSanPhamToCart(int sanPhamId, int quantity/*, int real*/)
        {
            MySqlResultState result = new MySqlResultState();
            Customer customer = await AuthentCustomerAsync();
            if (customer == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng.";
                return JsonConvert.SerializeObject(result);
            }

            //// Làm mới dữ liệu trước đó real = 0
            //await OrderMySql.RefreshRealOfCartAsync(customer.id);

            Cart cart = new Cart();
            cart.sanPhamId = sanPhamId;
            cart.quantity = quantity;
            //cart.real = real;
            result = await CustomerMySql.AddCartAsync(customer.id, cart);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> CartPageLoadCart()
        {
            Customer cus = await AuthentCustomerAsync();
            List<Cart> ls = null;
            // Đọc cart từ request body (JSON)
            List<Cart> lslocalStorage = await Common.ReadJsonFromRequestBody<List<Cart>>(Request);
            if (cus!= null)
            {
                // Khách đăng nhập - đọc từ DB
                ls = await OrderMySql.GetListCartAsync(cus.id);

                // Tìm sản phẩm được chọn mua
                foreach (Cart cart in lslocalStorage)
                {
                    foreach (Cart cartDb in ls)
                    {
                        if (cart.sanPhamId == cartDb.sanPhamId)
                        {
                            cartDb.real = cart.real;
                            break;
                        }
                    }
                }
            }
            else
            {
                ls = lslocalStorage;
            }    
            await OrderMySql.GetCartsSanPhamBasicInfoAsync(ls);

            return JsonConvert.SerializeObject(ls);
        }

        public ActionResult Cart()
        {
            ViewData["title"] = "Giỏ Hàng";
            return View();
        }

        [HttpPost]
        public async Task<string> CheckoutPageLoadCart()
        {
            // Đọc cart từ request body (JSON)
            List<Cart> lslocalStorage = await Common.ReadJsonFromRequestBody<List<Cart>>(Request);
            await OrderMySql.GetCartsSanPhamBasicInfoAsync(lslocalStorage);

            return JsonConvert.SerializeObject(lslocalStorage);
        }

        // Danh sách sản phẩm đã chọn mua, phí vận chuyển,
        // giảm giá thêm: giảm giá cho khách quen, giảm giá cho đơn lơn hơn 500k,...

        public async Task<ActionResult> Checkout()
        {
            ViewData["title"] = "Thanh Toán";

            // Load bank account info để hiển thị cho payment method = BANK_TRANSFER
            var bankAccount = await MVCPlayWithMe.Models.BankAccount.BankAccountMySql.GetActiveBankAccountAsync();
            ViewBag.BankAccount = bankAccount;

            // Pass orderDeadline để hiển thị hạn thanh toán
            ViewBag.OrderDeadline = Common.orderDeadline;

            return View();
        }

        [HttpPost]
        public async Task<string> GetAdministrativeAddress()
        {
            List<AdministrativeAddress> ls = await AdministrativeAddressMySql.GetListAdministrativeAddressAsync();
            return JsonConvert.SerializeObject(ls);
        }

        // Check id model đúng, check số lượng cần mua có đủ, check giá bìa, giá bán thực tế có chính xác
        /// <summary>
        /// LAYER 1: Validate từng sản phẩm trong giỏ hàng
        /// - Kiểm tra sản phẩm có tồn tại
        /// - Kiểm tra GIÁ từ client vs DB (CRITICAL - chống hack giá!)
        /// - Kiểm tra số lượng tồn kho
        /// </summary>
        private void CheckCartValidation(
            List<Cart> cartListFromClient,
            List<SanPhamBasicInfo> SanPhamBasicInfos,
            string messageWhenValidationFail,
            MySqlResultState result)
        {
            // Với tồn kho không còn đủ, kiểm tra cả danh sách
            string notEnought = string.Empty;
            // Validate từng sản phẩm
            foreach (var cart in cartListFromClient)
            {
                // 1. LẤY THÔNG TIN THẬT TỪ DATABASE (KHÔNG TIN CLIENT!)
                SanPhamBasicInfo sanPhamFromDb = SanPhamBasicInfos.Find(item=>item.Id == cart.sanPhamId);

                if (sanPhamFromDb == null)
                {
                    result.State = EMySqlResultState.DONT_EXIST;
                    result.Message = "Thông tin giỏ hàng đã thay đổi. Vui lòng tải lại trang.";
                    MyLogger.GetInstance().Warn($"⚠️ PRODUCT NOT FOUND - SanPhamId={cart.sanPhamId}");
                    return;
                }

                // 2. VALIDATE GIÁ bán (CRITICAL - CHỐNG HACK!)
                if (cart.sanPhamBasicInfo == null ||
                    cart.sanPhamBasicInfo.SalePrice == null ||
                    cart.sanPhamBasicInfo.SalePrice != sanPhamFromDb.SalePrice)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = messageWhenValidationFail;

                    // Log chi tiết để admin phát hiện hack attempt
                    MyLogger.GetInstance().Warn($"🚨 PRICE MISMATCH DETECTED!");
                    MyLogger.GetInstance().Warn($"   SanPhamId: {cart.sanPhamId}");
                    MyLogger.GetInstance().Warn($"   SanPham: {sanPhamFromDb.Name}");
                    MyLogger.GetInstance().Warn($"   Client Price: {cart.sanPhamBasicInfo.SalePrice:N0}đ");
                    MyLogger.GetInstance().Warn($"   DB Price: {sanPhamFromDb.SalePrice:N0}đ");
                    MyLogger.GetInstance().Warn($"   Difference: {Math.Abs(cart.sanPhamBasicInfo.SalePrice - sanPhamFromDb.SalePrice):N0}đ");
                    MyLogger.GetInstance().Warn("client cart: " + JsonConvert.SerializeObject(cart));
                    MyLogger.GetInstance().Warn("sanPhamFromDb : " + JsonConvert.SerializeObject(sanPhamFromDb));

                    return; // CHẶN NGAY!
                }
                // KHông check giá bìa vì không ảnh hưởng

                // 3. VALIDATE SỐ LƯỢNG TỒN KHO
                if (sanPhamFromDb.Quantity < cart.quantity)
                {
                    //result.State = EMySqlResultState.OVER_MAX;
                    //result.Message = $"'{sanPhamFromDb.Name}' chỉ còn {sanPhamFromDb.Quantity} sản phẩm. Vui lòng chọn lại.";
                    notEnought = notEnought + $"'{sanPhamFromDb.Name}' chỉ còn {sanPhamFromDb.Quantity} sản phẩm.\n";

                    MyLogger.GetInstance().Warn($"⚠️ INSUFFICIENT STOCK - {sanPhamFromDb.Name}");
                    MyLogger.GetInstance().Warn($"   Requested: {cart.quantity}, Available: {sanPhamFromDb.Quantity}");
                    MyLogger.GetInstance().Warn("client cart: " + JsonConvert.SerializeObject(cart));
                    MyLogger.GetInstance().Warn("sanPhamFromDb : " + JsonConvert.SerializeObject(sanPhamFromDb));
                }

                // 4. (Optional) Validate sản phẩm còn kinh doanh
                if (sanPhamFromDb.Status != 0) // Ngừng kinh doanh
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = $"Sản phẩm '{sanPhamFromDb.ShortName ?? sanPhamFromDb.Name}' đã ngừng kinh doanh.";
                    MyLogger.GetInstance().Warn($"⚠️ PRODUCT DISCONTINUED - {sanPhamFromDb.Name}");
                    MyLogger.GetInstance().Warn("client cart: " + JsonConvert.SerializeObject(cart));
                    MyLogger.GetInstance().Warn("sanPhamFromDb : " + JsonConvert.SerializeObject(sanPhamFromDb));

                    return;
                }

                // Validate tên sản phẩm
                if(sanPhamFromDb.Name != cart.sanPhamBasicInfo.Name)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = messageWhenValidationFail;

                    // Log chi tiết để admin phát hiện hack attempt
                    MyLogger.GetInstance().Warn($"🚨 NAME MISMATCH DETECTED!");
                    MyLogger.GetInstance().Warn($"   SanPham DB Name: {sanPhamFromDb.Name}");
                    MyLogger.GetInstance().Warn($"   SanPham client Name: {cart.sanPhamBasicInfo.Name}");
                    MyLogger.GetInstance().Warn("client cart: " + JsonConvert.SerializeObject(cart));
                    MyLogger.GetInstance().Warn("sanPhamFromDb : " + JsonConvert.SerializeObject(sanPhamFromDb));

                    return;
                }
            }

            if (!string.IsNullOrEmpty(notEnought))
            {
                result.State = EMySqlResultState.OVER_MAX;
                result.Message = $"{notEnought} Vui lòng chọn lại.";
                return;
            }

            MyLogger.GetInstance().Info($"✅ CheckCartAsync PASSED - {cartListFromClient.Count} items validated");
        }

        private void CheckMoneyValidation(List<OrderPay> lsOrderPay,
            int totalMoney,
            int shipFee,
            int shipFeeDiscount,
            int totalMoneyDiscount,
            int finalAmount,
            string messageWhenValidationFail,
            MySqlResultState result
            )
        {
            // So sánh tổng tiền hàng
            if (totalMoney != (lsOrderPay.Find(x => x.type == (int)EOrderPayType.TOTAL)?.value ?? 581989))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = messageWhenValidationFail;
                MyLogger.GetInstance().Warn($"🚨 total money MISMATCH!");
                MyLogger.GetInstance().Warn($"🚨 total money from DB: " + totalMoney);

                return;
            }

            // So sánh phí ship
            if (shipFee != (lsOrderPay.Find(x => x.type == EOrderPayType.SHIP)?.value ?? 581989))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = messageWhenValidationFail;
                MyLogger.GetInstance().Warn($"🚨 ship fee MISMATCH!");
                MyLogger.GetInstance().Warn($"🚨 ship fee from DB: " + shipFee);

                return;
            }

            // So sánh giảm giá phí ship
            if (shipFeeDiscount != (lsOrderPay.Find(x => x.type == EOrderPayType.PROMOTION && x.orderSimplePromotion.Type == EOrderSimplePromotionType.SHIP_DISCOUNT)?.value ?? 581989))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = messageWhenValidationFail;
                MyLogger.GetInstance().Warn($"🚨 ship fee discount MISMATCH!");
                MyLogger.GetInstance().Warn($"🚨 ship fee discount from DB: " + shipFeeDiscount);

                return;
            }

            // So sánh giảm giá tổng tiền hàng theo bậc 100k
            if (totalMoneyDiscount != (lsOrderPay.Find(x => x.type == EOrderPayType.PROMOTION && x.orderSimplePromotion.Type == EOrderSimplePromotionType.TOTAL_DISCOUNT)?.value ?? 581989))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = messageWhenValidationFail;
                MyLogger.GetInstance().Warn($"🚨 total money discount MISMATCH!");
                MyLogger.GetInstance().Warn($"🚨 total money discount from DB: " + totalMoneyDiscount);
            }

            // So sánh thanh toán cuối cùng
            if (finalAmount != (lsOrderPay.Find(x => x.type == EOrderPayType.FINAL)?.value ?? 581989))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = messageWhenValidationFail;
                MyLogger.GetInstance().Warn($"🚨 final amount MISMATCH!");
                MyLogger.GetInstance().Warn($"🚨 final amount from DB: " + finalAmount);
            }
        }
        // Tạo lsOrderPayFromDb
        private List<OrderPay> CaculateListOrderPay(
            int newOrderId,
            int totalMoney,
            int shipFee,
            int shipFeeDiscount,
            int totalMoneyDiscount,
            int finalAmount,
            List<OrderSimplePromotion> promotions
            )
        {
            List<OrderPay> lsOrderPayFromDb = new List<OrderPay>();
            lsOrderPayFromDb.Add(new OrderPay { type = EOrderPayType.TOTAL, value = totalMoney, orderId = newOrderId, orderSimplePromoId = 0 });
            lsOrderPayFromDb.Add(new OrderPay { type = EOrderPayType.SHIP, value = shipFee, orderId = newOrderId, orderSimplePromoId = 0 });

            // Khuyến mãi giảm phí ship
            {
                OrderPay orderPay = new OrderPay { type = EOrderPayType.PROMOTION, value = shipFeeDiscount, orderId = newOrderId };
                orderPay.orderSimplePromotion = promotions.Find(item => item.Type == (int)EOrderSimplePromotionType.SHIP_DISCOUNT);
                orderPay.orderSimplePromoId = orderPay.orderSimplePromotion?.Id ?? 0;
                lsOrderPayFromDb.Add(orderPay);
            }

            // Khuyến mãi giảm tổng tiền hàng
            {
                OrderPay orderPay = new OrderPay { type =EOrderPayType.PROMOTION, value = totalMoneyDiscount, orderId = newOrderId };
                orderPay.orderSimplePromotion = promotions.Find(item => item.Type == EOrderSimplePromotionType.TOTAL_DISCOUNT);
                orderPay.orderSimplePromoId = orderPay.orderSimplePromotion?.Id ?? 0;

                lsOrderPayFromDb.Add(orderPay);
            }

            lsOrderPayFromDb.Add(new OrderPay { type = EOrderPayType.FINAL, value = finalAmount, orderId = newOrderId });
            return lsOrderPayFromDb;
        }

        private async Task<string> RoolBackWhenOrderError(MySqlTransaction transaction, MySqlResultState result)
        {
            // ROLLBACK nếu có lỗi
            await transaction.RollbackAsync();
            result.Message = "Không tạo được đơn hàng. Vui lòng thử lại sau.";
            return JsonConvert.SerializeObject(result);
        }

        private async Task GetSanPhamBasicInfosFromDBAsync(
            List<SanPhamBasicInfo> sanPhamBasicInfos,
            List<Cart> lsBuyedCart,
            MySqlResultState result
            )
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    foreach (var cartItem in lsBuyedCart)
                    {
                        // Lấy giá THẬT từ DB (lần nữa để double-check)
                        SanPhamBasicInfo sanPham = await SanPhamMySql.GetSanPhamBasicInfo_ConnectOutAsync(cartItem.sanPhamId, conn);
                        if (sanPham != null)
                        {
                            sanPhamBasicInfos.Add(sanPham);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
        }

        // 1. TÍNH LẠI TỔNG TIỀN HÀNG TỪ DATABASE
        private void CalculateMoneyFromDB(
            List<Cart> lsBuyedCart,
            List<SanPhamBasicInfo> sanPhamBasicInfos,
            List<OrderSimplePromotion> promotions,
            Address cusInfor,
            ref int totalMoney,
            ref int shipFee,
            ref int shipFeeDiscount,
            ref int totalMoneyDiscount,
            ref int finalAmount,
            MySqlResultState result
            )
        {
            // 1. TÍNH LẠI TỔNG TIỀN HÀNG TỪ DATABASE
            totalMoney = 0;
            foreach (var cartItem in lsBuyedCart)
            {
                // Lấy giá THẬT từ DB (lần nữa để double-check)
                SanPhamBasicInfo sanPhamFromDb = sanPhamBasicInfos.Find(item => item.Id == cartItem.sanPhamId);
                totalMoney += sanPhamFromDb.SalePrice * cartItem.quantity;
            }

            // 2. TÍNH LẠI PHÍ SHIP TỪ DATABASE
            shipFee = 0;
            if (cusInfor.province.Contains("Hà Nội"))
            {
                shipFee = Common.standardShipFeeInHaNoi; // 15,000đ
            }
            else
            {
                shipFee = Common.standardShipFeeOutHaNoi; // 30,000đ
            }

            // 3. TÍNH LẠI DISCOUNT TỪ DATABASE (truyền shipFee để tính Type 0 - Free Ship)
            // Tách riêng 2 loại giảm giá (giống client để dễ so sánh)
            shipFeeDiscount = 0;      // Giảm phí ship (Type = 0)
            totalMoneyDiscount = 0;    // Giảm tổng tiền hàng (Type = 1)

            try
            {
                foreach (var promo in promotions)
                {
                    if (promo.Type == EOrderSimplePromotionType.SHIP_DISCOUNT)
                    {
                        // ===== TYPE 0: MIỄN PHÍ SHIP =====
                        // Điều kiện: totalMoney >= MinOrderValue (giống client)
                        // Giảm giá = shipFee (KHÔNG phải promo.Discount!)

                        if (totalMoney >= promo.MinOrderValue)
                        {
                            shipFeeDiscount = shipFee * -1; // ← Giảm bằng phí ship (giống client!)
                            //break; // Chỉ áp dụng promotion đầu tiên thỏa điều kiện
                        }
                    }
                    else if (promo.Type == EOrderSimplePromotionType.TOTAL_DISCOUNT)
                    {
                        // ===== TYPE 1: GIẢM THEO BẬC 100K =====
                        // Điều kiện: totalMoney >= MinOrderValue (STRICT >=, giống client)
                        // Công thức: ((totalMoney - MinOrderValue) / 100,000 + 1) × Discount

                        if (totalMoney >= promo.MinOrderValue)  // ← STRICT >= (giống client!)
                        {
                            int extraAmount = totalMoney - promo.MinOrderValue;
                            int multiplier = (extraAmount / 100000) + 1;
                            totalMoneyDiscount = multiplier * promo.Discount * -1;
                            //break; // Chỉ áp dụng promotion đầu tiên thỏa điều kiện
                        }
                    }
                }
                finalAmount = totalMoney + shipFee + shipFeeDiscount + totalMoneyDiscount;
                // Log breakdown để dễ debug
                //{
                //    MyLogger.GetInstance().Info($"Discount breakdown - Total: {totalMoney:N0}đ, ShipFee: {shipFee:N0}đ");
                //    if (shipFeeDiscount > 0)
                //        MyLogger.GetInstance().Info($"  ✓ Free ship discount: {shipFeeDiscount:N0}đ");
                //    if (totalMoneyDiscount > 0)
                //        MyLogger.GetInstance().Info($"  ✓ Total money discount: {totalMoneyDiscount:N0}đ");
                //}
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
        }
        // Cần kiểm tra vì khách có thể f12 trên web, sửa javascipt, html
        [HttpPost]
        public async Task<string> CheckOrderOnSever(string cart, string customerInfor,
            string listOrderPay, string noteToShop, SByte paymentMethod)
        {
            MyLogger.GetInstance().Info("CheckOrderOnSever START");
            MyLogger.GetInstance().Info("cart: " + cart);
            MyLogger.GetInstance().Info("customerInfor: " + customerInfor);
            MyLogger.GetInstance().Info("listOrderPay: " + listOrderPay);

            MySqlResultState result = new MySqlResultState();
            List<Cart> lsBuyedCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
            Address cusInfor = JsonConvert.DeserializeObject<Address>(customerInfor);
            List<OrderPay> lsOrderPay = JsonConvert.DeserializeObject<List<OrderPay>>(listOrderPay);

            if (lsBuyedCart == null || lsBuyedCart.Count == 0)
            {
                result.State = EMySqlResultState.EMPTY;
                result.Message = "Giỏ hàng trống.";
                MyLogger.GetInstance().Warn("CheckCartAsync: Giỏ hàng trống");
                return JsonConvert.SerializeObject(result);
            }

            // Lấy dữ liệu từ db 1 lần để so sánh
            // Lấy tất cả promotion đang bật
            List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsAsync();

            List<SanPhamBasicInfo> sanPhamBasicInfos = new List<SanPhamBasicInfo>();
            await GetSanPhamBasicInfosFromDBAsync(sanPhamBasicInfos, lsBuyedCart, result);
            if (result.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(result);
            }

            if (sanPhamBasicInfos.Count == 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Thông tin giỏ hàng đã thay đổi. Vui lòng tải lại trang.";

                MyLogger.GetInstance().Warn($"🚨 Cant get SanPhamBasicInfos from DB. SanPhamBasicInfos.Count == 0");
                return JsonConvert.SerializeObject(result);
            }

            string messageWhenValidationFail = "Thông tin giỏ hàng đã thay đổi. Vui lòng tải lại trang.";
            // ===== LAYER 1: VALIDATE TỪNG SẢN PHẨM =====
            CheckCartValidation(lsBuyedCart, sanPhamBasicInfos, messageWhenValidationFail, result);
            if (result.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(result);
            }

            // ===== LAYER 2: TÍNH LẠI VÀ VALIDATE TỔNG TIỀN =====
            // KHÔNG TIN DỮ LIỆU TỪ CLIENT - Tính lại hoàn toàn từ DB!

            // 1. TÍNH LẠI TIỀN TỪ DATABASE
            int totalMoney = 0;

            // 2. TÍNH LẠI PHÍ SHIP TỪ DATABASE
            int shipFee = 0;

            // 3. TÍNH LẠI DISCOUNT TỪ DATABASE (truyền shipFee để tính Type 0 - Free Ship)
            // Tách riêng 2 loại giảm giá (giống client để dễ so sánh)
            int shipFeeDiscount = 0;      // Giảm phí ship (Type = 0)
            int totalMoneyDiscount = 0;    // Giảm tổng tiền hàng (Type = 1)
            int finalAmount = 0;// Tổng tiền thanh toán cuối cùng (totalMoney + shipFee + shipFeeDiscount + totalMoneyDiscount)

            CalculateMoneyFromDB(lsBuyedCart, sanPhamBasicInfos, promotions, cusInfor,
                ref totalMoney, ref shipFee, ref shipFeeDiscount, ref totalMoneyDiscount, ref finalAmount, result);


            CheckMoneyValidation(lsOrderPay, totalMoney, shipFee, shipFeeDiscount,
                totalMoneyDiscount, finalAmount, messageWhenValidationFail, result);
            if (result.State != EMySqlResultState.OK)
            {
                return JsonConvert.SerializeObject(result);
            }

            // ===== TẤT CẢ VALIDATION PASSED - TIẾP TỤC TẠO ĐƠN HÀNG =====

            // Với khách đăng nhập
            Customer cus = await AuthentCustomerAsync();
            int customerId = cus != null ? cus.id : -1;

            int newOrderId = -1;

            // danh sách (sanPhamId, quantity) cần trừ tồn kho tb_san_pham
            var sanPhamQuantities = lsBuyedCart.Select(c => (c.sanPhamId, c.quantity)).ToList();

            // Lấy danh sách (productId, quantity) cần trừ tồn kho tbProducts
            var productIdQuantities = await SanPhamMappingMySql.GetListProductIdQuantity_ConnectOutAsync(sanPhamQuantities);

            // Sinh mã đơn hàng unique
            string orderCode = string.Empty;
            try
            {
                orderCode = await OrderCodeSequenceMySql.GenerateUniqueOrderCodeAsync();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                return JsonConvert.SerializeObject(result);
            }

            // VietQR URL (chỉ generate khi payment = BANK_TRANSFER)
            string qrCodeUrl = null;
            MVCPlayWithMe.Models.BankAccount.BankAccount bankAccount = null;

            // Sau khi check thông tin đơn hàng chính xác
            if (paymentMethod == (int)EPaymentMethod.BANK_TRANSFER)
            {
                // Lấy bank account và generate VietQR với OrderCode
                bankAccount = await MVCPlayWithMe.Models.BankAccount.BankAccountMySql.GetActiveBankAccountAsync();

                if (bankAccount != null)
                {
                    // Generate VietQR URL
                    qrCodeUrl = bankAccount.GenerateVietQR(
                        amount: finalAmount,
                        orderCode: orderCode,
                        template: "compact2"
                    );

                    MyLogger.GetInstance().Info($"🏦 Generated VietQR: {qrCodeUrl}");
                }
            }

            // Start TRANSACTION
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();

                using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // 2. Insert Order
                        result = await OrderMySql.AddOrderTransactionAsync(conn, transaction,
                            customerId, noteToShop, (SByte)EOrderFrom.VOI_BE_NHO, orderCode,
                            (SByte)EOrderStatus.PROCESSING,
                            (SByte)EOrderPayStatus.PENDING,
                            paymentMethod,
                            DateTime.Now.AddHours(Common.orderDeadline), cusInfor);

                        if(result.State != EMySqlResultState.OK)
                        {
                            return await RoolBackWhenOrderError(transaction, result);
                        }
                        newOrderId = result.myAnything;

                        // KHÔNG CẦN: Insert theo trigger mysql
                        //// 3. Insert OrderTrack
                        //result = await OrderMySql.AddTrackOrderTransactionAsync(conn, transaction, newOrderId, (int)EOrderStatus.PROCESSED);
                        //if (result.State != EMySqlResultState.OK)
                        //{
                        //    return await RoolBackWhenOrderError(transaction, result);
                        //}

                        // 4. Insert OrderDetail
                        result = await OrderMySql.AddDetailOrderTransactionAsync(conn, transaction, newOrderId, lsBuyedCart);
                        if (result.State != EMySqlResultState.OK)
                        {
                            return await RoolBackWhenOrderError(transaction, result);
                        }

                        // 4.5. Trừ số lượng sản phẩm trong tb_san_pham
                        result = await SanPhamMySql.UpdateQuantityAfterSaleTransactionAsync(conn, transaction, sanPhamQuantities);
                        if (result.State != EMySqlResultState.OK)
                        {
                            return await RoolBackWhenOrderError(transaction, result);
                        }

                        // Temporary comments
                        // 4.6. Trừ số lượng sản phẩm trong tbProducts
                        //result = await ProductMySql.UpdateQuantityAfterSaleVBNTransactionAsync(conn, transaction, productIdQuantities);
                        //if (result.State != EMySqlResultState.OK)
                        //{
                        //    return await RoolBackWhenOrderError(transaction, result);
                        //}

                        // 5. Tạo list OrderPay
                        List<OrderPay> lsOrderPayFromDb = CaculateListOrderPay(newOrderId,
                            totalMoney,
                            shipFee,
                            shipFeeDiscount,
                            totalMoneyDiscount,
                            finalAmount,
                            promotions);

                        // 6. Insert OrderPay
                        result = await OrderMySql.AddPayOrderTransactionAsync(conn, transaction, newOrderId, lsOrderPayFromDb);
                        if (result.State != EMySqlResultState.OK)
                        {
                            return await RoolBackWhenOrderError(transaction, result); ;
                        }

                        // Xóa sản phẩm khỏi cart
                        if (cus != null)
                        {
                            result = await OrderMySql.DeleteListCartTransactionAsync(conn, transaction, cus.id, lsBuyedCart);
                            if (result.State != EMySqlResultState.OK)
                            {
                                return await RoolBackWhenOrderError(transaction, result);
                            }
                        }

                        // COMMIT TRANSACTION - Tất cả insert thành công
                        await transaction.CommitAsync();
                        MyLogger.GetInstance().Info($"🎉 Transaction committed successfully! OrderId={newOrderId}, OrderCode={orderCode}");

                        result.Message = orderCode;
                        result.myAnything = newOrderId;
                    }
                    catch (Exception ex)
                    {
                        // ROLLBACK nếu có lỗi
                        await transaction.RollbackAsync();
                        MyLogger.GetInstance().Error($"❌ Transaction rollback: {ex.Message}\n{ex.StackTrace}");

                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Không tạo được đơn hàng. Vui lòng thử lại sau.";
                        return JsonConvert.SerializeObject(result);
                    }
                }
            }
            // End TRANSACTION

            // Trả về OrderCode trong result.Message, OrderId trong result.myAnything
            MyLogger.GetInstance().Info($"✅ CheckOrderOnSever DONE! OrderID={newOrderId}, OrderCode={orderCode}");

            // Nếu thanh toán bằng chuyển khoản, return thêm QR code info
            if (paymentMethod == (int)EPaymentMethod.BANK_TRANSFER && qrCodeUrl != null)
            {
                var responseWithQR = new
                {
                    State = (int)result.State,
                    Message = result.Message,  // OrderCode
                    OrderId = newOrderId,
                    PaymentMethod = paymentMethod,
                    QRCodeUrl = qrCodeUrl,
                    BankAccount = new
                    {
                        bankAccount.BankName,
                        bankAccount.AccountNumber,
                        bankAccount.AccountHolder,
                        bankAccount.Branch
                    },
                    TotalAmount = finalAmount
                };

                return JsonConvert.SerializeObject(responseWithQR);
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> DeleteSanPhamOnCart(int sanPhamId)
        {
            Customer cus = await AuthentCustomerAsync();
            MySqlResultState result = new MySqlResultState();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
            }
            else
            {
                result = await OrderMySql.DeleteSanPhamOnCartAsync(cus.id, sanPhamId);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateSanPhamQuantityOnCart(int sanPhamId, int quantity)
        {
            Customer cus = await AuthentCustomerAsync();
            MySqlResultState result = new MySqlResultState();
            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
            }
            else
            {
                result = await OrderMySql.UpdateSanPhamQuantityOnCartAsync(cus.id, sanPhamId, quantity);
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Reset real = 0 cho tất cả items trong cart (khi vào Cart page từ trang khác)
        /// </summary>
        [HttpPost]
        public async Task<string> RefreshRealOfCart()
        {
            Customer cus = await AuthentCustomerAsync();
            MySqlResultState result = new MySqlResultState();

            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
            }
            else
            {
                result = await OrderMySql.RefreshRealOfCartAsync(cus.id);
            }

            return JsonConvert.SerializeObject(result);
        }

        ///// <summary>
        ///// Update real = 1 cho các sản phẩm được chọn mua (khi click "Mua Hàng")
        ///// </summary>
        //[HttpPost]
        //public async Task<string> CartPageUploadRealCart()
        //{
        //    Customer cus = await AuthentCustomerAsync();
        //    MySqlResultState result = new MySqlResultState();

        //    if (cus == null)
        //    {
        //        result.State = EMySqlResultState.AUTHEN_FAIL;
        //        result.Message = "Bạn cần đăng nhập để thực hiện thao tác này.";
        //    }
        //    else
        //    {
        //        // Đọc list sanPhamIds từ request body (JSON array)
        //        List<int> sanPhamIds = await Common.ReadJsonFromRequestBody<List<int>>(Request);

        //        if (sanPhamIds == null)
        //        {
        //            result.State = EMySqlResultState.ERROR;
        //            result.Message = "Không nhận được dữ liệu giỏ hàng.";
        //        }
        //        else
        //        {
        //            result = await OrderMySql.UpdateRealCartAsync(cus.id, sanPhamIds);
        //        }
        //    }

        //    return JsonConvert.SerializeObject(result);
        //}

        [HttpPost]
        public async Task<string> CheckoutPageLoadRealCart()
        {
            Customer cus = await AuthentCustomerAsync();

            if (cus == null)
            {
                // Guest: trả về empty, frontend dùng localStorage
                return JsonConvert.SerializeObject(new List<Cart>());
            }

            // Logged-in: lấy cart với real=1 từ database
            List<Cart> realCart = await OrderMySql.GetRealCartAsync(cus.id);
            return JsonConvert.SerializeObject(realCart);
        }

        [HttpPost]
        public async Task<string> BatchUpdateCartQuantities()
        {
            Customer cus = await AuthentCustomerAsync();
            MySqlResultState result = new MySqlResultState();

            if (cus == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Bạn cần đăng nhập để thực hiện thao tác này.";
            }
            else
            {
                // Đọc dictionary { sanPhamId: quantity } từ request body
                Dictionary<int, int> updates = await Common.ReadJsonFromRequestBody<Dictionary<int, int>>(Request);

                if (updates == null || updates.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Không nhận được dữ liệu cập nhật.";
                }
                else
                {
                    result = await OrderMySql.UpdateSanPhamQuantityListOnCartAsync(cus.id, updates);
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// API lấy danh sách promotion đang hoạt động (Status = 0)
        /// </summary>
        /// <returns>JSON array của OrderSimplePromotion</returns>
        [HttpPost]
        public async Task<string> GetActiveOrderSimplePromotions()
        {
            // Không cần check đăng nhập, vì promotion áp dụng cho tất cả khách hàng

            try
            {
                List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsAsync();
                return JsonConvert.SerializeObject(promotions);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetActiveOrderSimplePromotions failed: {ex.Message}");
                return "[]"; // Trả về mảng rỗng nếu có lỗi
            }
        }
    }
}
