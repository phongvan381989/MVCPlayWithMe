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
        public string attributes { get; set; }
        public string image { get; set; }
        public List<string> images { get; set; }
        public List<object> option_attributes { get; set; }
        public List<Variant> variants { get; set; }
        public List<CertificateFile> certificate_files { get; set; }
        public MetaData meta_data { get; set; }
    }

    public class Attributes
    {
        public int bulky { get; set; }
        public string vat { get; set; }
        public string publisher_vn { get; set; }
        public string po_type { get; set; }
    }

    public class Variant
    {
        public string sku { get; set; }
        public int min_code { get; set; }
        public int price { get; set; }
        public string inventory_type { get; set; }
        public string seller_warehouse { get; set; }
        public List<WarehouseStock> warehouse_stocks { get; set; }
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