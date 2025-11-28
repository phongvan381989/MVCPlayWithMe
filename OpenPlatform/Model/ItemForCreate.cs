using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    // Tạo sản phẩm từ sản phẩm khác.
    // Sản phẩm được lấy trên web khác bằng browser extension
    public class ItemForCreate
    {
        // Sàn lấy dữ liệu (SHOPEE, LAZADA, TIKI, SENDO)
        public string siteNameCopyFrom { get; set; } = "";

        // Tên sàn (SHOPEE, LAZADA, TIKI, SENDO) sẽ tạo item
        public string siteName { get; set; } = "";
        // Lỗi nếu có
        public string error { get; set; } = "";
        // Phân loại sản phẩm trên sàn
        public string category { get; set; } = "";
        // Id Phân loại sản phẩm trên sàn
        public long categoryId { get; set; } = 0;

        // Tên sản phẩm
        public string name { get; set; } = "";

        public string isbn { get; set; } = "9786046546900";

        // Có biến thể hay không
        public bool hasModels { get; set; } = false;
        // Link video
        public string srcVideo { get; set; } = "";
        public string pathVideo { get; set; } = "";
        // Id hoặc url video sau khi được up lên sàn mới
        public string idVideo { get; set; } = "";

        // Mảng link ảnh
        public List<string> srcImages { get; set; } = new List<string>();
        // Đường dẫn tuyệt đối trên server
        public List<string> pathImages { get; set; } = new List<string>();
        // Mảng link ảnh mới
        public List<string> srcImagesTo { get; set; } = new List<string>();

        // Tên phân loại biến thể (nếu có)
        // Ví dụ: Tên Sách
        public string variationName { get; set; }

        // Danh sách biến thể
        public List<ModelForCreate> models { get; set; } = new List<ModelForCreate>();

        // Thương hiệu / tác giả
        public string brand { get; set; } = "";
        // Loại phiên bản: thường, đặc biệt, giới hạn
        public string editionType { get; set; } = "";
        // Nhập khẩu hay trong nước: true - nhập khẩu, false - trong nước
        public bool isImported { get; set; } = false;
        // Ngôn ngữ
        public string language { get; set; } = "";
        // Loại bìa sách (bìa mềm, bìa cứng)
        public string bookCoverType { get; set; } = "";
        // Nhà phát hành
        public string publisher { get; set; } = "";
        // Năm xuất bản
        public int publishYear { get; set; } = 0;
        // Số trang
        public int pageNumber { get; set; } = 0;
        // Nhà xuất bản
        public string publishingCompany { get; set; } = "";
        // Kích thước dài (mm)
        public int sizeLength { get; set; } = 100;
        // Kích thước rộng (mm)
        public int sizeWidth { get; set; } = 100;
        // Kích thước dày (mm)
        public int sizeHeight { get; set; } = 100;
        // Trọng lượng (gram)
        public int weight { get; set; } = 100;

        // Mảng mô tả sản phẩm
        public List<DescriptionForCreate> descriptions { get; set; } = new List<DescriptionForCreate>();
    }

    public class ModelForCreate
    {
        // Lỗi nếu có
        public string error { get; set; } = "";
        // Giá bìa sách
        public int bookCoverPrice { get; set; } = 0;
        // Link video
        public string srcVideo { get; set; } = "";
        // Mảng link ảnh
        public List<string> srcImages { get; set; } = new List<string>();
        // Đường dẫn tuyệt đối trên server
        public List<string> pathImages { get; set; } = new List<string>();
        // Mảng link ảnh mới
        public List<string> srcImagesTo { get; set; } = new List<string>();

        // Tên biến thể nếu có
        public string name { get; set; }
    }

    // Dùng để chứa mô tả sản phẩm, có thể là ảnh hoặc text
    public class DescriptionForCreate
    {
        // Loại: true nếu là "text" ngược lại là "image"
        public bool isText { get; set; } = true;
        // Nội dung text hoặc link ảnh
        public string content { get; set; } = "";
        // Đường dẫn tuyệt đối trên server của ảnh nếu isText là false
        public string path { get; set; } = "";
        // link ảnh mới
        public string srcImageTo { get; set; } = "";
    }
}