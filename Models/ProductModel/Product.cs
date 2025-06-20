using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ProductModel
{
    public class Product : ProductCommonInfoWithCombo
    {
        // id trong tbProducts
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

        // Ảnh đầu tiên tên 0.* sẽ là ảnh dùng làm avartar
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
                int quantityIn,
                string codeIn,
                string barcodeIn,
                string nameIn,
                int comboIdIn,
                int categoryIdIn,
                int bookCoverPriceIn,
                float discountIn,
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
                string bookLangugeIn,
                int minAgeIn,
                int maxAgeIn,
                int parentIdIn,
                int republishIn,
                string detailIn,
                int statusIn,
                int pageNumberIn)
                :base(
                comboIdIn,
                categoryIdIn,
                bookCoverPriceIn,
                discountIn,
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
                bookLangugeIn,
                minAgeIn,
                maxAgeIn,
                republishIn,
                statusIn,
                pageNumberIn
                    )
        {
            id = idIn;
            quantity = quantityIn;
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
            imageSrc = Common.GetProductImageSrc(id.ToString());
            videoSrc = Common.GetProductVideoSrc(id.ToString());
        }

        // Lấy ảnh đầu tiên của imageSrc cho nhanh
        public void SetFirstSrcImage()
        {
            string src = Common.GetFirstProductImageSrc(id.ToString());
            if (!string.IsNullOrEmpty(src))
                imageSrc.Add(src);
        }
    }
}