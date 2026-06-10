using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeLogistic;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder
{
    public class ShopeeGetBookingDetail
    {
        /// <summary>
        /// Lấy thông tin chi tiết booking
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<List<ShopeeBookingDetail>> ShopeeOrderGetBookingDetailAsync(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/order/get_booking_detail";

            IRestResponse response = await CommonShopeeAPI.ShopeeGetMethodAsync(path, ls);
            if (response == null)
                return null;

            ShopeeGetBookingDetailResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetBookingDetailResponseHTTP>(response.Content, Common.jsonSerializersettings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }
            if (objResponse == null
                || objResponse.response == null
                || objResponse.response.booking_list == null)
                return null;

            return objResponse.response.booking_list;
        }

        /// <summary>
        /// Lấy thông tin chi tiết danh sách booking từ danh sách mã booking
        /// </summary>
        /// <param name="booking_sn_list">booking_sn_list số phần tử phải nhỏ hơn hoặc bằng 50</param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<List<ShopeeBookingDetail>> ShopeeOrderGetBookingDetailFromListBookingSNAsync(
            List<string> booking_sn_list)
        {
            if (booking_sn_list == null)
                return null;

            StringBuilder strBooking_sn_list = new StringBuilder();
            foreach (string s in booking_sn_list)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                strBooking_sn_list.Append(s);
                strBooking_sn_list.Append(",");
            }
            strBooking_sn_list.Remove(strBooking_sn_list.Length - 1, 1);
            List<DevNameValuePair> ls = new List<DevNameValuePair>();

            ls.Add(new DevNameValuePair("booking_sn_list", strBooking_sn_list.ToString()));
            ls.Add(new DevNameValuePair("response_optional_fields",
                "item_list,cancel_by,cancel_reason,fulfillment_flag,pickup_done_time,shipping_carrier"));

            return await ShopeeOrderGetBookingDetailAsync(ls);
        }

        /// <summary>
        /// Lấy thông tin chi tiết booking từ mã booking
        /// </summary>
        public static async Task<ShopeeBookingDetail> ShopeeOrderGetBookingDetailFromBookingSNAsync(
            string booking_sn)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();

            ls.Add(new DevNameValuePair("booking_sn_list", booking_sn));
            ls.Add(new DevNameValuePair("response_optional_fields",
                "item_list,cancel_by,cancel_reason,fulfillment_flag,pickup_done_time,shipping_carrier"));

            List<ShopeeBookingDetail> bookings = await ShopeeOrderGetBookingDetailAsync(ls);
            if (bookings != null && bookings.Count > 0)
                return bookings[0];

            return null;
        }

        /// <summary>
        /// Lấy thông tin chi tiết danh sách booking từ danh sách mã booking
        /// </summary>
        /// <param name="booking_sn_list"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<List<ShopeeBookingDetail>> ShopeeOrderGetBookingDetailFromListBookingSNAllAsync(
            List<string> booking_sn_list)
        {
            List<ShopeeBookingDetail> rs = new List<ShopeeBookingDetail>();
            if (booking_sn_list == null || booking_sn_list.Count() == 0)
            {
                return rs;
            }

            int range = 50; // Giới hạn

            int indexMax = booking_sn_list.Count() - 1;
            int index = 0;
            List<string> booking_sn_listTemp = null;
            while (index <= indexMax)
            {
                if (indexMax - index >= range)
                    booking_sn_listTemp = booking_sn_list.GetRange(index, range);
                else
                    booking_sn_listTemp = booking_sn_list.GetRange(index, indexMax - index + 1);
                List<ShopeeBookingDetail> rsTemp = await ShopeeOrderGetBookingDetailFromListBookingSNAsync(booking_sn_listTemp);
                if (rsTemp == null)
                {
                    break;
                }
                rs.AddRange(rsTemp);
                index = index + range;
            }

            return rs;
        }

        /// <summary>
        /// Lấy thông tin chi tiết danh sách booking theo thời gian và trạng thái booking
        /// </summary>
        public static async Task<List<ShopeeBookingDetail>> ShopeeOrderGetBookingDetailAllAsync(
            DateTime time_from,
            DateTime time_to,
            string status,
            MySqlConnection conn)
        {
            List<ShopeeBookingDetail> rs = new List<ShopeeBookingDetail>();
            List<ShopeeGetBookingListBaseInfo> lsBaseInfo =
                await ShopeeGetBookingList.ShopeeOrderGetBookingListBaseFromToAsync(time_from, time_to, status);
            if (lsBaseInfo.Count() == 0)
                return rs;

            List<string> lsBookingCode = new List<string>();

            foreach (var e in lsBaseInfo)
            {
                lsBookingCode.Add(e.booking_sn);
            }

            rs = await ShopeeOrderGetBookingDetailFromListBookingSNAllAsync(lsBookingCode);
            await ShopeeGetTrackingNumber.GetBookingTrackingNumberFromDBAsync(rs, conn);
            return rs;
        }

        public static async Task<List<ShopeeBookingDetail>> ShopeeOrderGetBookingDetailToPickUpAsync(
            DateTime time_from,
            DateTime time_to,
            MySqlConnection conn)
        {
            List<ShopeeBookingDetail> lsShopeeShopeeFullInfo = await ShopeeOrderGetBookingDetailAllAsync(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeBookingStatusArray[(int)ShopeeOrderStatus.EnumShopeeBookingStatus.PROCESSED],
                conn);

            lsShopeeShopeeFullInfo.AddRange(
                await ShopeeOrderGetBookingDetailAllAsync(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeBookingStatusArray[(int)ShopeeOrderStatus.EnumShopeeBookingStatus.READY_TO_SHIP],
                conn)
                );

            return lsShopeeShopeeFullInfo;
        }
    }
}
