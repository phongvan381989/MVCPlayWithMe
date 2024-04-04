using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }

        /// <summary>
        /// 0: Bán bình thường, 1: Ngừng kinh doanh, 2: Hết hàng
        /// </summary>
        public int status { get; set; }

        public string detail { get; set; }

        public int quota { get; set; }

        public DateTime date { get; set; }

        public List<Model> models { get; set; }

        public List<string> imageSrc { get; set; }

        // Phiên bản kích thước nhỏ của imageSrc
        public List<string> thumbnailSrc { get; set; }

        // Chứa ảnh đầu tiên, phiên bản nhỏ
        public string thumbnailFirst { get; set; }

        public string videoSrc { get; set; }

        public Item()
        {
            id = -1;
            quota = Common.quota;
            models = new List<Model>();
            imageSrc = new List<string>();
            thumbnailSrc = new List<string>();
        }

        public Item(string inName,
            int inStatus,
            int inQuota,
            string inDetail
            )
        {
            id = -1;
            name = inName;
            status = inStatus;
            quota = inQuota;
            detail = inDetail;
            models = new List<Model>();
            imageSrc = new List<string>();
            thumbnailSrc = new List<string>();
        }

        public Item(int inId, 
            string inName,
            int inStatus,
            int inQuota,
            string inDetail
            )
        {
            id = inId;
            name = inName;
            status = inStatus;
            quota = inQuota;
            detail = inDetail;
            models = new List<Model>();
            imageSrc = new List<string>();
            thumbnailSrc = new List<string>();
        }

        public void SetSrcImageVideo()
        {
            imageSrc = Common.GetItemImageSrc(id);
            videoSrc = Common.GetItemVideoSrc(id);
        }

        // 
        public void SetThumbnailSrc()
        {
            thumbnailSrc = Common.GetItemThumbnailSrc(id);
        }

        public void SetThumbnailFirst()
        {
            thumbnailFirst = Common.GetItemthumbnailFirst(id);
        }
    }
}