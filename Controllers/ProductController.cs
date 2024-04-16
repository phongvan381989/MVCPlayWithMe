using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
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
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
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
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
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
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
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
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
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
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            var productId = Request.Headers["productId"];
            string path = Common.GetAbsoluteProductMediaFolderPath(productId);
            if (path == null)
            {
                path = Common.CreateAbsoluteProductMediaFolderPath(productId);
            }

            return SaveImageVideo(path);
        }

        /// <summary>
        /// Lấy thông tin sản phẩm từ id sản phẩm
        /// </summary>
        /// <param name="id">id sản phẩm</param>
        /// <returns></returns>
        [HttpPost]
        public string GetProduct(int id)
        {
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
            Product pro = sqler.GetProductFromFirstComboId(id);

            return JsonConvert.SerializeObject(pro);
        }

        [HttpPost]
        public string GetProductIdCodeBarcodeNameBooCoverkPrice()
        {
            List<ProductIdCodeBarcodeNameBookCoverPrice> ls = sqler.GetProductIdCodeBarcodeNameBookCoverPrice();
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public string AddImport(string listObject)
        {
            string strResult = string.Empty;
            List<Import> ls = null;
            try
            {
                ls = JsonConvert.DeserializeObject<List<Import>>(listObject);
                if(ls == null || ls.Count == 0)
                {
                    MySqlResultState rss = new MySqlResultState();
                    rss.State = EMySqlResultState.ERROR;
                    rss.Message = "Danh sách cần nhập không đúng.";
                    strResult = JsonConvert.SerializeObject(rss);
                }
                strResult = JsonConvert.SerializeObject(sqler.AddListImport(ls));
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.ToString());
                MySqlResultState rss = new MySqlResultState();
                rss.State = EMySqlResultState.ERROR;
                rss.Message = ex.ToString();
                strResult = JsonConvert.SerializeObject(rss);
            }

            return strResult;
        }

        [HttpPost]
        public string GetListImport(string fromDate, string toDate)
        {
            if (Common.ParameterOfURLQueryIsNullOrEmpty(fromDate))
                fromDate = "2018-08-05";
            if (Common.ParameterOfURLQueryIsNullOrEmpty(toDate))
                toDate = DateTime.Now.ToString(Common.dateFormat);
            List<Import> ls = sqler.GetImportList(fromDate, toDate);
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public string UpdateImport(string listObject)
        {
            string strResult = string.Empty;
            List<Import> ls = null;
            try
            {
                ls = JsonConvert.DeserializeObject<List<Import>>(listObject);
                if (ls == null || ls.Count == 0)
                {
                    MySqlResultState rss = new MySqlResultState();
                    rss.State = EMySqlResultState.ERROR;
                    rss.Message = "Danh sách cần cập nhât không đúng.";
                    strResult = JsonConvert.SerializeObject(rss);
                }
                strResult = JsonConvert.SerializeObject(sqler.UpdateListImport(ls));
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.ToString());
                MySqlResultState rss = new MySqlResultState();
                rss.State = EMySqlResultState.ERROR;
                rss.Message = ex.ToString();
                strResult = JsonConvert.SerializeObject(rss);
            }

            return strResult;
        }

        /// <summary>
        /// Tìm kiếm sản phẩm trong kho
        /// </summary>
        /// <param name="namePara"></param>
        /// <returns></returns>
        [HttpGet]
        public string SearchProductCount(string codeOrBarcode, string name, string combo)
        {
            // Đếm số sản phẩm trong kết quả tìm kiếm
            int count = 0;
            ProductSearchParameter searchParameter = new ProductSearchParameter();
            searchParameter.codeOrBarcode = codeOrBarcode;
            searchParameter.name = name;
            searchParameter.combo = combo;
            count = sqler.SearchProductCount(searchParameter);
            return count.ToString();
        }

        [HttpGet]
        public string ChangePage(string codeOrBarcode, string name, string combo, int start, int offset)
        {
            ProductSearchParameter searchParameter = new ProductSearchParameter();
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
            return JsonConvert.SerializeObject(sqler.UpdateName(id, name));
        }

        [HttpGet]
        public string UpdateCode(int id, string code)
        {
            return JsonConvert.SerializeObject(sqler.UpdateCode(id, code));
        }

        [HttpGet]
        public string UpdateISBN(int id, string isbn)
        {
            return JsonConvert.SerializeObject(sqler.UpdateISBN(id, isbn));
        }

        [HttpPost]
        public string GetListAuthor()
        {
            return JsonConvert.SerializeObject(sqler.GetListAuthor());
        }

        
        [HttpPost]
        public string GetListTranslator()
        {
            return JsonConvert.SerializeObject(sqler.GetListTranslator());
        }

        [HttpPost]
        public string GetListPublishingCompany()
        {
            return JsonConvert.SerializeObject(sqler.GetListPublishingCompany());
        }

        [HttpPost]
        public string GetListProductName()
        {
            return JsonConvert.SerializeObject(sqler.GetListProductName());
        }
    }
}