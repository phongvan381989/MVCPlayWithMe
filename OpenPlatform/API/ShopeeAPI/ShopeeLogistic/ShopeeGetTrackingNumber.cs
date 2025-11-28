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
using MySql.Data.MySqlClient;
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
        /// Lấy được tracking_number tức mã vận đơn của đơn hàng
        /// </summary>
        /// <returns></returns>
        public static string ShopeeOrderGetTrackingNumber(string order_sn, string package_number)
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static ShopeeGetBookingTrackingNumberResponseHTTP ShopeeGetBookingTrackingNumberBase(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/logistics/get_booking_tracking_number";
            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
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
        public static string ShopeeGetBookingTrackingNumber(string booking_sn)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            // Required
            // Shopee's unique identifier for a booking.
            ls.Add(new DevNameValuePair("booking_sn", booking_sn));

            ShopeeGetBookingTrackingNumberResponseHTTP res = ShopeeGetBookingTrackingNumberBase(ls);
            if (res == null ||
                res.response == null ||
                string.IsNullOrEmpty(res.response.tracking_number))
                return string.Empty;

            return res.response.tracking_number;
        }

        // Lấy mã vận đơn / ship code / tracking number
        public static void GetTrackingNumberFromDB(List<ShopeeOrderDetail> rs, MySqlConnection conn)
        {
            // Ta lấy từ tbecommerceorder nếu tồn tại, ngược lại lấy từ API shopee
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            shopeeMySql.GetTrackingNumberToListConnectOut(rs, conn);

            // Đơn ở trạng thái: UNPAID, READY_TO_SHIP => chưa được sàn sinh mã vận chuyển.
            // Nhà bán chưa xác nhận đơn, khách hủy (trạng thái sẽ là CANCELLED) => chưa được sinh mã vận chuyển
            // Ngược lại đã được sinh mã đơn.
            // Ở trạng thái PROCESSED: Nhà bán đã xác nhận nhưng có thể chưa được đóng nên chưa
            // có mã vận chuyển trong db. Ta lưu vào db khi lấy được mã vận đơn

            // Nhiều khi đưa shipper đơn nhưng vẫn chưa cập nhật đã đóng => 
            // trạng thái SHIPPED, COMPLETE mà vẫn chưa có mã vận chuyển trong db
            foreach (var e in rs)
            {
                // Mã vận chuyển đã được sinh nhưng chưa được lưu ở db
                if (string.IsNullOrEmpty(e.shipCode) &&
                    e.order_status != "UNPAID" &&
                    e.order_status != "READY_TO_SHIP" &&
                    e.order_status != "CANCELLED") // Mã vận chuyển được lấy ở xử lý event
                {
                    e.shipCode = ShopeeGetTrackingNumber.ShopeeOrderGetTrackingNumber(e.order_sn, string.Empty);
                    shopeeMySql.UpdateTrackingNumberToDBConnectOut(e.order_sn, e.shipCode, conn);
                }
            }
        }

        // Lấy mã vận đơn / ship code / tracking number
        public static void GetBookingTrackingNumberFromDB(List<ShopeeBookingDetail> rs, MySqlConnection conn)
        {
            // Ta lấy từ tbecommerceorder nếu tồn tại, ngược lại lấy từ API shopee
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            shopeeMySql.GetBookingTrackingNumberToListConnectOut(rs, conn);

            // Booking ở trạng thái:  READY_TO_SHIP => chưa được sàn sinh mã vận chuyển.
            // Ở trạng thái PROCESSED: Nhà bán đã xác nhận nhưng có thể chưa được đóng nên chưa
            // có mã vận chuyển trong db. Ta lưu vào db khi lấy được mã vận đơn

            // Nhiều khi đưa shipper đơn nhưng vẫn chưa cập nhật đã đóng => 
            // trạng thái SHIPPED mà vẫn chưa có mã vận chuyển trong db
            foreach (var e in rs)
            {
                // Mã vận chuyển đã được sinh nhưng chưa được lưu ở db
                if (string.IsNullOrEmpty(e.shipCode) &&
                    e.booking_status != "READY_TO_SHIP" &&
                    e.booking_status != "CANCELLED") // Mã vận chuyển được lấy ở xử lý event
                {
                    e.shipCode = ShopeeGetTrackingNumber.ShopeeGetBookingTrackingNumber(e.booking_sn);
                    shopeeMySql.UpdateTrackingNumberToDBConnectOut(e.order_sn, e.shipCode, conn);
                }
            }
        }
    }
}
