using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeLogistic;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeLogistic
{
    public class ShopeeGetTrackingNumber
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static ShopeeGetTrackingNumberResponseHTTP ShopeeGetTrackingNumberBase(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/logistics/get_tracking_number";
            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
            if (response == null)
                return null;

            ShopeeGetTrackingNumberResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetTrackingNumberResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }
            return objResponse;
        }

        /// <summary>
        /// Lấy được tracking_number tức mã vận đơn
        /// </summary>
        /// <returns></returns>
        public static string ShopeeGetShipCode(string order_sn, string package_number)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            // Required
            // Shopee's unique identifier for an order.
            ls.Add(new DevNameValuePair("order_sn", order_sn));

            // Shopee's unique identifier for the package under an order.
            // You should't fill the field with empty string when there isn't a package number.
            if (!string.IsNullOrEmpty(package_number))
                ls.Add(new DevNameValuePair("package_number", package_number));

            // Indicate response fields you want to get. Please select from the below response parameters. 
            // If you input an object field, all the params under it will be included automatically in the response.
            // If there are multiple response fields you want to get, you need to use English comma to connect them.
            // Available values: plp_number, first_mile_tracking_number,last_mile_tracking_number

            ls.Add(new DevNameValuePair("response_optional_fields", "plp_number, first_mile_tracking_number,last_mile_tracking_number"));

            ShopeeGetTrackingNumberResponseHTTP res = ShopeeGetTrackingNumberBase(ls);
            if(res == null || res.response == null || string.IsNullOrEmpty(res.response.tracking_number))
                return string.Empty;

            return res.response.tracking_number;
        }
    }
}
