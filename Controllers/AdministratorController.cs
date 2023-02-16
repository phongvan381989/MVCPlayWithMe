using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class AdministratorController : Controller
    {
        private Administrator AuthentAdministrator()
        {
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

            /// Check cookie đã được lưu trong db
            return Cookie.GetAdministratorFromCookieUId(cookieResult);
        }

        /// <summary>
        /// Nơi đến khi xác thực thất bại hoặc logout
        /// </summary>
        /// <returns></returns>
        private ActionResult AuthenticationFail()
        {
            return View("~/Views/Administrator/Login.cshtml");
        }

        // GET: Administrator
        public ActionResult Index()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

             return View();
        }

        public ActionResult New()
        {
            if (AuthentAdministrator() == null)
                return AuthenticationFail();
            return View();
        }

        [HttpPost]
        public string New_AddAdministrator(string userName, int userNameType, string passWord, int privilege)
        {
            MySqlResultState result = null;
            if (AuthentAdministrator() == null)
            {
                result = new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage);
            }
            result = MyMySql.AddNewAdministrator(userName, userNameType, passWord, privilege);
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Login()
        {
            if (AuthentAdministrator() == null)
                return View();

            return View("~/Views/Administrator/Index.cshtml");
        }

        [HttpPost]
        public string Logout()
        {
            CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

            //if(string.IsNullOrEmpty(cookieResult.uId))
                //return "{\"state\": 4}";

            MyMySql.AdministratorLogout(cookieResult.uId);
            Cookie.RecreateCookie(HttpContext);
            return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.LogoutMessage));
        }

        [HttpPost]
        public string Login_Login(string userName, string passWord)
        {
            MySqlResultState result = MyMySql.LoginAdministrator(userName, passWord);

            // Set cookie cho tài khoản quản trị
            if (result.State == EMySqlResultState.OK)
            {
                CookieResultState cookieResult = Cookie.SetAndGetUserIdCookie(HttpContext);

                // Lấy thông tin adminstrator
                Administrator administrator = MyMySql.GetAdministratorFromUserName(userName);

                // Lưu cookie vào bảng tbcookie_administrator
                MySqlResultState resultInsert = MyMySql.AddNewCookieAdministrator(cookieResult.uId, administrator.id);
                if(resultInsert.State != EMySqlResultState.OK)
                {
                    MyLogger.GetInstance().Warn(resultInsert.Message);
                    result = resultInsert;
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public ActionResult CreateProduct()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            List<Publisher> ls = MyMySql.GetListPublisher();
            ViewData["lsPublisher"] = ls;
            return View();
        }

        public string AddNewPro(string barcode,
                string productName,
                string comboName,
                int bookCoverPrice,
                string author,
                string translator,
                int publisherId,
                string publishingCompany,
                string publishingTime,
                int productLong,
                int productWide,
                int productHigh,
                int productWeight,
                string positionInWarehouse,
                string detail,
                string category,
                int productIdForUpdate //productIdForUpdate = -1: tạo mới sản phẩm, ngược lại là update sản phẩm
                )
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }
            MySqlResultState result = MyMySql.AddNewPro( barcode,
                 productName,
                 comboName,
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
                 detail,
                 category,
                 productIdForUpdate);


            // Lấy id của sản phẩm vừa thêm mới thành công
            if (productIdForUpdate == -1)
                result.myAnything = MyMySql.GetProductIdFromName(productName);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string UploadImage(object obj)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }
            var length = Request.ContentLength;
            var bytes = new byte[length];
            Request.InputStream.Read(bytes, 0, length);


            var imgFileName = Request.Headers["imgFileName"];
            var productId = Request.Headers["productId"];

            // Tên ảnh lưu có định dạng: id_name VD: 12_1.jpg, 12_2.jpg
            var saveToFileLoc = string.Format("{0}\\{1}",
                                           Server.MapPath("/Media"),
                                           productId + "_" + imgFileName);

            // save the file.
            var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
            fileStream.Write(bytes, 0, length);
            fileStream.Close();

            return JsonConvert.SerializeObject(new MySqlResultState());
        }

        /// <summary>
        /// Xóa ảnh, video cũ
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public string DeleteOldImage(string productId)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }
            string[] dirs = Directory.GetFiles(Server.MapPath("/Media"), productId + "_*");
            foreach (string dir in dirs)
            {
                System.IO.File.Delete(dir);
            }
            return JsonConvert.SerializeObject(new MySqlResultState());
        }

        /// <summary>
        /// Cập nhật thông tin chung của các sản phẩm thuộc cùng 1 combo VD: tên nhà phát hành, nhà xuất bản, kích thước,..
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateCommonInfoWithComboName()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            List<Publisher> ls = MyMySql.GetListPublisher();
            ViewData["lsPublisher"] = ls;

            List<string> listComboName = MyMySql.GetListComboName();
            ViewData["lsComboName"] = listComboName;

            List<string> listCategory = MyMySql.GetListCategory();
            ViewData["lsCategory"] = listCategory;

            List<string> listAuthor = MyMySql.GetListAuthor();
            ViewData["lsAuthor"] = listAuthor;

            List<string> listTranslator = MyMySql.GetListTranslator();
            ViewData["lsTranslator"] = listTranslator;

            List<string> listPublishingCompany = MyMySql.GetListPublishingCompany();
            ViewData["lsPublishingCompany"] = listPublishingCompany;

            return View();
        }

        /// <summary>
        /// Update thông tin lên db
        /// </summary>
        /// <param name="comboName"></param>
        /// <param name="bookCoverPrice"></param>
        /// <param name="author"></param>
        /// <param name="translator"></param>
        /// <param name="publisherId"></param>
        /// <param name="publishingCompany"></param>
        /// <param name="publishingTime"></param>
        /// <param name="productLong"></param>
        /// <param name="productWide"></param>
        /// <param name="productHigh"></param>
        /// <param name="productWeight"></param>
        /// <param name="positionInWarehouse"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public string UpdateCommonInfoWithComboName_Update(
                        string comboName,
                        int bookCoverPrice,
                        string author,
                        string translator,
                        int publisherId,
                        string publishingCompany,
                        string publishingTime,
                        int productLong,
                        int productWide,
                        int productHigh,
                        int productWeight,
                        string positionInWarehouse,
                        string category)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }


            MySqlResultState result = MyMySql.UpdateCommonInfoWithComboNae(
                 comboName,
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
                         category);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Từ tên combo lấy được thông tin chung đã có trên db
        /// Thông tin chung là thông tin của sản phẩm đầu tiên thuộc combo
        /// </summary>
        /// <param name="comboName"></param>
        /// <returns></returns>
        public string UpdateCommonInfoWithComboName_Get(
                        string comboName)
        {
            return MyMySql.GetJsonCommonInfoFromComboName(comboName);
        }

        public ActionResult UpdateProduct()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            List<string> lsBarcode = MyMySql.GetListBarcode();
            ViewData["lsBarcode"] = lsBarcode;

            List<string> lsProductName = MyMySql.GetListProductName();
            ViewData["lsProductName"] = lsProductName;

            List<Publisher> ls = MyMySql.GetListPublisher();
            ViewData["lsPublisher"] = ls;

            List<string> listComboName = MyMySql.GetListComboName();
            ViewData["lsComboName"] = listComboName;

            List<string> listCategory = MyMySql.GetListCategory();
            ViewData["lsCategory"] = listCategory;

            List<string> listAuthor = MyMySql.GetListAuthor();
            ViewData["lsAuthor"] = listAuthor;

            List<string> listTranslator = MyMySql.GetListTranslator();
            ViewData["lsTranslator"] = listTranslator;

            List<string> listPublishingCompany = MyMySql.GetListPublishingCompany();
            ViewData["lsPublishingCompany"] = listPublishingCompany;

            return View();
        }

        public ActionResult CreatePublisher()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            return View();
        }


        public string AddNewPublisher(string publisherName, string detail)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = MyMySql.AddNewPublisher( publisherName,  detail);
            return JsonConvert.SerializeObject(result);
        }

        public string LoadPublisher()
        {
            StringBuilder sb = new StringBuilder();
            List<Publisher> ls = MyMySql.GetListPublisher();
            if (ls != null && ls.Count() > 0)
            {
                sb.Append(@"<tr>
                            <th> Tên Nhà Phát Hành </th>
                            <th> Thông Tin </th>
                          </tr>");
                foreach (var pub in ls)
                {
                    sb.Append(@"<tr>");
                    sb.Append("<td>" + pub.publisherName + @"</td >");
                    sb.Append("<td>" + pub.detail + @"</td >");
                    sb.Append(@"</ tr>");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///Trả lại thông tin product json để hiển thị khi thay đổi barcode
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public string UpdateProduct_Get_ChangeBarcode(string barcode)
        {
            Product product = MyMySql.GetProductFromBarcode(barcode);
            if(product == null)
                return string.Empty;
            product.SetSrcImageVideo();
            return JsonConvert.SerializeObject(product);
        }

        public string UpdateProduct_Get_ChangeproductName(string productName)
        {
            Product product = MyMySql.GetProductFromProductName(productName);
            if (product == null)
                return string.Empty;
            product.SetSrcImageVideo();
            return JsonConvert.SerializeObject(product); ;
        }

        public string UpdateProduct_UpdateBarcode(int id, string newBarcode)
        {
            MySqlResultState result = MyMySql.UpdateProductBarcode(id, newBarcode);
            return JsonConvert.SerializeObject(result);
        }

        public string UpdateProduct_AddMoreProductBarcode(int id, string newBarcode)
        {
            MySqlResultState result = MyMySql.AddMoreProductBarcode(id, newBarcode);
            return JsonConvert.SerializeObject(result);
        }

        public string UpdateProduct_UpdateProductName(int id, string newProductName)
        {
            MySqlResultState result = MyMySql.UpdateProductName(id, newProductName);
            return JsonConvert.SerializeObject(result);
        }
    }
}