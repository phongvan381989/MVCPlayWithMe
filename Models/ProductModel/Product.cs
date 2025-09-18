using MVCPlayWithMe.General;
using System;
using System.Collections.Generic;
using System.IO;
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


        // Độ dài mô tả không quá 5000 ký tự, và không ngắn hơn 100 ký tự
        public static int minLengthDetail = 100;
        public static int maxLengthDetail = 5000;
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
            imageSrc = new List<string>();
            videoSrc = new List<string>();
        }

        public Product(int idIn, string nameIn)
        {
            id = idIn;
            name = nameIn;
            imageSrc = new List<string>();
            videoSrc = new List<string>();
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

        // Sinh tên tự động từ tên combo nếu có, tên sản phẩm để đăng lên sàn
        // Nếu sản phẩm là sách dựa vào category thì thêm chữ "Sách" ở đầu tên
        static public string GenerateName(Product product)
        {
            // Tên đăng gồm: Sách Tên combo "-" tên sản phẩm.
            string name = product.name;
            if (!string.IsNullOrEmpty(product.comboName))
            {
                name = product.comboName + " - " + product.name;
            }
            // Bỏ chữ combo ở đầu tên nếu có
            // Kiểm tra nếu chuỗi bắt đầu bằng "combo" (không phân biệt hoa thường)
            if (name.TrimStart().StartsWith("combo", StringComparison.OrdinalIgnoreCase))
            {
                // Bỏ từ "combo" ở đầu và loại bỏ khoảng trắng
                name = name.TrimStart().Substring(5).Trim();
            }
            else
            {
                // Loại bỏ khoảng trắng nếu không có "combo"
                name = name.Trim();
            }

            if (product.categoryName.StartsWith("sách", StringComparison.OrdinalIgnoreCase))
            {
                // Nếu tên có chữ sách ở đầu rồi thì thôi, không thêm vào.
                if (!name.StartsWith("sách", StringComparison.OrdinalIgnoreCase))
                {
                    name = "Sách " + name;
                }
            }

            return name;
        }

        // Sinh tên tự động từ tên combo nếu có, tên sản phẩm để đăng lên sàn
        static public string ShopeeGenerateNameForBook(Product product)
        {
            // Tên đăng gồm: Sách + tên sản phẩm.
            string name = product.name.Trim();

            if (product.categoryName.StartsWith("sách", StringComparison.OrdinalIgnoreCase))
            {
                // Nếu tên có chữ sách ở đầu rồi thì thôi, không thêm vào.
                if (!name.StartsWith("sách", StringComparison.OrdinalIgnoreCase))
                {
                    // Nếu combo có chữ ehon  thì thêm
                    if (product.comboName.IndexOf("ehon", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        name = "Sách Ehon " + name;
                    }
                    else
                    {
                        name = "Sách " + name;
                    }
                }
            }

            return name;
        }

        /// <summary>
        /// Từ id sản phẩm, lấy được đường dẫn tuyệt đối của ảnh sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<string> GetProductImageAbsolutePath(string productId)
        {
            List<string> src = new List<string>();

            string path = Common.absoluteProductMediaFolderPath + productId + @"/";
            if (!Directory.Exists(path))
            {
                return src;
            }

            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (Common.ImageExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    src.Add(file);
                }
            }

            Common.SortSourceFile(src);
            return src;
        }
    }
}