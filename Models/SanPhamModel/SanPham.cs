using System;
using System.Collections.Generic;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public class SanPham
    {
        public int Id { get; set; }

        /// <summary>
        /// Mã sản phẩm theo nhà sản xuất: 89123456
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mã ISBN của sách, mã ISBN cách nhau bởi -
        /// </summary>
        public string Barcode { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        /// <summary>
        /// Không xác định lấy giá trị mặc định -1
        /// </summary>
        public int? ComboId { get; set; }

        /// <summary>
        /// Phân loại sản phẩm ví dụ: Sách ehon, máy đọc truyện. Không xác định lấy giá trị mặc định -1
        /// </summary>
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int BookCoverPrice { get; set; }

        public string Author { get; set; }

        public string Translator { get; set; }

        /// <summary>
        /// Không xác định lấy giá trị mặc định -1
        /// </summary>
        public int? PublisherId { get; set; }
        public string PublisherName { get; set; }

        /// <summary>
        /// Cột này hợp lý hơn phải là cột Id nhà xuất bản, maping với cột Id trong bảng tb_publishing_company.
        /// Nhưng do thiết kế ban đầu lười, lỗi nên dữ nguyên là string.
        /// </summary>
        public string PublishingCompany { get; set; }

        /// <summary>
        /// Năm phát hành
        /// </summary>
        public int? PublishingTime { get; set; }

        /// <summary>
        /// Đơn vị mm
        /// </summary>
        public int ProductLong { get; set; }

        /// <summary>
        /// Đơn vị mm
        /// </summary>
        public int ProductWide { get; set; }

        /// <summary>
        /// Đơn vị mm
        /// </summary>
        public int ProductHigh { get; set; }

        /// <summary>
        /// Đơn vị gam
        /// </summary>
        public int ProductWeight { get; set; }

        public string PositionInWarehouse { get; set; }

        /// <summary>
        /// Bìa cứng: 1, Bìa mềm: 0
        /// </summary>
        public int? HardCover { get; set; }

        /// <summary>
        /// Tuổi nhỏ nhất nên dùng. Đơn vị tháng. Không giới hạn khi MinAge hoặc / và MaxAge đều = -1
        /// </summary>
        public int? MinAge { get; set; }

        /// <summary>
        /// Tuổi lớn nhất nên dùng. Đơn vị tháng. Không giới hạn khi MinAge và MaxAge đều = -1
        /// </summary>
        public int? MaxAge { get; set; }

        /// <summary>
        /// Mục đích là để chỉ về cùng 1 sản phẩm. Ví dụ: 1 cuốn sách nhưng nhiều phiên bản cập nhật
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Tái bản lần thứ mấy
        /// </summary>
        public int? Republish { get; set; }

        public string Detail { get; set; }

        /// <summary>
        /// Trạng thái sản phẩm. 0: Đang kinh doanh bình thường, 1: Nhà phát hành tạm thời hết hàng, 2: Ngừng kinh doanh
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Số lượng hàng tồn kho, giá trị này được cập nhật khi có thông tin nhập/ xuất kho
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Số trang cuốn sách
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Chiết khấu so với giá bìa khi nhập hàng, dùng để tính giá bán.
        /// Nếu không được set giá trị mặc định là 0, tính giá bán theo chiết khấu chung của nhà bán.
        /// </summary>
        public float Discount { get; set; }

        /// <summary>
        /// Giá bán thực tế đã được tính toán từ giá bìa, chiết khấu, chi phí và lợi nhuận mong muốn
        /// </summary>
        public int SalePrice { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// Ngày bắt đầu kinh doanh sản phẩm / thêm mới sản phẩm
        /// </summary>
        public DateTime? Date { get; set; }

        public int? SoldQuantity { get; set; }

        /// <summary>
        /// Đường dẫn tương đối của sản phẩm: /sanpham/ehon-moi-moi-12
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Từ khóa phục vụ SEO
        /// </summary>
        public string SEOKeyword { get; set; }

        // Constructor mặc định
        public SanPham()
        {
            // Giá trị mặc định theo DB
            ProductLong = 0;
            ProductWide = 0;
            ProductHigh = 0;
            ProductWeight = 0;
            Status = 0;
            Quantity = 0;
            Discount = 0;
            SalePrice = 0;
        }

        // Constructor đầy đủ
        public SanPham(
            string code, string barcode, string name, string shortName,
            int? comboId, int? categoryId, int bookCoverPrice,
            string author, string translator, int? publisherId,
            string publishingCompany, int? publishingTime,
            int productLong, int productWide, int productHigh, int productWeight,
            string positionInWarehouse, int? hardCover,
            int? minAge, int? maxAge, int? parentId, int? republish,
            string detail, int status, int quantity, int? pageNumber,
            float discount, int salePrice, string language, DateTime? date,
            int? soldQuantity, string url, string seoKeyword)
        {
            Code = code;
            Barcode = barcode;
            Name = name;
            ShortName = shortName;
            ComboId = comboId;
            CategoryId = categoryId;
            BookCoverPrice = bookCoverPrice;
            Author = author;
            Translator = translator;
            PublisherId = publisherId;
            PublishingCompany = publishingCompany;
            PublishingTime = publishingTime;
            ProductLong = productLong;
            ProductWide = productWide;
            ProductHigh = productHigh;
            ProductWeight = productWeight;
            PositionInWarehouse = positionInWarehouse;
            HardCover = hardCover;
            MinAge = minAge;
            MaxAge = maxAge;
            ParentId = parentId;
            Republish = republish;
            Detail = detail;
            Status = status;
            Quantity = quantity;
            PageNumber = pageNumber;
            Discount = discount;
            SalePrice = salePrice;
            Language = language;
            Date = date;
            SoldQuantity = soldQuantity;
            URL = url;
            SEOKeyword = seoKeyword;
        }

        /// <summary>
        /// Tính giá bán dựa trên giá bìa và chiết khấu
        /// </summary>
        public int GetSellingPrice()
        {
            if (Discount > 0)
            {
                return (int)(BookCoverPrice * (1 - Discount));
            }
            return BookCoverPrice;
        }

        /// <summary>
        /// Kiểm tra sản phẩm còn hàng
        /// </summary>
        public bool IsInStock()
        {
            return Quantity > 0 && Status == 0;
        }

        /// <summary>
        /// Kiểm tra sản phẩm đang kinh doanh
        /// </summary>
        public bool IsActive()
        {
            return Status == 0;
        }

        /// <summary>
        /// Lấy tên trạng thái
        /// </summary>
        public string GetStatusText()
        {
            switch (Status)
            {
                case 0:
                    return "Đang kinh doanh";
                case 1:
                    return "Tạm hết hàng";
                case 2:
                    return "Ngừng kinh doanh";
                default:
                    return "Không xác định";
            }
        }

        /// <summary>
        /// Lấy thông tin bìa sách
        /// </summary>
        public string GetCoverTypeText()
        {
            if (HardCover == null)
                return "Không xác định";
            return HardCover == 1 ? "Bìa cứng" : "Bìa mềm";
        }

        /// <summary>
        /// Lấy độ tuổi phù hợp dưới dạng text
        /// VD: "2-5 tuổi", "3 tuổi trở lên", "Không giới hạn"
        /// </summary>
        public string GetAgeRangeText()
        {
            if (MinAge == null || MaxAge == null || (MinAge == -1 && MaxAge == -1))
            {
                return "Không giới hạn";
            }

            int minYears = MinAge.Value / 12;
            int maxYears = MaxAge.Value / 12;

            if (MinAge == -1)
            {
                return $"Đến {maxYears} tuổi";
            }

            if (MaxAge == -1)
            {
                return $"{minYears} tuổi trở lên";
            }

            if (minYears == maxYears)
            {
                return $"{minYears} tuổi";
            }

            return $"{minYears}-{maxYears} tuổi";
        }

        public List<SanPhamMedia> MediaList { get; set; } = new List<SanPhamMedia>();

        public List<SanPhamMapping> Mappings { get; set; } = new List<SanPhamMapping>();
    }
}
