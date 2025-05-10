using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ProductModel
{
    public class ProductCommonInfoWithCombo : BasicObject
    {
        public int comboId { get; set; }
        public string comboName { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public int bookCoverPrice { get; set; }
        public float discount { get; set; }
        public string author { get; set; }
        public string translator { get; set; }
        public int publisherId { get; set; }
        public string publisherName { get; set; }
        public string publishingCompany { get; set; }
        public int publishingTime { get; set; }
        ///// <summary>
        ///// publishingTimeyyyyMMdd định dạng string yyyy-MM-dd của publishingTime
        ///// </summary>
        //public string publishingTimeyyyyMMdd { get; set; }
        public int productLong { get; set; }
        public int productWide { get; set; }
        public int productHigh { get; set; }
        public int productWeight { get; set; }
        public string positionInWarehouse { get; set; }

        /// <summary>
        /// Bìa cứng: 1, Bìa mềm: 0
        /// </summary>
        public int hardCover { get; set; }

        /// <summary>
        /// Tuổi nhỏ nhất nên dùng. Đơn vị tháng
        /// </summary>
        public int minAge { get; set; }

        /// <summary>
        /// Tuổi lớn nhất nên dùng. Đơn vị tháng
        /// </summary>
        public int maxAge { get; set; }

        /// <summary>
        /// Tái bản lần thứ mấy
        /// </summary>
        public int republish { get; set; }

        /// <summary>
        /// Trạng thái sản phẩm.
        /// 0: Đang kinh doanh bình thường
        /// 1: Nhà phát hành tạm thời hết hàng,
        /// 2: Ngừng kinh doanh,
        /// </summary>
        public int status { get; set; }

        public int pageNumber { get; set; }

        public ProductCommonInfoWithCombo()
        {

        }

        public ProductCommonInfoWithCombo(
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
                int minAgeIn,
                int maxAgeIn,
                int republishIn,
                int statusIn,
                int pageNumberIn
            )
        {
            comboId = comboIdIn;
            bookCoverPrice = bookCoverPriceIn;
            discount = discountIn;
            author = authorIn;
            translator = translatorIn;
            publisherId = publisherIdIn;
            publishingCompany = publishingCompanyIn;
            publishingTime = publishingTimeIn;
            productLong = productLongIn;
            productWide = productWideIn;
            productHigh = productHighIn;
            productWeight = productWeightIn;
            positionInWarehouse = positionInWarehouseIn;
            categoryId = categoryIdIn;
            hardCover = hardCoverIn;
            minAge = minAgeIn;
            maxAge = maxAgeIn;
            republish = republishIn;
            status = statusIn;
            pageNumber = pageNumberIn;
        }
    }
}