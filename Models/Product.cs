using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class Product : ProductCommonInfoWithCombo
    {
        public int id { get; set; }
        public string code { get; set; }
        public string barcode { get; set; }
        public string name { get; set; }
        /// <summary>
        /// Mục đích là để chỉ về cùng 1 sản phẩm. Ví dụ: 1 cuốn sách nhưng nhiều phiên bản cập nhật
        /// </summary>
        public int parentId { get; set; }
        public string parentName { get; set; }
        public string detail { get; set; }

        // Số lượng hàng tồn kho, giá trị này được cập nhật khi có thông tin nhập / xuất kho
        public int quantity { get; set; }

        public List<string> imageSrc { get; set; }
        public List<string> videoSrc { get; set; }

        public Product() :base()
        {
            id = -1;
            imageSrc = new List<string>();
            videoSrc = new List<string>();
        }

        public Product(int idIn)
        {
            id = idIn;
        }

        public Product(
                int idIn,
                string codeIn,
                string barcodeIn,
                string nameIn,
                int comboIdIn,
                int categoryIdIn,
                int bookCoverPriceIn,
                string authorIn,
                string translatorIn,
                int publisherIdIn,
                string publishingCompanyIn,
                int publishingTimeIn,
                int productLongIn,
                int productWideIn,
                int productHighIn,
                int productWeightIn,
                string positionInWarehouseIn,
                int hardCoverIn,
                int minAgeIn,
                int maxAgeIn,
                int parentIdIn,
                int republishIn,
                string detailIn,
                int statusIn)
                :base(
                comboIdIn,
                categoryIdIn,
                bookCoverPriceIn,
                authorIn,
                translatorIn,
                publisherIdIn,
                publishingCompanyIn,
                publishingTimeIn,
                productLongIn,
                productWideIn,
                productHighIn,
                productWeightIn,
                positionInWarehouseIn,
                hardCoverIn,
                minAgeIn,
                maxAgeIn,
                republishIn,
                statusIn
                    )
        {
            id = idIn;
            code = codeIn;
            barcode = barcodeIn;
            name = nameIn;
            parentId = parentIdIn;
            detail = detailIn;
            imageSrc = new List<string>();
            videoSrc = new List<string>();
        }

        public void SetSrcImageVideo()
        {
            imageSrc = Common.GetImageSrc(id.ToString());
            videoSrc = Common.GetVideoSrc(id.ToString());
        }
    }
}