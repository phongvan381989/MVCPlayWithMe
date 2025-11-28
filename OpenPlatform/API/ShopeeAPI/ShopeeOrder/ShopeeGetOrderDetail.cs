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
        /// <param name="time_from"></param>
        /// <param name="time_to"></param>
        /// <param name="status">Theo từng trại thái hoặc tất cả trạng thái</param>
        /// <returns>null nếu không lấy thành công</returns>
        public static List<ShopeeOrderDetail> ShopeeOrderGetOrderDetailAll(
            DateTime time_from,
            DateTime time_to,
            string status,
            MySqlConnection conn)
        {
            List<ShopeeOrderDetail> rs = new List<ShopeeOrderDetail>();
            List<ShopeeGetOrderListBaseInfo> lsBaseInfo = ShopeeGetOrderList.ShopeeOrderGetOrderListBaseAll(time_from, time_to, status);
            if (lsBaseInfo.Count() == 0)
                return rs;

            // Tạo list mã order
            List<string> lsOrderCode = new List<string>();
            foreach (ShopeeGetOrderListBaseInfo e in lsBaseInfo)
            {
                lsOrderCode.Add(e.order_sn);
            }

            rs = ShopeeOrderGetOrderDetailFromListOrderSNAll(lsOrderCode);
            ShopeeGetTrackingNumber.GetTrackingNumberFromDB(rs, conn);
            return rs;
        }

        public static List<ShopeeOrderDetail> ShopeeOrderGetOrderDetailToPickUp(
            DateTime time_from,
            DateTime time_to,
            MySqlConnection conn)
        {
            List<ShopeeOrderDetail> lsOrderShopeeFullInfo = ShopeeOrderGetOrderDetailAll(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.PROCESSED],
                conn);

            lsOrderShopeeFullInfo.AddRange(
                ShopeeOrderGetOrderDetailAll(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.READY_TO_SHIP],
                conn)
                );

            lsOrderShopeeFullInfo.AddRange(
                ShopeeOrderGetOrderDetailAll(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.UNPAID],
                conn)
                );

            lsOrderShopeeFullInfo.AddRange(
                ShopeeOrderGetOrderDetailAll(
                time_from,
                time_to,
                ShopeeOrderStatus.shopeeOrderStatusArray[(int)ShopeeOrderStatus.EnumShopeeOrderStatus.RETRY_SHIP],
                conn)
                );

            return lsOrderShopeeFullInfo;
        }

        /// <summary>
        /// Lấy thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>null nếu không lấy thành công</returns>
        public static List<ShopeeOrderDetail> ShopeeOrderGetOrderDetail(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/order/get_order_detail";

            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
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
        public static List<ShopeeOrderDetail> ShopeeOrderGetOrderDetailFromListOrderSN(
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
            // Xóa "," cuối cùng
            strorder_sn_list.Remove(strorder_sn_list.Length - 1, 1);
            List<DevNameValuePair> ls = new List<DevNameValuePair>();

            // Required
            // The set of order_sn. If there are multiple order_sn, you need to use English comma to connect them. limit [1,50]
            ls.Add(new DevNameValuePair("order_sn_list", strorder_sn_list.ToString()));

            // Indicate response fields you want to get. Please select from the below response parameters. 
            // If you input an object field, all the params under it will be included automatically in the response.
            // If there are multiple response fields you want to get, you need to use English comma to connect them.
            // Available values: buyer_user_id,buyer_username,estimated_shipping_fee,recipient_address,actual_shipping_fee ,goods_to_declare,note,note_update_time,item_list,pay_time,dropshipper,dropshipper_phone,split_up,buyer_cancel_reason,cancel_by,cancel_reason,actual_shipping_fee_confirmed,buyer_cpf_id,fulfillment_flag,pickup_done_time,package_list,shipping_carrier,payment_method,total_amount,buyer_username,invoice_data, checkout_shipping_carrier, reverse_shipping_fee etc.
            ls.Add(new DevNameValuePair("response_optional_fields", "buyer_user_id,buyer_username,estimated_shipping_fee,recipient_address,actual_shipping_fee,goods_to_declare,note,note_update_time,item_list,pay_time,dropshipper,dropshipper_phone,split_up,buyer_cancel_reason,cancel_by,cancel_reason,actual_shipping_fee_confirmed,buyer_cpf_id,fulfillment_flag,pickup_done_time,package_list,shipping_carrier,payment_method,total_amount,buyer_username,invoice_data,checkout_shipping_carrier,reverse_shipping_fee"));

            return ShopeeOrderGetOrderDetail(ls);
        }

        // Lấy thông tin chi tiết đơn hàng từ mã đơn hàng
        public static ShopeeOrderDetail ShopeeOrderGetOrderDetailFromOrderSN(string orderSN)
        {
            if (string.IsNullOrEmpty(orderSN))
            {
                return null;
            }

            List<DevNameValuePair> ls = new List<DevNameValuePair>();

            // Required
            // The set of order_sn. If there are multiple order_sn, you need to use English comma to connect them. limit [1,50]
            ls.Add(new DevNameValuePair("order_sn_list", orderSN));

            // Indicate response fields you want to get. Please select from the below response parameters. 
            // If you input an object field, all the params under it will be included automatically in the response.
            // If there are multiple response fields you want to get, you need to use English comma to connect them.
            // Available values: buyer_user_id,buyer_username,estimated_shipping_fee,recipient_address,actual_shipping_fee ,goods_to_declare,note,note_update_time,item_list,pay_time,dropshipper,dropshipper_phone,split_up,buyer_cancel_reason,cancel_by,cancel_reason,actual_shipping_fee_confirmed,buyer_cpf_id,fulfillment_flag,pickup_done_time,package_list,shipping_carrier,payment_method,total_amount,buyer_username,invoice_data, checkout_shipping_carrier, reverse_shipping_fee etc.
            ls.Add(new DevNameValuePair("response_optional_fields", "buyer_user_id,buyer_username,estimated_shipping_fee,recipient_address,actual_shipping_fee,goods_to_declare,note,note_update_time,item_list,pay_time,dropshipper,dropshipper_phone,split_up,buyer_cancel_reason,cancel_by,cancel_reason,actual_shipping_fee_confirmed,buyer_cpf_id,fulfillment_flag,pickup_done_time,package_list,shipping_carrier,payment_method,total_amount,buyer_username,invoice_data,checkout_shipping_carrier,reverse_shipping_fee"));

            List<ShopeeOrderDetail> lsOrderDetail = ShopeeOrderGetOrderDetail(ls);
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
        public static List<ShopeeOrderDetail> ShopeeOrderGetOrderDetailFromListOrderSNAll(
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
                List<ShopeeOrderDetail> rsTemp = ShopeeOrderGetOrderDetailFromListOrderSN(order_sn_listTemp);
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
        /// <param name="lsOrderShopeeFullInfo"></param>
        /// <returns></returns>
        public static Dictionary<string, int> ShopeeGetDictionaryOfProductQuantityFromListDetailOrder(List<ShopeeOrderDetail> lsOrderShopeeFullInfo)
        {
            //if (lsOrderShopeeFullInfo == null || lsOrderShopeeFullInfo.Count() == 0)
                return null;

            //// Danh sách sản phẩm trên sàn thương mại điện tử và số lượng
            //Dictionary<string, int> lsProdtuctOnTMDT = new Dictionary<string, int>();
            //string codeTemp = string.Empty;
            //foreach (ShopeeOrderDetail eDetail in lsOrderShopeeFullInfo)
            //{
            //    foreach (ShopeeGetOrderDetailItem e in eDetail.item_list)
            //    {
            //        codeTemp = e.model_id.ToString();
            //        if (lsProdtuctOnTMDT.ContainsKey(codeTemp))
            //        {
            //            lsProdtuctOnTMDT[codeTemp] = lsProdtuctOnTMDT[codeTemp] + e.model_quantity_purchased;
            //        }
            //        else
            //            lsProdtuctOnTMDT.Add(e.model_id.ToString(), e.model_quantity_purchased);
            //    }
            //}

            //// Từ mã sản phẩm trên sàn thương mại điện tử / số lượng lấy được mã sản phẩm trong kho thực tế / số lượng
            //Dictionary<string, int> lsProdtuctInWarehouse = new Dictionary<string, int>();
            //foreach (KeyValuePair<string, int> ite in lsProdtuctOnTMDT)
            //{
            //    List<ModelMappingSanPhamTMDT_SanPhamKho> ls = null;
            //    ls = ModelMappingSanPhamTMDT_SanPhamKho.Shopee_GetListModelMappingSanPhamTMDT_SanPhamKhoFromIDProTMDT(ite.Key);
            //    foreach (ModelMappingSanPhamTMDT_SanPhamKho obj in ls)
            //    {
            //        codeTemp = obj.code;
            //        if (lsProdtuctInWarehouse.ContainsKey(codeTemp))
            //        {
            //            lsProdtuctInWarehouse[codeTemp] = lsProdtuctInWarehouse[codeTemp] + ite.Value * Common.ConvertStringToInt32(obj.quantity);
            //        }
            //        else
            //        {
            //            lsProdtuctInWarehouse.Add(codeTemp, ite.Value * Common.ConvertStringToInt32(obj.quantity));
            //        }
            //    }

            //}
            //return lsProdtuctInWarehouse;
        }
    }
}
