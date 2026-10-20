using MVCPlayWithMe.Controllers.ProductSupporting;
using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.LazadaAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Category;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
//using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers
{
    public class ProductController : BasicController
    {
        // GET: Product
        public async Task<ActionResult> Create()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            //GetViewDataForInput();

            return View();
        }

        [HttpPost]
        public async Task<string> GetProductFromId(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }
            Product product = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                product = await ProductMySql.GetProductFromIdAsync(id, conn);
            }

            return JsonConvert.SerializeObject(product);
        }

        public async Task<ActionResult> UpdateDelete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            //GetViewDataForInput();

            return View();
        }

        public async Task<ActionResult> Import()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListProductName();

            return View();
        }

        public async Task<ActionResult> RecheckImport()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListProductName();

            return View();
        }

        public async Task<ActionResult> SellingStatistics()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }


        public async Task<ActionResult> HintQuantityFromPublisher()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        public async Task<ActionResult> ChangeImport()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        public async Task<ActionResult> Search()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListCombo();
            //ViewDataGetListProductName();
            return View();
        }

        //private void AddUpdateParasCommon(
        //    ref int comboId, string comboName,
        //    ref int categoryId, string categoryName,
        //    ref int publisherId, string publisherName,
        //    ref int parentId, string parentName
        //    )
        //{
        //    if (string.IsNullOrWhiteSpace(comboName))
        //        comboId = -1;
        //    else
        //        comboId = await ComboMySql.GetComboIdFromName(comboName);

        //    if (string.IsNullOrWhiteSpace(categoryName))
        //        categoryId = -1;
        //    else
        //        categoryId = await CategoryMySql.GetCategoryIdFromName(categoryName);

        //    if (string.IsNullOrWhiteSpace(publisherName))
        //        publisherId = -1;
        //    else
        //        publisherId = await PublisherMySql.GetPublisherIdFromName(publisherName);

        //    if (string.IsNullOrWhiteSpace(parentName))
        //        parentId = -1;
        //    else
        //        parentId = await ProductMySql.GetProductIdFromName(parentName);
        //}

        [HttpPost]
        public async Task<string> AddNewPro(
                int quantity,
                string code,
                string barcode,
                string name,
                int comboId,
                int categoryId,
                int bookCoverPrice,
                float discount,
                string author,
                string translator,
                int publisherId,
                string publishingCompany,
                int publishingTime,
                int productLong,
                int productWide,
                int productHigh,
                int productWeight,
                string positionInWarehouse,
                int hardCover,
                string bookLanguge,
                int minAge,
                int maxAge,
                int parentId, // Nếu không có sản phẩm cha, parentId = -1
                int republish,// Nếu không xác định, parentId = -1
                string detail,
                int status,
                int pageNumber
                )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            string decodeDetail = WebUtility.UrlDecode(detail);

            Product pro = new Product(-1,
                 quantity,
                 code,
                 barcode,
                 name,
                 comboId,
                 categoryId,
                 bookCoverPrice,
                 discount,
                 author,
                 translator,
                 publisherId,
                 publishingCompany,
                 publishingTime,
                 productLong,
                 productWide,
                 productHigh,
                 productWeight,
                 positionInWarehouse,
                 hardCover,
                 bookLanguge,
                 minAge,
                 maxAge,
                 parentId,
                 republish,
                 decodeDetail,
                 status,
                 pageNumber
                 );

             MySqlResultState result = await ProductMySql.AddNewProAsync(pro);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> AddNewProsFromCSVPromise(int publisherId, string listObject)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            List<ProductFromCsv> products = null;
            MySqlResultState result = new MySqlResultState();
            try
            {
                products = JsonConvert.DeserializeObject<List<ProductFromCsv>>(listObject);
                if (products == null || products.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Danh sách cần nhập rỗng hoặc lỗi.";
                }
                else
                {
                   result = await ProductMySql.AddNewProsFromCSVPromise(products, publisherId);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);

        }

        [HttpPost]
        public async Task<string> UpdateProduct(
                int productId,
                int quantity,
                string code,
                string barcode,
                string name,
                int comboId,
                int categoryId,
                int bookCoverPrice,
                float discount,
                string author,
                string translator,
                int publisherId,
                string publishingCompany,
                int publishingTime,
                int productLong,
                int productWide,
                int productHigh,
                int productWeight,
                string positionInWarehouse,
                int hardCover,
                string bookLanguge,
                int minAge,
                int maxAge,
                int parentId, // Nếu không có sản phẩm cha, parentId = -1
                int republish,// Nếu không xác định, parentId = -1
                string detail,
                int status,
                int pageNumber
                )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            string decodeDetail = WebUtility.UrlDecode(detail);

            Product pro = new Product(
                productId,
                quantity,
                code,
                barcode,
                 name,
                 comboId,
                 categoryId,
                 bookCoverPrice,
                 discount,
                 author,
                 translator,
                 publisherId,
                 publishingCompany,
                 publishingTime,
                 productLong,
                 productWide,
                 productHigh,
                 productWeight,
                 positionInWarehouse,
                 hardCover,
                 bookLanguge,
                 minAge,
                 maxAge,
                 parentId,
                 republish,
                 decodeDetail,
                 status,
                 pageNumber
                 );

            MySqlResultState result = await ProductMySql.UpdateProductAsync(pro);

            return JsonConvert.SerializeObject(result);
        }

        // Cập nhật một vài thông tin sản phẩm từ url web fahasa từ tool bên ngoài
        [HttpPost]
        public async Task<string> UpdateProductFromFahasa(
                int productId,
                string author,
                int publishingTime,
                int productLong,
                int productWide,
                int productHigh,
                int productWeight,
                int hardCover,
                int minAge,
                int maxAge,
                string detail,
                int pageNumber,
                string user, // user và password méo mã hóa đâu, khoai lắm hardcode ba lăng nhăng
                string password
                )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            string decodeDetail = WebUtility.UrlDecode(detail);

            MySqlResultState result = await ProductMySql.UpdateProductFromFahasaAsync(
                productId,
                author,
                publishingTime,
                productLong,
                productWide,
                productHigh,
                productWeight,
                hardCover,
                minAge,
                maxAge,
                decodeDetail,
                pageNumber
                );
            string re = "Ok";
            if(result.State != EMySqlResultState.OK)
            {
                re = "Fail";
            }
            return re;
        }

        /// <summary>
        /// Xóa sản phẩm
        /// Sản phẩm chỉ có thế xóa khi đang không liên kết với sản phẩm nào trên
        /// Shopee, Tiki, Lazada, web voibenho, trong đơn hàng đã được bán, thông tin nhập xuất thực tế,...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> DeleteProduct(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await ProductMySql.DeleteProductAsync(id);

            // xóa thư mục ảnh video của sản phẩm
            string path = Common.GetAbsoluteProductMediaFolderPath(id.ToString());
            if (path != null)
            {
                Common.DeleteMediaFolder(path);
            }

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UpdateCommonInfoWithCombo(
                int comboId,
                int categoryId,
                int bookCoverPrice,
                float discount,
                string author,
                string translator,
                int publisherId,
                string publishingCompany,
                int publishingTime,
                int productLong,
                int productWide,
                int productHigh,
                int productWeight,
                string positionInWarehouse,
                int hardCover,
                string bookLanguge,
                int minAge,
                int maxAge,
                int republish,
                int status,
                int pageNumber
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            ProductCommonInfoWithCombo pro = new ProductCommonInfoWithCombo(
                 comboId,
                 categoryId,
                 bookCoverPrice,
                 discount,
                 author,
                 translator,
                 publisherId,
                 publishingCompany,
                 publishingTime,
                 productLong,
                 productWide,
                 productHigh,
                 productWeight,
                 positionInWarehouse,
                 hardCover,
                 bookLanguge,
                 minAge,
                 maxAge,
                 republish,
                 status,
                 pageNumber
                );
            MySqlResultState result = await ProductMySql.UpdateCommonInfoWithComboAsync(pro);
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UpdateCommonHardCoverWithCombo(
                int comboId,
                int hardCover
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonHardCoverWithComboAsync(comboId, hardCover, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UpdateCommonAgeWithCombo(
                int comboId,
                int minAge,
                int maxAge
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonAgeWithComboAsync(comboId, minAge, maxAge, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        //string bookLanguge
        public async Task<string> UpdateCommonLanguageWithCombo(
                int comboId,
                string language
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonLanguageWithComboAsync(comboId, language, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UpdateCommonDimensionWithCombo(
                int comboId,
                int productLong,
                int productWide,
                int productHigh,
                int productWeight
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonDimensionWithComboAsync(comboId, productLong, productWide, productHigh, productWeight, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        //
        public async Task<string> UpdateCommonCategoryWithCombo(
                int comboId,
                int categoryId
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonCategoryWithComboAsync(comboId, categoryId, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UpdateCommonPageNumberWithCombo(
                int comboId,
                int pageNumber
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonPageNumberWithComboAsync(comboId, pageNumber, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateCommonPublishingCompanyWithCombo(
                int comboId,
                string publishingCompany
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonPublishingCompanyWithComboAsync(comboId, publishingCompany, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateCommonPublishingTimeWithCombo(
                int comboId,
                int publishingTime
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonPublishingTimeWithComboAsync(comboId, publishingTime, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateCommonBookCoverPriceWithCombo(
                int comboId,
                int bookCoverPrice
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonBookCoverPriceWithComboAsync(comboId, bookCoverPrice, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UpdateCommonTranslatorWithCombo(
                int comboId,
                string translator
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonTranslatorWithComboAsync(comboId, translator, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UpdateCommonDiscountWithCombo(
                int comboId,
                double discount
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonDiscountWithComboAsync(comboId, discount, conn);
            }
            return JsonConvert.SerializeObject(result);
        }
        // 
        public async Task<string> UpdateCommonStatusWithCombo(
                int comboId,
                int status
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonStatusWithComboAsync(comboId, status, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateCommonDetailWithCombo(
                int comboId,
                string detail
            )
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                result = await ProductMySql.UpdateCommonDetailWithComboAsync(comboId, detail, conn);
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="fileType">isImage hoặc isVideo</param>
        /// <returns></returns>
        //[HttpPost]
        public async Task<string> DeleteAllFileWithType(int id, string fileType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            string path = Common.GetAbsoluteProductMediaFolderPath(id.ToString());
            // Folder được tạo khi có image/video tương ứng
            if (path == null)
            {
                MySqlResultState rs = new MySqlResultState();
                return JsonConvert.SerializeObject(rs);
            }
            return await DeleteAllFileWithTypeBasicAsync(path, id, fileType);
        }

        [HttpPost]
        public async Task<string> UploadFile(object obj)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            var productId = Request.Headers["productId"];
            // Kiểm tra xem id sản phẩm có tồn tại. Có trường hợp sản phẩm đã xóa thành công,
            // nhưng web vẫn hiển thị nên cập nhật được ảnh
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                Product product = await ProductMySql.GetProductFromIdAsync(Common.ConvertStringToInt32(productId), conn);
                if(product == null)
                {
                    return JsonConvert.SerializeObject(
                        new MySqlResultState(EMySqlResultState.DONT_EXIST, "Sản phẩm không tồn tại."));
                }
            }

            string path = Common.GetAbsoluteProductMediaFolderPath(productId);
            if (path == null)
            {
                path = Common.CreateAbsoluteProductMediaFolderPath(productId);
            }

            return JsonConvert.SerializeObject(SaveImageVideo(path));
        }

        // Xóa ảnh của sản phẩm trước khi up ảnh mới từ thư mục
        [HttpPost]
        public async Task<string> DeleteImageBeforeUploadImageFromLocalTool(string productId)
        {
            string path = Common.GetAbsoluteProductMediaFolderPath(productId);
            if (path != null)
            {
                // Xóa ảnh ở thư mục cũ nếu có
                Common.DeleteAllImage(path);
            }
            return "Ok";
        }

        // Nhận file ảnh sản phẩm upload từ tool phía client, xóa ảnh cũ nếu có
        [HttpPost]
        public async Task<string> UploadImageFromLocalTool(HttpPostedFileBase file, string productId)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = Common.GetAbsoluteProductMediaFolderPath(productId);
                if (path == null)
                {
                    path = Common.CreateAbsoluteProductMediaFolderPath(productId);
                }
                //else
                //{
                //    // Xóa ảnh ở thư mục cũ nếu có
                //    Common.DeleteAllImage(path);
                //}

                // Tạo đường dẫn lưu ảnh
                string fileName = Path.GetFileName(file.FileName);

                path = path + fileName;
                // Lưu ảnh
                file.SaveAs(path);

                // Thêm watermark logo voi bé nhỏ và save ảnh phiên bản 320
                if (Common.ImageExtensions.Contains(Path.GetExtension(path).ToLower()))
                {
                    // Tiki không cho dùng ảnh có logo khi đăng sản phẩm nên commnet chức năng thêm logo
                    //string newsaveToFileLoc = Common.AddWatermark_DeleteOriginalImageFunc(saveToFileLoc);
                    //Common.ReduceImageSizeAndSave(newsaveToFileLoc);
                    Common.ReduceImageSizeTo320AndSave(path);
                }

                return "Ok. Upload thành công: " + fileName;
            }

            return "Ok. Không có file nào được gửi.";
        }

        [HttpPost]
        public async Task<string> UploadExcelFile(object obj)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            try
            {
                var length = Request.ContentLength;
                if (length > 0)
                {
                    var bytes = new byte[length];
                    Request.InputStream.Read(bytes, 0, length);

                
                    // Lưu file tạm ở thưc mục Media/Temporary/temp.xlsx
                    var saveToFileLoc = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["TemporaryMediaFolderPath"]) + "temp.xlsx";

                    // xóa file nếu đã tồn tại
                    System.IO.File.Delete(saveToFileLoc);

                    // save the file.
                    var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
                    fileStream.Write(bytes, 0, length);
                    fileStream.Close();
                }

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                result.State = EMySqlResultState.ERROR;
            }

            return JsonConvert.SerializeObject(new MySqlResultState());
        }

        ///// <summary>
        ///// Lấy thông tin chung của combo từ sản phẩm đầu tiên thuộc combo
        ///// </summary>
        ///// <param name="id">id combo</param>
        ///// <returns></returns>
        //[HttpPost]
        //public string GetProductCommonInfoWithComboFromFirst(int id)
        //{
        //    if ((await AuthentAdministratorAsync()) == null)
        //    {
        //        return JsonConvert.SerializeObject(null);
        //    }

        //    Product pro = sqler.GetProductFromFirstComboId(id);

        //    return JsonConvert.SerializeObject(pro);
        //}

        [HttpPost]
        public async Task<string> GetProductIdCodeBarcodeNameBooCoverkPrice(int publisherId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product> ls = await ProductMySql.GetProductIdCodeBarcodeNameBookCoverPriceAsync(publisherId);
            return JsonConvert.SerializeObject(ls);
        }

        // đã check số lượng khác 0 ở frontend
        [HttpPost]
        public async Task<string> AddImport(string listObject)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            List<Import> ls = null;
            MySqlResultState result = new MySqlResultState();
            try
            {
                ls = JsonConvert.DeserializeObject<List<Import>>(listObject);
                if (ls == null || ls.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Danh sách cần nhập rỗng hoặc lỗi.";
                }
                else
                {
                    result = await ProductMySql.AddListImportAsync(ls);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> OrderManually()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            // View copy từ Product/Import
            return View();
        }

        [HttpPost]
        public async Task<string> CreateOrderManually(string listObject, string customerInfor,
            string listOrderPay, string noteToShop, int sumPay)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            List<Import> ls = null;
            MySqlResultState result = new MySqlResultState();
            try
            {
                ls = JsonConvert.DeserializeObject<List<Import>>(listObject);

                // Tạm thời chưa xử lý phần dữ liệu này
                //Address cusInfor = JsonConvert.DeserializeObject<Address>(customerInfor);
                //List<OrderPay> lsOrderPay = JsonConvert.DeserializeObject<List<OrderPay>>(listOrderPay);
                //string note = noteToShop;

                if (ls == null || ls.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Danh sách cần nhập rỗng hoặc lỗi.";
                }
                else
                {
                    result = await ProductMySql.CreateOrderManuallyAsync(ls, sumPay);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public async Task<string> GetListImport(string fromDate, string toDate, string publisher)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Import>());
            }

            if (Common.ParameterOfURLQueryIsNullOrEmpty(fromDate))
            {
                fromDate = Common.GetDefaultDateTime().ToString(Common.dateFormat);
            }
            if (Common.ParameterOfURLQueryIsNullOrEmpty(toDate))
                toDate = DateTime.Now.ToString(Common.dateFormat);
            List<Import> ls = await ProductMySql.GetImportListAsync(fromDate, toDate, publisher);
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public async Task<string> UpdateImport(string listObject)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            List<Import> ls = null;
            try
            {
                ls = JsonConvert.DeserializeObject<List<Import>>(listObject);
                if (ls == null || ls.Count == 0)
                {
                    result.State = EMySqlResultState.ERROR;
                    result.Message = "Danh sách cần cập nhât không đúng.";
                }
                result = await ProductMySql.UpdateListImportAsync(ls);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        ///// <summary>
        ///// Tìm kiếm sản phẩm trong kho
        ///// </summary>
        ///// <param name="namePara"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public string SearchProductCount(string publisher,
        //    string codeOrBarcode, string name, string combo)
        //{
        //    if ((await AuthentAdministratorAsync()) == null)
        //    {
        //        return "0";
        //    }

        //    // Đếm số sản phẩm trong kết quả tìm kiếm
        //    int count = 0;
        //    ProductSearchParameter searchParameter = new ProductSearchParameter();
        //    searchParameter.publisher = publisher;
        //    searchParameter.codeOrBarcode = codeOrBarcode;
        //    searchParameter.name = name;
        //    searchParameter.combo = combo;
        //    count = sqler.SearchProductCount(searchParameter);
        //    return count.ToString();
        //}

        /// <summary>
        /// Tìm kiếm sản phẩm trong kho không phân trang
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> SearchProduct(string publisher,
            string codeOrBarcode, string name, string combo)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            ProductSearchParameter searchParameter = new ProductSearchParameter();
            searchParameter.publisher = publisher;
            searchParameter.codeOrBarcode = codeOrBarcode;
            searchParameter.name = name;
            searchParameter.combo = combo;
            //searchParameter.status = status;
            List<Product> lsSearchResult;
            lsSearchResult = await ProductMySql.SearchProduct(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public async Task<string> SearchProductForMapping(
            string codeOrBarcode, string name, string combo)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            ProductSearchParameter searchParameter = new ProductSearchParameter();
            searchParameter.codeOrBarcode = codeOrBarcode;
            searchParameter.name = name;
            searchParameter.combo = combo;
            List<Product> lsSearchResult = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    lsSearchResult = await ProductMySql.SearchProductForMappingAsync(searchParameter, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        // Lấy những sản phẩm trong kho có tên được chứa hoàn toàn
        // trong tên sản phẩm trên sàn thương mại điện tử hoặc ngược lại, không phân biệt hoa thường
        // Hàm này mục đích hiển thị những sản phẩm mapping tiềm năng khi mở modal mapping,
        // đỡ phải tìm kiếm mỏi tay.
        [HttpGet]
        public async Task<string> SearchProductFromTMDTNameForMapping(string tmdtItemName, string tmdtModelName)
        {
            List<Product> ls = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    ls = await ProductMySql.SearchProductFromTMDTNameForMappingAsync(tmdtItemName, tmdtModelName, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                ls = new List<Product>();
            }
            return JsonConvert.SerializeObject(ls);
        }

        // isSignle: true lấy sản phẩm trong kho chưa được bán như 1 sản phẩm riêng lẻ trên sàn (mapping chỉ có 1 mình sản phẩm), web.
        // Ngược lại lấy sản phẩm trong kho chưa được bán trên sàn cả 1 sản phẩm riêng lẻ, combo.
        [HttpGet]
        public async Task<string> SearchDontSellOnECommerce(Boolean isSingle, string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product> lsSearchResult = new List<Product>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                lsSearchResult = await ProductMySql.SearchDontSellOnECommerce(isSingle, eType, conn);
            }

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        // Chưa đăng bán đầy đủ combo, riêng lẻ ở cùng 1 sản phẩm cha / Item trên sàn
        // Các sản phẩm trong combo đều đang kinh doanh
        [HttpGet]
        public async Task<string> SearchDontSellFullComboAndSigleOnECommerce(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product> lsSearchResult = new List<Product>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Lấy danh sách sản phẩm có combo
                    List<Product> lsProduct = await ProductMySql.GetActiveProductHasComboActiveAll(conn);

                    // Tạo danh sách combo
                    List<Combo> lsCombo = new List<Combo>();
                    int count = lsProduct.Count();

                    Product proTemp = null;
                    Combo comboTemp = null;
                    for (int i = 0; i < count; i++)
                    {
                        proTemp = lsProduct[i];
                        if (comboTemp == null || proTemp.comboId != comboTemp.id)
                        {
                            lsCombo.Add(new Combo(proTemp.comboId, proTemp.comboName));
                            comboTemp = lsCombo[lsCombo.Count - 1];
                        }
                        comboTemp.products.Add(proTemp);
                    }

                    if(eType == Common.eTiki)
                    {
                        //lsSearchResult = TikiSearchDontSellFullComboAndSigleOnECommerce(lsCombo, conn);
                        foreach(var combo in lsCombo)
                        {
                            if(!await ProductMySql.TikiDontSellFullComboAndSigleConnectOut(combo, conn))
                            {
                                lsSearchResult.AddRange(combo.products);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        // Chưa đăng bán như biến thể
        [HttpGet]
        public async Task<string> SearchDontSellSigleWithParrentOnECommerce(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product> lsSearchResult = new List<Product>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    //// Lấy danh sách sản phẩm có combo
                    //List<Product> lsProduct = await ProductMySql.GetActiveProductHasComboActiveAll(conn);

                    //// Tạo danh sách combo
                    //List<Combo> lsCombo = new List<Combo>();
                    //int count = lsProduct.Count();

                    //Product proTemp = null;
                    //Combo comboTemp = null;
                    //for (int i = 0; i < count; i++)
                    //{
                    //    proTemp = lsProduct[i];
                    //    if (comboTemp == null || proTemp.comboId != comboTemp.id)
                    //    {
                    //        lsCombo.Add(new Combo(proTemp.comboId, proTemp.comboName));
                    //        comboTemp = lsCombo[lsCombo.Count - 1];
                    //    }
                    //    comboTemp.products.Add(proTemp);
                    //}

                    //if (eType == Common.eTiki)
                    //{
                    //    foreach (var combo in lsCombo)
                    //    {
                    //        if (!await ProductMySql.TikiDontSellSigleWithParrentConnectOut(combo, conn))
                    //        {
                    //            lsSearchResult.AddRange(combo.products);
                    //        }
                    //    }
                    //}
                    //else if(eType == Common.eLazada)
                    //{
                    //    foreach (var combo in lsCombo)
                    //    {
                    //        if (!await ProductMySql.Shopee_LazadaDontSellSigleWithParrentConnectOut(eType, combo, conn))
                    //        {
                    //            lsSearchResult.AddRange(combo.products);
                    //        }
                    //    }
                    //}
                     lsSearchResult = await ProductMySql.SearchDontSellSigleWithParrentOnECommerce(eType, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        // Chưa đăng bán hoàn toàn riêng lẻ, xét với tất cả sản phẩm đang kinh doanh
        [HttpGet]
        public async Task<string> SearchDontSellSigleWithNoParrentOnECommerce(string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product> lsSearchResult = new List<Product>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    lsSearchResult = await
                        ProductMySql.SearchDontSellSigleWithNoParrentOnECommerceConnectOut(eType, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public async Task<string> ChangePage(string publisher, string codeOrBarcode,
            string name, string combo,
            int start, int offset)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            ProductSearchParameter searchParameter = new ProductSearchParameter();
            searchParameter.publisher = publisher;
            searchParameter.codeOrBarcode = codeOrBarcode;
            searchParameter.name = name;
            searchParameter.combo = combo;
            searchParameter.start = start;
            searchParameter.offset = offset;

            List<Product> lsSearchResult;
            lsSearchResult = await ProductMySql.SearchProductChangePage(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public async Task<string> UpdateName(int id, string name)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateNameAsync(id, name));
        }

        [HttpGet]
        public async Task<string> UpdateShortName(int id, string shortname)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateShortNameAsync(id, shortname));
        }

        [HttpGet]
        public async Task<string> UpdateCode(int id, string code)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateCodeAsync(id, code));
        }

        [HttpPost]
        public async Task<string> UpdateQuantity(int id, int quantity)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateQuantityAsync(id, quantity));
        }

        [HttpGet]
        public async Task<string> UpdateISBN(int id, string isbn)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateISBNAsync(id, isbn));
        }

        [HttpPost]
        public async Task<string> UpdateDetail(int id, string detail)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            string decodeDetail = WebUtility.UrlDecode(detail);
            return JsonConvert.SerializeObject(await ProductMySql.UpdateDetailAsync(id, decodeDetail));
        }

        [HttpGet]
        public async Task<string> UpdateBookCoverPrice(int id, int bookCoverPrice)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateBookCoverPriceAsync(id, bookCoverPrice));
        }

        [HttpGet]
        public async Task<string> UpdateDiscountWhenImport(int id, float discount)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateDiscountWhenImportAsync(id, discount));
        }

        [HttpGet]
        public async Task<string> UpdatePositionInWarehouse(int id, string positionInWarehouse)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdatePositionInWarehouseAsync(id, positionInWarehouse));
        }

        [HttpGet]
        public async Task<string> UpdateStatusOfProduct(int id, int statusOfProduct)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateStatusOfProductAsync(id, statusOfProduct));
        }

        [HttpGet]
        public async Task<string> UpdateComboId(int id, int comboId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateComboIdAsync(id, comboId));
        }

        [HttpGet]
        public async Task<string> UpdateCategoryId(int id, int categoryId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateCategoryIdAsync(id, categoryId));
        }

        [HttpGet]
        public async Task<string> UpdatePublisherId(int id, int publisherId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdatePublisherIdAsync(id, publisherId));
        }

        [HttpGet]
        public async Task<string> UpdatePublishingCompany(int id, string publishingCompany)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdatePublishingCompanyAsync(id, publishingCompany));
        }

        [HttpGet]
        public async Task<string> UpdateLanguage(int id, string language)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateLanguageAsync(id, language));
        }

        [HttpGet]
        public async Task<string> UpdateHardCover(int id, int hardCover)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateHardCoverAsync(id, hardCover));
        }

        [HttpGet]
        public async Task<string> UpdateAuthor(int id, string author)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateAuthorAsync(id, author));
        }

        [HttpGet]
        public async Task<string> UpdateTranslator(int id, string translator)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateTranslatorAsync(id, translator));
        }

        [HttpGet]
        public async Task<string> UpdatePublishingTime(int id, int publishingTime)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdatePublishingTimeAsync(id, publishingTime));
        }
        //
        [HttpGet]
        public async Task<string> UpdatePageNumber(int id, int pageNumber)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdatePageNumberAsync(id, pageNumber));
        }

        [HttpGet]
        public async Task<string> UpdateDimensionsAndWeight(int id, int productLong, int productWide, int productHigh, int productWeight)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateDimensionsAndWeightAsync(id, productLong, productWide, productHigh, productWeight));
        }

        [HttpGet]
        public async Task<string> UpdateAgeRange(int id, int minAge, int maxAge)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(await ProductMySql.UpdateAgeRangeAsync(id, minAge, maxAge));
        }

        //// str có dạng: 12,45,24
        //private void GetListIntFromString(List<int> lsint, string str)
        //{
        //    try
        //    {
        //        string[] arr = str.Split(new char[] { ',' });
        //        for (int i = 0; i < arr.Length; i++)
        //        {
        //            lsint.Add(int.Parse(arr[i]));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn("str: " + str + ". " + ex.ToString());
        //        lsint.Clear();
        //    }
        //}

        [HttpPost]
        public async Task<string> UpdateQuantityFromListBelow(string listId, string listQuantity)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            List<int> lsId = JsonConvert.DeserializeObject<List<int>>(listId);
            List<int> lsQuantity = JsonConvert.DeserializeObject<List<int>>(listQuantity);

            return JsonConvert.SerializeObject(await ProductMySql.UpdateQuantityFromListAsync(lsId, lsQuantity));
        }

        [HttpPost]
        public async Task<string> GetListAuthor()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<string>());
            }

            return JsonConvert.SerializeObject(await ProductMySql.GetListAuthorAsync());
        }

        
        [HttpPost]
        public async Task<string> GetListTranslator()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<string>());
            }

            return JsonConvert.SerializeObject(await ProductMySql.GetListTranslatorAsync());
        }

        [HttpPost]
        public async Task<string> GetListPublishingCompany()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<string>());
            }

            return JsonConvert.SerializeObject(await ProductMySql.GetListPublishingCompanyAsync());
        }

        [HttpPost]
        public async Task<string> GetListProductName()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<ProductIdName>());
            }
            return JsonConvert.SerializeObject(await ProductMySql.GetListProductNameAsync());
        }

        [HttpGet]
        public async Task<ActionResult> NeedUpdateQuatity()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        [HttpPost]
        public async Task<string> GetListNeedUpdateQuantityAndUpdate()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(await CommonProductSupporting.GetListNeedUpdateQuantityAndUpdate_CoreAsync());
        }

        [HttpPost]
        public async Task<string> GetListProductInWarehouseChangedQuantity()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }
            List<Product> ls = null;
            ls = await ProductMySql.GetListProductInWarehouseChangedQuantityAsync();
            return JsonConvert.SerializeObject(ls);
        }

        [HttpGet]
        public async Task<ActionResult> MappingOfProduct(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        //private List<CommonItem> ShopeeGetListMappingOfProduct(int id, MySqlConnection conn)
        //{
        //    // Danh sách sản phẩm Shopee
        //    List<CommonItem> shopeeList = ProductMySql.ShopeeGetListMappingOfProduct(id, conn);
        //    //ShopeeGetStatusImageSrcQuantitySellable(shopeeList);
        //    return shopeeList;
        //}

        //private List<CommonItem> LazadaGetListMappingOfProduct(int id, MySqlConnection conn)
        //{
        //    // Danh sách sản phẩm Shopee
        //    List<CommonItem> ls = LazadaMySql.LazadaGetListMappingOfProduct(id, conn);
        //    return ls;
        //}

        //private List<CommonItem> TikiGetListMappingOfProductAsync(int id, MySqlConnection conn)
        //{
        //    // Danh sách sản phẩm Tiki
        //    List<CommonItem> tikiList = await TikiMySql.TikiGetListMappingOfProductAsync(id, conn);
        //    //TikiGetStatusImageSrcQuantitySellable(tikiList);
        //    return tikiList;
        //}

        private async Task<List<CommonItem>> GetListMappingOfProductCoreAsync(int id, MySqlConnection conn)
        {
            List<CommonItem> ls = new List<CommonItem>();
            try
            {
                List<CommonItem> shopeeList = await ShopeeMySql.ShopeeGetListMappingOfProductAsync(id, conn);
                List<CommonItem> tikiList = await TikiMySql.TikiGetListMappingOfProductAsync(id, conn);
                List<CommonItem> lazadaList = await LazadaMySql.LazadaGetListMappingOfProductAsync(id, conn);
                List<CommonItem> vbnList = await SanPhamMySql.GetListMappingOfProductAsync(id, conn);
                ls.AddRange(tikiList);
                ls.AddRange(shopeeList);
                ls.AddRange(lazadaList);
                ls.AddRange(vbnList);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return ls;
        }

        [HttpPost]
        public async Task<string> GetListMappingOfProduct(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            List<CommonItem> ls = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    ls = await GetListMappingOfProductCoreAsync(id, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(ls);
        }

        // Update số lượng của 1 model lên sàn
        [HttpPost]
        public async Task<string> UpdateQuantityOfOneItemModel(string eType, long itemId, long modelId)
        {
            MySqlResultState result = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    if ((await AuthentAdministratorConnectOutAsync(conn)) == null)
                    {
                        return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
                    }

                    if (eType == Common.eTiki)
                    {
                        result = await TikiProductSuporting.TikiUpdateQuantityOfOneItemModelAsync((int)itemId, conn);
                    }
                    else if (eType == Common.eShopee)
                    {
                        result = await ShopeeProductSupporting.ShopeeUpdateQuantityOfOneItemModelAsync(itemId, modelId, conn);
                    }
                    else if (eType == Common.eLazada)
                    {
                        result = await LazadaProductSupporting.LazadaUpdateQuantityOfOneItemModelAsync(itemId, modelId, conn);
                    }
                    else if (eType == Common.ePlayWithMe)
                    {
                        result = await VBNProductSupporting.VBNUpdateQuantityOfOneItemModelAsync((int)itemId, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.ToString());
            }

            return JsonConvert.SerializeObject(result);
        }

        public static async Task UpdateQuantityToTMDTFromListCommonItemConnectOutAsync(
            EECommerceType eECommerceType,
            List<CommonItem> listCommonItem,
            MySqlConnection conn
            )
        {
            if (eECommerceType == EECommerceType.TIKI)
            {
                foreach (CommonItem commonItem in listCommonItem)
                {
                    await TikiProductSuporting.TikiUpdateQuantityOfOneItemAsync(commonItem/*, conn*/);
                }
            }
            else if (eECommerceType == EECommerceType.SHOPEE)
            {
                foreach (CommonItem commonItem in listCommonItem)
                {
                    await ShopeeProductSupporting.ShopeeUpdateQuantityOfOneItemAsync(commonItem/*, conn*/);
                }
            }
            else if (eECommerceType == EECommerceType.LAZADA)
            {
                List<CommonItem> lazadaList = new List<CommonItem>();
                foreach (CommonItem commonItem in listCommonItem)
                {
                    lazadaList.Add(commonItem);
                }
                await LazadaProductSupporting.LazadaUpdateQuantity_CoreAsync(lazadaList);
            }
            else if(eECommerceType == EECommerceType.PLAY_WITH_ME)
            {
                await SanPhamMySql.BulkUpdateQuantityAsync(conn, listCommonItem);
            }
        }

        // Cập nhật số lượng lên sàn và cập nhật trạng thái sản phẩm ở tbNeedUpdateQuantity
        public static async Task UpdateQuantityToTMDT_DbFromListCommonItemAsync(
            EECommerceType eECommerceType,
            List<CommonItem> listCommonItem,
            List<int> productIdList)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    await UpdateQuantityToTMDTFromListCommonItemConnectOutAsync(eECommerceType, listCommonItem, conn);

                    // Cập nhật lên sàn ok, ta cập nhật trạng thái status = 0 ở tbNeedUpdateQuantity

                    await CommonProductSupporting.UpdateZeroStatusOfNeedUpdateQuantityConnectOutAsync(eECommerceType, productIdList, listCommonItem, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Update số lượng của 1 list common item tương ứng với 1 sản phẩm hoặc combo trong kho
        [HttpPost]
        public async Task<string> UpdateQuantityToTMDTFromListCommonItem(
            Boolean isCombo,
            int productOrComboId,
            string listCommonItem)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            List<CommonItem> ls = null;
            try
            {
                ls = JsonConvert.DeserializeObject<List<CommonItem>>(listCommonItem);
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            if (ls != null && ls.Count > 0)
            {
                List<int> listProductId = new List<int>();
                if (!isCombo)
                {
                    listProductId.Add(productOrComboId);
                }
                else
                {
                    // Lấy danh sách id sản phẩm đang kinh doanh thuộc combo Id
                    listProductId = await ComboMySql.GetProductIdsOfComboAsync(productOrComboId);
                }

                await UpdateQuantityFromListProductId_CommonItemAsync(listProductId, ls);
            }

            return JsonConvert.SerializeObject(ls);
        }

        public static async Task UpdateQuantityFromListProductId_CommonItemAsync(
            List<int> listProductId,
            List<CommonItem> ls)
        {
            List<CommonItem> shopeeList = new List<CommonItem>();
            List<CommonItem> tikiList = new List<CommonItem>();
            List<CommonItem> lazadaList = new List<CommonItem>();
            List<CommonItem> vbnList = new List<CommonItem>();
            foreach (CommonItem commonItem in ls)
            {
                if (commonItem.eType == Common.eTiki)
                {
                    tikiList.Add(commonItem);
                }
                else if (commonItem.eType == Common.eShopee)
                {
                    shopeeList.Add(commonItem);
                }
                else if (commonItem.eType == Common.eLazada)
                {
                    lazadaList.Add(commonItem);
                }
                else if (commonItem.eType == Common.ePlayWithMe)
                {
                    vbnList.Add(commonItem);
                }
            }

            await UpdateQuantityToTMDT_DbFromListCommonItemAsync(EECommerceType.TIKI, tikiList, listProductId);
            await UpdateQuantityToTMDT_DbFromListCommonItemAsync(EECommerceType.SHOPEE, shopeeList, listProductId);
            await UpdateQuantityToTMDT_DbFromListCommonItemAsync(EECommerceType.LAZADA, lazadaList, listProductId);
            await UpdateQuantityToTMDT_DbFromListCommonItemAsync(EECommerceType.PLAY_WITH_ME, vbnList, listProductId);
        }

        //Update tồn kho lên sàn, web theo id sản phẩm hoặc combo
        [HttpPost]
        public async Task<string> UpdateQuantityToTMDTFromProductComboId(
            Boolean isCombo,
            int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            List<CommonItem> ls = null;
            try

            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    if (!isCombo)
                    {
                        ls = await GetListMappingOfProductCoreAsync(id, conn);
                    }
                    else
                    {
                        ls = await ComboController.GetListMappingOfComboCoreAsync(id, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            if (ls != null && ls.Count > 0)
            {
                List<int> listProductId = new List<int>();
                if (!isCombo)
                {
                    listProductId.Add(id);
                }
                else
                {
                    // Lấy danh sách id sản phẩm đang kinh doanh thuộc combo Id
                    listProductId = await ComboMySql.GetProductIdsOfComboAsync(id);
                }
                await UpdateQuantityFromListProductId_CommonItemAsync(listProductId, ls);
            }

            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public async Task<string> GetSellingStatistics(string eType, int intervalDay)
        {
            List<ProductSellingStatistics> statisticsList = null;

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<ProductSellingStatistics>());
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    statisticsList = await ProductMySql.GetProductSellingStatisticsAsync(
                        Common.GetEECommerceTypeFromString(eType),
                        intervalDay, -1, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                statisticsList = new List<ProductSellingStatistics>();
            }
            return JsonConvert.SerializeObject(statisticsList);
        }

        [HttpPost]
        public async Task<string> GetHintQuantityFromPublisher(int publisherId, int intervalDay)
        {
            List<ProductSellingStatistics> statisticsList = null;

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<ProductSellingStatistics>());
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    statisticsList = await ProductMySql.GetProductSellingStatisticsAsync(
                        EECommerceType.ALL, intervalDay, publisherId, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                statisticsList = new List<ProductSellingStatistics>();
            }
            return JsonConvert.SerializeObject(statisticsList);
        }

        // Lấy lịch sử bán hàng của một sản phẩm trong kho
        [HttpPost]
        public async Task<string> GetOutputOfProduct(int id)
        {
            List<Output> outputList = null;

            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Output>());
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    outputList =await ProductMySql.GetOutputOfProductAsync(id, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                outputList = new List<Output>();
            }
            return JsonConvert.SerializeObject(outputList);
        }


        [HttpPost]
        public async Task<string> CreateProductOnECommerce(int id, string eType, string name)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                // Lấy danh sách sản phẩm chưa đăng bán riêng lẻ không phải biến thể
                List<int> needList =
                    await ProductMySql.SearchDontSellSigleWithNoParrentOnECommerce_GetIdListOnly_ConnectOutAsync(
                    eType, conn
                    );

                if (!needList.Contains(id))
                {
                    result.State = EMySqlResultState.EXIST;
                    result.Message = "Sản phẩm đã được đăng riêng lẻ không phải biến thể.";
                }
                else
                {
                    if (eType == Common.eTiki)
                    {
                        await TikiProductSuporting.TikiCreateProductFromProductIdInWarehouse(id, name, result, conn);
                    }
                    else if (eType == Common.eShopee)
                    {
                        result = await ShopeeProductSupporting.ShopeeCreateProductFromProductIdInWarehouse(id, name, true, conn);
                    }
                    else if (eType == Common.eLazada)
                    {
                        result = await LazadaProductSupporting.LazadaCreateProductFromProductIdInWarehouse(id, name, true, conn);
                    }
                    else
                    {
                        result.State = EMySqlResultState.INVALID;
                        result.Message = "Chưa hỗ trợ tạo sản phẩm trên " + eType;
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> UploadImageOnECommerce(int id, string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                if (eType == Common.eShopee ||
                    eType == Common.eLazada)
                {
                    result = await CommonProductSupporting.UploadImageOnECommerce_Core(id, eType, conn);
                }
                else
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Chưa hỗ trợ trên " + eType;
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public async Task<string> TikiUpdateProductImage(int id, int tikiProductId)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();

            try
            {
                Product product = null;
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    product = await ProductMySql.GetProductFromIdAsync(id, conn);
                }
                if(product == null)
                {
                    result.State = EMySqlResultState.EMPTY;
                    result.Message = "Không lấy được sản phẩm trong kho";
                }    

                if (product.imageSrc.Count > 0)
                {
                    TikiForUpdatingVariant tikiForUpdatingVariant = new TikiForUpdatingVariant();

                    // Bắt buộc có kích thước, cân nặng theo Backend Tiki 
                    tikiForUpdatingVariant.attributes.product_length = TikiCreateProduct.TikiGetStringOneDimension(product.productLong);
                    tikiForUpdatingVariant.attributes.product_width = TikiCreateProduct.TikiGetStringOneDimension(product.productWide);
                    tikiForUpdatingVariant.attributes.product_height = TikiCreateProduct.TikiGetStringOneDimension(product.productHigh);
                    tikiForUpdatingVariant.attributes.product_weight_kg = TikiCreateProduct.TikiGetStringWeight(product.productWeight);

                    tikiForUpdatingVariant.product_id = tikiProductId;
                    tikiForUpdatingVariant.image = Common.httpsVoiBeNho + product.imageSrc[0];
                    for (int i = 1; i < product.imageSrc.Count; i++)
                    {
                        tikiForUpdatingVariant.images.Add(Common.httpsVoiBeNho + product.imageSrc[i]);
                    }

                    // Gửi yêu cầu cập nhật variant
                    await TikiUpdateStock.TikiProductUpdateSomethingForVariant(tikiForUpdatingVariant, result);
                }
            }
            catch(Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> CreateProductOnECommerceFromList(string listId, string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            List<int> productIds = JsonConvert.DeserializeObject<List<int>>(listId);

            return await CommonProductSupporting.CreateProductOnECommerceFromList_Core(productIds, eType);
        }

        [HttpPost]
        public async Task<string> CreateSingleProductOfComboOnECommerce(int comboId, string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // Lấy danh sách id sản phẩm đang kinh doanh thuộc combo Id
            List<int> productIds = await ComboMySql.GetProductIdsOfComboAsync(comboId);

            return await CommonProductSupporting.CreateProductOnECommerceFromList_Core(productIds, eType);
        }

        [HttpPost]
        public async Task<string> CreateVariantsOfComboOnECommerce(int comboId, string eType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    if (eType == Common.eTiki)
                    {
                        await TikiProductSuporting.TikiCreateVariantsFromComboId(comboId, result, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);

        }

        // Tạo sản phẩm từ sản phẩm khác.
        // Sản phẩm được lấy trên web khác bằng browser extension
        [HttpPost]
        public async Task<string> CreateItemFromOther()
        {
            MyLogger.GetInstance().Info("CreateItemFromOther Called");
            MySqlResultState result = new MySqlResultState();
            // Đọc request body
            string requestBody;
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }
                MyLogger.GetInstance().Info("requestBody: " + requestBody);
                ItemForCreate item = JsonConvert.DeserializeObject<ItemForCreate>(requestBody, Common.jsonSerializersettings);

                //// Tải media
                //Boolean isOk = await DownloadMediaOfItemForCreate(item);
                //if(!isOk)
                //{
                //    result.State = EMySqlResultState.ERROR;
                //    result.Message = "Tải media có lỗi";
                //    return JsonConvert.SerializeObject(result);
                //}

                if (item.siteName == Common.eLazada)
                {
                    result = await LazadaProductSupporting.LazadaCreateItemFromOther(item);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return JsonConvert.SerializeObject(result);
        }

        // Từ src tải file ảnh, video
        // Chỉ cần tải lỗi sẽ return. Sau đó cần tải lại từ đầu
        public static async Task<Boolean> DownloadMediaOfItemForCreate(ItemForCreate item)
        {
            int count = 0;
            string fileName = "";
            string extension = "";
            string path = string.Empty;
            // Xóa tất cả file trong thư mục temporary trước
            Common.DeleteFilesInDirectory(Common.absoluteForCreateMediaFolderPath);
            // Tải ảnh của item
            foreach(var src in item.srcImages)
            {
                extension = Path.GetExtension(src);
                if(string.IsNullOrEmpty(extension)
                    || extension.ToLower().Contains("webp")
                     || extension.ToLower().Contains("gif"))
                {
                    extension = ".jpg";
                }

                fileName = count.ToString() + extension;
                path = await Common.DownloadImageFromImageSrc_CoreAsync(
                    Common.absoluteForCreateMediaFolderPath,
                    src,
                    fileName);
                if(string.IsNullOrEmpty(path))
                {
                    return false;
                }
                item.pathImages.Add(path);
                count++;
            }
            // Tải ảnh của models
            foreach(var model in item.models)
            {
                foreach(var src in model.srcImages)
                {
                    extension = Path.GetExtension(src);
                    if (string.IsNullOrEmpty(extension)
                        || extension.ToLower().Contains("webp")
                         || extension.ToLower().Contains("gif"))
                    {
                        extension = ".jpg";
                    }
                    fileName = count.ToString() + extension;
                    path = await Common.DownloadImageFromImageSrc_CoreAsync(
                        Common.absoluteForCreateMediaFolderPath,
                        src,
                        fileName);
                    if (string.IsNullOrEmpty(path))
                    {
                        return false;
                    }
                    model.pathImages.Add(path);
                    count++;
                }
            }
            // Tải ảnh trong description
            foreach(var des in item.descriptions)
            {
                if(!des.isText)
                {
                    string src = des.content;
                    extension = Path.GetExtension(src);
                    if (string.IsNullOrEmpty(extension)
                        || extension.ToLower().Contains("webp")
                         || extension.ToLower().Contains("gif"))
                    {
                        extension = ".jpg";
                    }
                    fileName = count.ToString() + extension;
                    path = await Common.DownloadImageFromImageSrc_CoreAsync(
                        Common.absoluteForCreateMediaFolderPath,
                        src,
                        fileName);
                    if (string.IsNullOrEmpty(path))
                    {
                        return false;
                    }
                    des.path = path;
                    count++;
                }
            }
            // Tải video nếu có
            if(!string.IsNullOrEmpty(item.srcVideo))
            {
                string filePath = Path.Combine(Common.absoluteForCreateMediaFolderPath, "0.mp4");
                path = await Common.DownloadVideo(item.srcVideo, filePath);
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }
                item.pathVideo = path;
            }

            return true;
        }

        // Cập nhật tồn kho lên sàn những sản phẩm trong đơn
        public static async Task<MySqlResultState> UpdateQuantityOfProductsInOrder(
            EECommerceType eECommerceType,
            string orderCode,
            MySqlConnection conn)
        {
            MySqlResultState reuslt = new MySqlResultState();
            try
            {
                if (eECommerceType == EECommerceType.SHOPEE)
                {
                    // Lấy sản phẩm trong đơn
                    ShopeeOrderDetail shopeeOrderDetail = await ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailFromOrderSNAsync(orderCode);
                    if (shopeeOrderDetail != null)
                    {
                        CommonOrder commonOrder = new CommonOrder(shopeeOrderDetail);
                        await ShopeeMySql.ShopeeGetMappingOfCommonOrderConnectOutAsync(commonOrder, conn);

                        // Lấy danh sách commonItem để cập nhật số lượng
                        List<CommonItem> commonItems = new List<CommonItem>();
                        for (int i = 0; i < commonOrder.listItemId.Count; i++)
                        {
                            CommonItem commonItem = null;
                            Boolean isExist = false;
                            foreach (CommonItem objItem in commonItems)
                            {
                                if (objItem.itemId == commonOrder.listItemId[i])
                                {
                                    isExist = true;
                                    commonItem = objItem;
                                    break;
                                }
                            }
                            if (!isExist)
                            {
                                commonItem = new CommonItem();
                                commonItem.itemId = commonOrder.listItemId[i];
                                commonItems.Add(commonItem);
                            }

                            CommonModel commonModel = new CommonModel();
                            commonModel.modelId = commonOrder.listModelId[i];
                            commonModel.mapping = commonOrder.listMapping[i];
                            commonItem.models.Add(commonModel);
                        }

                        foreach (var commonItem in commonItems)
                        {
                            await ShopeeProductSupporting.ShopeeUpdateQuantityOfOneItemAsync(commonItem/*, conn*/);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, reuslt);
            }
            return reuslt;
        }

        [HttpPost]
        public async Task<string> UpdateInventoryForOrder(string code = null, int? ecommerce = null)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = new MySqlResultState();
            // Nếu không có parameters từ form, đọc từ JSON body
            if (code == null || ecommerce == null)
            {
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    string body = await reader.ReadToEndAsync();
                    var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                    code = jsonData["code"]?.ToString();
                    ecommerce = Convert.ToInt32(jsonData["ecommerce"]);
                }
            }

            if (string.IsNullOrEmpty(code))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Mã đơn hàng không được để trống";
                return JsonConvert.SerializeObject(result);
            }

            EECommerceType ecomType = (EECommerceType)ecommerce;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    await UpdateQuantityOfProductsInOrder(ecomType, code, conn);
                }

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                return JsonConvert.SerializeObject(result);
            }
        }

        public async Task<string> CreateSanPhamVoibenhoFromProductId(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            // Tạo sản phẩm mới
            MySqlResultState result = await ProductMySql.CreateSanPhamVoibenhoFromProductId(id);

            // tạo mapping cho sản phẩm mới
            if (result.State == EMySqlResultState.OK)
            {
                SanPhamMapping mapping = new SanPhamMapping
                {
                    SanPhamBanId = result.myAnything,
                    SanPhamKhoId = id,
                    Quantity = 1,
                };

                result = await SanPhamMappingMySql.InsertAsync(mapping);
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}
