using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Model.Dev.ShopeeApp.ShopeeCreateProduct
{
    public class ShopeeImageUrlList
    {
        // Region of image url
        public string image_url_region { get; set; }

        // image url
        public string image_url { get; set; }
    }

    public class ShopeeImageInfo
    {
        // Id of image
        public string image_id { get; set; }

        // Image URL of each region
        public List<ShopeeImageUrlList> image_url_list { get; set; }
    }

    public class ShopeeImageInfoList
    {
        // the index of images
        public int id { get; set; }

        // Indicate error type if this index's image upload processing hit error. 
        // Empty if no error happened for this index's image .
        public string error { get; set; }

        // Indicate error detail if this index's image upload processing hit error.
        // Empty if no error happened for this index's image .
        public string message { get; set; }

        public ShopeeImageInfo image_info{get;set;}
    }

    public class ShopeeUploadImageResponse
    {
        public ShopeeImageInfo image_info { get; set; }
        public List<ShopeeImageInfoList> image_info_list { get; set; }
    }

    public class ShopeeUploadImageResponseHTTP : CommonResponseHTTP
    {
        public ShopeeUploadImageResponse response { get; set; }
    }
}
