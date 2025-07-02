using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    // Chứa những class phục vụ việc tạo body cho api tạo sản phẩm
    public class TikiCreatingProduct
    {
        public int category_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int market_price { get; set; }
        public Attributes attributes { get; set; }
        public string image { get; set; }
        public List<string> images { get; set; }
        public List<object> option_attributes { get; set; }
        public List<Variant> variants { get; set; }
        public List<CertificateFile> certificate_files { get; set; }
        public MetaData meta_data { get; set; }

        public TikiCreatingProduct()
        {
            attributes = new Attributes();
            images = new List<string>();
            option_attributes = new List<object>();
            variants = new List<Variant>();
            certificate_files = new List<CertificateFile>();
            meta_data = new MetaData();
        }
    }

    public class Attributes
    {
        public List<string> age_group { get; set; }
        public string book_cover { get; set; }
        //public string description { get; set; }
        public string language_book { get; set; }
        public string manufacturer { get; set; }
        //public string name { get; set; }

        // Chiều cao của sản phẩm sau khi đã được đóng gói, thùng, hộp...Đơn vị tính: cm
        public string product_height { get; set; }

        // Chiều rộng của sản phẩm sau khi đã được đóng gói, thùng, hộp... Đơn vị tính: cm
        public string product_width { get; set; }

        // Chiều dài của sản phẩm sau khi đã được đóng gói, thùng, hộp... Đơn vị tính: cm
        public string product_length { get; set; }

        // Trọng lượng của sản phẩm sau khi đã được đóng gói, thùng, hộp... Đơn vị tính: kg
        public string product_weight_kg { get; set; }

        public string publisher_vn { get; set; }
        public string number_of_page { get; set; }

        // Kích thước thực tế của sản phẩm. Nên mô tả chi tiết những kích thước quan trọng theo từng loại sản phẩm để Khách hàng tham khảo khi mua.
        // \nVD: \n(Dài x Rộng x Cao) 22 x 10 x 5 cm\nĐường kính: 20cm
        public string dimensions { get; set; }
        public string dich_gia { get; set; }
        public List<string> author { get; set; }
        //public float price { get; set; }

        public Attributes()
        {
            age_group = new List<string>();
            author = new List<string>();
        }
    }

    public class Variant
    {
        public string sku { get; set; }
        public long min_code { get; set; }
        public int price { get; set; }
        public string inventory_type { get; set; }
        public string seller_warehouse { get; set; }
        public List<WarehouseStock> warehouse_stocks { get; set; }

        public Variant()
        {
            warehouse_stocks = new List<WarehouseStock>();
        }
    }

    public class WarehouseStock
    {
        public int warehouseId { get; set; }
        public int qtyAvailable { get; set; }
    }

    public class CertificateFile
    {
        public string url { get; set; }
        public string type { get; set; }
        public int document_id { get; set; }
    }

    public class MetaData
    {
        public bool is_auto_turn_on { get; set; }
    }
}