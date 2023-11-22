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

namespace MVCPlayWithMe.General
{
    public class Common
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".apng", ".avif", ".gif", ".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png", ".svg", ".webp" };
        public static readonly List<string> VideoExtensions = new List<string> { ".mp4", ".webm", ".ogg" };
        public static readonly string dateFormat = "yyyy-MM-dd";
        public static readonly int quota = 5;
        public static readonly string srcNoImageThumbnail = "/Media/NoImageThumbnail.png";
        public static readonly int offset = 20;

        // Cookie const
        #region Cookie
        public static readonly string userIdKey = "uId";
        // cookie có dạng: cart=id=123#q=10#real=1$id=321#q=1#real=0$....$id=321#q=2#real=0
        // id: mã model, q: số lượng thêm vào giỏ hàng, real: 1-thực sự chọn mua, 0-có thể mua sau này
        public static readonly string cartKey = "cart";
        #endregion

        /// <summary>
        /// Đường dẫn thư mục chứa file ảnh
        /// </summary>
        public static string ProductMediaFolderPath;
        public static string ItemMediaFolderPath;
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
            string path = System.Web.HttpContext.Current.Server.MapPath(ProductMediaFolderPath) + productId + @"/";
            MyLogger.GetInstance().Info(path);
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
                path = null;
            }
            return path;
        }

        public static string CreateAbsoluteProductMediaFolderPath(string productId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ProductMediaFolderPath) + productId + @"/";
            Directory.CreateDirectory(path);
            return path;
        }

        /// </summary>
        /// <param name="productId"></param>
        /// <summary>
        /// Lấy tất cả file của sản phẩm
        /// <returns></returns>
        static private string[] GetAllAbsoluteFileNameOfProduct(string productId)
        {
            string path = GetAbsoluteProductMediaFolderPath(productId);
            if (path == null)
                return new string[0] ;

            return Directory.GetFiles(path);
        }

        /// <summary>
        /// Từ id sản phẩm, lấy được đường dẫn tới ảnh dùng cho thẻ img
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<string> GetImageSrc(string productId)
        {
            List<string> src = new List<string>();
            string[] files = GetAllAbsoluteFileNameOfProduct(productId);

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

        /// <summary>
        /// Từ tên chuyển đổi sang số int
        /// </summary>
        /// <param name="fullFileName">Gồm đường dẫn</param>
        /// <returns></returns>
        private static int ConvertNameToInt(string fullFileName)
        {
            int intName = -1;
            intName = Common.ConvertStringToInt32(Path.GetFileNameWithoutExtension(fullFileName));
            return intName;
        }

        /// <summary>
        /// Sắp xếp tên file theo thứ tự tăng dần trực tiếp trên tham số
        /// </summary>
        /// <param name="src"> Mảng tên file gồm cả đường dẫn</param>
        /// <returns></returns>
        private static void SortSourceFile(List<string> src)
        {
            IDictionary<int, string> dicSrc = new Dictionary<int, string>();
            foreach(var file in src)
            {
                dicSrc.Add(ConvertNameToInt(file), file);
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
        public static List<string> GetListThumbnailImageSrd(List<int> lsProductId)
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
        public static List<string> GetVideoSrc(string productId)
        {
            List<string> src = new List<string>();
            string[] files = GetAllAbsoluteFileNameOfProduct(productId);

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
            MyLogger.GetInstance().Info("maxName: " + maxName);
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
            MyLogger.GetInstance().Info("maxName: " + maxName);
            return maxName;
        }

        /// <summary>
        /// Xóa file ảnh hoặc video có tên không kể đuôi giống tên file trong tham số
        /// </summary>
        /// <param name="path">tên gồm đường dẫn</param>
        public static void DeleteImageVideoWithoutExtension(string path)
        {
            MyLogger.GetInstance().Info(" Start DeleteImageVideoWithoutExtension: " + path);
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
                        MyLogger.GetInstance().Info("Delete: " + f);
                        break;
                    }
                }
                else
                {
                    if (VideoExtensions.Contains(Path.GetExtension(f).ToLower()))
                    {
                        System.IO.File.Delete(f);
                        MyLogger.GetInstance().Info("Delete: " + f);
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

        public static void DeleteAllImage(string path, string productId)
        {
            string[] files =  Directory.GetFiles(path);

            foreach (var f in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                {
                    System.IO.File.Delete(f);
                }
            }
            return;
        }

        public static void DeleteAllVideo(string path, string productId)
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

        public static Int32 ConvertStringToInt32(string str)
        {
            int rs;
            try
            {
                rs = Int32.Parse(str);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
                rs = System.Int32.MinValue;
            }

            return rs;
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
            string path = System.Web.HttpContext.Current.Server.MapPath(ItemMediaFolderPath) + itemId.ToString() + @"/";
            MyLogger.GetInstance().Info(path);
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
                path = null;
            }
            return path;
        }


        public static string CreateAbsoluteItemMediaFolderPath(int itemId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ItemMediaFolderPath) + itemId.ToString() + @"/";
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Lấy được đường dẫn tuyệt đối thư mục chứa media của item hoặc trả về null nếu không có.
        /// </summary>
        /// <returns></returns>
        public static string GetAbsoluteModelMediaFolderPath(int itemId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ItemMediaFolderPath) + itemId.ToString() + @"/Model/";
            MyLogger.GetInstance().Info(path);
            if (!Directory.Exists(path))
            {
                path = null;
            }
            return path;
        }

        public static string CreateAbsoluteModelMediaFolderPath(int itemId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ItemMediaFolderPath) + itemId.ToString() + @"/Model/";
            Directory.CreateDirectory(path);
            return path;
        }

        // Lấy tất cả file của item
        static private string[] GetAllFileNameOfItem(int itemId)
        {
            string path = GetAbsoluteItemMediaFolderPath(itemId);
            if (path == null)
                return new string[0];

            return Directory.GetFiles(path);
        }

        /// <summary>
        /// Từ id item, lấy được đường dẫn tới ảnh dùng cho thẻ img
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static List<string> GetItemImageSrc(int itemId)
        {
            List<string> src = new List<string>();
            string[] files = GetAllFileNameOfItem(itemId);

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

        /// <summary>
        /// Từ id item, lấy được đường dẫn tới video dùng cho thẻ video
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetItemVideoSrc(int itemId)
        {
            string src= "";
            string[] files = GetAllFileNameOfItem(itemId);

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
    }
}