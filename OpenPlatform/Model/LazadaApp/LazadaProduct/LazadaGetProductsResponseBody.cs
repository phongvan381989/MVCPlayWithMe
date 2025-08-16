using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct
{
    public class LazadaGetProductsResponseBody : CommonLazadaResponseHTTP
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public int total_products { get; set; }
        public List<LazadaProduct> products { get; set; }
    }

    public class LazadaProduct
    {
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public List<string> images { get; set; }
        public List<LazadaSku> skus { get; set; }
        public long item_id { get; set; }
        public string hiddenStatus { get; set; }
        public List<object> suspendedSkus { get; set; }
        public string subStatus { get; set; }
        public Boolean trialProduct { get; set; }
        public List<RejectReason> rejectReason { get; set; }
        public long primary_category { get; set; }
        public List<string> marketImages { get; set; }
        public Attributes attributes { get; set; }
        public string hiddenReason { get; set; }
        public string status { get; set; }
        public Variations variation { get; set; }
    }

    public class LazadaSku
    {
        public string Status { get; set; }
        public int quantity { get; set; }
        public string product_weight { get; set; }
        public List<string> Images { get; set; }
        public string SellerSku { get; set; }
        public string ShopSku { get; set; }
        public string Url { get; set; }
        public string package_width { get; set; }
        public string special_to_time { get; set; }
        public string special_from_time { get; set; }
        public string package_height { get; set; }
        public decimal special_price { get; set; }
        public decimal price { get; set; }
        public string package_length { get; set; }
        public string package_weight { get; set; }
        public int Available { get; set; }
        public long SkuId { get; set; }
        public string special_to_date { get; set; }
        public List<WarehouseInfo> multiWarehouseInventories{ get;set; }

        //"SellerSku": "11345135593-NHÀ DU HÀNH VŨ TRỤ", của sku của item có hơn 1 sku
        //"SellerSku": "3063228760-1743410643073-0", của sku của item có 1 sku
        // NOTE: Chỉ lấy tên sku nếu item có nhiều sku
        public string GetSkuName()
        {
            string result = string.Empty;
            string[] parts = SellerSku.Split('-');

            if (parts.Length >= 2)
            {
                result = parts[1];
            }
            return result;
        }

        // Tự thêm vào
        public string mySkuName { get; set; }
    }

    public class RejectReason
    {
        public string suggestion { get; set; }
        public string violationDetail { get; set; }
    }

    public class WarehouseInfo
    {
        public string bizCode { get; set; }
        public int bizType { get; set; }
        public int occupyQuantity { get; set; }
        public int quantity { get; set; }
        public int sellableQuantity { get; set; }
        public int totalQuantity { get; set; }
        public string warehouseCode { get; set; }
        public string warehouseType { get; set; }
        public int withholdQuantity { get; set; }
    }

    public class Attributes
    {
        public string author { get; set; }
        public string isbn_issn { get; set; }
        public string language { get; set; }
        public string short_description { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string name_engravement { get; set; }
        public string warranty_type { get; set; }
        public string gift_wrapping { get; set; }
        public int preorder_days { get; set; }
        public string brand { get; set; }
        public string preorder { get; set; }
    }

    public class Variations
    {
        public Variation variation1 { get; set; }
        public Variation variation2 { get; set; }
        public Variation variation3 { get; set; }
        public Variation variation4 { get; set; }
    }


    public class Variation
    {
        public bool customize { get; set; }
        public bool hasImage { get; set; }
        public string label { get; set; }
        public string name { get; set; }
        public List<string> options { get; set; }
    }
}
