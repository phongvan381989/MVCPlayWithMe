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
            ViewDataGetListCombo();
            ViewDataGetListCategory();
            ViewDataGetListAuthor();
            ViewDataGetListTranslator();
            ViewDataGetListPublisher();
            ViewDataGetListPublishingCompany();
            ViewDataGetListProductName();

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

            GetViewDataForInput();

            return View();
        }

        public ActionResult UpdateDelete()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            GetViewDataForInput();

            return View();
        }

        public ActionResult UpdateWithCombo()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            GetViewDataForInput();

            return View();
        }

        public ActionResult Import()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            ViewDataGetListProductName();

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
            // Lấy id của sản phẩm vừa thêm mới thành công
            result.myAnything = sqler.GetProductIdFromName(name);
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
            // xóa file của sản phẩm
            DeleteOldFile(id);
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

        ///// <summary>
        /////Trả lại thông tin product json để hiển thị khi thay đổi barcode
        ///// </summary>
        ///// <param name="barcode"></param>
        ///// <returns></returns>
        //public string UpdateProduct_Get_ChangeBarcode(string barcode)
        //{
        //    Product product = sqler.GetProductFromBarcode(barcode);
        //    if (product == null)
        //        return string.Empty;
        //    product.SetSrcImageVideo();
        //    return JsonConvert.SerializeObject(product);
        //}

        //public string UpdateProduct_Get_ChangeproductName(string productName)
        //{
        //    Product product = sqler.GetProductFromProductName(productName);
        //    if (product == null)
        //        return string.Empty;
        //    product.SetSrcImageVideo();
        //    return JsonConvert.SerializeObject(product);
        //}

        //public string UpdateProduct_UpdateBarcode(int id, string newBarcode)
        //{
        //    MySqlResultState result = sqler.UpdateProductBarcode(id, newBarcode);
        //    return JsonConvert.SerializeObject(result);
        //}

        //public string UpdateProduct_AddMoreProductBarcode(int id, string newBarcode)
        //{
        //    MySqlResultState result = sqler.AddMoreProductBarcode(id, newBarcode);
        //    return JsonConvert.SerializeObject(result);
        //}

        //public string UpdateProduct_UpdateProductName(int id, string newProductName)
        //{
        //    MySqlResultState result = sqler.UpdateProductName(id, newProductName);
        //    return JsonConvert.SerializeObject(result);
        //}

        ///// <summary>
        ///// Cập nhật thông tin chung của các sản phẩm thuộc cùng 1 combo VD: tên nhà phát hành, nhà xuất bản, kích thước,..
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult UpdateCommonInfoWithComboName()
        //{
        //    if (AuthentAdministrator() == null)
        //    {
        //        return AuthenticationFail();
        //    }

        //    ViewDataGetListPublisher();

        //    List<string> listComboName = sqler.GetListComboName();
        //    ViewData["lsComboName"] = listComboName;

        //    List<string> listPublishingCompany = sqler.GetListPublishingCompany();
        //    ViewData["lsPublishingCompany"] = listPublishingCompany;

        //    return View();
        //}

        /// <summary>
        /// Xóa ảnh, video cũ
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public string DeleteOldFile(int productId)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            string path = Common.GetProductMediaFolderPath(productId.ToString());
            if (path != null)
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                //foreach (FileInfo file in di.GetFiles())
                //{
                //    file.Delete();
                //}
                di.Delete(true);
            }

            return JsonConvert.SerializeObject(new MySqlResultState());
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
            string path = Common.GetProductMediaFolderPath(id.ToString());
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
            string path = Common.GetProductMediaFolderPath(productId);
            if (path == null)
            {
                path = Common.CreateProductMediaFolderPath(productId);
            }

            return UploadImageVideo(path);
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
            //List<string> lsImage = Common.GetImageSrc(id.ToString());
            //List<string> lsVideo = Common.GetVideoSrc(id.ToString());
            //List<List<string>> ls = new List<List<string>>();
            //ls.Add(lsImage);
            //ls.Add(lsVideo);
            //pro.anything = JsonConvert.SerializeObject(ls);
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
            if (string.IsNullOrEmpty(fromDate))
                fromDate = "2018-08-05";
            if (string.IsNullOrEmpty(toDate))
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
    }
}