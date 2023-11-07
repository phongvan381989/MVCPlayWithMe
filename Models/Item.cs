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

        /// <summary>
        /// Số lượng tổng model sản phẩm đã được bán
        /// </summary>
        public int sumQuantitySold { get; set; }

        // Model có giá bìa rẻ nhất, để hiển thị khi khách hàng duyệt web
        public Model cheapestModel { get; set; }

        // Model có giá bìa đắt nhất
        public Model mostExpensiveModel { get; set; }

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
        }

        public void SetSrcImageVideo()
        {
            imageSrc = Common.GetItemImageSrc(id);
            videoSrc = Common.GetItemVideoSrc(id);
        }

        // Từ danh sách model tính được tổng số lượng model đã bán, model rẻ nhất, model đắt nhất
        public void SetPriceAndQuantity()
        {
            cheapestModel = null;
            mostExpensiveModel = null;
            sumQuantitySold = 0;
            int price = 0;
            foreach (var model in models)
            {
                price = 0;
                sumQuantitySold = sumQuantitySold + model.quantitySold;
                foreach (var pro in model.mapping)
                {
                    price = price + pro.bookCoverPrice;
                }
                // Model giá bìa rẻ nhất
                if (cheapestModel == null)
                {
                    cheapestModel = model;
                }
                else
                {
                    if (cheapestModel.bookCoverPrice > price)
                    {
                        cheapestModel = model;
                    }
                }
                // Model giá bìa đắt nhất
                if (mostExpensiveModel == null)
                {
                    mostExpensiveModel = model;
                }
                else
                {
                    if (mostExpensiveModel.bookCoverPrice < price)
                    {
                        mostExpensiveModel = model;
                    }
                }
            }
        }
    }
}