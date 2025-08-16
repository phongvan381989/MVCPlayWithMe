using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaConfig
{
    public class CountryUserInfo
    {
        public string country { get; set; }
        public string seller_id { get; set; }
        public string short_code { get; set; }
        public string user_id { get; set; }
    }

    public class LazadaAuthTokenCreateResponseBody : CommonLazadaResponseHTTP
    {
        public string _trace_id_ { get; set; }
        public string access_token { get; set; }
        public string account { get; set; }
        public string account_platform { get; set; }
        public string country { get; set; }
        public List<CountryUserInfo> country_user_info { get; set; }
        public int expires_in { get; set; }
        public int refresh_expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
