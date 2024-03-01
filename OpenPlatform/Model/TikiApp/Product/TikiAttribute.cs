using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiAttribute
    {
        public string author { get; set; }

        public int book_cover { get; set; }

        public int bulky { get; set; }

        public string competitor_info { get; set; }

        public string danh_muc_hang_hoa { get; set; }

        public string delivery_attributes { get; set; }

        public string description { get; set; }

        public string dich_gia { get; set; }

        public string dimensions { get; set; }

        public float fob_price { get; set; }

        public float fob_price_usd { get; set; }

        public int giftwrap { get; set; }

        public string image { get; set; }

        public int is_advertisable_gg { get; set; }

        public int is_erp_dimensions_updated { get; set; }

        public int is_free_gift { get; set; }

        public int is_fresh { get; set; }

        public int is_hot { get; set; }

        public int is_imported { get; set; }

        public int is_p2p_delivery { get; set; }

        public int is_sampling { get; set; }

        public string jd_id { get; set; }

        public string jd_stock_range { get; set; }

        public string kich_thuoc_dong_goi { get; set; }

        public float list_price { get; set; }

        public int manufacturer { get; set; }

        public int manufacturer_book_vn { get; set; }

        public string meta_description { get; set; }

        public string meta_title { get; set; }

        public string name { get; set; }

        public string number_of_page { get; set; }

        public string option1 { get; set; }

        public string option1_label { get; set; }

        public string option2 { get; set; }

        public string option2_label { get; set; }

        public int plastic_cover_suitable { get; set; }

        public TikiPoType po_type { get; set; }

        public int preorder { get; set; }

        public float price { get; set; }

        public string product_height { get; set; }

        public string product_height_internal { get; set; }

        public string product_length { get; set; }

        public string product_length_internal { get; set; }

        public string product_volume_metric { get; set; }

        public string product_weight_kg { get; set; }

        public string product_weight_kg_internal { get; set; }

        public string product_width { get; set; }

        public string product_width_internal { get; set; }

        public long publication_date { get; set; }

        public int publisher_vn { get; set; }

        public string request { get; set; }

        public string seller_delivery_leadtime_minutes { get; set; }

        public string seller_warehouse { get; set; }

        public string shipping_weight { get; set; }

        public string shipping_weight_internal { get; set; }

        public string small_image { get; set; }

        public List<TikiAtributeSpecification> specifications { get; set; }

        public float standard_price { get; set; }

        public int status { get; set; }

        public TikiSupplier supplier { get; set; }

        public int support_cod { get; set; }

        public int support_next_day_delivery { get; set; }

        public int support_p24h { get; set; }

        public int support_p24h_delivery { get; set; }

        public int support_p2h_delivery { get; set; }

        public int support_subscription { get; set; }

        public string table_of_contents { get; set; }

        public string tag_coupon_code { get; set; }

        public string tag_discount { get; set; }

        public int tag_is_app_only { get; set; }

        public int tag_status { get; set; }

        public string thumbnail { get; set; }

        public string tiki_recommendation { get; set; }

        public TikiUnit unit { get; set; }

        public string url_key { get; set; }

        public string url_path { get; set; }

        public TikiProductVat vat { get; set; }

        public int visibility { get; set; }

        public float weight { get; set; }
    }
}
