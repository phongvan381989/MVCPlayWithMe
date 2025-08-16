using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaConfig
{
    public class LazadaAuth
    {
        public string appKey { get; set; }
        public string appSecret { get; set; }

        // Access token.
        public string accessToken { get; set; }

        // The expiring time of the access token, in seconds. For APPs in "Test" status,
        // the value is 7 days. For APPs in "Online" status, the value is 30 days.
        public int expiresIn { get; set; }

        // Refresh token, used to refresh the token when “refresh_expires_in”>0.
        public string refreshToken { get; set; }

        // The expiring time of the refresh token. For APPs in "Test" status,
        // the value is 30 days. For APPs in "Online" status, the value is 180 days.
        public int refreshExpiresIn { get; set; }

        // Thời điểm làm mới access token
        public DateTime refreshDatetime { get; set; }
    }
}
