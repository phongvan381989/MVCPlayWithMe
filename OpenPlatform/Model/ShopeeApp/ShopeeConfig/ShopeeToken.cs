using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeConfig
{
    public class ShopeeToken
    {
        // ID of API requests; always returned. Used to diagnose problems.
        public string request_id { get; set; }

        // Error codes for API requests; always returned.When the API call is successful, the error code returned is empty.
        public string error { get; set; }

        // Returned when the API call is successful.Use refresh_token to get a new access_token.
        // Valid for each shop_id and merchant_id respectively, for 30 days.
        public string refresh_token { get; set; }

        // Returned when the API call is successful.A dynamic token that can be used
        // multiple times and expires after 4 hours.
        public string access_token { get; set; }

        // Returned when the API call is successful.The validity period of the access_token, in seconds.
        public int expire_in { get; set; }

        // Always returned. Provides detailed error information.
        public string message { get; set; }

        //// Returned when there is main_account_id in the input parameter, including all the merchant_ids authorized this time under the main account.
        //public List<int> merchant_id_list { get; set; }

        //// Returned when there is main_account_id in the input parameter, including all shop_ids authorized this time under the main account.
        //public List<int> shop_id_list { get; set; }
    }
}
