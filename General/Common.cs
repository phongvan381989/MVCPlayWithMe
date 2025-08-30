using MVCPlayWithMe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using RestSharp;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Threading;
using System.Text.RegularExpressions;

namespace MVCPlayWithMe.General
{
    public class Common
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".jpg", ".jpeg", ".jfif", ".png", ".svg"};
        public static readonly List<string> VideoExtensions = new List<string> { ".mp4" };
        public static readonly string dateFormat = "yyyy-MM-dd";
        public static readonly int quota = 5;
        public static readonly string srcNoImageThumbnail = "/Media/NoImageThumbnail.png";
        public static readonly int offset = 20;
        public static readonly int rowOnPage = 6; // Số dòng item trên trang kết quả tìm kiếm
        public static readonly string httpsVoiBeNho = "https://voibenho.com";

        public static readonly JsonSerializerSettings jsonSerializersettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        // Từ thực tế thấy:
        // set deal giảm giá khi tồn kho >= 3, khi tồn kho < 3 deal của tiki bị tắt
        // Nếu sản phẩm có tồn kho < 3, ta set deal mới thì không bị tắt
        public static readonly int minQuantityOfDealTiki = 2;

        public static readonly RestClient client = new RestClient();

        // Lần đầu trình duyệt truy cập chưa có thông tin itemOnRow nên cần giá trị mặc định
        // Giá trị cần lớn nhất có thể
        public static readonly int itemOnRowDefault = 6;
        // Cookie const
        #region Cookie
        // Giá trị là UserCookieIdentify, phục vụ check khách hàng đăng nhập
        public static readonly string userIdKey = "uid";
        // Chỉ có cookie này khi đăng nhập như người quản trị.
        // Giá trị là AdministratorCookieIdentify, phục vụ check admin đăng nhập
        public static readonly string visitorType = "visitorType";
        // cookie có dạng: cart=id=123#q=10#real=1$id=321#q=1#real=0$....$id=321#q=2#real=0
        // id: mã model, q: số lượng thêm vào giỏ hàng, real: 1-thực sự chọn mua, 0-có thể mua sau này
        public static readonly string cartKey = "cart";

        public static readonly string customerInforKey = "cusinfor";
        public static readonly string itemOnRowSearchPage = "itemOnRow";
        public static readonly string orderIdList = "orderList"; // danh sách mã đơn hàng đối với khách vãng lai
        public static readonly int standardShipFeeInHaNoi = 15000; // Phí ship tiêu chuẩn trong Hà Nội
        public static readonly int standardShipFeeOutHaNoi = 30000; // Phí ship tiêu chuẩn ngoài Hà Nội
        #endregion

        public enum EECommerceType
        {
            PLAY_WITH_ME,
            TIKI,
            SHOPEE,
            LAZADA,
            ALL
        }

        // Trả về kiểu Enum
        public static EECommerceType GetEECommerceTypeFromString(string eType)
        {
            EECommerceType eEnumType = EECommerceType.ALL;
            if(eType == ePlayWithMe)
            {
                eEnumType = EECommerceType.PLAY_WITH_ME;
            }
            else if (eType == eTiki)
            {
                eEnumType = EECommerceType.TIKI;
            }
            else if (eType == eShopee)
            {
                eEnumType = EECommerceType.SHOPEE;
            }
            else if (eType == eLazada)
            {
                eEnumType = EECommerceType.LAZADA;
            }
            return eEnumType;
        }

        // Trả về kiểu int với ALL type là -1;
        public static int GetIntECommerceTypeFromString(string eType)
        {
            EECommerceType eEnumType = EECommerceType.ALL;
            if (eType == ePlayWithMe)
            {
                eEnumType = EECommerceType.PLAY_WITH_ME;
            }
            else if (eType == eTiki)
            {
                eEnumType = EECommerceType.TIKI;
            }
            else if (eType == eShopee)
            {
                eEnumType = EECommerceType.SHOPEE;
            }
            else if (eType == eLazada)
            {
                eEnumType = EECommerceType.LAZADA;
            }

            if (eEnumType == EECommerceType.ALL)
            {
                return -1;
            }
            return (int)eEnumType;
        }

        public enum ECommerceOrderStatus
        {
            PACKED, // Đơn thực tế được đóng
            RETURNED,// Đơn thực tế hoàn về kho
            BOOKED, // Đã trừ số lượng trong kho theo đơn phát sinh trên sàn, nhưng chưa PACKED
            UNBOOKED,// Đã cộng số lượng trong kho khi đơn trên sàn hủy và trạng thái là BOOKED
            DONT_EXIST// Chưa tồn tại trong DB
        }

        public static string eShopee = "SHOPEE";
        public static string eTiki = "TIKI";
        public static string eLazada = "LAZADA";
        public static string ePlayWithMe = "PLAYWITHME";
        public static string eAll = "ALL";

        public static string tikiActive = "ACTIVE";
        public static string tikiUnactive = "UNACTIVE";

        public static string tikiPWMHome = "https://tiki.vn/cua-hang/play-with-me";

        #region Tiki Deal Discount
        // Thời gian bắt đầu mặc định là thời điểm hiện tại + 10 phút
        // Khoảng thời gian mặc định là 180 ngày
        // qty_max, qty_limit không giới hạn mặc định là 10000
        public static int qtyMaxTikiDealDiscount = 10000;
        public static int qtyLimitTikiDealDiscount = 10000;
        public static int intervalTikiDealDiscount = 180; // 180 ngày
        public static int minutesAddNowTikiDealDiscount = 10; // 10 phút
        public static int maxOfCreatingRequestBodyObject = 100;// Khi tạo deal, số sku tối đa
        #endregion

        public static string[] OrderStatusArray = { "Đã Đóng", "Đã Hoàn", "Giữ Chỗ", "Hủy Giữ Chỗ", "Chưa Tồn Tại" };
        //public static string returnedOrder = "Đã Hoàn";
        //public static string packedOrder = "Đã Đóng";
        //public static string bookedOrder = "Giữ Chỗ";
        //public static string unbookedOrder = "Hủy Giữ Chỗ";

        /// <summary>
        /// Đường dẫn thư mục chứa file ảnh
        /// </summary>
        public static string ProductMediaFolderPath;
        public static string absoluteProductMediaFolderPath;

        public static string ItemMediaFolderPath;
        public static string absoluteItemMediaFolderPath;

        public static string MediaFolderPath;
        public static string TemporaryImageShopeeMediaFolderPath;
        public static string TemporaryImageTikiMediaFolderPath;

        public static string srcCertificateFolderPath;

        public static string ConvertIntToVNDFormat(int money)
        {
            // Thêm ','
            StringBuilder sb = new StringBuilder();
            sb.Append(money.ToString());
            int length = sb.Length;
            if (length > 9)
            {
                sb.Insert(length - 3, ',');
                sb.Insert(length - 6, ',');
                sb.Insert(length - 9, ',');

            }
            else if (length > 6)
            {
                sb.Insert(length - 3, ',');
                sb.Insert(length - 6, ',');

            }
            else if (length > 3)
            {
                sb.Insert(length - 3, ',');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Lấy được đường dẫn tuyệt đối thư mục chứa media của sản phẩm hoặc trả về null nếu không có.
        /// </summary>
        /// <returns></returns>
        public static string GetAbsoluteProductMediaFolderPath(string productId)
        {
            string path = absoluteProductMediaFolderPath + productId + @"/";
            //MyLogger.GetInstance().Info(path);
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
                path = null;
            }
            return path;
        }

        /// <summary>
        /// Lấy được đường dẫn Src thư mục chứa certificate của sàn, nhà phát hành.
        /// eType: TIKI, SHOPEE, LAZADA,..
        /// </summary>
        /// <returns></returns>
        public static string GetSrcCertificateFolderPath(string eType)
        {
            string path = string.Empty;
            if (eType == eTiki)
            {
                path = srcCertificateFolderPath + eTiki + @"/";
            }
            else if (eType == eShopee)
            {
                path = srcCertificateFolderPath + eShopee + @"/";
            }
            else if (eType == eLazada)
            {
                path = srcCertificateFolderPath + eLazada + @"/";
            }
            return path;
        }

        public static string CreateAbsoluteProductMediaFolderPath(string productId)
        {
            string path = absoluteProductMediaFolderPath + productId + @"/";
            Directory.CreateDirectory(path);
            // Tạo thư mục 320
            Directory.CreateDirectory(absoluteProductMediaFolderPath + productId + @"_320");
            return path;
        }

        ///// </summary>
        ///// <param name="productId"></param>
        ///// <summary>
        ///// Lấy tất cả file của sản phẩm
        ///// <returns></returns>
        //static private string[] GetAllAbsoluteFileNameOfProduct(string productId)
        //{
        //    string path = GetAbsoluteProductMediaFolderPath(productId);
        //    if (path == null)
        //        return new string[0] ;

        //    return Directory.GetFiles(path);
        //}

        // Không lấy file trong subfolder không sắp xếp tăng dần
        // mediaType: 0-lấy video và ảnh, 1-lấy ảnh, 2-lấy video
        public static void GetAllMediaFilesIncludeSubfolderDontSort(List<string> listMediaFiles, string folderPath, int mediaType)
        {
            listMediaFiles.Clear();

            // Check thư mục chứa tồn tại
            if (String.IsNullOrEmpty(folderPath))
                return;
            if (!Directory.Exists(folderPath))
                return;

            if (mediaType == 0 || mediaType == 1)
            {
                // Image formats
                foreach (string str in ImageExtensions)
                {
                    listMediaFiles.AddRange(Directory.GetFiles(folderPath, "*" + str, SearchOption.AllDirectories).ToList());
                }
            }
            if (mediaType == 0 || mediaType == 2)
            {
                    foreach (string str in VideoExtensions)
                    {
                        listMediaFiles.AddRange(Directory.GetFiles(folderPath, str, SearchOption.AllDirectories).ToList());
                    }
            }
        }

        /// <summary>
        /// Từ id sản phẩm, lấy được đường dẫn tới ảnh dùng cho thẻ img
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<string> GetProductImageSrc(string productId)
        {
            List<string> src = new List<string>();

            string path = absoluteProductMediaFolderPath + productId + @"/";
            if (!Directory.Exists(path))
            {
                return src;
            }

            string[] files = Directory.GetFiles(path);

            string relPath = ProductMediaFolderPath + productId + @"/";

            foreach (var file in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    src.Add(relPath + Path.GetFileName(file));
                }
            }
            SortSourceFile(src);
            return src;
        }

        //// Lấy ảnh đầu tiên của imageSrc cho nhanh
        //public static string GetFirstProductImageSrc(string productId)
        //{
        //    string path = System.Web.HttpContext.Current.Server.MapPath(ProductMediaFolderPath) + productId + @"/";
        //    //MyLogger.GetInstance().Info(path);
        //    if (!Directory.Exists(path))
        //    {
        //        return string.Empty;
        //    }

        //    string src = string.Empty;
        //    string[] files = Directory.GetFiles(path, "0.*");

        //    string relPath = ProductMediaFolderPath + productId + @"/";

        //    foreach (var file in files)
        //    {
        //        if (ImageExtensions.Contains(Path.GetExtension(file).ToLower()))
        //        {
        //            src = relPath + Path.GetFileName(file);
        //            break;
        //        }
        //    }
        //    return src;
        //}

        // Lấy ảnh đầu tiên của imageSrc cho nhanh
        public static string GetFirstProductImageSrc(string productId)
        {
            string src = string.Empty;
            try
            {
                string path = Common.absoluteProductMediaFolderPath + productId;
                if (Directory.Exists(path))
                {
                    // Tìm tất cả file có tên là "0" và phần mở rộng hợp lệ
                    string matchedFile = Directory
                        .EnumerateFiles(path)
                        .FirstOrDefault(f =>
                            Path.GetFileNameWithoutExtension(f).Equals("0", StringComparison.OrdinalIgnoreCase)
                            && ImageExtensions.Contains(Path.GetExtension(f).ToLower())
                        );

                    if (matchedFile != null)
                    {
                        src = ProductMediaFolderPath + productId + @"/" + Path.GetFileName(matchedFile);
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                src = string.Empty;
            }
            return src;
        }

        /// <summary>
        /// Từ tên chuyển đổi sang số int
        /// </summary>
        /// <param name="fullFileName">Gồm đường dẫn</param>
        /// <returns></returns>
        private static int ConvertNameToInt(string fullFileName)
        {
            int intName = Common.ConvertStringToInt32(Path.GetFileNameWithoutExtension(fullFileName));
            return intName;
        }

        /// <summary>
        /// Sắp xếp tên file theo thứ tự tăng dần trực tiếp trên tham số
        /// </summary>
        /// <param name="src"> Mảng tên file gồm cả đường dẫn</param>
        /// <returns></returns>
        public static void SortSourceFile(List<string> src)
        {
            IDictionary<int, string> dicSrc = new Dictionary<int, string>();
            int indexTemp = 0;
            foreach(var file in src)
            {
                indexTemp = ConvertNameToInt(file);
                if (indexTemp != Int32.MinValue && !dicSrc.ContainsKey(indexTemp))
                {
                    dicSrc.Add(ConvertNameToInt(file), file);
                }
            }

            src.Clear();
            foreach (var k in dicSrc.OrderBy(x => x.Key))
            {
                src.Add(k.Value);
            }
        }

        /// <summary>
        /// Từ id sản phẩm, lấy ảnh đầu tiên (tên là 0.*) làm ảnh đại diện
        /// Nếu không có ảnh lấy ảnh mặc định
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static string GetThumbnailImageSrc(int productId)
        {
            string str = srcNoImageThumbnail;
            string path = GetAbsoluteProductMediaFolderPath(productId.ToString());
            string[] files = Directory.GetFiles(Path.GetDirectoryName(path), "0.*");

            string relPath = ProductMediaFolderPath + productId + @"/";

            foreach (var file in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    str = relPath + Path.GetFileName(file);
                    break;
                }
            }
            return str;
        }

        /// <summary>
        ///  Từ list id sản phẩm, lấy list ảnh đầu tiên (tên là 0.*) làm ảnh đại diện
        /// </summary>
        /// <param name="lsProductId"></param>
        /// <returns></returns>
        public static List<string> GetListThumbnailImageSrc(List<int> lsProductId)
        {
            List<string> ls = new List<string>();
            if (lsProductId == null)
                return ls;

            foreach(var i in lsProductId)
            {
                ls.Add(GetThumbnailImageSrc(i));
            }
            return ls;
        }

        /// <summary>
        /// Từ id sản phẩm, lấy được đường dẫn tới video dùng cho thẻ video
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<string> GetProductVideoSrc(string productId)
        {
            List<string> src = new List<string>();

            string path = absoluteProductMediaFolderPath + productId + @"/";
            if (!Directory.Exists(path))
            {
                return src;
            }

            string[] files = Directory.GetFiles(path);

            string relPath = ProductMediaFolderPath + productId + @"/";

            foreach (var file in files)
            {
                if (VideoExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    src.Add(relPath + Path.GetFileName(file));
                }
            }
            SortSourceFile(src);
            return src;
        }

        /// <summary>
        ///  Lấy được tên ảnh lớn nhất trong thư mục
        /// </summary>
        /// <param name="path"> đường dẫn</param>
        /// <returns>-1 nếu trong thư mục không chứa ảnh</returns>
        public static int GetMaxNameImage(string path)
        {
            int maxName = -1;
            string[] files = Directory.GetFiles(path);
            foreach(var file in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    int intName = ConvertNameToInt(file);
                    if (maxName < intName)
                        maxName = intName;
                }
            }
            //MyLogger.GetInstance().Info("maxName: " + maxName);
            return maxName;
        }

        /// <summary>
        ///  Lấy được tên ảnh lớn nhất trong thư mục
        /// </summary>
        /// <param name="path"> đường dẫn</param>
        /// <returns>-1 nếu trong thư mục không chứa video</returns>
        public static int GetMaxNameVideo(string path)
        {
            int maxName = -1;
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (VideoExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    int intName  = ConvertNameToInt(file);
                    if (maxName < intName)
                        maxName = intName;
                }
            }
            //MyLogger.GetInstance().Info("maxName: " + maxName);
            return maxName;
        }

        /// <summary>
        /// VD: file ảnh gốc là: C:\Users\phong\OneDrive\Desktop\576\0.jfif.
        /// Ta tạo có check exist thư mục C:\Users\phong\OneDrive\Desktop\576_320 và lưu ảnh mới 0.jpg
        /// Ảnh mới có kính thước 320, chất lượng 100% ảnh gốc
        /// </summary>
        /// <param name="path"> Gồm cả tên file</param>
        /// <param name="isCreateFolder">Có tạo thư mục chứa phiên bản 320 không?</param>
        /// <returns></returns>
        public static string Get320PathFromOriginal(string path, bool isCreateFolder)
        {
            string newPath = string.Empty;

            // Lấy được foler ông
            string dir = Path.GetDirectoryName(path) + "_320";
            if(isCreateFolder && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string x = Path.GetExtension(path);
            if (Path.GetExtension(path).ToLower() == ".png")
                newPath = dir + "\\" + Path.GetFileNameWithoutExtension(path) + ".png";
            else
                newPath = dir + "\\" + Path.GetFileNameWithoutExtension(path) + ".jpg";
            return newPath;
        }

        // Từ đường dẫn đến ảnh bình thường, xóa ảnh bình thường và ảnh nhỏ 320 
        public static void DeleteNormalAnd320Image(string path)
        {
            // Xóa ảnh gốc
            if (File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            // Xóa ảnh phiên bản 320
            string path320 = Get320PathFromOriginal(path, false);
            if (File.Exists(path320))
            {
                System.IO.File.Delete(path320);
            }
        }

        /// <summary>
        /// Xóa file ảnh hoặc video có tên không kể đuôi giống tên file trong tham số
        /// </summary>
        /// <param name="path">tên gồm đường dẫn</param>
        public static void DeleteImageVideoWithoutExtension(string path)
        {
            //MyLogger.GetInstance().Info(" Start DeleteImageVideoWithoutExtension: " + path);
            //// Check tên file là image hoặc video.
            Boolean isImage = ImageExtensions.Contains(Path.GetExtension(path).ToLower());

            // Lấy chỉ tên file
            string onlyName = Path.GetFileNameWithoutExtension(path);

            string[] files = Directory.GetFiles(Path.GetDirectoryName(path), onlyName + ".*");//Nếu có mảng có 1 phần tử
            List<string> src = new List<string>();
            foreach (var f in files)
            {
                if (isImage)
                {
                    if (ImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                    {
                        System.IO.File.Delete(f);
                        //DeleteNormalAnd320Image(f);
                        //MyLogger.GetInstance().Info("Delete: " + f);
                        break;
                    }
                }
                else
                {
                    if (VideoExtensions.Contains(Path.GetExtension(f).ToLower()))
                    {
                        System.IO.File.Delete(f);
                        //MyLogger.GetInstance().Info("Delete: " + f);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///  Xóa file có tên lớn hơn tên trong tham số.Ví dụ: tham số là :5.png xóa tất các ảnh 6.jpg, 7png,...
        /// </summary>
        /// <param name="path">tên gồm đường dẫn</param>
        public static void DeleteImageVideoNameGreat(string path)
        {
            Boolean isImage = ImageExtensions.Contains(Path.GetExtension(path).ToLower());
            int maxName = -1;
            if (isImage)
            {
                maxName = GetMaxNameImage(Path.GetDirectoryName(path));
            }
            else
            {
                maxName = GetMaxNameVideo(Path.GetDirectoryName(path));
            }
            int intName = ConvertNameToInt(path);
            if (intName != int.MinValue)
            {
                intName++;
                // Bắt đầu xóa file có tên lớn hơn intName
                while (intName <= maxName)
                {
                    string fileNameTemp = intName.ToString() + Path.GetExtension(path);
                    DeleteImageVideoWithoutExtension(Path.GetDirectoryName(path) + @"/" + fileNameTemp);
                    intName++;
                }
            }
        }

        // Xóa file trong thư mục, không xóa thư mục con
        // Xóa ảnh ở phiên bản _320
        public static void DeleteAllMediaFileInclude320(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (var f in files)
            {
                System.IO.File.Delete(f);
            }

            // Xóa cả thư mục ảnh phiên bản 320
            string x = Path.GetDirectoryName(path) + "_320";
            if (Directory.Exists(x))
            {
                Directory.Delete(x, true);
            }
            return;
        }


        // Xóa dữ liệu media ở Media\Item\itemId\Model
        public static void DeleteImageModelInclude320(int itemId, int modelId)
        {
            string path = Common.GetAbsoluteModelMediaFolderPath(itemId);
            if (path == null)
            {
                return;
            }

            string[] files = Directory.GetFiles(path, modelId.ToString() + ".*");
            foreach(var f in files)
            {
                System.IO.File.Delete(f);
            }

            // Xóa phiên bản 320
            // Vì path có "/" cuối cùng, ta xử lý cắt bỏ
            path = path + @"_320";
            path = Path.GetDirectoryName(path) + @"_320";
            if (Directory.Exists(path))
            {
                string[] files320 = Directory.GetFiles(path, modelId.ToString() + ".*");
                foreach (var f in files320)
                {
                    System.IO.File.Delete(f);
                }
            }
        }

        // Xóa dữ liệu media ở Media\Item\itemId
        public static void DeleteMediaItemInclude320(int itemId)
        {
            string path = Common.GetAbsoluteItemMediaFolderPath(itemId);
            if (path == null)
            {
                return;
            }
            if (!Directory.Exists(path))
                return;

            Directory.Delete(path,true);

            // Xóa phiên bản 320
            // Vì path có "/" cuối cùng, ta xử lý cắt bỏ
            path = Path.GetDirectoryName(path) + @"_320";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Folder chứa image/video</param>
        public static void DeleteAllImage(string path)
        {
            string[] files =  Directory.GetFiles(path);

            foreach (var f in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                {
                    System.IO.File.Delete(f);
                }
            }
            DeleteAllImage320(path);
        }

        // Xóa tất cả ảnh của thư mục 320 nếu có
        static public void DeleteAllImage320(string path)
        {
            // Không xóa thư mục, xóa file ảnh trong thư mục
            string x = Path.GetDirectoryName(path) + "_320";
            string[] files = Directory.GetFiles(x);

            foreach (var f in files)
            {
                System.IO.File.Delete(f);
            }
        }

        public static void DeleteAllVideo(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (var f in files)
            {
                if (VideoExtensions.Contains(Path.GetExtension(f).ToLower()))
                {
                    System.IO.File.Delete(f);
                }
            }
            return;
        }

        /// <summary>
        /// Xóa ảnh, video trong thư mục
        /// </summary>
        /// <param name="path">Thư mục ảnh/video</param>
        /// <returns></returns>
        public static void DeleteMediaFolder(string path)
        {
            // Xóa thư mục ảnh/ video gốc
            if(!Directory.Exists(path))
            {
                return;
            }

            Directory.Delete(path, true);

            // Xóa cả thư mục ảnh phiên bản 320
            string x = Path.GetDirectoryName(path) + "_320";
            if (Directory.Exists(x))
            {
                Directory.Delete(x, true);
            }

        }

        public static Int32 ConvertStringToInt32(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return System.Int32.MinValue;
            }

            int rs;
            try
            {
                rs = Int32.Parse(str);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(str);
                MyLogger.GetInstance().Warn(ex.ToString());
                rs = System.Int32.MinValue;
            }

            return rs;
        }

        #region Xử lý ảnh lấy từ bên sàn thương mại điện tử
        /// <summary>
        ///  Từ url lấy được tên file
        /// </summary>
        /// <param name="url"https://salt.tikicdn.com/cache/280x280/ts/product/c5/53/ad/991011e797c67d6910b87491ddeee138.png </param>
        ///                  https://cf.shopee.vn/file/673f310b9b9152f0898752eb56e67ac6_tn
        /// <returns></returns>
        public static String GetNameFromURL(string url)
        {
            string fileName = string.Empty;
            if (string.IsNullOrEmpty(url))
                return fileName;

            // Lấy tên file ảnh
            // Từ url lấy được tên ảnh
            int lastIndex = url.LastIndexOf('/');
            if (lastIndex == -1 || lastIndex == url.Length - 1)
            {
            }
            else
            {
                fileName = url.Substring(lastIndex + 1);
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                string extention = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(extention))
                    fileName = fileName + ".jfif";
            }
            return fileName;
        }

        /// <summary>
        /// Lấy được image source, nếu cần tải ảnh từ sàn TMDT
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pathFolder"></param>
        public static string GetFullPathOfImage(string url, string pathFolder)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(pathFolder))
                return string.Empty;

            string fileName = GetNameFromURL(url);

            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            if (!File.Exists(Path.Combine(pathFolder, fileName)))
            {
                // Nếu ảnh chưa có ta download ảnh
                RestRequest request = new RestRequest(url, Method.GET);

                try
                {
                    var fileBytes = client.DownloadData(request);
                    File.WriteAllBytes(Path.Combine(pathFolder, fileName), fileBytes);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    return string.Empty;
                }
            }

            return Path.Combine(pathFolder, fileName);
        }

        /// <summary>
        /// Tên ảnh được lấy từ url
        /// Check xem ảnh đã tồn tại trong thư mục hay chưa? Nếu chưa tải ảnh từ địa chỉ web và lưu
        /// </summary>
        /// <param name="url">https://salt.tikicdn.com/cache/280x280/ts/product/c5/53/ad/991011e797c67d6910b87491ddeee138.png</param>
        ///                   https://cf.shopee.vn/file/673f310b9b9152f0898752eb56e67ac6_tn
        /// <param name="fileName">Tên gồm đường dẫn</param>
        public static int DownloadImageAndSaveWithName(string url, string fileName)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileName))
            {
                return 0;
            }

            // Check xem ảnh đã tồn tại hay chưa?
            if (File.Exists(fileName))
                return 0;

            //IRestResponse response = client.Execute(request);

            try
            {
                RestRequest request = new RestRequest(url, Method.GET);
                var fileBytes = client.DownloadData(request);
                File.WriteAllBytes(fileName, fileBytes);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Check xem ảnh đã tồn tại trong thư mục hay chưa? Nếu chưa ải ảnh từ địa chỉ web và lưu
        /// </summary>
        /// <param name="url">https://salt.tikicdn.com/cache/280x280/ts/product/c5/53/ad/991011e797c67d6910b87491ddeee138.png</param>
        ///                   https://cf.shopee.vn/file/673f310b9b9152f0898752eb56e67ac6_tn
        /// <param name="fileName">Tên gồm đường dẫn</param>
        public static void DownloadVideoAndSaveWithName(string url, string fileName)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            // Check xem video đã tồn tại hay chưa?
            if (File.Exists(fileName))
                return;

            RestRequest request = new RestRequest(url, Method.GET);
            //IRestResponse response = client.Execute(request);

            try
            {
                var fileBytes = client.DownloadData(request);
                File.WriteAllBytes(fileName, fileBytes);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        /// <summary>
        /// Tải ảnh từ url, add water mark và sinh phiên bản _320
        /// Tiki không cho dùng ảnh có logo khi đăng sản phẩm nên commnet chức năng thêm logo nên khi tải sản phẩm
        /// cho Product ta không add watermark
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="isAddWatermark">isAddWatermark: true sinh watermark và xóa ảnh cũ, false thì không</param>
        public static void DownloadImageAddWaterMarkAndReduce(
            string url,
            string fileName,
            Boolean isAddWatermark)
        {
            if(string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            int rs = DownloadImageAndSaveWithName(url, fileName);
            if(rs == 1)
            {
                // Tiki không cho dùng ảnh có logo khi đăng sản phẩm nên commnet chức năng thêm logo
                if (isAddWatermark)
                {
                    // Thêm water mark
                    string newsaveToFileLoc = Common.AddWatermark_DeleteOriginalImageFunc(fileName);
                    // sinh phiên bản _320
                    ReduceImageSizeTo320AndSave(newsaveToFileLoc);
                }
                else
                {
                    ReduceImageSizeTo320AndSave(fileName);
                }
            }
        }
        #endregion

        public static long ConvertStringToInt64(string str)
        {
            long rs;

            try
            {
                rs = Int64.Parse(str);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(str);
                MyLogger.GetInstance().Warn(ex.ToString());
                rs = System.Int64.MinValue;
            }

            return rs;
        }

        /// <summary>
        /// Giá trị num trong kiểu int, nhưng đang được lưu kiểu long, ta convert sang string rồi convert sang int
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int ConvertLongToInt(long num)
        {
            return ConvertStringToInt32(num.ToString());
        }

        public static List<int> ConvertJsonArrayToListInt(string json)
        {
            List<string> lsStr = JsonConvert.DeserializeObject<List<string>>(json);
            List<int> lsInt = new List<int>();

            foreach(var str in lsStr)
            {
                lsInt.Add(Common.ConvertStringToInt32(str));
            }
            return lsInt;
        }

        public static void SetResultException(Exception ex, MySqlResultState result)
        {
            MyLogger.GetInstance().Warn(ex.ToString());
            result.State = EMySqlResultState.EXCEPTION;
            result.Message = ex.ToString();
        }

        /// <summary>
        /// Kiểm tra tham số request có null hay không
        /// str=null, empty hay "null" trả về true
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static Boolean ParameterOfURLQueryIsNullOrEmpty(string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            if (str == "null")
                return true;

            return false;
        }

        #region Xử lý tiền
        public static int FloorMoney(int money)
        {
            return money / 100 * 100;
        }
        #endregion

        #region Xử chung Item/model
        /// <summary>
        /// Lấy được đường dẫn tuyệt đối thư mục chứa media của item hoặc trả về null nếu không có.
        /// </summary>
        /// <returns></returns>
        public static string GetAbsoluteItemMediaFolderPath(int itemId)
        {
            string path = absoluteItemMediaFolderPath + itemId.ToString() + @"/";
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
                path = null;
            }
            return path;
        }

        public static string CreateAbsoluteItemMediaFolderPath(int itemId)
        {
            string path = absoluteItemMediaFolderPath + itemId.ToString() + @"/";
            Directory.CreateDirectory(path);
            // Tạo thư mục 320
            Directory.CreateDirectory(absoluteItemMediaFolderPath + itemId.ToString() + @"_320");
            return path;
        }

        /// <summary>
        /// Lấy được đường dẫn tuyệt đối thư mục chứa media của model hoặc trả về null nếu không có.
        /// </summary>
        /// <returns></returns>
        public static string GetAbsoluteModelMediaFolderPath(int itemId)
        {
            string path = absoluteItemMediaFolderPath + itemId.ToString() + @"/Model/";
            //MyLogger.GetInstance().Debug(path);
            if (!Directory.Exists(path))
            {
                path = null;
            }
            return path;
        }

        public static string CreateAbsoluteModelMediaFolderPath(int itemId)
        {
            string path = absoluteItemMediaFolderPath + itemId.ToString() + @"/Model/";
            Directory.CreateDirectory(path);
            // Tạo thư mục 320
            Directory.CreateDirectory(absoluteItemMediaFolderPath + itemId.ToString() + @"/Model_320");
            return path;
        }

        /// <summary>
        /// Từ id item, lấy được đường dẫn tới ảnh dùng cho thẻ img
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static List<string> GetItemImageSrc(int itemId)
        {
            List<string> src = new List<string>();
            string[] files = null;

            try
            {
                string path = absoluteItemMediaFolderPath + itemId.ToString() + @"/";

                files = Directory.GetFiles(path);
            }
            catch(Exception)
            {
                return src;
            }

            string relPath = ItemMediaFolderPath + itemId.ToString() + @"/";

            foreach (var file in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    src.Add(relPath + Path.GetFileName(file));
                }
            }
            SortSourceFile(src);
            return src;
        }

        // Từ src ảnh lấy được src phiên bản 320
        // path là tên gồm đường dẫn
        public static string Get320VersionOfImageSrc(string path)
        {
            // Nếu đã là phiên bản 320 bỏ qua
            if (path.Contains("_320"))
                return path;
            // Nếu là NoImageThumbnail.png bỏ qua
            if (path.Contains("NoImageThumbnail"))
                return path;

            var fileNameWithoutExtention = Path.GetFileNameWithoutExtension(path);
            var fileExtention = Path.GetExtension(path);
            var fileDir = Path.GetDirectoryName(path);


            var newFileName = "";
            if (fileExtention != "png" &&
                fileExtention != "jpg")
            {
                newFileName = fileNameWithoutExtention + ".jpg";
            }
            else
            {
                newFileName = Path.GetFileName(path);
            }
            var newSrc = fileDir + "_320/" + newFileName;
            return newSrc;
        }

        //// Lấy ảnh đầu tiên của imageSrc cho nhanh
        //public static string GetFirstItemImageSrc(int itemId)
        //{
        //    string src = string.Empty;
        //    string[] files = null;

        //    try
        //    {
        //        string path = absoluteItemMediaFolderPath + itemId.ToString() + @"/";

        //        files = Directory.GetFiles(path, "0.*"); // trả về file ảnh và video nếu có
        //    }
        //    catch (Exception)
        //    {
        //        return src;
        //    }

        //    string relPath = ItemMediaFolderPath + itemId.ToString() + @"/";

        //    foreach (var file in files)
        //    {
        //        if (ImageExtensions.Contains(Path.GetExtension(file).ToLower()))
        //        {
        //            src = relPath + Path.GetFileName(file);
        //            break;
        //        }
        //    }

        //    return src;
        //}

        // Lấy ảnh đầu tiên của imageSrc cho nhanh
        public static string GetFirstItemImageSrc(int itemId )
        {
            string src = string.Empty;
            string path = Common.absoluteItemMediaFolderPath + itemId.ToString() + @"/0.jpg";
            if(!File.Exists(path))
            {
                path = Common.absoluteItemMediaFolderPath + itemId.ToString() + @"/0.png";
                if (!File.Exists(path))
                {
                    path = Common.absoluteItemMediaFolderPath + itemId.ToString() + @"/0.jfif";
                    if (!File.Exists(path))
                        return src;
                }
            }
            src = ItemMediaFolderPath + itemId.ToString() + @"/" + Path.GetFileName(path);

            return src;
        }

        /// <summary>
        /// Từ id item, lấy được đường dẫn tới video dùng cho thẻ video
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetItemVideoSrc(int itemId)
        {
            string src= "";
            string[] files = null;

            try
            {
                string path = absoluteItemMediaFolderPath + itemId.ToString() + @"/";

                files = Directory.GetFiles(path);
            }
            catch (Exception)
            {
                return src;
            }

            string relPath = ItemMediaFolderPath + itemId.ToString() + @"/";

            foreach (var file in files)
            {
                if (VideoExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    src = relPath + Path.GetFileName(file);
                    break;
                }
            }
            return src;
        }

        /// <summary>
        /// Từ id model, lấy được đường dẫn tới thumbnail dùng cho thẻ img
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetModelImageSrc(int itemId, int modelId)
        {
            string path = GetAbsoluteModelMediaFolderPath(itemId);
            if(path == null)
                return Common.srcNoImageThumbnail;

            string src = "";
            string relPath = ItemMediaFolderPath + itemId.ToString() + @"/Model/";

            // Model không có video nên không cần check
            string[] files = Directory.GetFiles(path, modelId.ToString() + ".*");
            if (files.Length != 0)
                src = relPath + Path.GetFileName(files[0]);
            else
                src = Common.srcNoImageThumbnail;
            return src;
        }
        #endregion

        #region Xử lý mã hóa login
        public const int SHA512Size = 64;

        /// <summary>
        /// Create a salt value.
        /// </summary>
        /// <returns>Salt value.</returns>
        public static byte[] CreateSalt()
        {
            // Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[Common.SHA512Size];
            rng.GetBytes(buff);
            return buff;
        }

        /// <summary>
        /// Generate a hash value with a salt added to orignal input
        /// </summary>
        /// <param name="password"> Original input.</param>
        /// <param name="salt"> Salt value.</param>
        /// <returns>Byte array.</returns>
        public static byte[] GenerateSaltedHash(string password, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA512Managed();
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            byte[] plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];
            Array.Copy(plainText, plainTextWithSaltBytes, plainText.Length);
            Array.Copy(salt, 0, plainTextWithSaltBytes, plainText.Length, salt.Length);

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        public static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Thời gian
        /// <summary>
        /// Định dạng: YYYY
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Boolean CheckYear(string text)
        {
            Int32 year;
            try
            {
                year = Int32.Parse(text);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }
            if (year > 9999 || year < 1900)
                return false;

            return true;
        }

        /// <summary>
        /// Định dạng: M hoặc MM
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Boolean CheckMonth(string text)
        {
            Int32 month;
            try
            {
                month = Int32.Parse(text);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }
            if (month > 12 || month < 1)
                return false;

            return true;
        }

        /// <summary>
        /// Định dạng D hoặc DD
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Boolean CheckDayOfMonth(string text)
        {
            Int32 day;
            try
            {
                day = Int32.Parse(text);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }
            if (day > 31 || day < 1)
                return false;

            return true;
        }

        /// <summary>
        ///  Check text có thể convert sang dạng thời gian
        ///  03/08/1989 -> DD/MM/YYYY
        ///  3/8/1989 -> DD/MM/YYYY
        ///  8/1989 -> MM/YYYY
        ///  1989->YYYY
        /// </summary>
        /// <param name="text"></param>
        /// <param name="enableNullOrEmpty">True: Cho phép text là null hay empty</param>
        /// <returns></returns>
        public static Boolean CheckTimeValid(string text, Boolean enableNullOrEmpty)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (enableNullOrEmpty)
                    return true;
                else
                    return false;
            }

            char[] delimiterChars = { '_', '.', '-', '/' };
            string[] words = text.Split(delimiterChars);
            Boolean isOk = true;
            do
            {
                int length = words.Length;
                if (length > 3 || length < 1)
                {
                    isOk = false;
                    break;
                }

                // Text dạng YYYY
                if (length == 1)
                {
                    if (!CheckYear(words[0]))
                    {
                        isOk = false;
                        break;
                    }
                }
                else if (length == 2) // Text dạng MM/YYYY
                {
                    if (!CheckMonth(words[0]) || !CheckYear(words[1]))
                    {
                        isOk = false;
                        break;
                    }
                }
                else // Text dạng DD/MM/YYYY
                {
                    if (!CheckDayOfMonth(words[0]) || !CheckMonth(words[1]) || !CheckYear(words[2]))
                    {
                        try
                        {
                            DateTime dt = DateTime.ParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            string ms = ex.Message;
                            isOk = false;
                        }
                    }
                }

            } while (false);

            if (!isOk)
            {
                return false;
            }
            return true;
        }

        public static long GetTimestampNow()
        {
            DateTime start = DateTime.Now;
            long timest = ((DateTimeOffset)start).ToUnixTimeSeconds();
            return timest;
        }

        public static long GetTimestamp(DateTime dt)
        {
            long timest = ((DateTimeOffset)dt).ToUnixTimeSeconds();
            return timest;
        }

        public static DateTime GetDateFromTimestamp(long timest)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timest);
            DateTime dateTime = dateTimeOffset.LocalDateTime;
            return dateTime;
        }

        // input ex: "2025-08-14 13:40:52 +0700"
        public static DateTime LazadaParseCustomDateTime(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return GetDefaultDateTime();
            }

            // nếu offset ở cuối có dạng +0700 thì thêm dấu ":" -> +07:00
            if (input.Length >= 5 && 
                (input[input.Length - 5] == '+' || input[input.Length - 5] == '-'))
            {
                input = input.Insert(input.Length - 2, ":");
            }

            string format = "yyyy-MM-dd HH:mm:ss zzz";

            if (DateTimeOffset.TryParseExact(input, format,
                                             CultureInfo.InvariantCulture,
                                             DateTimeStyles.None,
                                             out DateTimeOffset dto))
            {
                return dto.DateTime;   // trả về theo local time
            }

            return GetDefaultDateTime(); ;
        }

        public static string GetTimeNowddMMyyyy()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string GetTimeNowyyyyMMddHHmmss(DateTime dt)
        {
            return dt.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        // "2022-01-13 02:59:59" => 2022-01-13%2002%3A59%3A59
        public static string EncodeDatetimeTiki(string str)
        {
            return str.Replace(" ", "%20").Replace(":", "%3A");
        }

        // Trả giá trị mặc định là ngày sinh Sâu béo
        public static DateTime GetDefaultDateTime()
        {
            return new DateTime(2018, 8, 5);
        }

        public static DateTime ConvertStringToDateTime(string str)
        {
            // Trả giá trị mặc định là ngày sinh Sâu béo
            if (string.IsNullOrEmpty(str))
                return GetDefaultDateTime();

            return DateTime.ParseExact(str, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        #endregion

        #region Shopee
        #endregion

        #region Giảm kích thước ảnh về 320
        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        static private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path"> Path to which the image would be saved. </param> 
        /// <param name="quality"> An integer from 0 to 100, with 100 being the highest quality. </param> 
        static void SaveJpeg(string path, System.Drawing.Image img, int quality)
        {
            try
            {
                // Encoder parameter for image quality 
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                // JPEG image codec 
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;
                img.Save(path, jpegCodec, encoderParams);
                encoderParams.Dispose();
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn("SaveJpeg called with: " + path);
                MyLogger.GetInstance().Warn(ex.Message);
            }
        }

        static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {
            try
            {
                int sourceWidth = imgToResize.Width;
                int sourceHeight = imgToResize.Height;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeight);

                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap b = new Bitmap(destWidth, destHeight);
                using (Graphics g = Graphics.FromImage((System.Drawing.Image)b))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                }
                return (System.Drawing.Image)b;
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn("ResizeImage called");
                MyLogger.GetInstance().Warn(ex.Message);
            }
            return null;
        }

        // Từ thư mục sản phẩm gốc, sinh ra và lưu ảnh 320
        public static Boolean ReduceImageSizeFromFolder(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (var f in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                {
                    if(!ReduceImageSizeTo320AndSave(f))
                    {
                        Thread.Sleep(2000);
                        // Thử lại 1 lần
                        if(!ReduceImageSizeTo320AndSave(f))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Giảm kích thước ảnh về 320 và lưu vào thư mục tương ứng có thêm _320 VD: 570_320
        /// </summary>
        /// <param name="path">Đường dẫn và tên file</param>
        public static Boolean ReduceImageSizeTo320AndSave(string path)
        {
            try
            {
                System.Drawing.Image myImage = null;
                System.Drawing.Image newImage = null;

                System.Drawing.Size size = new System.Drawing.Size(320, 320);
                string path320 = Get320PathFromOriginal(path, true);

                // First load the image somehow
                using (myImage = System.Drawing.Image.FromFile(path, true))
                {
                    using (newImage = ResizeImage(myImage, size))
                    {
                        if (newImage != null)
                        {
                            if (Path.GetExtension(path).ToLower() == ".png" ||
                                Path.GetExtension(path).ToLower() == ".jpg")
                            {
                                newImage.Save(path320);
                            }
                            else
                            {
                                SaveJpeg(path320, newImage, 100);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Info("ReduceImageSizeAndSave Call with path: " + path);
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Có những ảnh kích thước lớn, 12MB, up ở Lazada sẽ báo lỗi. Ta giảm kích thước về khoảng nhỏ hơn 5MB
        /// Xóa ảnh lớn và lưu ảnh mới trùng tên.
        /// </summary>
        /// <param name="path">Đường dẫn và tên file</param>
        public static Boolean ReduceImageSizeToMediumAndSave(string path, double maximumMB)
        {
            try
            {
                do
                {
                    FileInfo fi = new FileInfo(path);
                    double sizeInMB = fi.Length / (1024.0 * 1024.0);
                    if (sizeInMB < maximumMB)
                    {
                        return true;
                    }

                    System.Drawing.Image myImage = null;
                    System.Drawing.Image newImage = null;

                    string tempFile = path + ".tmp";

                    // First load the image somehow
                    using (myImage = System.Drawing.Image.FromFile(path, true))
                    {
                        int divide = 2;
                        System.Drawing.Size size = new System.Drawing.Size(myImage.Width / divide,
                            myImage.Height / divide);

                        using (newImage = ResizeImage(myImage, size))
                        {
                            if (newImage != null)
                            {
                                if (Path.GetExtension(path).ToLower() == ".png" ||
                                    Path.GetExtension(path).ToLower() == ".jpg")
                                {
                                    newImage.Save(tempFile);
                                }
                                else
                                {
                                    SaveJpeg(tempFile, newImage, 100);
                                }
                            }
                        }
                    }

                    // --- Xóa file cũ và thay bằng file mới ---
                    File.Delete(path);
                    File.Move(tempFile, path);
                    Thread.Sleep(100);
                }
                while (true);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info("ReduceImageSizeAndSave Call with path: " + path);
                MyLogger.GetInstance().Warn(ex.Message);
                return false;
            }

            return true;
        }
        #endregion

        #region Add water mark logo voi bé nhỏ vào giữa ảnh

        private static BitmapSource ToBitmapSource(DrawingImage source)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(source, new Rect(new System.Windows.Point(0, 0), new System.Windows.Size(source.Width, source.Height)));
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }

        /// <summary>
        /// Thêm watermark logo voi bé nhỏ vào ảnh, xóa ảnh cũ thay bằng ảnh mới
        /// </summary>
        /// <param name="inputPathFull"> Đường dẫn cả tên file ảnh đầu vào</param>
        public static string AddWatermark_DeleteOriginalImageFunc(string inputPathFull)
        {
            string outputFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(inputPathFull);
            BitmapImage bitmapLogo = new BitmapImage(
                new Uri(System.Web.HttpContext.Current.Server.MapPath(Common.MediaFolderPath) + @"\200_200LOGO.png"));
            BitmapImage bitmapInput = new BitmapImage();// new BitmapImage(new Uri(inputPathFull, UriKind.Absolute));

            using (var stream = File.OpenRead(inputPathFull))
            {
                bitmapInput.BeginInit();
                bitmapInput.StreamSource = stream;
                bitmapInput.CacheOption = BitmapCacheOption.OnLoad;
                bitmapInput.EndInit();
            }

            // Create a DrawingGroup to combine the ImageDrawing objects.
            DrawingGroup watermarkDrawings = new DrawingGroup();

            ImageDrawing watermark = new ImageDrawing();
            watermark.ImageSource = bitmapLogo;
            watermark.Rect = new Rect((bitmapInput.PixelWidth - bitmapLogo.PixelWidth) / 2,
                                        (bitmapInput.PixelHeight - bitmapLogo.PixelHeight) / 2,
                                        bitmapLogo.PixelWidth, bitmapLogo.PixelHeight);

            watermarkDrawings.Children.Add(watermark);
            watermarkDrawings.Opacity = 0.3;

            ImageDrawing inputImage = new ImageDrawing();
            inputImage.Rect = new Rect(0, 0, bitmapInput.PixelWidth, bitmapInput.PixelHeight);
            inputImage.ImageSource = bitmapInput;

            DrawingGroup imageDrawings = new DrawingGroup();
            imageDrawings.Children.Add(inputImage);
            imageDrawings.Children.Add(watermarkDrawings);

            DrawingImage drawingImageSource = new DrawingImage(imageDrawings);

            drawingImageSource.Freeze();
            // Xóa ảnh cũ
            // Tên file ảnh mới do có thể thay đổi định dạng, thay đổi đuôi
            System.IO.File.Delete(inputPathFull);
            //string outputPathFull = string.Empty;
            string newPathFile = string.Empty;
            if (System.IO.Path.GetExtension(inputPathFull).ToLower() == ".png")
            {
                newPathFile = inputPathFull;
                var encoderPNG = new PngBitmapEncoder();
                encoderPNG.Frames.Add(BitmapFrame.Create(ToBitmapSource(drawingImageSource)));
                using (FileStream stream = new FileStream(newPathFile, FileMode.Create))
                    encoderPNG.Save(stream);
            }
            else
            {
                newPathFile = System.IO.Path.ChangeExtension(inputPathFull, ".jpg");
                var encoderJPG = new JpegBitmapEncoder();
                encoderJPG.Frames.Add(BitmapFrame.Create(ToBitmapSource(drawingImageSource)));
                using (FileStream stream = new FileStream(newPathFile, FileMode.Create))
                    encoderJPG.Save(stream);
            }
            return newPathFile;
        }

        //// Thêm logo voi bé nhỏ, xóa ảnh gốc, và thêm phiên bản thu nhỏ 320
        //// Hàm chỉ gọi 1 lần trong đời do thư mục ảnh chưa được add logo, thêm phiên bản thu nhỏ 320
        //static public void AddWatermark_DeleteOriginalImageFunc_ReduceSize_Folder(string folderPath)
        //{
        //    try
        //    {
        //        List<string> ls = new List<string>();
        //        GetAllMediaFilesIncludeSubfolderDontSort(ls,
        //            folderPath,
        //            1);
        //        foreach (var fi in ls)
        //        {
        //            AddWatermark_DeleteOriginalImageFunc(fi);
        //            ReduceImageSizeAndSave(fi);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        MyLogger.GetInstance().Debug(ex.Message);
        //    }

        //}
        #endregion

        #region Encode/Decode base64
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        // Thay thế 3 hoặc nhiều dấu xuống dòng liên tiếp bằng 2 dấu xuống dòng
        public static string ReplaceMoreNewLineCharacterByTwo(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. Chuẩn hóa xuống dòng: chuyển tất cả về \n
            input = input.Replace("\r\n", "\n").Replace("\r", "\n");

            // 2. Rút gọn các dòng trống: thay 3+ dòng trống bằng 2 dòng trống
            string result = Regex.Replace(input, @"\n{3,}", "\n\n");

            return result.Trim(); // bỏ khoảng trắng đầu/cuối nếu có
        }

        // Thay thế 3 hoặc nhiều dấu xuống dòng liên tiếp bằng 1 dấu xuống dòng
        public static string ReplaceMoreNewLineCharacterByOne(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. Chuẩn hóa xuống dòng: chuyển tất cả về \n
            input = input.Replace("\r\n", "\n").Replace("\r", "\n");

            // 2. Rút gọn các dòng trống: thay 2+ dòng trống bằng 1 dòng trống
            string result = Regex.Replace(input, @"\n{2,}", "\n");

            return result.Trim(); // bỏ khoảng trắng đầu/cuối nếu có
        }

        // Xóa tag html<>, "&nbsp;", "&nbsp" nhiều ký tự xuống dòng giữ lại 2
        public static string CleanHtml_ByTwo(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. Xóa tất cả thẻ HTML
            string noTags = Regex.Replace(input, "<.*?>", "");
            // 2. Xóa các dạng &nbsp;, &nbsp (không phân biệt chữ hoa thường)
            string noNbsp = Regex.Replace(noTags, @"&nbsp;?", "", RegexOptions.IgnoreCase);

            return ReplaceMoreNewLineCharacterByTwo(noNbsp); // bỏ khoảng trắng đầu/cuối nếu có
        }

        // Xóa tag html<>, "&nbsp;", "&nbsp" nhiều ký tự xuống dòng giữ lại chỉ 1
        public static string CleanHtml_ByOne(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. Xóa tất cả thẻ HTML
            string noTags = Regex.Replace(input, "<.*?>", "");
            // 2. Xóa các dạng &nbsp;, &nbsp (không phân biệt chữ hoa thường)
            string noNbsp = Regex.Replace(noTags, @"&nbsp;?", "", RegexOptions.IgnoreCase);

            return ReplaceMoreNewLineCharacterByOne(noNbsp); // bỏ khoảng trắng đầu/cuối nếu có
        }
    }
}