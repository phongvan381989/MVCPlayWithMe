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
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MySqlConnector;
using MVCPlayWithMe.OpenPlatform.Model;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeLogistic
{
    // Lấy được mã vận đơn của đơn hàng, booking
    public class ShopeeGetTrackingNumber
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<ShopeeGetTrackingNumberResponseHTTP> ShopeeGetTrackingNumberBaseAsync(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/logistics/get_tracking_number";
            IRestResponse response = await CommonShopeeAPI.ShopeeGetMethodAsync(path, ls);
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
        /// Lấy được tracking_number tức mã vận đơn của đơn hàng
        /// </summary>
        /// <returns></returns>
        public static async Task<string> ShopeeOrderGetTrackingNumberAsync(string order_sn, string package_number)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("order_sn", order_sn));

            if (!string.IsNullOrEmpty(package_number))
                ls.Add(new DevNameValuePair("package_number", package_number));

            ls.Add(new DevNameValuePair("response_optional_fields", "plp_number, first_mile_tracking_number,last_mile_tracking_number"));

            ShopeeGetTrackingNumberResponseHTTP res = await ShopeeGetTrackingNumberBaseAsync(ls);
            if(res == null || res.response == null || string.IsNullOrEmpty(res.response.tracking_number))
                return string.Empty;

            return res.response.tracking_number;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<ShopeeGetBookingTrackingNumberResponseHTTP> ShopeeGetBookingTrackingNumberBaseAsync(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/logistics/get_booking_tracking_number";
            IRestResponse response = await CommonShopeeAPI.ShopeeGetMethodAsync(path, ls);
            if (response == null)
                return null;

            ShopeeGetBookingTrackingNumberResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetBookingTrackingNumberResponseHTTP>(
                        response.Content, Common.jsonSerializersettings);
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
        /// Lấy được tracking_number tức mã vận đơn của đơn hàng
        /// </summary>
        /// <returns></returns>
        public static async Task<string> ShopeeGetBookingTrackingNumberAsync(string booking_sn)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("booking_sn", booking_sn));

            ShopeeGetBookingTrackingNumberResponseHTTP res = await ShopeeGetBookingTrackingNumberBaseAsync(ls);
            if (res == null ||
                res.response == null ||
                string.IsNullOrEmpty(res.response.tracking_number))
                return string.Empty;

            return res.response.tracking_number;
        }

        // Lấy mã vận đơn / ship code / tracking number
        public static async Task GetTrackingNumberFromDBAsync(List<ShopeeOrderDetail> rs, MySqlConnection conn)
        {
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            await shopeeMySql.GetTrackingNumberToListConnectOutAsync(rs, conn);

            foreach (var e in rs)
            {
                if (string.IsNullOrEmpty(e.shipCode) &&
                    e.order_status != "UNPAID" &&
                    e.order_status != "READY_TO_SHIP" &&
                    e.order_status != "CANCELLED")
                {
                    e.shipCode = await ShopeeGetTrackingNumber.ShopeeOrderGetTrackingNumberAsync(e.order_sn, string.Empty);
                    await shopeeMySql.UpdateTrackingNumberToDBConnectOutAsync(e.order_sn, e.shipCode, conn);
                }
            }
        }

        // Lấy mã vận đơn / ship code / tracking number
        public static async Task GetBookingTrackingNumberFromDBAsync(List<ShopeeBookingDetail> rs, MySqlConnection conn)
        {
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            await shopeeMySql.GetBookingTrackingNumberToListConnectOutAsync(rs, conn);

            foreach (var e in rs)
            {
                if (string.IsNullOrEmpty(e.shipCode) &&
                    e.booking_status != "READY_TO_SHIP" &&
                    e.booking_status != "CANCELLED")
                {
                    e.shipCode = await ShopeeGetTrackingNumber.ShopeeGetBookingTrackingNumberAsync(e.booking_sn);
                    await shopeeMySql.UpdateTrackingNumberToDBConnectOutAsync(e.order_sn, e.shipCode, conn);
                }
            }
        }
    }
}
