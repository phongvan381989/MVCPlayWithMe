using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder
{
    /// <summary>
    /// TikTok Shop Recipient Address Information
    /// Field names match exactly with TikTok Shop JSON response
    /// </summary>
    public class TikTokRecipientAddress
    {
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string first_name_local_script { get; set; }  // JP market katakana
        public string last_name_local_script { get; set; }   // JP market katakana

        public string phone_number { get; set; }

        public string full_address { get; set; }
        public string address_detail { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }  // BR market
        public string address_line4 { get; set; }  // BR market

        public string postal_code { get; set; }
        public string post_town { get; set; }  // UK market
        public string region_code { get; set; }

        public List<TikTokDistrictInfo> district_info { get; set; }
        public TikTokDeliveryPreferences delivery_preferences { get; set; }
    }

    public class TikTokDistrictInfo
    {
        public string address_level { get; set; }       // L0/L1/L2/L3/L4
        public string address_level_name { get; set; }  // Country/State/City/etc
        public string address_name { get; set; }
        public string iso_code { get; set; }
    }

    public class TikTokDeliveryPreferences
    {
        public string drop_off_location { get; set; }  // US market only
    }
}
