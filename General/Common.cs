using MVCPlayWithMe.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
        /// Lấy được thư mục chứa media của sản phẩm hoặc trả về null nếu không có.
        /// </summary>
        /// <returns></returns>
        public static string GetProductMediaFolderPath(string productId)
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

        public static string CreateProductMediaFolderPath(string productId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ProductMediaFolderPath) + productId + @"/";
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Lấy tất cả file của sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        static private string[] GetAllFileOfProduct(string productId)
        {
            string path = GetProductMediaFolderPath(productId);
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
            string[] dirs = GetAllFileOfProduct(productId);

            string relPath = ProductMediaFolderPath + productId + @"/";
            foreach (var dir in dirs)
            {
                if (ImageExtensions.Contains(Path.GetExtension(dir).ToLower()))
                {
                    src.Add(relPath + Path.GetFileName(dir));
                }
            }
            return src;
        }

        /// <summary>
        /// Từ id sản phẩm, lấy được đường dẫn tới ảnh dùng cho thẻ video
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<string> GetVideoSrc(string productId)
        {
            List<string> src = new List<string>();
            string[] dirs = GetAllFileOfProduct(productId);

            string relPath = ProductMediaFolderPath + productId + @"/";
            foreach (var dir in dirs)
            {
                if (VideoExtensions.Contains(Path.GetExtension(dir).ToLower()))
                {
                    src.Add(relPath + Path.GetFileName(dir));
                }
            }
            return src;
        }

        /// <summary>
        /// Xóa file ảnh hoặc video có tên không kể đuôi giống tên file trong tham số
        /// </summary>
        /// <param name="path">tên gồm đường dẫn</param>
        public static void DeleteImageVideoWithoutExtension(string path)
        {
            //// Check tên file là image hoặc video. Có vẻ hơi thừa
            Boolean isImage = ImageExtensions.Contains(Path.GetExtension(path).ToLower());
            Boolean isVideo = VideoExtensions.Contains(Path.GetExtension(path).ToLower());
            //if (!isImage & ! isVideo)
            //    return;

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
                        break;
                    }
                }
                if (isVideo)
                {
                    if (VideoExtensions.Contains(Path.GetExtension(f).ToLower()))
                    {
                        System.IO.File.Delete(f);
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
            // Lấy chỉ tên file
            string fileName = Path.GetFileName(path);
            // Lấy chỉ tên file
            string onlyName = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            int intName = Common.ConvertStringToInt32(onlyName);
            if (intName != int.MinValue)
            {
                // Bắt đầu xóa file có tên lớn hơn intName
                int countBreak = 0;
                while (true)
                {
                    intName++;
                    string[] files = Directory.GetFiles(Path.GetDirectoryName(path), intName.ToString() + ".*");
                    if (files.Length == 0)
                    {
                        if(countBreak == 20)
                        {
                            countBreak ++;
                            continue;
                        }
                        break;
                    }
                    string fileNameTemp = intName.ToString() + extension;
                    DeleteImageVideoWithoutExtension(Path.GetDirectoryName(path) + @"/" + fileNameTemp);
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

        #region Xử chung Item/model
        /// <summary>
        /// Lấy được thư mục chứa media của item hoặc trả về null nếu không có.
        /// </summary>
        /// <returns></returns>
        public static string GetItemMediaFolderPath(int itemId)
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

        public static string CreateItemMediaFolderPath(int itemId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ItemMediaFolderPath) + itemId.ToString() + @"/";
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Lấy được thư mục chứa media của item hoặc trả về null nếu không có.
        /// </summary>
        /// <returns></returns>
        public static string GetModelMediaFolderPath(int itemId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ItemMediaFolderPath) + itemId.ToString() + @"/Model/";
            MyLogger.GetInstance().Info(path);
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
                path = null;
            }
            return path;
        }

        public static string CreateModelMediaFolderPath(int itemId)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(ItemMediaFolderPath) + itemId.ToString() + @"/Model/";
            Directory.CreateDirectory(path);
            return path;
        }
        #endregion
    }
}