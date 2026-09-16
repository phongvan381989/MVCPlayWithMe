using Anthropic.SDK.Common;
using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // ✅ SEO: Dynamic title dựa vào keyword (giống UpdatePageTitle() trong Search.js)
            string keyword = Request.QueryString["keyword"];
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                ViewData["title"] = $"Tìm kiếm \"{keyword.Trim()}\" | {Common.titleVoiBeNho}";
            }
            else
            {
                ViewData["title"] = Common.titleVoiBeNho;
            }

            // ✅ SSR: Load ALL items từ page 1 → current page (keyset pagination)
            const int MAX_SSR_PAGE = 5;  // Giới hạn SSR ở 5 pages đầu (max 150 items)
            const int ITEMS_PER_PAGE = 30;

            List<SanPhamBasicInfo> initialProducts = new List<SanPhamBasicInfo>();
            bool hasMore = false;
            int currentPage = 1;

            // Parse page number từ QueryString
            string pageParam = Request.QueryString["page"];
            if (!string.IsNullOrEmpty(pageParam))
            {
                int.TryParse(pageParam, out currentPage);
            }
            if (currentPage < 1) currentPage = 1;

            // ✅ SSR: Load data cho page 1 đến min(currentPage, MAX_SSR_PAGE)
            // Nếu page > 5: load 150 items (page 1-5), client sẽ Load More phần còn thiếu
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // ✅ Clamp to MAX_SSR_PAGE (page 8 → load 150 items, client fetch thêm)
                    int ssrPage = Math.Min(currentPage, MAX_SSR_PAGE);
                    int totalItemsToLoad = ssrPage * ITEMS_PER_PAGE;

                    SanPhamSearchParameter searchParameter = new SanPhamSearchParameter
                    {
                        name = Request.QueryString["keyword"],
                        author = Request.QueryString["author"],
                        translator = Request.QueryString["translator"],
                        category = Request.QueryString["category"],
                        publishingCompany = Request.QueryString["publishingCompany"],
                        publisher = Request.QueryString["publisher"],
                        lastId = 0,  // ✅ Start from beginning (keyset)
                        limit = totalItemsToLoad,  // ✅ 30/60/90/120/150 items
                        page = null  // ❌ Không dùng page (offset pagination)
                    };

                    var (lsSearchResult, hasMoreResults) = await SanPhamMySql.SearchSanPhamWithCursorAsync(
                        searchParameter,
                        conn);

                    initialProducts = lsSearchResult;
                    hasMore = hasMoreResults;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"Search SSR load failed: {ex.Message}");
            }

            ViewBag.InitialProducts = initialProducts;
            ViewBag.HasMore = hasMore;
            ViewBag.CurrentPage = currentPage;

            return View();
        }

        [HttpGet]
        public ActionResult Error()
        {
            ViewData["title"] = Common.titleVoiBeNho;
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
        public async Task<JsonResult> HomeSearch(string keyword,
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

            return Json(result, JsonRequestBehavior.AllowGet);
        }

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

        /// <summary>
        /// Trang chi tiết sản phẩm cho người mua (tb_san_pham)
        /// URL format: /Home/SanPham/ten-sach-123
        /// Server-side rendering để tối ưu SEO
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

            // Load sản phẩm cùng variants (cùng ComboId)
            List<SanPham> variants = await SanPhamMySql.GetSanPhamWithVariantsAsync(id);
            SanPham sanPham = variants?.FirstOrDefault(v => v.Id == id);

            if (sanPham == null || sanPham.Status != (int)ESanPhamStatus.DANG_KINH_DOANH)
            {
                return RedirectToAction("Error");
            }

            // Tạo slug chuẩn từ tên sản phẩm
            string correctSlugId = Common.GenerateSlugId(sanPham.Name, id);

            // Nếu slug không đúng, redirect về URL chuẩn (SEO 301)
            if (!string.Equals(slugId, correctSlugId, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToActionPermanent("SanPham", new { slugId = correctSlugId });
            }

            List<SanPhamMedia> mediaList = sanPham.MediaList;
            SanPhamMedia firstImage = null;
            string ogImageAlt = string.Empty;
            if (mediaList != null && mediaList.Count > 0)
            {
                if (mediaList[0].MediaType == "image")
                {
                    firstImage = mediaList[0];
                    ogImageAlt = GenerateAltText(sanPham, firstImage, false, 0);
                }
                else
                {
                    if(mediaList.Count > 1)
                    firstImage = mediaList[1];
                    ogImageAlt = GenerateAltText(sanPham, firstImage, false, 1);
                }
            }

            // Ảnh đại diện cho OG image (ảnh đầu tiên trong gallery)
            string ogImageUrl = firstImage?.FileName != null
                ? $"{Common.httpsVoiBeNho}{Common.SanPhamMediaFolderPath}{id}/{firstImage.FileName}"
                : $"{Common.httpsVoiBeNho}/Media/NoImageThumbnail.png";

            // Kích thước ảnh cho Open Graph (lấy từ DB, fallback 1000×1000)
            uint ogImageWidth = firstImage?.Width ?? 1000;
            uint ogImageHeight = firstImage?.Height ?? 1000;

            // Generate meta description tối ưu SEO (160 ký tự)
            string metaDescription = GenerateMetaDescription(sanPham);

            // Generate Product JSON-LD cho Google Rich Results
            string productJsonLD = GenerateProductJsonLD(sanPham);

            // Generate SSR HTML cho SEO (JavaScript vẫn dùng khi switch variant)
            string specificationsHtml = GenerateSpecificationsHtml(sanPham);
            string descriptionHtml = ProcessDescriptionHtml(sanPham);
            string thumbnailGalleryHtml = GenerateThumbnailGalleryHtml(sanPham);
            string variationsHtml = GenerateVariationsHtml(variants, sanPham.Id);

            // Canonical URL (match custom route: San-Pham/{slugId})
            string canonicalUrl = $"{Common.httpsVoiBeNho}/San-Pham/{correctSlugId}";

            // Pass data vào ViewBag
            ViewBag.SanPham = sanPham;
            ViewBag.Variants = variants;
            //ViewBag.GalleryMedia = galleryMedia;
            //ViewBag.DescriptionMedia = descriptionMedia;
            ViewBag.Title = sanPham.Name;
            ViewBag.MetaDescription = metaDescription;
            ViewBag.OgImageUrl = ogImageUrl;
            ViewBag.OgImageWidth = ogImageWidth;
            ViewBag.OgImageHeight = ogImageHeight;
            ViewBag.CanonicalUrl = canonicalUrl;
            ViewBag.ProductJsonLD = productJsonLD;
            ViewBag.SpecificationsHtml = specificationsHtml;
            ViewBag.DescriptionHtml = descriptionHtml;
            ViewBag.ThumbnailGalleryHtml = thumbnailGalleryHtml;
            ViewBag.VariationsHtml = variationsHtml;
            ViewBag.titleVoiBeNho = Common.titleVoiBeNho;
            ViewBag.ogImageAlt = ogImageAlt;

            return View();
        }

        /// <summary>
        /// API lấy danh sách sản phẩm cùng ComboId (bao gồm cả sản phẩm với ID được query)
        /// Tối ưu: Chỉ 1 stored procedure call, return list
        /// JavaScript sẽ tự tìm sản phẩm chính theo ID
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetSanPhamWithVariants(int id)
        {
            // Gọi 1 stored procedure duy nhất để lấy danh sách variants (bao gồm sản phẩm chính)
            List<SanPham> variants = await SanPhamMySql.GetSanPhamWithVariantsAsync(id);

            if (variants == null || variants.Count == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            // Return list, JavaScript sẽ tự tìm sản phẩm chính
            return Json(variants, JsonRequestBehavior.AllowGet);
        }

        // Nguyên tắc: real luôn luôn = 0 trong db, sản phẩm nào được chọn trên giao diện sẽ gửi riêng
        [HttpPost]
        public async Task<JsonResult> AddSanPhamToCart(int sanPhamId, int quantity/*, int real*/)
        {
            MySqlResultState result = new MySqlResultState();
            Customer customer = await AuthentCustomerAsync();
            if (customer == null)
            {
                result.State = EMySqlResultState.AUTHEN_FAIL;
                result.Message = "Không lấy được thông tin khách hàng.";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //// Làm mới dữ liệu trước đó real = 0
            //await OrderMySql.RefreshRealOfCartAsync(customer.id);

            Cart cart = new Cart();
            cart.sanPhamId = sanPhamId;
            cart.quantity = quantity;
            //cart.real = real;
            result = await CustomerMySql.AddCartAsync(customer.id, cart);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CartPageLoadCart()
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

            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cart()
        {
            ViewData["title"] = "Giỏ Hàng";
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CheckoutPageLoadCart()
        {
            // Đọc cart từ request body (JSON)
            List<Cart> lslocalStorage = await Common.ReadJsonFromRequestBody<List<Cart>>(Request);
            await OrderMySql.GetCartsSanPhamBasicInfoAsync(lslocalStorage);

            return Json(lslocalStorage, JsonRequestBehavior.AllowGet);
        }

        // Danh sách sản phẩm đã chọn mua, phí vận chuyển,
        // giảm giá thêm: giảm giá cho khách quen, giảm giá cho đơn lơn hơn 500k,...

        public async Task<ActionResult> Checkout()
        {
            ViewData["title"] = "Thanh Toán";

            // Load bank account info để hiển thị cho payment method = BANK_TRANSFER
            ViewBag.BankAccount = await Common.GetBankAccountAsync();

            // Pass orderDeadline để hiển thị hạn thanh toán
            ViewBag.OrderDeadline = Common.orderDeadline;

            // ✅ SSR: Load promotions sẵn (thay vì gọi API từ JS)
            List<OrderSimplePromotion> promotions = new List<OrderSimplePromotion>();
            try
            {
                promotions = await OrderSimplePromotionMySql.GetActivePromotionsAsync();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"Checkout load promotions failed: {ex.Message}");
                // promotions = empty list
            }
            ViewBag.Promotions = promotions;

            // ✅ SSR: Load addresses cho logged-in user (anonymous user sẽ dùng localStorage)
            List<Address> addresses = new List<Address>();
            var customer = await AuthentCustomerAsync();
            if (customer != null)
            {
                addresses = await CustomerMySql.GetListAddressAsync(customer.id);
            }
            ViewBag.Addresses = addresses;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAdministrativeAddress()
        {
            List<AdministrativeAddress> ls = await AdministrativeAddressMySql.GetListAdministrativeAddressAsync();
            return Json(ls, JsonRequestBehavior.AllowGet);
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

        private async Task<JsonResult> RoolBackWhenOrderError(MySqlTransaction transaction, MySqlResultState result)
        {
            // ROLLBACK nếu có lỗi
            await transaction.RollbackAsync();
            result.Message = "Không tạo được đơn hàng. Vui lòng thử lại sau.";
            return Json(result, JsonRequestBehavior.AllowGet);
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
        public async Task<JsonResult> CheckOrderOnSever(string cart, string customerInfor,
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
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            // Lấy dữ liệu từ db 1 lần để so sánh
            // Lấy tất cả promotion đang bật
            List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsAsync();

            List<SanPhamBasicInfo> sanPhamBasicInfos = new List<SanPhamBasicInfo>();
            await GetSanPhamBasicInfosFromDBAsync(sanPhamBasicInfos, lsBuyedCart, result);
            if (result.State != EMySqlResultState.OK)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (sanPhamBasicInfos.Count == 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Thông tin giỏ hàng đã thay đổi. Vui lòng tải lại trang.";

                MyLogger.GetInstance().Warn($"🚨 Cant get SanPhamBasicInfos from DB. SanPhamBasicInfos.Count == 0");
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            string messageWhenValidationFail = "Thông tin giỏ hàng đã thay đổi. Vui lòng tải lại trang.";
            // ===== LAYER 1: VALIDATE TỪNG SẢN PHẨM =====
            CheckCartValidation(lsBuyedCart, sanPhamBasicInfos, messageWhenValidationFail, result);
            if (result.State != EMySqlResultState.OK)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
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
                return Json(result, JsonRequestBehavior.AllowGet);
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
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            // VietQR URL (chỉ generate khi payment = BANK_TRANSFER)
            string qrCodeUrl = null;
            MVCPlayWithMe.Models.BankAccount.BankAccount bankAccount = null;

            // Sau khi check thông tin đơn hàng chính xác
            if (paymentMethod == (int)EPaymentMethod.BANK_TRANSFER)
            {
                // Lấy bank account và generate VietQR với OrderCode
                bankAccount = await Common.GetBankAccountAsync();

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
                        return Json(result, JsonRequestBehavior.AllowGet);
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

                return Json(responseWithQR, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteSanPhamOnCart(int sanPhamId)
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
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateSanPhamQuantityOnCart(int sanPhamId, int quantity)
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
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reset real = 0 cho tất cả items trong cart (khi vào Cart page từ trang khác)
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> RefreshRealOfCart()
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

            return Json(result, JsonRequestBehavior.AllowGet);
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
        public async Task<JsonResult> CheckoutPageLoadRealCart()
        {
            Customer cus = await AuthentCustomerAsync();

            if (cus == null)
            {
                // Guest: trả về empty, frontend dùng localStorage
                return Json(new List<Cart>(), JsonRequestBehavior.AllowGet);
            }

            // Logged-in: lấy cart với real=1 từ database
            List<Cart> realCart = await OrderMySql.GetRealCartAsync(cus.id);
            return Json(realCart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> BatchUpdateCartQuantities()
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

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// API lấy danh sách promotion đang hoạt động (Status = 0)
        /// </summary>
        /// <returns>JSON array của OrderSimplePromotion</returns>
        [HttpPost]
        public async Task<JsonResult> GetActiveOrderSimplePromotions()
        {
            // Không cần check đăng nhập, vì promotion áp dụng cho tất cả khách hàng

            try
            {
                List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsAsync();
                return Json(promotions, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetActiveOrderSimplePromotions failed: {ex.Message}");
                return Json(new List<OrderSimplePromotion>(), JsonRequestBehavior.AllowGet); // Trả về mảng rỗng nếu có lỗi
            }
        }

        /// <summary>
        /// Generate Product JSON-LD cho SEO (Google Rich Results)
        /// Kết hợp @type Product + Book để hiển thị giá trong kết quả tìm kiếm
        /// </summary>
        private string GenerateProductJsonLD(SanPham sanPham)
        {
            if (sanPham == null)
                return "{}";

            // Build JSON-LD object (dùng anonymous type để serialize)
            var jsonLd = new
            {
                context = "https://schema.org/",
                type = new[] { "Product", "Book" },  // Kết hợp Product + Book
                name = sanPham.Name ?? "",
                description = !string.IsNullOrWhiteSpace(sanPham.Detail) ? sanPham.Detail : sanPham.Name,
                url = $"{Common.httpsVoiBeNho}{Common.GenerateSanPhamUrlForCustomer(sanPham.Name, sanPham.Id)}",
                image = GetProductImages(sanPham),
                isbn = sanPham.Barcode,  // ISBN (nếu có)
                sku = sanPham.Code ?? sanPham.Code ?? sanPham.Id.ToString(),
                author = !string.IsNullOrWhiteSpace(sanPham.Author) ? new { type = "Person", name = sanPham.Author } : null,
                publisher = !string.IsNullOrWhiteSpace(sanPham.PublishingCompany)
                    ? new { type = "Organization", name = sanPham.PublishingCompany }
                    : null,
                bookFormat = sanPham.HardCover == ESanPhamCoverType.BIA_CUNG
                    ? "https://schema.org/Hardcover"
                    : "https://schema.org/Paperback",
                inLanguage = GetLanguageCode(sanPham.Language),
                numberOfPages = sanPham.PageNumber > 0 ? (int?)sanPham.PageNumber : null,
                offers = new
                {
                    type = "Offer",
                    priceCurrency = "VND",
                    price = sanPham.SalePrice.ToString(),
                    availability = sanPham.Quantity > 0
                        ? "https://schema.org/InStock"
                        : "https://schema.org/OutOfStock",
                    itemCondition = "https://schema.org/NewCondition",
                    seller = new
                    {
                        type = "Organization",
                        name = Common.titleVoiBeNho
                    },
                    // Giá bìa (ListPrice) - chỉ thêm khi có giảm giá
                    priceSpecification = sanPham.BookCoverPrice > sanPham.SalePrice
                        ? new[]
                        {
                            new
                            {
                                type = "UnitPriceSpecification",
                                priceType = "https://schema.org/ListPrice",
                                price = sanPham.BookCoverPrice.ToString(),
                                priceCurrency = "VND"
                            }
                        }
                        : null
                }
            };

            // Serialize với @context/@type format đúng (replace @ prefix)
            string json = JsonConvert.SerializeObject(jsonLd, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore  // Bỏ qua field null
            });

            // Replace "context" → "@context", "type" → "@type"
            json = json.Replace("\"context\":", "\"@context\":")
                       .Replace("\"type\":", "\"@type\":");

            return json;
        }

        /// <summary>
        /// Lấy danh sách URL ảnh cho JSON-LD (tất cả ảnh, không bao gồm video)
        /// </summary>
        private string[] GetProductImages(SanPham sanPham)
        {
            if (sanPham.MediaList == null || sanPham.MediaList.Count == 0)
            {
                return new[] { $"{Common.httpsVoiBeNho}/Media/NoImageThumbnail.png" };
            }

            var imageUrls = sanPham.MediaList
                .Where(m => m.MediaType == "image")
                .Select(m => $"{Common.httpsVoiBeNho}{Common.SanPhamMediaFolderPath}{sanPham.Id}/{m.FileName}")
                .ToArray();

            return imageUrls.Length > 0
                ? imageUrls
                : new[] { $"{Common.httpsVoiBeNho}/Media/NoImageThumbnail.png" };
        }

        /// <summary>
        /// Map từ text ngôn ngữ sang language code (vi/en/["vi","en"])
        /// </summary>
        private object GetLanguageCode(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return "vi";  // Default: Tiếng Việt

            string lang = language.ToLower().Trim();

            if (lang.Contains("song ngữ") || lang.Contains("song ngu"))
                return new[] { "vi", "en" };  // Bilingual

            if (lang.Contains("tiếng anh") || lang.Contains("tieng anh") || lang == "english")
                return "en";

            if (lang.Contains("tiếng việt") || lang.Contains("tieng viet") || lang == "vietnamese")
                return "vi";

            return "vi";  // Fallback
        }

        /// <summary>
        /// Convert độ tuổi từ tháng → năm và format thành text
        /// VD: "2-5 tuổi", "Từ 3 tuổi", "Đến 6 tuổi", "4 tuổi", ""
        /// </summary>
        private string GetAgeRangeText(int? minAge, int? maxAge)
        {
            // 1. Cả hai null hoặc -1 → ""
            if ((!minAge.HasValue || minAge.Value == -1) && (!maxAge.HasValue || maxAge.Value == -1))
            {
                return "";
            }

            // 2. Convert tháng → năm (làm tròn xuống)
            int minYears = (minAge.HasValue && minAge.Value != -1) ? (int)Math.Floor(minAge.Value / 12.0) : -1;
            int maxYears = (maxAge.HasValue && maxAge.Value != -1) ? (int)Math.Floor(maxAge.Value / 12.0) : -1;

            // 3. Chỉ có max → "Đến X tuổi"
            if (!minAge.HasValue || minAge.Value == -1)
            {
                return $"Đến {maxYears} tuổi";
            }

            // 4. Chỉ có min → "Từ X tuổi"
            if (!maxAge.HasValue || maxAge.Value == -1)
            {
                return $"Từ {minYears} tuổi";
            }

            // 5. Min = Max → "X tuổi"
            if (minYears == maxYears)
            {
                return $"{minYears} tuổi";
            }

            // 6. Min khác Max → "X-Y tuổi"
            return $"{minYears}-{maxYears} tuổi";
        }

        /// <summary>
        /// Generate HTML cho bảng Specifications (THÔNG TIN CHI TIẾT)
        /// SSR cho initial load, JavaScript vẫn dùng để render khi switch variant
        /// </summary>
        /// <summary>
        /// Generate thumbnail gallery HTML cho SSR (chỉ images, không video)
        /// JavaScript sẽ re-render khi switch variant
        /// </summary>
        private string GenerateThumbnailGalleryHtml(SanPham sanPham)
        {
            if (sanPham == null || sanPham.MediaList == null || sanPham.MediaList.Count == 0)
                return "";

            var html = new StringBuilder();

            // Filter chỉ lấy images (video sẽ dùng iframe embed trong description)
            var imageList = sanPham.MediaList.Where(m => m.MediaType == "image").ToList();

            for (int i = 0; i < imageList.Count; i++)
            {
                SanPhamMedia media = imageList[i];

                // Tạo thumbnail URL với thư mục _320
                string thumbnailSrc = $"{Common.SanPhamMediaFolderPath}{sanPham.Id}_320/{media.FileName}";

                // Generate alt text cho thumbnail (isThumbnail = true)
                string alt = GenerateAltText(sanPham, media, isThumbnail: true, i);

                // Tạo <div class="small-media" data-index="{i}"> với border màu đỏ cho thumbnail đầu tiên (selected)
                html.Append($"<div class=\"small-media\" data-index=\"{i}\"");

                // Thumbnail đầu tiên có border màu đỏ (match JavaScript ChangeBorderColorOfSelectedSmallItem)
                if (i == 0)
                {
                    html.Append(" style=\"border-color: rgb(255, 0, 0);\"");
                }

                html.Append(">");

                // <img> với lazy loading cho thumbnail sau 3 ảnh đầu
                html.Append("<img ");
                html.Append($"src=\"{HttpUtility.HtmlAttributeEncode(thumbnailSrc)}\" ");
                html.Append($"alt=\"{HttpUtility.HtmlAttributeEncode(alt)}\" ");

                if (i > 3)
                {
                    html.Append("loading=\"lazy\" ");
                }

                html.Append("style=\"object-fit:contain; max-width:100%; max-height:100%; display:block;\"");
                html.Append(">");

                html.Append("</div>");
            }

            return html.ToString();
        }

        private string GenerateVariationsHtml(List<SanPham> variants, int currentVariantId)
        {
            if (variants == null || variants.Count <= 1)
                return ""; // Không có variants hoặc chỉ 1 sản phẩm → không hiển thị

            var html = new StringBuilder();

            // Tiêu đề phân loại
            html.Append("<div class=\"variation-title\">Phân loại</div>");

            // Container chứa các button variant
            html.Append("<div class=\"variation-buttons-container\">");

            foreach (var variant in variants)
            {
                // <button class="variation-button [out-of-stock]" data-variant-id="{id}">
                html.Append("<button class=\"variation-button");

                // Hết hàng → thêm class out-of-stock
                if (variant.Quantity <= 0)
                {
                    html.Append(" out-of-stock");
                }

                html.Append("\" data-variant-id=\"");
                html.Append(variant.Id);
                html.Append("\"");

                // Variant đang chọn → inline style border-color + color đỏ (match ApplyVariantHighlight)
                if (variant.Id == currentVariantId)
                {
                    html.Append(" style=\"border-color: rgb(255, 0, 0); color: rgb(255, 0, 0);\"");
                }

                html.Append(">");

                // Text: ShortName hoặc Name
                string displayName = !string.IsNullOrWhiteSpace(variant.ShortName) ? variant.ShortName : variant.Name;
                html.Append(HttpUtility.HtmlEncode(displayName));

                // Variant đang chọn → thêm SVG tick icon (match ApplyVariantHighlight)
                if (variant.Id == currentVariantId)
                {
                    html.Append("<div id=\"check-container\">");
                    html.Append("<svg viewBox=\"0 0 12 12\" class=\"icon-tick-bold\">");
                    html.Append("<polyline fill=\"none\" points=\"1.5 6 4.5 9 10.5 3\" stroke-width=\"2\" stroke=\"currentColor\"></polyline>");
                    html.Append("</svg>");
                    html.Append("</div>");
                }

                html.Append("</button>");
            }

            html.Append("</div>");

            return html.ToString();
        }

        private string GenerateSpecificationsHtml(SanPham sanPham)
        {
            if (sanPham == null)
                return "";

            var html = new StringBuilder();

            // Helper: Thêm 1 spec row
            void AddSpecRow(string label, string value, string url = null)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                html.Append("<div class=\"spec-row\">");
                html.Append($"<div class=\"spec-label\">{HttpUtility.HtmlEncode(label)}</div>");
                html.Append("<div class=\"spec-value\">");

                if (!string.IsNullOrWhiteSpace(url))
                {
                    html.Append($"<a href=\"{HttpUtility.HtmlAttributeEncode(url)}\" class=\"spec-link\">{HttpUtility.HtmlEncode(value)}</a>");
                }
                else
                {
                    html.Append(HttpUtility.HtmlEncode(value));
                }

                html.Append("</div>");
                html.Append("</div>");
            }

            // Tác giả
            if (!string.IsNullOrWhiteSpace(sanPham.Author))
            {
                AddSpecRow("Tác giả", sanPham.Author, $"/Home/Search?author={HttpUtility.UrlEncode(sanPham.Author)}");
            }

            // Người dịch
            if (!string.IsNullOrWhiteSpace(sanPham.Translator))
            {
                AddSpecRow("Người dịch", sanPham.Translator, $"/Home/Search?translator={HttpUtility.UrlEncode(sanPham.Translator)}");
            }

            // Danh mục
            if (!string.IsNullOrWhiteSpace(sanPham.CategoryName) && sanPham.CategoryId > 0)
            {
                AddSpecRow("Danh mục", sanPham.CategoryName, $"/Home/Search?category={HttpUtility.UrlEncode(sanPham.CategoryName)}");
            }

            // Nhà xuất bản
            if (!string.IsNullOrWhiteSpace(sanPham.PublishingCompany))
            {
                AddSpecRow("Nhà xuất bản", sanPham.PublishingCompany, $"/Home/Search?publishingCompany={HttpUtility.UrlEncode(sanPham.PublishingCompany)}");
            }

            // Nhà phát hành
            if (!string.IsNullOrWhiteSpace(sanPham.PublisherName) && sanPham.PublisherId > 0)
            {
                AddSpecRow("Nhà phát hành", sanPham.PublisherName, $"/Home/Search?publisher={HttpUtility.UrlEncode(sanPham.PublisherName)}");
            }

            // Năm xuất bản (chỉ hiển thị nếu cách năm hiện tại <= 3)
            if (sanPham.PublishingTime.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                int publishingYear = sanPham.PublishingTime.Value;

                if (currentYear - publishingYear <= 3)
                {
                    AddSpecRow("Năm xuất bản", publishingYear.ToString());
                }
            }

            // Ngôn ngữ
            if (!string.IsNullOrWhiteSpace(sanPham.Language))
            {
                AddSpecRow("Ngôn ngữ", sanPham.Language);
            }

            // Tuổi phù hợp
            string ageRangeText = GetAgeRangeText(sanPham.MinAge, sanPham.MaxAge);
            if (!string.IsNullOrWhiteSpace(ageRangeText))
            {
                AddSpecRow("Tuổi phù hợp", ageRangeText);
            }

            // Kích thước
            if (sanPham.ProductLong > 0 && sanPham.ProductWide > 0 && sanPham.ProductHigh > 0)
            {
                string dimensions = $"{sanPham.ProductLong} × {sanPham.ProductWide} × {sanPham.ProductHigh} mm";
                AddSpecRow("Kích thước", dimensions);
            }

            // Trọng lượng
            if (sanPham.ProductWeight > 0)
            {
                AddSpecRow("Trọng lượng", $"{sanPham.ProductWeight} gram");
            }

            // Số trang
            if (sanPham.PageNumber > 0)
            {
                AddSpecRow("Số trang", sanPham.PageNumber.ToString());
            }

            // Hình thức (Bìa cứng/mềm)
            string coverType = sanPham.HardCover == ESanPhamCoverType.BIA_CUNG ? "Bìa cứng" : "Bìa mềm";
            AddSpecRow("Hình thức", coverType);


            // Mã ISBN/Code
            string code = !string.IsNullOrWhiteSpace(sanPham.Code) ? sanPham.Code : sanPham.Barcode;
            if (!string.IsNullOrWhiteSpace(code))
            {
                AddSpecRow("Mã", code);
            }

            return html.ToString();
        }

        /// <summary>
        /// Process Description HTML - parse {{image:filename}} thành &lt;figure&gt; tags
        /// SSR cho initial load, JavaScript vẫn dùng để render khi switch variant
        /// </summary>
        private string ProcessDescriptionHtml(SanPham sanPham)
        {
            if (sanPham == null || string.IsNullOrWhiteSpace(sanPham.Detail))
                return "";

            string detailHtml = sanPham.Detail;

            // Parse {{image:filename}} → <figure> HTML
            detailHtml = System.Text.RegularExpressions.Regex.Replace(
                detailHtml,
                @"\{\{image:([^}]+)\}\}",
                match =>
                {
                    string filename = match.Groups[1].Value;

                    // Tìm metadata
                    SanPhamMedia media = null;
                    if (sanPham.MediaList != null && sanPham.MediaList.Count > 0)
                    {
                        media = sanPham.MediaList.FirstOrDefault(m => m.FileName == filename);
                    }

                    // Build image URL
                    string imgSrc = $"{Common.SanPhamMediaFolderPath}{sanPham.Id}/{filename}";
                    string alt = media != null
                        ? (!string.IsNullOrWhiteSpace(media.AltText) ? media.AltText : sanPham.Name)
                        : filename;
                    string caption = media?.Description ?? media?.Title ?? "";

                    // Build HTML
                    var figureHtml = new StringBuilder();
                    figureHtml.Append("<figure class=\"product-detail-image\">");
                    figureHtml.Append($"<img src=\"{HttpUtility.HtmlAttributeEncode(imgSrc)}\" alt=\"{HttpUtility.HtmlAttributeEncode(alt)}\" loading=\"lazy\">");

                    if (!string.IsNullOrWhiteSpace(caption))
                    {
                        figureHtml.Append($"<figcaption>{HttpUtility.HtmlEncode(caption)}</figcaption>");
                    }

                    figureHtml.Append("</figure>");

                    return figureHtml.ToString();
                }
            );

            // Xử lý newlines dư thừa
            // 1. Xóa newlines trước thẻ <p> và </p>
            detailHtml = System.Text.RegularExpressions.Regex.Replace(detailHtml, @"\n+(</?p[^>]*>)", "$1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 2. Giảm 3+ newlines liên tiếp xuống còn 2
            detailHtml = System.Text.RegularExpressions.Regex.Replace(detailHtml, @"\n{3,}", "\n\n");

            return detailHtml;
        }

        private string GenerateMetaDescription(SanPham sanPham)
        {
            if (sanPham == null)
                return "Mua sách online giá tốt tại Voi Bé Nhỏ";

            StringBuilder metaDesc = new StringBuilder();

            // CHIẾN LƯỢC: Giảm mạnh (>= 20%) → Giá lên đầu để tăng CTR
            bool hasStrongDiscount = sanPham.Discount >= 20;

            if (hasStrongDiscount)
            {
                // ===== GIÁ Ở ĐẦU (Giảm >= 20%) =====

                // 1. Giá + Giảm giá (nổi bật ngay từ đầu)
                metaDesc.Append($"Giá {sanPham.SalePrice:N0}đ (giảm {(int)sanPham.Discount}%)");

                // 2. Tên sách
                metaDesc.Append($" - {sanPham.Name}");

                // 3. Độ tuổi (nếu có)
                string ageRangeText = GetAgeRangeText(sanPham.MinAge, sanPham.MaxAge);
                if (!string.IsNullOrWhiteSpace(ageRangeText))
                {
                    metaDesc.Append($" ({ageRangeText})");
                }

                // 4. Thể loại (nếu có và còn chỗ)
                if (!string.IsNullOrWhiteSpace(sanPham.CategoryName) && metaDesc.Length < 120)
                {
                    metaDesc.Append($" - {sanPham.CategoryName}");
                }

                // 5. Nhà phát hành (nếu có và còn chỗ)
                if (!string.IsNullOrWhiteSpace(sanPham.PublisherName) && metaDesc.Length < 110)
                {
                    metaDesc.Append($" - {sanPham.PublisherName}");
                }

                // 6. Tồn kho (nếu còn chỗ và > 0)
                if (sanPham.Quantity > 0 && metaDesc.Length < 140)
                {
                    metaDesc.Append($". Còn {sanPham.Quantity} cuốn");
                }
            }
            else
            {
                // ===== TÊN Ở ĐẦU (Giảm < 20% hoặc không giảm) =====

                // 1. Tên sách (bắt buộc)
                metaDesc.Append(sanPham.Name);

                // 2. Độ tuổi (nếu có)
                string ageRangeText = GetAgeRangeText(sanPham.MinAge, sanPham.MaxAge);
                if (!string.IsNullOrWhiteSpace(ageRangeText))
                {
                    metaDesc.Append($" ({ageRangeText})");
                }

                // 3. Thể loại (nếu có)
                if (!string.IsNullOrWhiteSpace(sanPham.CategoryName))
                {
                    metaDesc.Append($" - {sanPham.CategoryName}");
                }

                // 4. Nhà phát hành (nếu có)
                if (!string.IsNullOrWhiteSpace(sanPham.PublisherName))
                {
                    metaDesc.Append($" - {sanPham.PublisherName}");
                }

                // 5. Giá (bắt buộc)
                metaDesc.Append($". Giá {sanPham.SalePrice:N0}đ");

                // 6. Giảm giá (nếu có)
                if (sanPham.Discount > 0)
                {
                    metaDesc.Append($" (giảm {(int)sanPham.Discount}%)");
                }

                // 7. Tồn kho (nếu còn chỗ và > 0)
                if (sanPham.Quantity > 0 && metaDesc.Length < 140)
                {
                    metaDesc.Append($". Còn {sanPham.Quantity} cuốn");
                }
            }

            // Giới hạn 160 ký tự
            string result = metaDesc.Length > 160
                ? metaDesc.ToString().Substring(0, 157) + "..."
                : metaDesc.ToString();

            return result;
        }

        // Từ thứ tự ảnh trong metadata sinh alt
        private string GenerateAltText(SanPham sanPhamObject,
            SanPhamMedia media,
            bool isThumbnail, int i)
        {
            Boolean metadataHasVideo = sanPhamObject.MediaList[0].MediaType != "image" ? true : false;
            string alt = "";
            if (isThumbnail)
            {
                if (!string.IsNullOrEmpty(media.Title))
                {
                    alt = media.Title;
                }
                else
                {
                    alt = sanPhamObject.Name + " - Trang " + (metadataHasVideo ? i : (i + 1)); // Có video thì video có thứ tự i = 0 nên ảnh sẽ từ 1,2,3 ngược lại ảnh sẽ từ 0,1,2
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(media.AltText))
                {
                    alt = media.AltText;
                }
                else
                {
                    alt = sanPhamObject.Name + " - Trang " + (metadataHasVideo ? i : (i + 1)); // Có video thì video có thứ tự i = 0 nên ảnh sẽ từ 1,2,3 ngược lại ảnh sẽ từ 0,1,2
                }
            }
            return alt;
        }
    }
}
