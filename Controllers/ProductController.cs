using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
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
        public string GetItemObjectFromId(int id)
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

        public ActionResult UpdateWithCombo()
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

        private void AddUpdateParasCommon(
            ref int comboId, string comboName,
            ref int categoryId, string categoryName,
            ref int publisherId, string publisherName,
            ref int parentId, string parentName
            )
        {
            if (string.IsNullOrWhiteSpace(comboName))
                comboId = -1;
            else
                comboId = comboSqler.GetComboIdFromName(comboName);

            if (string.IsNullOrWhiteSpace(categoryName))
                categoryId = -1;
            else
                categoryId = categorySqler.GetCategoryIdFromName(categoryName);

            if (string.IsNullOrWhiteSpace(publisherName))
                publisherId = -1;
            else
                publisherId = publisherSqler.GetPublisherIdFromName(publisherName);

            if (string.IsNullOrWhiteSpace(parentName))
                parentId = -1;
            else
                parentId = sqler.GetProductIdFromName(parentName);
        }

        public string AddNewPro(
                string code,
                string barcode,
                string name,
                //int comboId,
                string comboName, // Nếu comboId = -1, ta thêm mới comboName
                //int categoryId,
                string categoryName, // Nếu categoryId = -1, ta thêm mới categoryName
                int bookCoverPrice,
                string author,
                string translator,
                //int publisherId,
                string publisherName, // Nếu publisherId = -1, ta thêm mới publisherName
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
                string parentName, // Nếu không có sản phẩm cha, parentId = -1
                int republish,// Nếu không xác định, parentId = -1
                string detail,
                int status
                )
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            int comboId = 0, categoryId = 0, publisherId = 0, parentId = 0;
            AddUpdateParasCommon(ref comboId, comboName, ref categoryId, categoryName,
                ref publisherId, publisherName, ref parentId, parentName);

            Product pro = new Product(-1,
                 code,
                 barcode,
                 name,
                 comboId,
                 categoryId,
                 bookCoverPrice,
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
                 status
                 );

             MySqlResultState result = sqler.AddNewPro(pro);
            return JsonConvert.SerializeObject(result);
        }

        public string UpdateProduct(
                string code,
                string barcode,
                string name,
                //int comboId,
                string comboName, // Nếu comboId = -1, ta thêm mới comboName
                                  //int categoryId,
                string categoryName, // Nếu categoryId = -1, ta thêm mới categoryName
                int bookCoverPrice,
                string author,
                string translator,
                //int publisherId,
                string publisherName, // Nếu publisherId = -1, ta thêm mới publisherName
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
                string parentName, // Nếu không có sản phẩm cha, parentId = -1
                int republish,// Nếu không xác định, parentId = -1
                string detail,
                int status
                )
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            int comboId = 0, categoryId = 0, publisherId = 0, parentId = 0;
            AddUpdateParasCommon(ref comboId, comboName, ref categoryId, categoryName,
                ref publisherId, publisherName, ref parentId, parentName);

            Product pro = new Product(
                -1,
                code,
                barcode,
                 name,
                 comboId,
                 categoryId,
                 bookCoverPrice,
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
                 status
                 );

            MySqlResultState result = sqler.UpdateProduct(pro);

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Xóa sản phẩm
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
            Common.DeleteMediaFolder(path);

            return JsonConvert.SerializeObject(result);
        }

        public string UpdateCommonInfoWithCombo(
                //int comboId,
                string comboName, // Nếu comboId = -1, ta thêm mới comboName
                                  //int categoryId,
                string categoryName, // Nếu categoryId = -1, ta thêm mới categoryName
                int bookCoverPrice,
                string author,
                string translator,
                //int publisherId,
                string publisherName, // Nếu publisherId = -1, ta thêm mới publisherName
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
                int republish,   // Nếu không xác định, parentId = -1
                int status
            )
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            int comboId = 0, categoryId = 0, publisherId = 0, parentId = 0;
            AddUpdateParasCommon(ref comboId, comboName, ref categoryId, categoryName,
                ref publisherId, publisherName, ref parentId, string.Empty);
            ProductCommonInfoWithCombo pro = new ProductCommonInfoWithCombo(
                 comboId,
                 categoryId,
                 bookCoverPrice,
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
                 status
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

        /// <summary>
        /// Lấy thông tin sản phẩm từ id sản phẩm
        /// </summary>
        /// <param name="id">id sản phẩm</param>
        /// <returns></returns>
        [HttpPost]
        public string GetProduct(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            Product pro = sqler.GetProductFromId(id);
            return JsonConvert.SerializeObject(pro);
        }

        /// <summary>
        /// Lấy thông tin chung của combo từ sản phẩm đầu tiên thuộc combo
        /// </summary>
        /// <param name="id">id combo</param>
        /// <returns></returns>
        [HttpPost]
        public string GetProductCommonInfoWithComboFromFirst(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            Product pro = sqler.GetProductFromFirstComboId(id);

            return JsonConvert.SerializeObject(pro);
        }

        [HttpPost]
        public string GetProductIdCodeBarcodeNameBooCoverkPrice(string publisher)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<Product>());
            }

            List<Product> ls = sqler.GetProductIdCodeBarcodeNameBookCoverPrice(publisher);
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

        /// <summary>
        /// Tìm kiếm sản phẩm trong kho
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public string SearchProductCount(string publisher,
            string codeOrBarcode, string name, string combo)
        {
            if (AuthentAdministrator() == null)
            {
                return "0";
            }

            // Đếm số sản phẩm trong kết quả tìm kiếm
            int count = 0;
            ProductSearchParameter searchParameter = new ProductSearchParameter();
            searchParameter.publisher = publisher;
            searchParameter.codeOrBarcode = codeOrBarcode;
            searchParameter.name = name;
            searchParameter.combo = combo;
            count = sqler.SearchProductCount(searchParameter);
            return count.ToString();
        }


        /// <summary>
        /// Tìm kiếm sản phẩm trong kho không phân trang
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public string SearchProduct(string publisher,
            string codeOrBarcode, string name, string combo, int status)
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
            searchParameter.status = status;
            List<Product> lsSearchResult;
            lsSearchResult = sqler.SearchProduct(searchParameter);

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

        [HttpGet]
        public string UpdateISBN(int id, string isbn)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            return JsonConvert.SerializeObject(sqler.UpdateISBN(id, isbn));
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
        private void ShopeeGetStatusImageSrcQuantitySellable(List<CommonItem> shopeeList)
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

        // Cập nhật số lượng lên sàn Shopee
        private void ShopeeUpdateQuatity(List<CommonItem> shopeeList)
        {
            int qty = 0;
            foreach (var item in shopeeList)
            {
                if (!item.bActive)
                    continue;

                foreach (var model in item.models)
                {
                    qty = model.GetQuatityFromListMapping();
                    ShopeeItemId shopeeitemId = new ShopeeItemId(item.itemId,
                        model.modelId, qty);
                    Boolean isOk = MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct.ShopeeUpdateStock.ShopeeProductUpdateStock(shopeeitemId);
                    if (!isOk)
                    {
                        model.whyUpdateFail = Common.CommonErrorMessage;
                    }
                }
            }
        }

        private List<CommonItem> ShopeeGetListNeedUpdateQuantityAndUpdate(MySqlConnection conn)
        {
            // Danh sách sản phẩm Shopee
            List<CommonItem> shopeeList = sqler.ShopeeGetListNeedUpdateQuantity(conn);
            ShopeeGetStatusImageSrcQuantitySellable(shopeeList);
            ShopeeUpdateQuatity(shopeeList);

            return shopeeList;
        }

        // Lấy trạng thái sản phẩm, image đại diện, số lượng trên sàn Tiki.
        // Cập nhật số lượng với sản phẩm active
        private void TikiGetStatusImageSrcQuantitySellable(List<CommonItem> tikiList)
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

        // Cập nhật số lượng lên sàn Tiki
        private void TikiUpdateQuatity(List<CommonItem> tikiList)
        {
            int qty = 0;
            foreach (var item in tikiList)
            {
                if (!item.bActive)
                    continue;

                TikiUpdateQuantity st = new TikiUpdateQuantity((int)item.itemId, TikiConstValues.intIdKho28Ngo3TTDL);
                // Cập nhật tồn kho cho tham số
                qty = item.models[0].GetQuatityFromListMapping();

                st.UpdateQuantity(qty);
                Boolean isOk = TikiUpdateStock.TikiProductUpdateQuantity(st);
                if (!isOk)
                {
                    // Tiếp tục cập nhật số lượng sp khác, lưu lý do cập nhật lỗi
                    item.models[0].whyUpdateFail = Common.CommonErrorMessage;
                }
            }
        }

        private List<CommonItem>TikiGetListNeedUpdateQuantityAndUpdate(MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> tikiList = sqler.TikiGetListNeedUpdateQuantity(conn);
            TikiGetStatusImageSrcQuantitySellable(tikiList);
            TikiUpdateQuatity(tikiList);
            return tikiList;
        }

        [HttpPost]
        public string GetListNeedUpdateQuantityAndUpdate()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            // CommonItem chưa có ảnh đại diện cho item, model ta lấy và lưu vào db
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<CommonItem> ls = new List<CommonItem>();
            try
            {
                conn.Open();
                List<CommonItem> shopeeList = ShopeeGetListNeedUpdateQuantityAndUpdate(conn);
                List<CommonItem> tikiList = TikiGetListNeedUpdateQuantityAndUpdate(conn);

                ls.AddRange(tikiList);
                ls.AddRange(shopeeList);
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(ls);
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
            ShopeeGetStatusImageSrcQuantitySellable(shopeeList);
            return shopeeList;
        }

        private List<CommonItem> TikiGetListMappingOfProduct(int id, MySqlConnection conn)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> tikiList = sqler.TikiGetListMappingOfProduct(id, conn);
            TikiGetStatusImageSrcQuantitySellable(tikiList);
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

    }
}