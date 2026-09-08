using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.ProductModel;
using MySqlConnector;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Controllers
{
    public class BasicController : Controller
    {
        public BasicController()
        {
            //ViewData["title"] = Common.titleVoiBeNho;
        }


        /// <summary>
        /// Nơi đến khi xác thực thất bại hoặc logout
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthenticationFail()
        {
            //return View("~/Views/Administrator/Login.cshtml");
            return Redirect("~/Administrator/Login");
        }

        // Nhận và lưu image/video khi upload cho sản phẩm
        // trong kho hoặc item (ProductControler và ItemModelControler)
        public MySqlResultState SaveImageVideo(string path)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                var length = Request.ContentLength;
                var bytes = new byte[length];
                Request.InputStream.Read(bytes, 0, length);

                var fileName = Request.Headers["fileName"];
                var id = Request.Headers["productId"];
                // originalFileName ví dụ: \Media\Product\553\0.png chứa cả đường dẫn từ thư mục media,
                //ta lấy chỉ tên
                var originalFileName = Request.Headers["originalFileName"];
                var exist = Request.Headers["exist"];
                var finish = Request.Headers["finish"];

                if (exist == "true")
                {
                    // originalFileName ví dụ: \Media\Product\553\0.png chứa cả đường dẫn từ thư mục media, ta lấy chỉ tên
                    originalFileName = Path.GetFileName(originalFileName);
                    if (originalFileName != fileName)
                    {
                        // Xóa file cũ cùng tên không kể đuôi nếu có
                        Common.DeleteImageVideoWithoutExtension(path + fileName);
                        System.IO.File.Move(path + originalFileName, path + fileName);
                    }
                }
                else
                {
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

                        //// Thêm watermark logo voi bé nhỏ và save ảnh phiên bản 320
                        //if (Common.ImageExtensions.Contains(Path.GetExtension(saveToFileLoc).ToLower()))
                        //{
                            //// Tiki không cho dùng ảnh có logo khi đăng sản phẩm nên commnet chức năng thêm logo
                            //string newsaveToFileLoc = Common.AddWatermark_DeleteOriginalImageFunc(saveToFileLoc);
                            //Common.ReduceImageSizeAndSave(newsaveToFileLoc);
                            //Common.ReduceImageSizeAndSave(saveToFileLoc);
                        //}
                    }
                }

                if (finish == "true")
                {
                    // Xóa bỏ ảnh/video không cần lưu nữa với những file có tên lớn hơn tên cuối cùng được lưu
                    Common.DeleteImageVideoNameGreat(path + fileName);

                    // Xóa ảnh version 320 và sinh lại
                    Common.DeleteAllImage320(path);

                    // Sinh lại phiên bản 320 từ thư mục gốc
                    if(!Common.ReduceImageSizeFromFolder(path))
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Giảm kích thước ảnh lỗi.";
                    }

                }
            }
            catch(Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
        }

        public async Task<Administrator> AuthentAdministratorAsync()
        {
            CookieResultState cookieResult = Cookie.GetVisitorTypeCookie(HttpContext);
            if (string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                MyLogger.GetInstance().Warn("cookieValue is null or empty");
                return null;
            }
            Administrator administrator = await AdministratorMySql.GetAdministratorFromCookieAsync(cookieResult.cookieValue);
            if (administrator == null)
                MyLogger.GetInstance().Warn("Authent administrator fail." + cookieResult.cookieValue);
            return administrator;
        }

        public async Task<Administrator> AuthentAdministratorConnectOutAsync(MySqlConnection conn)
        {
            CookieResultState cookieResult = Cookie.GetVisitorTypeCookie(HttpContext);
            if (string.IsNullOrEmpty(cookieResult.cookieValue))
            {
                MyLogger.GetInstance().Warn("cookieValue is null or empty");
                return null;
            }

            Administrator administrator = await AdministratorMySql.GetAdministratorFromCookieConnectOutAsync(
                cookieResult.cookieValue, conn);
            if (administrator == null)
                MyLogger.GetInstance().Warn("Authent administrator fail." + cookieResult.cookieValue);
            return administrator;
        }

        public async Task<Customer> AuthentCustomerAsync()
        {
            CookieResultState cookieResult = Cookie.GetUserIdCookie(HttpContext);
            if (string.IsNullOrEmpty(cookieResult.cookieValue))
                return null;

            Customer customer = await CustomerMySql.GetCustomerFromCookieAsync(cookieResult.cookieValue);
            if (customer == null)
            {
                Cookie.DeleteUserIdCookie(HttpContext);
                MyLogger.GetInstance().Warn("Authent customer fail." + cookieResult.cookieValue);
            }
            return customer;
        }

        // Xóa file trong thư mục tương ứng với id
        public async Task<string> DeleteAllFileWithTypeBasicAsync(string path, int id, string fileType)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }
            if (fileType == "isImage")
            {
                Common.DeleteAllImage(path);
            }
            else if (fileType == "isVideo")
            {
                Common.DeleteAllVideo(path);
            }
            MySqlResultState rs = new MySqlResultState();
            return JsonConvert.SerializeObject(rs);
        }
    }
}
