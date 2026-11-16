using System;
using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder
{
    /// <summary>
    /// TikTok Shop Order Model
    /// Maps to TikTok Shop Order API response
    /// Field names match exactly with TikTok Shop JSON response
    /// </summary>
    public class TikTokOrder
    {
        // Order Basic Info
        public string id { get; set; }
        public string status { get; set; }
        public long create_time { get; set; }
        public long update_time { get; set; }
        public long paid_time { get; set; }
        public string order_type { get; set; }
        public string fulfillment_type { get; set; }

        // Buyer Info
        public string buyer_email { get; set; }
        public string buyer_nickname { get; set; }
        public string buyer_avatar { get; set; }
        public string buyer_message { get; set; }
        public string user_id { get; set; }

        // Seller Info
        public string seller_note { get; set; }
        public string warehouse_id { get; set; }

        // Delivery Info
        public string delivery_option_id { get; set; }
        public string delivery_option_name { get; set; }
        public long delivery_option_required_delivery_time { get; set; }
        public string delivery_type { get; set; }
        public long delivery_sla_time { get; set; }
        public long delivery_time { get; set; }
        public long delivery_due_time { get; set; }

        // Shipping Info
        public string shipping_type { get; set; }
        public string shipping_provider { get; set; }
        public string shipping_provider_id { get; set; }
        public string tracking_number { get; set; }
        public long shipping_due_time { get; set; }
        public long rts_sla_time { get; set; }
        public long rts_time { get; set; }
        public long tts_sla_time { get; set; }
        public long recommended_shipping_time { get; set; }
        public long pick_up_cut_off_time { get; set; }
        public long fast_dispatch_sla_time { get; set; }
        public string fast_delivery_program { get; set; }
        public int fulfillment_priority_level { get; set; }

        // Collection Info
        public long collection_time { get; set; }
        public long collection_due_time { get; set; }

        // Cancellation Info
        public string cancel_reason { get; set; }
        public long cancel_time { get; set; }
        public string cancellation_initiator { get; set; }
        public long request_cancel_time { get; set; }
        public bool is_buyer_request_cancel { get; set; }
        public long cancel_order_sla_time { get; set; }

        // Order Flags
        public bool is_cod { get; set; }
        public bool is_sample_order { get; set; }
        public bool is_replacement_order { get; set; }
        public bool is_exchange_order { get; set; }
        public bool is_on_hold_order { get; set; }
        public bool has_updated_recipient_address { get; set; }

        // Special Order Info
        public string auto_combine_group_id { get; set; }
        public string split_or_combine_tag { get; set; }
        public string replaced_order_id { get; set; }
        public string exchange_source_order_id { get; set; }
        public long release_date { get; set; }
        public TikTokHandlingDuration handling_duration { get; set; }

        // Payment Info
        public TikTokOrderPayment payment { get; set; }
        public string payment_method_name { get; set; }

        // Address Info
        public TikTokRecipientAddress recipient_address { get; set; }

        // Line Items
        public List<TikTokOrderLineItem> line_items { get; set; }

        // Packages
        public List<TikTokOrderPackage> packages { get; set; }

        // Brazil Market
        public string cpf { get; set; }
        public string cpf_name { get; set; }
        public string need_upload_invoice { get; set; }

        // Indonesia Market
        public string commerce_platform { get; set; }
        public List<int> order_rights { get; set; }

        // ePharmacy
        public string consultation_id { get; set; }
    }

    public class TikTokHandlingDuration
    {
        public string days { get; set; }
        public string type { get; set; } // CALENDAR_DAY or BUSINESS_DAY
    }
}
