using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Product
    {
        public int id { get; set; }
        public string barcode { get; set; }
        public string productName { get; set; }
        public string comboName { get; set; }
        public int bookCoverPrice { get; set; }
        public string author { get; set; }
        public string translator { get; set; }
        public int publisherId { get; set; }
        public string publishingCompany { get; set; }
        public DateTime publishingTime { get; set; }
        /// <summary>
        /// publishingTimeyyyyMMdd định dạng string yyy-MM-dd của publishingTime
        /// </summary>
        public string publishingTimeyyyyMMdd { get; set; }
        public int productLong { get; set; }
        public int productWide { get; set; }
        public int productHigh { get; set; }
        public int productWeight { get; set; }
        public string positionInWarehouse { get; set; }
        public string detail { get; set; }
        public string category { get; set; }
        public List<string> imageSrc { get; set; }
        public string videoSrc { get; set; }

        public void SetSrcImageVideo()
        {
            imageSrc = Common.GetImageSrc(id.ToString());
            videoSrc = Common.GetVideoSrc(id.ToString());
        }
    }
}