using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeLogistic;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using MVCPlayWithMe.OpenPlatform.Model;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder
{
    public class ShopeeGetOrderDetail
    {
        /// <summary>
        /// Lấy thông tin chi tiết danh sách đơn hàng theo thời gian và trạng thái đơn hàng
        /// </summary>
        public static async Task<List<ShopeeOrderDetail>> ShopeeOrderGetOrderDetailAllAsync(
            DateTime time_from,
            DateTime time_to,
            string status,
            MySqlConnection conn)
        {
            List<ShopeeOrderDetail> rs = new List<ShopeeOrderDetail>();
            List<ShopeeGetOrderListBaseInfo> lsBaseInfo = await ShopeeGetOrderList.ShopeeOrderGetOrderListBaseAllAsync(time_from, time_to, status);
            if (lsBaseInfo.Count() == 0)
                return rs;

            List<string> lsOrderCode = new List<string>();
            foreach (ShopeeGetOrderListBaseInfo e in lsBaseInfo)
            {
                lsOrderCode.Add(e.order_sn);
            }

            rs = await ShopeeOrderGetOrderDetailFromListOrderSNAllAsync(lsOrderCode);
            await ShopeeGetTrackingNumber.GetTrackingNumberFromDBAsync(rs, conn);
            return rs;
        }

        public static async Task<List<ShopeeOrderDetail>> ShopeeOrderGetOrderDetailToPickUpAsync(
            DateTime time_from,
            DateTime time_to,
            MySqlConnection conn)
        {
            List<ShopeeOrderDetail> lsOrderShopeeFullInfo = await ShopeeOrderGetOrderDetailAllAsync(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.PROCESSED],
                conn);

            lsOrderShopeeFullInfo.AddRange(
                await ShopeeOrderGetOrderDetailAllAsync(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.READY_TO_SHIP],
                conn)
                );

            lsOrderShopeeFullInfo.AddRange(
                await ShopeeOrderGetOrderDetailAllAsync(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.UNPAID],
                conn)
                );

            //lsOrderShopeeFullInfo.AddRange(
            //    await ShopeeOrderGetOrderDetailAllAsync(
            //    time_from,
            //    time_to,
            //    ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.RETRY_SHIP],
            //    conn)
            //    );

            return lsOrderShopeeFullInfo;
        }

        /// <summary>
        /// Lấy thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<List<ShopeeOrderDetail>> ShopeeOrderGetOrderDetailAsync(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/order/get_order_detail";

            IRestResponse response = await CommonShopeeAPI.ShopeeGetMethodAsync(path, ls);
            if (response == null)
                return null;

            ShopeeGetOrderDetailResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetOrderDetailResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }
            if (objResponse == null
                || objResponse.response == null
                || objResponse.response.order_list == null)
                return null;

            return objResponse.response.order_list;
        }

        /// <summary>
        /// Lấy thông tin chi tiết danh sách đơn hàng từ danh sách mã đơn hàng
        /// </summary>
        /// <param name="order_sn_list">order_sn_list số phần tử phải nhỏ hơn hoặc bằng 50</param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<List<ShopeeOrderDetail>> ShopeeOrderGetOrderDetailFromListOrderSNAsync(
            List<string> order_sn_list)
        {
            if (order_sn_list == null)
                return null;

            StringBuilder strorder_sn_list = new StringBuilder();
            foreach (string s in order_sn_list)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                strorder_sn_list.Append(s);
                strorder_sn_list.Append(",");
            }
            strorder_sn_list.Remove(strorder_sn_list.Length - 1, 1);
            List<DevNameValuePair> ls = new List<DevNameValuePair>();

            ls.Add(new DevNameValuePair("order_sn_list", strorder_sn_list.ToString()));
            ls.Add(new DevNameValuePair("response_optional_fields", "buyer_user_id,buyer_username,estimated_shipping_fee,recipient_address,actual_shipping_fee,goods_to_declare,note,note_update_time,item_list,pay_time,dropshipper,dropshipper_phone,split_up,buyer_cancel_reason,cancel_by,cancel_reason,actual_shipping_fee_confirmed,buyer_cpf_id,fulfillment_flag,pickup_done_time,package_list,shipping_carrier,payment_method,total_amount,buyer_username,invoice_data,checkout_shipping_carrier,reverse_shipping_fee"));

            return await ShopeeOrderGetOrderDetailAsync(ls);
        }

        // Lấy thông tin chi tiết đơn hàng từ mã đơn hàng
        public static async Task<ShopeeOrderDetail> ShopeeOrderGetOrderDetailFromOrderSNAsync(string orderSN)
        {
            if (string.IsNullOrEmpty(orderSN))
            {
                return null;
            }

            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("order_sn_list", orderSN));
            ls.Add(new DevNameValuePair("response_optional_fields", "buyer_user_id,buyer_username,estimated_shipping_fee,recipient_address,actual_shipping_fee,goods_to_declare,note,note_update_time,item_list,pay_time,dropshipper,dropshipper_phone,split_up,buyer_cancel_reason,cancel_by,cancel_reason,actual_shipping_fee_confirmed,buyer_cpf_id,fulfillment_flag,pickup_done_time,package_list,shipping_carrier,payment_method,total_amount,buyer_username,invoice_data,checkout_shipping_carrier,reverse_shipping_fee"));

            List<ShopeeOrderDetail> lsOrderDetail = await ShopeeOrderGetOrderDetailAsync(ls);
            if (lsOrderDetail == null)
            {
                return null;
            }

            return lsOrderDetail[0];
        }

        /// <summary>
        /// Lấy thông tin chi tiết danh sách đơn hàng từ danh sách mã đơn hàng
        /// </summary>
        /// <param name="order_sn_list"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static async Task<List<ShopeeOrderDetail>> ShopeeOrderGetOrderDetailFromListOrderSNAllAsync(
            List<string> order_sn_list)
        {
            List<ShopeeOrderDetail> rs = new List<ShopeeOrderDetail>();
            if (order_sn_list == null || order_sn_list.Count() == 0)
            {
                return rs;
            }

            int range = 50; // Giới hạn

            int indexMax = order_sn_list.Count() - 1;
            int index = 0;
            List<string> order_sn_listTemp = null;
            while (index <= indexMax)
            {
                if (indexMax - index >= range)
                    order_sn_listTemp = order_sn_list.GetRange(index, range);
                else
                    order_sn_listTemp = order_sn_list.GetRange(index, indexMax - index + 1);
                List<ShopeeOrderDetail> rsTemp = await ShopeeOrderGetOrderDetailFromListOrderSNAsync(order_sn_listTemp);
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
        /// Từ danh sách đơn hàng lấy được mã sản phẩm kho (thực tế) và số lượng tương ứng cần xuất kho
        /// </summary>
        public static Dictionary<string, int> ShopeeGetDictionaryOfProductQuantityFromListDetailOrder(List<ShopeeOrderDetail> lsOrderShopeeFullInfo)
        {
            //if (lsOrderShopeeFullInfo == null || lsOrderShopeeFullInfo.Count() == 0)
                return null;
        }
    }
}
