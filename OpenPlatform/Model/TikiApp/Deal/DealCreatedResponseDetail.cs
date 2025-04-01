using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount
{
    //    {
    //        "product_id": 76217978,
    //        "sku": "3169092896056",
    //        "product_name": "Combo 6 cuốn sách Ehon Song Ngữ Kích Thích Thị Giác Cho Bé 0-3 tuổi Black and White books",
    //        "seller_id": 145228,
    //        "special_price": 189000,
    //        "special_from_date": "2025-09-01 02:59:59",
    //        "special_to_date": "2025-10-01 03:59:59",
    //        "qty_max": 50,
    //        "qty_limit": 1,
    //        "created_by_email": "6249716820922226@open.tiki.vn",
    //        "updated_by": null,
    //        "sort_order": 0,
    //        "tags": null,
    //        "discount_percent": 20,
    //        "is_hot_deal": 0,
    //        "price": 234000,
    //        "campaign_id": null,
    //        "url": null,
    //        "is_active": 1,
    //        "priority": 100,
    //        "created_at": "2025-03-31T04:44:33.083Z",
    //        "updated_at": "2025-03-31T04:44:33.083Z",
    //        "notes": null,
    //        "product_original_data": null,
    //        "image": null,
    //        "qty_ordered": null,
    //        "updated_by_email": null,
    //        "area_id": 0,
    //        "min_price": null,
    //        "step_price": null,
    //        "first_special_price": null,
    //        "is_child": 0,
    //        "seller_product_code": null,
    //        "queue": null,
    //        "id": 534570745
    //    }
    public class DealCreatedResponseDetail
    {
        public int product_id { get; set; }
        public string sku { get; set; }
        public string product_name { get; set; }
        //public int seller_id { get; set; }
        public int special_price { get; set; }

        // Format 2025-09-01 02:59:59
        public string special_from_date { get; set; }
        public string special_to_date { get; set; }

        public int qty_max { get; set; }
        public int qty_limit { get; set; }
        //public string created_by_email { get; set; }
        //public string updated_by { get; set; }
        public int sort_order { get; set; }
        //public string tags { get; set; }
        public int discount_percent { get; set; }
        public int is_hot_deal { get; set; }
        public int price { get; set; }
        public int? campaign_id { get; set; }
        //public string url { get; set; }

        // status of deal, 
        // values: INACTIVE = 0 | ACTIVE = 1 | RUNNING = 2 | EXPIRED = 3 | HOT_SALE = 4 | CLOSED = 5 | PAUSED = 6
        public int is_active { get; set; }
        public int priority { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string notes { get; set; }
        //public string product_original_data { get; set; }
        //public string image { get; set; }
        //public int? qty_ordered { get; set; }
        //public string updated_by_email { get; set; }
        //public int area_id { get; set; }
        //public int? min_price { get; set; }
        //public int? step_price { get; set; }
        //public int? first_special_price { get; set; }
        //public int is_child { get; set; }
        //public string seller_product_code { get; set; }
        //public string queue { get; set; }
        public int id { get; set; }
    }
}