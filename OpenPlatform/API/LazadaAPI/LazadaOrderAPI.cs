using Lazop.Api;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaOrder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.API.LazadaAPI
{
    public class LazadaOrderAPI
    {
        public static int limitOfGetOrders = 100;

        public static LazadaOrder LazadaGetOrder(long order_id)
        {
            LazadaOrder order = null;
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return order;
            }

            try
            {
                ILazopClient client = CommonLazadaAPI.GetLazopClient();

                LazopRequest request = new LazopRequest();
                request.SetApiName("/order/get");
                request.SetHttpMethod("GET");

                request.AddApiParameter("order_id", order_id.ToString());

                LazopResponse response = client.Execute(request,
                    LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                if (!response.IsError())
                {
                    LazadaGetOrderResponseBody objectRes =
                    JsonConvert.DeserializeObject<LazadaGetOrderResponseBody>(response.Body, Common.jsonSerializersettings);
                    order = objectRes.data;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                order = null;
            }
            return order;
        }

        // status = null or empty để lấy tất cả trạng thái
        public static List<LazadaOrder> LazadaGetOrders(DateTime fromDate,
            string status)
        {
            List<LazadaOrder> ls = new List<LazadaOrder>();
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return ls;
            }

            try
            {
                ILazopClient client = CommonLazadaAPI.GetLazopClient();

                LazopRequest request = new LazopRequest();
                request.SetApiName("/orders/get");
                request.SetHttpMethod("GET");

                //request.AddApiParameter("update_before", "2018-02-10T16:00:00+08:00");

                // Specify the sorting type. Possible values are ASC and DESC.
                request.AddApiParameter("sort_direction", "DESC");

                // Number of orders to skip at the beginning of the list.
                request.AddApiParameter("offset", "0");

                // The maximum number of orders that can be returned. The supported maximum number is 100.
                request.AddApiParameter("limit", limitOfGetOrders.ToString());
                //request.AddApiParameter("update_after", "2017-02-10T09:00:00+08:00");

                // Allows to choose the sorting column. Possible values are created_at and updated_at.
                request.AddApiParameter("sort_by", "created_at");

                //request.AddApiParameter("created_before", "2018-02-10T16:00:00+08:00");
                request.AddApiParameter("created_after",
                    CommonLazadaAPI.FormatDateTimeWithOffset(fromDate));

                //When set, limits the returned set of orders to loose orders, which return only entries which
                //fit the status provided. Possible values are unpaid, pending, canceled, ready_to_ship, 
                //delivered, returned, shipped , failed, topack,toship,shipping and lost
                if (!string.IsNullOrEmpty(status) ||
                    status == LazadaOrder.lazadaOrderStatusArray[(int)LazadaOrder.EnumLazadaOrderStatus.all])
                {
                    request.AddApiParameter("status", status);
                }

                int count = 0;
                int offset = 0;
                //int total_products = -1;
                while (count <= 200)
                {
                    count++;
                    request.UpdateApiParameter("offset", offset.ToString());
                    LazopResponse response = client.Execute(request,
                        LazadaAuthenAPI.lazadaAuthen.accessToken);
                    MyLogger.LazadaRestLog(request, response);

                    if (response.IsError())
                    {
                        break;
                    }

                    LazadaGetOrdersResponseBody objectRes =
                        JsonConvert.DeserializeObject<LazadaGetOrdersResponseBody>(response.Body, Common.jsonSerializersettings);


                    if (objectRes.data == null || objectRes.data.orders == null)
                    {
                        break;
                    }

                    if(objectRes.data.count == 0)
                    {
                        break;
                    }

                    ls.AddRange(objectRes.data.orders);

                    offset = offset + objectRes.data.count;
                    if (objectRes.data.count < limitOfGetOrders ||
                        offset >= objectRes.data.countTotal)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                ls.Clear();
            }
            return ls;
        }

        private static void LazadaGetItemsFromOrders(List<LazadaOrder> orders)
        {
            // Lấy thông tin item trong đơn hàng
            foreach (var order in orders)
            {
                order.orderItems = LazadaGetOrderItems(order.order_id);
            }
        }

        public static LazadaOrder LazadaGetOrderDetail(long order_id)
        {
            LazadaOrder order = null;
            try
            {
                order = LazadaGetOrder(order_id);
                if (order != null)
                {
                    List<LazadaOrder> orders = new List<LazadaOrder>();
                    orders.Add(order);
                    LazadaGetItemsFromOrders(orders);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                order = null;
            }
            return order;
        }

        // Đơn hàng nhà bán cần gửi cho bên vận chuyển
        public static List<LazadaOrder> LazadaGetOrdersDetailReadyToShip(DateTime fromDate)
        {
            // unpaid,
            List<LazadaOrder> orders = LazadaGetOrders(fromDate,
                LazadaOrder.lazadaOrderStatusArray[(int)LazadaOrder.EnumLazadaOrderStatus.unpaid]);

            // pending
            orders.AddRange(LazadaGetOrders(fromDate,
                LazadaOrder.lazadaOrderStatusArray[(int)LazadaOrder.EnumLazadaOrderStatus.pending]));

            // ready_to_ship
            orders.AddRange(LazadaGetOrders(fromDate,
                LazadaOrder.lazadaOrderStatusArray[(int)LazadaOrder.EnumLazadaOrderStatus.ready_to_ship]));

            LazadaGetItemsFromOrders(orders);

            return orders;
        }

        // Đơn hàng bị hủy có trạng thái cancelled
        public static List<LazadaOrder> LazadaGetOrdersDetailCanceled(DateTime fromDate)
        {
            // canceled,
            List<LazadaOrder> orders = LazadaGetOrders(fromDate,
                LazadaOrder.lazadaOrderStatusArray[(int)LazadaOrder.EnumLazadaOrderStatus.canceled]);

            LazadaGetItemsFromOrders(orders);
            return orders;
        }

        // Tất cả đơn hàng
        public static List<LazadaOrder> LazadaGetOrdersDetailAll(DateTime fromDate)
        {
            List<LazadaOrder> orders = LazadaGetOrders(fromDate, null);

            LazadaGetItemsFromOrders(orders);
            return orders;
        }

        public static List<LazadaOrderItem> LazadaGetOrderItems(long order_id)
        {
            List<LazadaOrderItem> orderItems = new List<LazadaOrderItem>();
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return orderItems;
            }

            try
            {
                ILazopClient client = CommonLazadaAPI.GetLazopClient();

                LazopRequest request = new LazopRequest();
                request.SetApiName("/order/items/get");
                request.SetHttpMethod("GET");
                request.AddApiParameter("order_id", order_id.ToString());

                LazopResponse response = client.Execute(request,
                    LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                if (!response.IsError())
                {
                    LazadaGetOrderItemsResponseBody objectRes =
                        JsonConvert.DeserializeObject<LazadaGetOrderItemsResponseBody>(response.Body, Common.jsonSerializersettings);

                    if (objectRes.data != null)
                    {
                        orderItems = objectRes.data;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                orderItems.Clear();
            }
            return orderItems;
        }

        public static void LazadaGetDocument(
            List<LazadaOrder> lazadaOrders
            )
        {
            if (!LazadaAuthenAPI.LazadaRefreshAccessTokenIfNeed())
            {
                return;
            }

            try
            {
                ILazopClient client = CommonLazadaAPI.GetLazopClient();

                LazopRequest request = new LazopRequest();
                // Use this API to retrieve order-related documents, including invoices and shipping labels.
                request.SetApiName("/order/document/get");
                request.SetHttpMethod("GET");
                // Document types, including 'invoice', 'shippingLabel', or 'carrierManifest'. Mandatory.
                request.AddApiParameter("doc_type", "carrierManifest");

                // Identifier of the order item for which the caller wants to get a document. Mandatory.
                // Ex: [279709, 279709]
                request.AddApiParameter("order_item_ids", "[513278818372637]");

                LazopResponse response = client.Execute(request,
                    LazadaAuthenAPI.lazadaAuthen.accessToken);
                MyLogger.LazadaRestLog(request, response);

                if (!response.IsError())
                {

                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                //ls.Clear();
            }
        }
    }
}