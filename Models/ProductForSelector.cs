using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    /// <summary>
    /// Thông tin sản phẩm phục vụ hiển thị cho khách chọn
    /// </summary>
    public class ProductForSelector
    {
        /// <summary>
        /// Giá trên bao bì
        /// </summary>
        public int priceOnPackage { get; set; }
        public string strPriceOnPackage { get; set; }

        /// <summary>
        /// Giá nhập từ nhà cung cấp
        /// </summary>
        public int priceFromSupplier { get; set; }

        /// <summary>
        /// Gía bán chưa bao gồm bất kì khuyến mại trực tiếp trên sản phẩm
        /// </summary>
        public int priceDontIncludePromotions { get; set; }

        /// <summary>
        /// Giá bán thực tế, đã bao gồm các khuyến mại trực tiếp trên sản phẩm
        /// </summary>
        public int priceIncludePromotion { get; set; }
        public string strPriceIncludePromotion { get; set; }

        /// <summary>
        /// % chiết khấu giá bán thực tế, đã bao gồm các khuyến mại trực tiếp trên sản phẩm
        /// </summary>
        public int discountForBuyer { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Đường dẫn tới ảnh nhỏ làm đại diện cho sản phẩm
        /// </summary>
        public string imageIcon { get; set; }

        /// <summary>
        /// Số lượng sản phẩm đã được bán
        /// </summary>
        public int quantitySold { get; set; }

        public ProductForSelector(string iName, 
            string iImageIcon,
            int iQuantitySold,
            int iPriceOnPackage,
            int iPriceFromSupplier,
            int iPriceIncludePromotion
            )
        {
            name = iName;
            imageIcon = Common.ProductMediaFolderPath + iImageIcon;
            quantitySold = iQuantitySold;
            priceOnPackage = iPriceOnPackage;
            strPriceOnPackage = Common.ConvertIntToVNDFormat(priceOnPackage);

            priceFromSupplier = iPriceFromSupplier;
            priceIncludePromotion = iPriceIncludePromotion;
            strPriceIncludePromotion = Common.ConvertIntToVNDFormat(priceIncludePromotion);

            discountForBuyer = 100 - priceIncludePromotion * 100 / priceOnPackage;
        }
    }
}