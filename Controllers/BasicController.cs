using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.ItemModel;

namespace MVCPlayWithMe.Controllers
{
    public class BasicController : Controller
    {
        public Administrator AuthentAdministrator()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);

            /// Check cookie đã được lưu trong db
            AdministratorMySql sqler = new AdministratorMySql();
            Administrator administrator = sqler.GetAdministratorFromCookie(cookieResult.cookieValue);
            //if(administrator == null)
            //{
            //    Cookie.DeleteUserIdCookie(HttpContext);
            //}
            return administrator;

        }

        public Customer AuthentCustomer()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);

            /// Check cookie đã được lưu trong db
            CustomerMySql sqler = new CustomerMySql();
            Customer customer = sqler.GetCustomerFromCookie(cookieResult.cookieValue);
            //if (customer == null)
            //{
            //    Cookie.DeleteUserIdCookie(HttpContext);
            //}
            return customer;
        }

        /// <summary>
        /// Nơi đến khi xác thực thất bại hoặc logout
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthenticationFail()
        {
            return View("~/Views/Administrator/Login.cshtml");
        }

        public void ViewDataGetListPublisher()
        {
            PublisherMySql sqlPubliser = new PublisherMySql();
            List<Publisher> ls = sqlPubliser.GetListPublisher();
            ViewData["lsPublisher"] = ls;
        }

        public void ViewDataGetListCombo()
        {
            ComboMySql sqler = new ComboMySql();
            List<Combo> ls = sqler.GetListCombo();
            ViewData["lsCombo"] = ls;
        }

        public void ViewDataGetListCategory()
        {
            CategoryMySql sqler = new CategoryMySql();
            List<Category> ls = sqler.GetListCategory();
            ViewData["lsCategory"] = ls;
        }
        
        public void ViewDataGetListProductName()
        {
            ProductMySql sqler = new ProductMySql();
            List<ProductIdName> ls = sqler.GetListParent();
            ViewData["lsProductName"] = ls;
        }

        public void ViewDataGetListPublishingCompany()
        {
            ProductMySql sqler = new ProductMySql();
            List<string> ls = sqler.GetListPublishingCompany();
            ViewData["lsPublishingCompany"] = ls;
        }

        public void ViewDataGetListAuthor()
        {
            ProductMySql sqler = new ProductMySql();
            List<string> listAuthor = sqler.GetListAuthor();
            ViewData["lsAuthor"] = listAuthor;
        }

        public void ViewDataGetListTranslator()
        {
            ProductMySql sqler = new ProductMySql();
            List<string> listTranslator = sqler.GetListTranslator();
            ViewData["lsTranslator"] = listTranslator;
        }

        public void ViewDataGetListProductLong()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductLong = sqler.GetListDifferenceIntValue(1);
            ViewData["lsProductLong"] = lsProductLong;
        }

        public void ViewDataGetListProductWide()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductWide = sqler.GetListDifferenceIntValue(2);
            ViewData["lsProductWide"] = lsProductWide;
        }

        public void ViewDataGetListProductHigh()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductHigh = sqler.GetListDifferenceIntValue(3);
            ViewData["lsProductHigh"] = lsProductHigh;
        }

        public void ViewDataGetListProductWeight()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsProductWeight = sqler.GetListDifferenceIntValue(4);
            ViewData["lsProductWeight"] = lsProductWeight;
        }

        public void ViewDataGetListMinAge()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsMinAge = sqler.GetListDifferenceIntValue(5);
            ViewData["lsMinAge"] = lsMinAge;
        }

        public void ViewDataGetListMaxAge()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsMaxAge = sqler.GetListDifferenceIntValue(6);
            ViewData["lsMaxAge"] = lsMaxAge;
        }

        public void ViewDataGetListPublishingTime()
        {
            ProductMySql sqler = new ProductMySql();
            List<int> lsPublishingTime = sqler.GetListDifferenceIntValue(7);
            ViewData["lsPublishingTime"] = lsPublishingTime;
        }

        public void ViewDataGetListItemName()
        {
            ItemModelMySql sqler = new ItemModelMySql();
            List<BasicIdName> ls = sqler.GetListItemName();
            ViewData["lsItemName"] = ls;
        }

        // Nhận và lưu image/video khi upload cho sản phẩm
        // trong kho hoặc item (ProductControler và ItemModelControler)
        public string SaveImageVideo(string path)
        {
            var length = Request.ContentLength;
            var bytes = new byte[length];
            Request.InputStream.Read(bytes, 0, length);


            var fileName = Request.Headers["fileName"];
            var id = Request.Headers["productId"];
            // originalFileName ví dụ: \Media\Product\553\0.png chứa cả đường dẫn từ thư mục media, ta lấy chỉ tên
            var originalFileName = Request.Headers["originalFileName"];
            var exist = Request.Headers["exist"];
            var finish = Request.Headers["finish"];

            if (exist == "true")
            {
                MySqlResultState rs = new MySqlResultState();
                try
                {
                    // originalFileName ví dụ: \Media\Product\553\0.png chứa cả đường dẫn từ thư mục media, ta lấy chỉ tên
                    originalFileName = Path.GetFileName(originalFileName);
                    if (originalFileName != fileName) // Xóa file khác giống tên mới
                    {
                        // Xóa file cũ cùng tên không kể đuôi nếu có
                        Common.DeleteImageVideoWithoutExtension(path + fileName);
                        System.IO.File.Move(path + originalFileName, path + fileName);
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Error(ex.ToString());

                    rs.State = EMySqlResultState.ERROR;
                    rs.Message = ex.ToString();
                }

                // Xóa bỏ ảnh/video không cần lưu nữa với những file có tên lớn hơn tên cuối cùng được lưu
                if (finish == "true")
                {
                    Common.DeleteImageVideoNameGreat(path + fileName);
                }
                return JsonConvert.SerializeObject(rs);
            }

            // Xóa file cũ cùng tên không kể đuôi nếu có
            Common.DeleteImageVideoWithoutExtension(path + fileName);

            if (length > 0)
            {
                // Tên ảnh lưu có định dạng: name VD:0.jpg, 1.png, 3.gif,...Đây là thứ tự của ảnh hiển thị trên web khi chọn ảnh/video
                var saveToFileLoc = string.Format("{0}{1}",
                                              path,
                                               fileName);

                // save the file.
                var fileStream = new FileStream(saveToFileLoc, FileMode.Create, FileAccess.ReadWrite);
                fileStream.Write(bytes, 0, length);
                fileStream.Close();
            }

            if (finish == "true")// Xóa bỏ ảnh/video không cần lưu nữa. Những file có tên lớn hơn tên cuối cùng được lưu
            {
                Common.DeleteImageVideoNameGreat(path + fileName);
            }

            return JsonConvert.SerializeObject(new MySqlResultState());
        }

        // Xóa file trong thư mục tương ứng với id
        public string DeleteAllFileWithTypeBasic(string path, int id, string fileType)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            }
            if (fileType == "isImage")
            {
                Common.DeleteAllImage(path, id.ToString());
            }
            else if (fileType == "isVideo")
            {
                Common.DeleteAllVideo(path, id.ToString());
            }
            MySqlResultState rs = new MySqlResultState();
            return JsonConvert.SerializeObject(rs);
        }
    }
}