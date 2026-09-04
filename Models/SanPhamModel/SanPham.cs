using System;
using System.Collections.Generic;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public enum ESanPhamStatus
    {
        DANG_KINH_DOANH = 0,
        TAM_HET_HANG = 1,
        NGUNG_KINH_DOANH = 2
    }

    public enum ESanPhamCoverType
    {
        BIA_MEM = 0,
        BIA_CUNG = 1
    }

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

        public string ComboName { get; set; } = string.Empty;

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
        public string PublisherName { get; set; } = string.Empty;

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
        public ESanPhamCoverType HardCover { get; set; }

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
        public ESanPhamStatus Status { get; set; }

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
            Status = ESanPhamStatus.DANG_KINH_DOANH;
            Quantity = 0;
            Discount = 0;
            SalePrice = 0;
            Mappings = new List<SanPhamMapping>();
            MediaList = new List<SanPhamMedia>();
        }

        // Constructor đầy đủ
        public SanPham(
            string code, string barcode, string name, string shortName,
            int? comboId, int? categoryId, int bookCoverPrice,
            string author, string translator, int? publisherId,
            string publishingCompany, int? publishingTime,
            int productLong, int productWide, int productHigh, int productWeight,
            string positionInWarehouse, ESanPhamCoverType hardCover,
            int? minAge, int? maxAge, int? parentId, int? republish,
            string detail, ESanPhamStatus status, int quantity, int? pageNumber,
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
            return Quantity > 0 && Status == ESanPhamStatus.DANG_KINH_DOANH;
        }

        /// <summary>
        /// Kiểm tra sản phẩm đang kinh doanh
        /// </summary>
        public bool IsActive()
        {
            return Status == ESanPhamStatus.DANG_KINH_DOANH;
        }

        /// <summary>
        /// Lấy tên trạng thái
        /// </summary>
        public string GetStatusText()
        {
            switch (Status)
            {
                case ESanPhamStatus.DANG_KINH_DOANH:
                    return "Đang kinh doanh";
                case ESanPhamStatus.TAM_HET_HANG:
                    return "Tạm hết hàng";
                default:
                    return "Ngừng kinh doanh";
            }
        }

        /// <summary>
        /// Lấy thông tin bìa sách
        /// </summary>
        public string GetCoverTypeText()
        {
            if (HardCover == ESanPhamCoverType.BIA_CUNG)
                return "Bìa cứng";
            return "Bìa mềm";
        }

        ///// <summary>
        ///// Lấy độ tuổi phù hợp dưới dạng text
        ///// VD: "2-5 tuổi", "3 tuổi trở lên", "Không giới hạn"
        ///// </summary>
        //public string GetAgeRangeText()
        //{
        //    return GetAgeRangeText(MinAge, MaxAge);
        //}

        /// <summary>
        /// Static overload - Convert độ tuổi từ tháng → năm và format thành text
        /// VD: "2-5 tuổi", "3 tuổi trở lên", "Không giới hạn"
        /// </summary>
        public static string GetAgeRangeText(int? minAge, int? maxAge)
        {
            if ((minAge == null || minAge == -1) && (maxAge == null || maxAge == -1))
            {
                return string.Empty;
            }

            int minYears = (minAge != null && minAge != -1) ? minAge.Value / 12 : -1;
            int maxYears = (maxAge != null && maxAge != -1) ? maxAge.Value / 12 : -1;

            if (minAge == -1)
            {
                return $"Đến {maxYears} tuổi";
            }

            if (maxAge == -1)
            {
                return $"Từ {minYears} tuổi";
            }

            if (minYears == maxYears)
            {
                return $"{minYears} tuổi";
            }

            return $"{minYears}-{maxYears} tuổi";
        }

        public List<SanPhamMedia> MediaList { get; set; } = new List<SanPhamMedia>();

        public List<SanPhamMapping> Mappings { get; set; } = new List<SanPhamMapping>();

        public int GetQuantityFromMappings()
        {
            if(Mappings == null || Mappings.Count == 0)
            {
                return 0;
            }

            int quantity = Int32.MaxValue;
            foreach(var mapping in Mappings)
            {
                if (mapping.SanPhamKhoQuantity <= 0 || mapping.Quantity <= 0)
                {
                    quantity = 0;
                }
                else
                {
                    int quantityTemp = mapping.SanPhamKhoQuantity / mapping.Quantity;
                    if (quantityTemp < quantity)
                    {
                        quantity = quantityTemp;
                    }
                }
            }
            if (quantity < 0)
            {
                quantity = 0;
            }

            return quantity;
        }
    }

    /// <summary>
    /// DTO lightweight cho thông tin cơ bản sản phẩm (dùng cho cart, checkout)
    /// </summary>
    public class SanPhamBasicInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int BookCoverPrice { get; set; }
        public int SalePrice { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string CoverImageFileName { get; set; } // Ảnh bìa từ tb_san_pham_media (DisplayOrder = nhỏ nhất)
        public string CoverImageAltText { get; set; }  // Alt text cho SEO/accessibility
        public string CoverImageTitle { get; set; }     // Title attribute (tooltip)
    }

    /// <summary>
    /// DTO lightweight cho trang Search - chỉ lấy thông tin cần thiết cho filter và hiển thị
    /// Không lấy các field dài như Detail, Author, Translator, dimensions, v.v.
    /// </summary>
    public class AdminSanPhamSearchInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ComboId { get; set; }
        public int? CategoryId { get; set; }
        public int? PublisherId { get; set; }
        public int BookCoverPrice { get; set; }
        public float Discount { get; set; }
        public int SalePrice { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}
