using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder
{
    public class ShopeeGetOrderList
    {
        /// <summary>
        /// Lấy thông tin cơ bản danh sách đơn hàng
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static ShopeeGetOrderListResponseHTTP ShopeeOrderGetOrderListBase(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/order/get_order_list";
            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
            if (response == null)
                return null;

            ShopeeGetOrderListResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetOrderListResponseHTTP>(response.Content, settings);
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
        /// Lấy thông tin cơ bản danh sách đơn hàng trong khoảng thời gian nhỏ hơn 15 ngày
        /// </summary>
        /// <returns>null nếu không lấy thành công</returns>
        public static List<ShopeeGetOrderListBaseInfo> ShopeeOrderGetOrderListBaseAllWithSmallTimeRange(long time_from, long time_to, ShopeeOrderStatus status)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            // Required
            // The kind of time_from and time_to. Available value: create_time, update_time.
            ls.Add(new DevNameValuePair("time_range_field", "create_time"));

            // Required
            // The time_from and time_to fields specify a date range for retrieving orders (based on the time_range_field).
            // The time_from field is the starting date range.The maximum date range that may be specified with the time_from and time_to fields is 15 days.
            ls.Add(new DevNameValuePair("time_from", time_from.ToString()));

            // Required
            // The time_from and time_to fields specify a date range for retrieving orders (based on the time_range_field).
            // The time_from field is the starting date range. The maximum date range that may be specified with the time_from and time_to fields is 15 days.
            ls.Add(new DevNameValuePair("time_to", time_to.ToString()));

            // Required
            // Each result set is returned as a page of entries. Use the "page_size" filters to control the maximum number of entries to retrieve per page (i.e., per call).
            // This integer value is used to specify the maximum number of entries to return in a single "page" of data.The limit of page_size if between 1 and 100.
            ls.Add(new DevNameValuePair("page_size", "20"));

            // The order_status filter for retriveing orders and each one only every request.
            // Available value: UNPAID/READY_TO_SHIP/PROCESSED/SHIPPED/COMPLETED/IN_CANCEL/CANCELLED/INVOICE_PENDING
            if (status.index != ShopeeOrderStatus.EnumShopeeOrderStatus.ALL)
                ls.Add(new DevNameValuePair("order_status", status.GetString()));

            // Optional fields in response. Available value: order_status.
            ls.Add(new DevNameValuePair("response_optional_fields", "order_status"));

            // Specifies the starting entry of data to return in the current call. Default is "".
            // If data is more than one page, the offset can be some entry to start next call.
            ls.Add(new DevNameValuePair("cursor", "")); // Thêm vào sau cùng list để tiện cập nhật

            string strCursor = "";
            List<ShopeeGetOrderListBaseInfo> rs = new List<ShopeeGetOrderListBaseInfo>();
            Boolean isOK = true;
            while (true)
            {
                ls.RemoveAt(ls.Count() - 1);
                ls.Add(new DevNameValuePair("cursor", strCursor));

                ShopeeGetOrderListResponseHTTP orderListTemp = ShopeeOrderGetOrderListBase(ls);
                if (orderListTemp == null)
                {
                    isOK = false;
                    break;
                }

                if (orderListTemp.response == null ||
                    orderListTemp.response.order_list == null ||
                    orderListTemp.response.order_list.Count() == 0)
                {
                    isOK = false;
                    break;
                }
                rs.AddRange(orderListTemp.response.order_list);
                if (!orderListTemp.response.more)
                    break;
                strCursor = orderListTemp.response.next_cursor;
            }
            if (!isOK)
                return null;
            return rs;
        }

        /// <summary>
        /// Lấy thông tin cơ bản danh sách đơn hàng trong khoảng thời gian không giới hạn
        /// </summary>
        /// <param name="time_from"></param>
        /// <param name="time_to"></param>
        /// <param name="status"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static List<ShopeeGetOrderListBaseInfo> ShopeeOrderGetOrderListBaseAll(DateTime time_from, DateTime time_to, ShopeeOrderStatus status)
        {
            long ltime_from = Common.GetTimestamp(time_from);
            long ltime_to = Common.GetTimestamp(time_to);
            long ltime_toTemp;
            List<ShopeeGetOrderListBaseInfo> rs = new List<ShopeeGetOrderListBaseInfo>();
            Boolean isOK = true;
            // Ta lấy trong khoảng thời gian là 14 ngày. Khoảng max là 15 ngày
            long rangeTime = 14 * 24 * 60 * 60;
            while (ltime_from < ltime_to)
            {
                ltime_toTemp = ltime_from + rangeTime;
                if (ltime_toTemp > ltime_to)
                    ltime_toTemp = ltime_to;
                List<ShopeeGetOrderListBaseInfo> rsTemp = ShopeeOrderGetOrderListBaseAllWithSmallTimeRange(ltime_from, ltime_toTemp, status);
                if (rsTemp == null)
                {
                    isOK = false;
                    break;
                }
                ltime_from = ltime_from + rangeTime;
                rs.AddRange(rsTemp);
            }
            if (!isOK)
                return null;

            return rs;
        }

    }
}
