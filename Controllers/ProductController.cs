using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class ProductController : BasicController
    {
        public ProductMySql sqler;
        public ComboMySql comboSqler;
        public CategoryMySql categorySqler;
        public PublisherMySql publisherSqler;
        public ProductController () : base ()
        {
            sqler = new ProductMySql();
            comboSqler = new ComboMySql();
            categorySqler = new CategoryMySql();
            publisherSqler = new PublisherMySql();
        }

        private void GetViewDataForInput()
        {
            //ViewDataGetListCombo();
            //ViewDataGetListCategory();
            //ViewDataGetListAuthor();
            //ViewDataGetListTranslator();
            //ViewDataGetListPublisher();
            //ViewDataGetListPublishingCompany();
            //ViewDataGetListProductName();

            ViewDataGetListProductLong();
            ViewDataGetListProductHigh();
            ViewDataGetListProductWide();
            ViewDataGetListProductWeight();
            ViewDataGetListMinAge();
            ViewDataGetListMaxAge();
            ViewDataGetListPublishingTime();
        }
        // GET: Product
        public ActionResult Create()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            //GetViewDataForInput();

            return View();
        }

        [HttpPost]
        public string GetProductFromId(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            return JsonConvert.SerializeObject(sqler.GetProductFromId(id));
        }

        public ActionResult UpdateDelete(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            //GetViewDataForInput();

            return View();
        }

        public ActionResult Import()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListProductName();

            return View();
        }

        public ActionResult ChangeImport()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        public ActionResult Search()
        {
            if (AuthentAdministrator() == null)
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
        //        comboId = comboSqler.GetComboIdFromName(comboName);

        //    if (string.IsNullOrWhiteSpace(categoryName))
        //        categoryId = -1;
        //    else
        //        categoryId = categorySqler.GetCategoryIdFromName(categoryName);

        //    if (string.IsNullOrWhiteSpace(publisherName))
        //        publisherId = -1;
        //    else
        //        publisherId = publisherSqler.GetPublisherIdFromName(publisherName);

        //    if (string.IsNullOrWhiteSpace(parentName))
        //        parentId = -1;
        //    else
        //        parentId = sqler.GetProductIdFromName(parentName);
        //}

        public string AddNewPro(
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
                int minAge,
                int maxAge,
                int parentId, // Nếu không có sản phẩm cha, parentId = -1
                int republish,// Nếu không xác định, parentId = -1
                string detail,
                int status,
                int pageNumber
                )
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

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
                 minAge,
                 maxAge,
                 parentId,
                 republish,
                 detail,
                 status,
                 pageNumber
                 );

             MySqlResultState result = sqler.AddNewPro(pro);
            return JsonConvert.SerializeObject(result);
        }

        public string UpdateProduct(
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
                int minAge,
                int maxAge,
                int parentId, // Nếu không có sản phẩm cha, parentId = -1
                int republish,// Nếu không xác định, parentId = -1
                string detail,
                int status,
                int pageNumber
                )
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

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
                 minAge,
                 maxAge,
                 parentId,
                 republish,
                 detail,
                 status,
                 pageNumber
                 );

            MySqlResultState result = sqler.UpdateProduct(pro);

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Xóa sản phẩm
        /// Sản phẩm chỉ có thế xóa khi đang không liên kết với sản phẩm nào trên
        /// Shopee, Tiki, Lazada, web voibenho, trong đơn hàng đã được bán, thông tin nhập xuất thực tế,...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string DeleteProduct(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = sqler.DeleteProduct(id);

            // xóa thư mục ảnh video của sản phẩm
            string path = Common.GetAbsoluteProductMediaFolderPath(id.ToString());
            if (path != null)
            {
                Common.DeleteMediaFolder(path);
            }

            return JsonConvert.SerializeObject(result);
        }

        public string UpdateCommonInfoWithCombo(
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
                int minAge,
                int maxAge,
                int republish,
                int status,
                int pageNumber
            )
        {
            if (AuthentAdministrator() == null)
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
                 minAge,
                 maxAge,
                 republish,
                 status,
                 pageNumber
                );
            MySqlResultState result = sqler.UpdateCommonInfoWithCombo(pro);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="fileType">isImage hoặc isVideo</param>
        /// <returns></returns>
        //[HttpPost]
        public string DeleteAllFileWithType(int id, string fileType)
        {
            if (AuthentAdministrator() == null)
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
            return DeleteAllFileWithTypeBasic(path, id, fileType);
        }

        [HttpPost]
        public string UploadFile(object obj)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            var productId = Request.Headers["productId"];
            string path = Common.GetAbsoluteProductMediaFolderPath(productId);
            if (path == null)
            {
                path = Common.CreateAbsoluteProductMediaFolderPath(productId);
            }

            return JsonConvert.SerializeObject(SaveImageVideo(path));
        }

        [HttpPost]
        public string UploadExcelFile(object obj)
        {
            if (AuthentAdministrator() == null)
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
        //    if (AuthentAdministrator() == null)
        //    {
        //        return JsonConvert.SerializeObject(null);
        //    }

        //    Product pro = sqler.GetProductFromFirstComboId(id);

        //    return JsonConvert.SerializeObject(pro);
        //}

        [HttpPost]
        public string GetProductIdCodeBarcodeNameBooCoverkPrice(int publisherId)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product> ls = sqler.GetProductIdCodeBarcodeNameBookCoverPrice(publisherId);
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public string AddImport(string listObject)
        {
            if (AuthentAdministrator() == null)
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
                    result = sqler.AddListImport(ls);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        public ActionResult OrderManually()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            // View copy từ Product/Import
            return View();
        }

        [HttpPost]
        public string CreateOrderManually(string listObject, string customerInfor,
            string listOrderPay, string noteToShop, int sumPay)
        {
            if (AuthentAdministrator() == null)
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
                    result = sqler.CreateOrderManually(ls, sumPay);
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public string GetListImport(string fromDate, string toDate, string publisher)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<Import>());
            }

            if (Common.ParameterOfURLQueryIsNullOrEmpty(fromDate))
                fromDate = "2018-08-05";
            if (Common.ParameterOfURLQueryIsNullOrEmpty(toDate))
                toDate = DateTime.Now.ToString(Common.dateFormat);
            List<Import> ls = sqler.GetImportList(fromDate, toDate, publisher);
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public string UpdateImport(string listObject)
        {
            if (AuthentAdministrator() == null)
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
                result = sqler.UpdateListImport(ls);
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
        //    if (AuthentAdministrator() == null)
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
        public string SearchProduct(string publisher,
            string codeOrBarcode, string name, string combo)
        {
            if (AuthentAdministrator() == null)
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
            lsSearchResult = sqler.SearchProduct(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        // isSignle: true lấy sản phẩm trong kho chưa được bán như 1 sản phẩm riêng lẻ trên sàn (mapping chỉ có 1 mình sản phẩm), web.
        // Ngược lại lấy sản phẩm trong kho chưa được bán trên sàn cả 1 sản phẩm riêng lẻ, combo.
        [HttpGet]
        public string SearchDontSellOnECommerce(Boolean isSingle, string eType)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product>  lsSearchResult = sqler.SearchDontSellOnECommerce(isSingle, eType);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public string ChangePage(string publisher, string codeOrBarcode,
            string name, string combo,
            int start, int offset)
        {
            if (AuthentAdministrator() == null)
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
            lsSearchResult = sqler.SearchProductChangePage(searchParameter);

            return JsonConvert.SerializeObject(lsSearchResult);
        }

        [HttpGet]
        public string UpdateName(int id, string name)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateName(id, name));
        }

        [HttpGet]
        public string UpdateCode(int id, string code)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateCode(id, code));
        }

        [HttpPost]
        public string UpdateQuantity(int id, int quantity)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateQuantity(id, quantity));
        }

        [HttpGet]
        public string UpdateISBN(int id, string isbn)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateISBN(id, isbn));
        }

        [HttpGet]
        public string UpdateBookCoverPrice(int id, int bookCoverPrice)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateBookCoverPrice(id, bookCoverPrice));
        }

        [HttpGet]
        public string UpdateDiscountWhenImport(int id, float discount)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateDiscountWhenImport(id, discount));
        }

        [HttpGet]
        public string UpdatePositionInWarehouse(int id, string positionInWarehouse)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdatePositionInWarehouse(id, positionInWarehouse));
        }

        [HttpGet]
        public string UpdateStatusOfProduct(int id, int statusOfProduct)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateStatusOfProduct(id, statusOfProduct));
        }

        [HttpGet]
        public string UpdateComboId(int id, int comboId)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateComboId(id, comboId));
        }

        [HttpGet]
        public string UpdateCategoryId(int id, int categoryId)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateCategoryId(id, categoryId));
        }

        [HttpGet]
        public string UpdatePublisherId(int id, int publisherId)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdatePublisherId(id, publisherId));
        }

        // str có dạng: 12,45,24
        private void GetListIntFromString(List<int> lsint, string str)
        {
            try
            {
                string[] arr = str.Split(new char[] { ',' });
                for (int i = 0; i < arr.Length; i++)
                {
                    lsint.Add(int.Parse(arr[i]));
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn("str: " + str + ". " + ex.ToString());
                lsint.Clear();
            }
        }

        public string UpdateQuantityFromListBelow(string listId, string listQuantity)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            List<int> lsId = new List<int>();
            List<int> lsQuantity = new List<int>();
            GetListIntFromString(lsId, listId);
            GetListIntFromString(lsQuantity, listQuantity);

            return JsonConvert.SerializeObject(sqler.UpdateQuantityFromList(lsId, lsQuantity));
        }

        [HttpPost]
        public string GetListAuthor()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<string>());
            }

            return JsonConvert.SerializeObject(sqler.GetListAuthor());
        }

        
        [HttpPost]
        public string GetListTranslator()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<string>());
            }

            return JsonConvert.SerializeObject(sqler.GetListTranslator());
        }

        [HttpPost]
        public string GetListPublishingCompany()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<string>());
            }

            return JsonConvert.SerializeObject(sqler.GetListPublishingCompany());
        }

        [HttpPost]
        public string GetListProductName()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<ProductIdName>());
            }
            return JsonConvert.SerializeObject(sqler.GetListProductName());
        }

        [HttpGet]
        public ActionResult NeedUpdateQuatity()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        // Hàm này tạm thời chưa dùng vì trạng thái item/model ở db chưa được cập nhật realtime
        // Nếu db chưa lưu image src của item, model ta lấy và lưu
        private void ShopeeUpdateImageSrcToDbIfNeed(List<CommonItem> shopeeList, MySqlConnection conn)
        {
            foreach (var item in shopeeList)
            {
                Boolean isNeedDownloadImage = false;

                if (string.IsNullOrEmpty(item.imageSrc))
                {
                    isNeedDownloadImage = true;
                }
                foreach (var model in item.models)
                {
                    if (string.IsNullOrEmpty(model.imageSrc))
                    {
                        isNeedDownloadImage = true;
                        break;
                    }
                }
                if (isNeedDownloadImage)
                {
                    ShopeeGetItemBaseInfoItem pro = ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromId(item.itemId);
                    if (pro != null)
                    {
                        // Lấy imageSrc cho item
                        if (string.IsNullOrEmpty(item.imageSrc))
                        {
                            item.imageSrc = pro.image.image_url_list[0];
                            sqler.UpdateImageSrcShopeeItem(item.itemId, item.imageSrc, conn);
                        }

                        // Lấy imageSrc cho model nếu có
                        if (pro.has_model)
                        {
                            ShopeeGetModelListResponse obj = ShopeeGetModelList.ShopeeProductGetModelList(pro.item_id);
                            if (obj != null)
                            {
                                ShopeeGetModelList_TierVariation tierVar = obj.tier_variation[0];
                                int count = tierVar.option_list.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    ShopeeGetModelList_Model model = CommonItem.GetModelFromModelListResponse(obj, i);
                                    ShopeeGetModelList_TierVariation_Option option = tierVar.option_list[i];
                                    if (option.image != null)
                                    {
                                        foreach (var m in item.models)
                                        {
                                            if (!string.IsNullOrEmpty(m.imageSrc))
                                                continue;

                                            if (m.modelId == model.model_id)
                                            {
                                                m.imageSrc = option.image.image_url;
                                                sqler.UpdateImageSrcShopeeModel(m.modelId, m.imageSrc, conn);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Lấy trạng thái sản phẩm, image đại diện, số lượng trên sàn Shopee.
        // Cập nhật số lượng với sản phẩm NORMAL
        // NOTE:
        // Hàm này không dùng nữa do mất thời gian.
        // Image và status được lưu trong db. Sau này có chức nặng cập nhật Image và status
        public void ShopeeGetStatusImageSrcQuantitySellable(List<CommonItem> shopeeList)
        {
            foreach (var item in shopeeList)
            {
                ShopeeGetItemBaseInfoItem pro = ShopeeGetItemBaseInfo.ShopeeProductGetItemBaseInfoFromId(item.itemId);
                if (pro != null)
                {
                    item.imageSrc = pro.image.image_url_list[0];
                    if (pro.item_status != "NORMAL")
                    {
                        item.bActive = false;
                        continue;
                    }

                    item.bActive = true;

                    // Lấy imageSrc cho model nếu có
                    if (pro.has_model)
                    {
                        ShopeeGetModelListResponse obj = ShopeeGetModelList.ShopeeProductGetModelList(pro.item_id);
                        if (obj != null)
                        {
                            ShopeeGetModelList_TierVariation tierVar = obj.tier_variation[0];
                            int count = tierVar.option_list.Count;
                            for (int i = 0; i < count; i++)
                            {
                                ShopeeGetModelList_Model model = CommonItem.GetModelFromModelListResponse(obj, i);
                                ShopeeGetModelList_TierVariation_Option option = tierVar.option_list[i];
                                // Lấy ảnh đại diện
                                if (option.image != null)
                                {
                                    foreach (var m in item.models)
                                    {
                                        if (m.modelId == model.model_id)
                                        {
                                            m.imageSrc = option.image.image_url;
                                            break;
                                        }
                                    }
                                }
                                // Lấy số lượng có thể bán trên sàn
                                foreach (var m in item.models)
                                {
                                    if (m.modelId == model.model_id)
                                    {
                                        m.quantity_sellable = model.stock_info_v2.seller_stock[0].stock; ;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        item.models[0].imageSrc = item.imageSrc;
                        item.models[0].quantity_sellable = pro.stock_info_v2.seller_stock[0].stock;
                    }
                }
            }
        }

        private List<CommonItem> ShopeeGetListNeedUpdateQuantityAndUpdate(MySqlConnection conn)
        {
            // Danh sách sản phẩm Shopee
            List<CommonItem> listCommonItem = sqler.ShopeeGetListNeedUpdateQuantityConnectOut(conn);
            //ShopeeGetStatusImageSrcQuantitySellable(listCommonItem);
            foreach(var commonItem in listCommonItem)
            {
                ShopeeUpdateQuantityOfOneItem(commonItem, conn);
            }

            return listCommonItem;
        }

        // Lấy trạng thái sản phẩm, image đại diện, số lượng trên sàn Tiki.
        // Cập nhật số lượng với sản phẩm active
        public void TikiGetStatusImageSrcQuantitySellable(List<CommonItem> tikiList)
        {
            foreach (var item in tikiList)
            {
                TikiProduct pro = null;
                pro = GetListProductTiki.GetProductFromOneShop((int)item.itemId);
                if (pro == null)
                    continue;

                item.imageSrc = pro.thumbnail;
                item.models[0].imageSrc = pro.thumbnail;

                if (pro.active == 1)
                    item.bActive = true;
                else
                    item.bActive = false;

                item.models[0].quantity_sellable = TikiUpdateStock.GetQuantityFromTikiProduct(pro);
                item.has_model = false;

                //// Lấy tên của super item nếu có
                //if(item.tikiSuperId != 0)
                //{
                //    pro = GetListProductTiki.GetProductFromOneShop(item.tikiSuperId);
                //    item.tikiSuperName = pro.name;
                //}
            }
        }
        private List<CommonItem>TikiGetListNeedUpdateQuantityAndUpdate(MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> listCommonItem = sqler.TikiGetListNeedUpdateQuantityConnectOut(conn);
            //TikiGetStatusImageSrcQuantitySellable(listCommonItem);
            foreach( var commonItem in listCommonItem)
            {
                 TikiUpdateQuantityOfOneItem(commonItem, conn);
            }
            return listCommonItem;
        }

        /// <summary>
        /// Tìm product id cập nhật số lượng thất bại qua thông tin sàn trả về
        /// </summary>
        /// <param name="listCommonItem"></param>
        /// <param name="listProductIdChanged"></param>
        /// <returns></returns>
        private List<int> GetListProductIdUpdateFail(List<CommonItem> listCommonItem, List<int> listProductIdChanged)
        {
            List<int> listProductIdUpdateFail = new List<int>();
            foreach(var commonItem in listCommonItem)
            {
                if (!commonItem.bActive) // tương ứng với commonItem.result = null
                    continue;

                if(commonItem.eType == Common.eTiki)
                {
                    // Sàn Tiki trả về cập nhật lỗi
                    if(commonItem.result.myJson == null ||
                        ((TikiUpdateQuantityResponse)commonItem.result.myJson).errors != null)
                    {
                        foreach(var mapping in commonItem.models[0].mapping)
                        {
                            if(listProductIdChanged.Contains(mapping.product.id) &&
                                !listProductIdUpdateFail.Contains(mapping.product.id))
                            {
                                listProductIdUpdateFail.Add(mapping.product.id);
                            }
                        }
                    }
                }
                else if(commonItem.eType == Common.eShopee)
                {
                    // Sàn Shoppee trả về cập nhật lỗi
                    if (commonItem.result.myJson == null ||
                        ((ShopeeUpdateStockResponseHTTP)commonItem.result.myJson).response.failure_list != null)
                    {
                        foreach (var fai in ((ShopeeUpdateStockResponseHTTP)commonItem.result.myJson).response.failure_list)
                        {
                            // Méo biết khi item không có model thì modelIdFail = 0 hay -1,...?
                            long modelIdFail = fai.model_id;
                            foreach(var model in commonItem.models)
                            {
                                if(modelIdFail == model.modelId || modelIdFail == 0)
                                {
                                    foreach (var mapping in model.mapping)
                                    {
                                        if (listProductIdChanged.Contains(mapping.product.id) &&
                                            !listProductIdUpdateFail.Contains(mapping.product.id))
                                        {
                                            listProductIdUpdateFail.Add(mapping.product.id);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return listProductIdUpdateFail;
        }

        private void UpdateStatusOfNeedUpdateQuantityConnectOut(
            MySqlConnection conn,
            List<CommonItem> listCommonItem,
            List<int> listProductIdChanged)
        {
            // Danh sách sản phẩm chưa cập nhật số lượng thành công
            List<int> listProductIdUpdateFail = GetListProductIdUpdateFail(listCommonItem, listProductIdChanged);

            // Danh sách sản phẩm cập nhật thành công
            List<int> listProductIdUpdateSuccess = new List<int>();
            foreach (var id in listProductIdChanged)
            {
                if (!listProductIdUpdateFail.Contains(id))
                {
                    listProductIdUpdateSuccess.Add(id);
                }
            }

            sqler.UpdateStatusOfNeedUpdateQuantityConnectOut(listProductIdUpdateSuccess, conn);
        }
        public List<CommonItem> GetListNeedUpdateQuantityAndUpdate_Core()
        {
            // CommonItem chưa có ảnh đại diện cho item, model ta lấy và lưu vào db
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<CommonItem> ls = new List<CommonItem>();
            try
            {
                conn.Open();
                // Danh sách sản phẩm trong kho có thay đổi số lượng cần cập nhật
                List<int> listProductId = sqler.GetListProductOfNeedUpdateQuantityConnectOut(conn);

                List<CommonItem> shopeeList = ShopeeGetListNeedUpdateQuantityAndUpdate(conn);
                List<CommonItem> tikiList = TikiGetListNeedUpdateQuantityAndUpdate(conn);

                ls.AddRange(tikiList);
                ls.AddRange(shopeeList);

                UpdateStatusOfNeedUpdateQuantityConnectOut(conn, ls, listProductId);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            // Lấy danh sách sản phẩm
            return ls;
        }

        [HttpPost]
        public string GetListNeedUpdateQuantityAndUpdate()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(GetListNeedUpdateQuantityAndUpdate_Core());
        }

        [HttpPost]
        public string GetListProductInWarehoueChangedQuantity()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }
            List<Product> ls = null;
            ls = sqler.GetListProductInWarehoueChangedQuantity();
            return JsonConvert.SerializeObject(ls);
        }

        [HttpGet]
        public ActionResult MappingOfProduct(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        private List<CommonItem> ShopeeGetListMappingOfProduct(int id, MySqlConnection conn)
        {
            // Danh sách sản phẩm Shopee
            List<CommonItem> shopeeList = sqler.ShopeeGetListMappingOfProduct(id, conn);
            //ShopeeGetStatusImageSrcQuantitySellable(shopeeList);
            return shopeeList;
        }

        private List<CommonItem> TikiGetListMappingOfProduct(int id, MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> tikiList = sqler.TikiGetListMappingOfProduct(id, conn);
            //TikiGetStatusImageSrcQuantitySellable(tikiList);
            return tikiList;
        }

        [HttpPost]
        public string GetListMappingOfProduct(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<CommonItem> ls = new List<CommonItem>();
            try
            {
                conn.Open();

                List<CommonItem> shopeeList = ShopeeGetListMappingOfProduct(id, conn);
                List<CommonItem> tikiList = TikiGetListMappingOfProduct(id, conn);

                ls.AddRange(tikiList);
                ls.AddRange(shopeeList);

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(ls);
        }

        // Cập nhật quantity của model trên sàn Shopee
        // Nếu lỗi này xuất hiện:
        //"failure_list": [{
        //        "failed_reason": "model ID not exist in sku",
        //        "model_id": 232454047582
        //    }
        //]
        // Ta xóa bỏ mọi thứ liên quan đến model_id vĩnh viễn khỏi cơ sở dữ liệu
        private void ShopeeProductUpdateStockAndImpactDb(
            MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st,
            MySqlResultState result)
        {
            try
            {
                ShopeeUpdateStockResponseHTTP rs =
                    MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct.ShopeeUpdateStock.ShopeeProductUpdateStock(st);
                result.myJson = rs;
                if (rs == null)
                {
                    return;
                }

                foreach (var f in rs.response.failure_list)
                {
                    if (f.failed_reason == "model ID not exist in sku")
                    {
                        // Xóa bỏ mọi thứ liên quan đến model_id vĩnh viễn khỏi cơ sở dữ liệu
                        ShopeeMySql sqlShopee = new ShopeeMySql();
                        sqlShopee.ShopeeDeleteModelOnDB(f.model_id);
                    }
                    else if (f.failed_reason.Contains(st.item_id.ToString() + " status is abnormal"))
                    {
                        //"response": {
                        //"failure_list": [{
                        //"failed_reason": "All the fields cannot be updated because the product  22046434841 status is abnormal"
                        //}
                        //],
                        //"success_list": []
                        //},

                        // Item trạng thái abnormal trên shopee. Xóa bỏ mọi thứ liên quan đến item vĩnh viễn khỏi csdl
                        ShopeeMySql sqlShopee = new ShopeeMySql();
                        sqlShopee.ShopeeDeleteItemOnDB(st.item_id);
                    }
                    else
                    {
                        result.State = EMySqlResultState.ERROR;
                    }
                }
            }
            catch(Exception ex)
            {
                Common.SetResultException(ex, result);
            }

        }

        // Cập nhật số lượng sản phẩm của 1 model từ itemId, modelId
        private MySqlResultState ShopeeUpdateQuantityOfOneItemModel(long itemId,
            long modelId,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            int quantity = sqler.ShopeeGetQuantityOfOneItemModelConnectOut(itemId, modelId, conn);

            MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st =
                    new MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();
            st.item_id = itemId;

            if (modelId == -1)
            {
                st.stock_list.Add(new ShopeeUpdateStockStock(0, quantity));
            }
            else
            {
                st.stock_list.Add(new ShopeeUpdateStockStock(modelId, quantity));
            }

            ShopeeProductUpdateStockAndImpactDb(st, result);
            return result;
        }


        // Cập nhật số lượng sản phẩm của tất cả model trong commonItem một lần
        private void ShopeeUpdateQuantityOfOneItem(CommonItem commonItem, MySqlConnection conn)
        {
            if (!commonItem.bActive)
            {
                commonItem.result = null;
                return;
            }

            MySqlResultState result = new MySqlResultState();

            long itemId = commonItem.itemId;
            long modelId = 0;
            int quantity = 0;
            MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st =
                    new MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();
            st.item_id = itemId;
            foreach (var model in commonItem.models)
            {
                modelId = model.modelId;
                //quantity = sqler.ShopeeGetQuantityOfOneItemModelConnectOut(itemId, modelId, conn);
                quantity = model.GetQuatityFromListMapping();

                if(modelId == -1)
                {
                    st.stock_list.Add(new ShopeeUpdateStockStock(0, quantity));
                }
                else
                {
                    st.stock_list.Add(new ShopeeUpdateStockStock(modelId, quantity));
                }
            }

            ShopeeProductUpdateStockAndImpactDb(st, result);
            commonItem.result = result;
        }

        private MySqlResultState TikiUpdateQuantityOfOneItemModel(int itemId, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            int quantity = sqler.TikiGetQuantityOfOneItemModelConnectOut(itemId, conn);

            TikiUpdateStock.TikiProductUpdateQuantity(itemId, quantity, result);

            return result;
        }

        // Với Tiki TikiUpdateQuantityOfOneItemModel, và TikiUpdateQuantityOfOneItem giống nhau, khác tham số truyền vào
        // TikiUpdateQuantityOfOneItem có thể check item active
        private void TikiUpdateQuantityOfOneItem(CommonItem commonItem, MySqlConnection conn)
        {
            if (!commonItem.bActive)
            {
                commonItem.result = null;
                return;
            }

            MySqlResultState result = new MySqlResultState();
            //int quantity = sqler.TikiGetQuantityOfOneItemModelConnectOut((int)commonItem.itemId, conn);

            int quantity = commonItem.models[0].GetQuatityFromListMapping();

            TikiUpdateStock.TikiProductUpdateQuantity((int)commonItem.itemId, quantity, result);

            commonItem.result = result;
        }

        // Update số lượng của 1 model lên sàn
        [HttpPost]
        public string UpdateQuantityOfOneItemModel(string eType, long itemId, long modelId)
        {
            MySqlResultState result = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();

                    if (AuthentAdministratorConnectOut(conn) == null)
                    {
                        return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
                    }

                    if (eType == Common.eTiki)
                    {
                        result = TikiUpdateQuantityOfOneItemModel((int)itemId, conn);
                    }
                    else if (eType == Common.eShopee)
                    {
                        result = ShopeeUpdateQuantityOfOneItemModel(itemId, modelId, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.ToString());
            }

            return JsonConvert.SerializeObject(result);
        }

        private void UpdateQuantityToTMDTFromListCommonItemConnectOut(List<CommonItem> listCommonItem,
            MySqlConnection conn
            )
        {
            foreach (CommonItem commonItem in listCommonItem)
            {
                if (commonItem.eType == Common.eTiki)
                {
                    TikiUpdateQuantityOfOneItem(commonItem, conn);
                }
                else if (commonItem.eType == Common.eShopee)
                {
                    ShopeeUpdateQuantityOfOneItem(commonItem, conn);
                }
            }
        }

        // Cập nhật số lượng lên sàn và cập nhật trạng thái sản phẩm ở tbNeedUpdateQuantity
        private void UpdateQuantityToTMDT_DbFromListCommonItem(
            List<CommonItem> listCommonItem,
            List<int> listProductId)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
                conn.Open();
                UpdateQuantityToTMDTFromListCommonItemConnectOut(listCommonItem, conn);
                // Cập nhật lên sàn ok, ta cập nhật trạng thái status = 0 ở tbNeedUpdateQuantity
                UpdateStatusOfNeedUpdateQuantityConnectOut(conn, listCommonItem, listProductId);
                conn.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Update số lượng của 1 list common item tương ứng với 1 sản phẩm trong kho
        [HttpPost]
        public string UpdateQuantityToTMDTFromListCommonItem(int productId, string listCommonItem)
        {
            if (AuthentAdministrator() == null)
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
            if (ls != null)
            {
                List<int> listProductId = new List<int>();
                listProductId.Add(productId);
                UpdateQuantityToTMDT_DbFromListCommonItem(ls, listProductId);
            }

            return JsonConvert.SerializeObject(ls);
        }
    }
}