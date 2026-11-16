using System.Collections.Generic;

namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokOrder
{
    /// <summary>
    /// TikTok Shop Get Order List Response
    /// API: /order/202309/orders/search (POST)
    /// </summary>
    public class TikTokOrderListResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string request_id { get; set; }
        public TikTokOrderListData data { get; set; }

        public bool IsSuccess()
        {
            return code == 0;
        }
    }

    public class TikTokOrderListData
    {
        public List<TikTokOrder> orders { get; set; }
        public int total_count { get; set; }
        public string next_page_token { get; set; }
    }
}
