using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ItemModel;
using System;
using System.Collections.Generic;

namespace MVCPlayWithMe.Models
{
    public class Item
    {
        public int id { get; set; }

        // item id của sản phẩm trên sàn shopee tương ứng
        public long shopeeItemId { get; set; }

        public string name { get; set; }

        /// <summary>
        /// 0: Bán bình thường, 1: Ngừng kinh doanh, 2: Hết hàng
        /// </summary>
        public int status { get; set; }

        public string detail { get; set; }

        public int quota { get; set; }

        public DateTime date { get; set; }

        /// <summary>
        /// mặc định là 0, chưa xét thể loại
        /// </summary>
        public int categoryId { get; set; }

        public List<Model> models { get; set; }

        public List<string> imageSrc { get; set; }

        public string videoSrc { get; set; }

        public Item()
        {
            id = -1;
            quota = Common.quota;
            models = new List<Model>();
            imageSrc = new List<string>();
        }

        public Item(string inName,
            int inStatus,
            int inQuota,
            string inDetail,
            int inCategoryId
            )
        {
            id = -1;
            name = inName;
            status = inStatus;
            quota = inQuota;
            detail = inDetail;
            categoryId = inCategoryId;
            models = new List<Model>();
            imageSrc = new List<string>();
        }

        public Item(int inId, 
            string inName,
            int inStatus,
            int inQuota,
            string inDetail,
            int inCategoryId
            )
        {
            id = inId;
            name = inName;
            status = inStatus;
            quota = inQuota;
            detail = inDetail;
            categoryId = inCategoryId;
            models = new List<Model>();
            imageSrc = new List<string>();
        }

        public void SetSrcImageVideo()
        {
            imageSrc = Common.GetItemImageSrc(id);
            videoSrc = Common.GetItemVideoSrc(id);
        }

        // Lấy ảnh đầu tiên của imageSrc cho nhanh
        public void SetFirstSrcImage()
        {
            string src = Common.GetFirstItemImageSrc(id);
            if (!string.IsNullOrEmpty(src))
                imageSrc.Add(src);
            else
            {
                imageSrc.Add(Common.srcNoImageThumbnail);
            }
        }

        public void SetShopeeItemId()
        {
            ItemModelMySql sqler = new ItemModelMySql();
            if (models.Count > 0)
            {
                MySqlResultState result = sqler.GetTMDTShopeeItemIdFromModelId(models[0].id);
                if (result.State == EMySqlResultState.OK)
                {
                    shopeeItemId = result.myAnythingLong;
                }
                else
                {
                    shopeeItemId = 0;
                }
            }
        }
    }
}