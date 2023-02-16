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
        /// <summary>
        /// Đường dẫn thư mục chứa file ảnh
        /// </summary>
        public static string MediaFolderPath;// = ConfigurationManager.AppSettings["MediaFolderPath"];
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

        //public static DateTime ConvertFromMysqlDate(string date)
        //{

        //}

        /// <summary>
        /// Từ id sản phẩm, lấy được đường dẫn tới ảnh dùng cho thẻ img
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<string> GetImageSrc(string productId)
        {
            string[] dirs = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath("/Media"), productId + "_*");

            List<string> src = new  List<string>();
            foreach(var dir in dirs)
            {
                if (ImageExtensions.Contains(Path.GetExtension(dir).ToLower()))
                {
                    src.Add(MediaFolderPath + Path.GetFileName(dir));
                }
            }
            return src;
        }

        /// <summary>
        /// Từ id sản phẩm, lấy được đường dẫn tới ảnh dùng cho thẻ video
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static string GetVideoSrc(string productId)
        {
            string[] dirs = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath("/Media"), productId + "_*");

            foreach (var dir in dirs)
            {
                if (VideoExtensions.Contains(Path.GetExtension(dir).ToLower()))
                {
                    return MediaFolderPath + Path.GetFileName(dir);
                }
            }
            return string.Empty;
        }
    }
}