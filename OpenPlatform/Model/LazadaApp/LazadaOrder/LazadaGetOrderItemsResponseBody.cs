using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaOrder
{
    public class LazadaGetOrderItemsResponseBody : CommonLazadaResponseHTTP
    {
        public List<LazadaOrderItem> data { get; set; }
    }

    public class LazadaOrderItem
    {
        // Product name
        public string name { get; set; }

        // Status
        // Possible values are unpaid, pending, canceled, ready_to_ship, 
        // delivered, returned, shipped , failed, topack,toship,shipping and lost
        public string status { get; set; }

        // Order ID
        public long order_id { get; set; }

        // 
        public string created_at { get; set; }

        // Product SKU. Đây là SellerSku, có thể sửa đổi bởi nhà bán
        public string sku { get; set; }

        // Order item ID
        public long order_item_id { get; set; }

        // Product outer ID. Đây là Lazada SKU không thể sửa đổi bởi nhà bán
        public string shop_sku { get; set; }

        // Sku ID, tương ứng với modeId trong db
        public string sku_id { get; set; }

        // Product ID, tương ứng với itemId trong db
        public string product_id { get; set; }

        // Đây là mã đơn hàng, có thể dùng máy quét mã vạch trên tờ đơn
        // Tracking code retrieved from 3PL shipment provider
        public string tracking_code { get; set; }

        // Product main image URL
        public string product_main_image { get; set; }

        // Ex: "TÊN SÁCH:COMBO 3 CUỐN". Trong đó "TÊN SÁCH" là tên variation
        public string variation { get; set; }

        // Nếu variation khác rỗng, ta lấy tên cụ thể của 1 variation
        public string GetVariationName()
        {
            string va = string.Empty;
            if(!string.IsNullOrEmpty(variation))
            {
                int index = variation.IndexOf(':');

                string result = index != -1 ? variation.Substring(index + 1).Trim() : "";
            }
            return va;
        }
    }
}