using Microsoft.Extensions.Hosting;
using MVCPlayWithMe.Controllers;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Order;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Event;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI.Event
{
    public class TikiPullEventService
    {
        // Xử lý event của đơn hàng
        private void HandleOrderEvent(TikiEvent tikiEvent, TikiMySql tikiSqler, MySqlConnection conn)
        {
            if (tikiEvent.type != "ORDER_CREATED_SUCCESSFULLY" &&
                tikiEvent.payload.status != "canceled")
            {
                return;
            }

            TbEcommerceOrder tbEcommerceOrder =
             tikiSqler.GetLastestStatusOfECommerceOrder(
                tikiEvent.payload.order_code,
                EECommerceType.TIKI, conn);
            ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceOrder.status;

            ECommerceOrderStatus status = ECommerceOrderStatus.BOOKED;

            if (tikiEvent.payload.status == "canceled")
            {
                status = ECommerceOrderStatus.UNBOOKED;
            }

            if (!tikiSqler.IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(status, oldStatus))
            {
                return;
            }

            try
            {
                // Giữ chỗ nếu đơn hàng vừa sinh ra.
                // Hủy giữ chỗ nếu đơn hàng bị khách hủy và đang ở trạng thái giữ chỗ.
                TikiOrder tikiOrder = TikiGetListOrders.TikiGetOrderFromCode(tikiEvent.payload.order_code);

                if (tikiOrder != null)
                {
                    CommonOrder commonOrder = new CommonOrder(tikiOrder);
                    tikiSqler.TikiGetMappingOfCommonOrderConnectOut(commonOrder, conn);

                    // Nếu đơn hàng vừa sinh ra, ta lưu id của item, model trong đơn.
                    if (tikiEvent.type == "ORDER_CREATED_SUCCESSFULLY")
                    {
                        tikiSqler.InsertTbItemOfEcommerceOder(commonOrder, EECommerceType.TIKI, conn);
                    }
                    else // if (tikiEvent.payload.status == "canceled") // Hủy đơn
                    {
                        tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOder(commonOrder, EECommerceType.TIKI, conn);
                    }

                    MySqlResultState resultState = tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOut(
                        commonOrder, status, tikiEvent.created_at, oldStatus,
                        EECommerceType.TIKI, conn);

                    if (resultState != null && resultState.myAnything == 1)
                    {
                        // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                        ProductController productController = new ProductController();
                        productController.GetListNeedUpdateQuantityAndUpdate_Core();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        private List<TikiEvent> GetListOrderEventFromEventTypeAll(List<TikiEvent> listTikiEvent)
        {
            // Vì trong khoảng thời gian nghỉ đơn hàng có thể thay đổi trạng thái vài lần
            // Ta chỉ lấy event có trạng thái mới nhất căn cứ vào thời điểm thông báo được tạo
            // Ta lấy từ cuối list vì server tiki trả về event từ cũ tới mới
            List<TikiEvent> listOrderEvent = new List<TikiEvent>();
            if (listTikiEvent == null || listTikiEvent.Count == 0)
            {
                return listOrderEvent;
            }

            bool isAdd = true;
            for (int i = listTikiEvent.Count - 1; i >= 0; i--)
            {
                if (listTikiEvent[i].type != "ORDER_CREATED_SUCCESSFULLY" &&
                    listTikiEvent[i].type != "ORDER_STATUS_UPDATED")
                {
                    continue;
                }

                isAdd = true;
                for (int j = 0; j < listOrderEvent.Count; j++)
                {
                    if (listTikiEvent[i].payload.order_code == listOrderEvent[j].payload.order_code)
                    {
                        isAdd = false;
                        break;
                    }
                }
                if (isAdd)
                {
                    listOrderEvent.Add(listTikiEvent[i]);
                }
            }

            return listOrderEvent;
        }

        // Xử lý event thay đổi tồn kho
        private void HandleProductInventoryUpdateEvent(
            TikiEvent tikiEvent,
            TikiDealDiscountMySql tikiSqler,
            MySqlConnection conn)
        {
            // Comment vì mỗi ngày check lại toàn bộ deal đang chạy
            //MyLogger.GetInstance().Info("Start HandleProductInventoryUpdateEvent");
            //if (tikiEvent.payload.quantity_sellable_to == Common.minQuantityOfDealTiki)
            //{
            //    MyLogger.GetInstance().Info("Start quantity_sellable_to = 0. Deal will turn off.");
            //    MyLogger.GetInstance().Info(JsonConvert.SerializeObject(tikiEvent));
            //    // Tồn kho bằng 0, sản phẩm hết hàng. Tiki sẽ tắt deal đang chạy.
            //    // Có thể trạng thái deal đã được cập nhật trực tiếp khi tồn kho trong kho giảm về 0 do trừ tồn kho theo đơn,
            //    // nhưng ta vẫn cập nhật ở đây lại cho chắc.
            //    tikiSqler.UpdateIsActiveCloseFromItemId(tikiEvent.payload.product_id, conn);
            //}
            //MyLogger.GetInstance().Info("Start End");
        }

        private List<TikiEvent> GetListProductInventoryUpdateFromEventTypeAll(List<TikiEvent> listTikiEvent)
        {
            List<TikiEvent> listProductInventoryUpdate = new List<TikiEvent>();
            if (listTikiEvent == null || listTikiEvent.Count == 0)
            {
                return listProductInventoryUpdate;
            }

            for (int i = 0; i < listTikiEvent.Count; i++)
            {
                if (listTikiEvent[i].type != "PRODUCT_INVENTORY_UPDATED")
                {
                    continue;
                }
                listProductInventoryUpdate.Add(listTikiEvent[i]);
            }

            return listProductInventoryUpdate;
        }

        public void DoWork(MySqlConnection conn)
        {
            // Ghi log
            MyLogger.GetInstance().Info("Start pulling Tiki events");
            // Lấy ack_id từ db
            try
            {
                TikiMySql tikiMySqler = new TikiMySql();
                TikiDealDiscountMySql tikiDealSqler = new TikiDealDiscountMySql();
                string ack_id = tikiMySqler.GetAckIdOfLastestPullConnectOut(conn);
                Event_Response event_Response = TikiPullEvent(ack_id);

                if (event_Response != null)
                {
                    tikiMySqler.UpdateAckIdOfLastestPullConnectOut(event_Response.ack_id, conn);

                    List<TikiEvent> listOrderEvent = GetListOrderEventFromEventTypeAll(event_Response.events);
                    foreach (var e in listOrderEvent)
                    {
                        HandleOrderEvent(e, tikiMySqler, conn);
                    }

                    //List<TikiEvent> listProductInventoryUpdate = GetListProductInventoryUpdateFromEventTypeAll(event_Response.events);

                    //MyLogger.GetInstance().Info(JsonConvert.SerializeObject(listProductInventoryUpdate));
                    //foreach (var e in listProductInventoryUpdate)
                    //{
                    //    HandleProductInventoryUpdateEvent(e, tikiDealSqler, conn);
                    //}
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            MyLogger.GetInstance().Info("End pulling Tiki events");
        }

        public Event_Response TikiPullEvent(string lastedAcK_Id)
        {
            Event_Response eventResponse = null;

            // Thêm body vào request (dưới dạng JSON)
            //{
            //    "ack_id": "c77c60d7-8a4a-4377-a852-96d249bdb42f"
            //}
            if (string.IsNullOrWhiteSpace(lastedAcK_Id))
            {
                lastedAcK_Id = "null";
            }
            else
            {
                lastedAcK_Id = "\"" + lastedAcK_Id + "\"";
            }
            string body = "{\"ack_id\":" + lastedAcK_Id + "}";

            string http = TikiConstValues.cstrPullEvent;
            IRestResponse response = CommonTikiAPI.PostExcuteRequest(http, body);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = response.Content;

                // Mẫu json response
                {
                    // Mẫu response ORDER_CREATED_SUCCESSFULLY:
                    //{
                    //    "events": [
                    //        {
                    //            "id": "11795810-c11a-490e-b36b-8a0f9d945bdc",
                    //            "sid": "74334E866033E601CD908A23D96E12B220EC0F68",
                    //            "created_at": 1740401636695,
                    //            "payload": {
                    //                "order_code": "510916693"
                    //            },
                    //            "type": "ORDER_CREATED_SUCCESSFULLY",
                    //            "version": "v1"
                    //        }
                    //    ],
                    //    "ack_id": "c068d557-35c9-4bed-9b2d-add8c199c8cb"
                    //}
                    //
                    //
                    // Mẫu response ORDER_STATUS_UPDATED:
                    //{
                    //    "events": [
                    //        {
                    //            "id": "a061c904-eb20-4b89-936d-e47a6ab59696",
                    //            "sid": "74334E866033E601CD908A23D96E12B220EC0F68",
                    //            "created_at": 1740438118300,
                    //            "payload": {
                    //                "order_code": "510916693",
                    //                "orderCode": "510916693",
                    //                "status": "picking"
                    //            },
                    //            "type": "ORDER_STATUS_UPDATED",
                    //            "version": "v1"
                    //        }
                    //    ],
                    //    "ack_id": "4c63bbb7-2fc2-433a-b1d4-322db7d34095"
                    //}

                    // Event type: PRODUCT_INVENTORY_UPDATED
                    //{
                    //    "created_at": 1748490404923,
                    //    "id": "7a43592e-911b-4674-bef9-96d4f18a4cb2",
                    //    "payload": {
                    //                    "original_sku": "",
                    //        "product_id": 71925545,
                    //        "quantity_available_from": 167,
                    //        "quantity_available_to": 167,
                    //        "quantity_from": 0,
                    //        "quantity_reserved_from": 0,
                    //        "quantity_reserved_to": 2,
                    //        "quantity_sellable_from": 167,
                    //        "quantity_sellable_to": 165,
                    //        "quantity_to": 0,
                    //        "warehouse_id": 387453
                    //    },
                    //    "sid": "74334E866033E601CD908A23D96E12B220EC0F68",
                    //    "type": "PRODUCT_INVENTORY_UPDATED",
                    //    "version": "v2"
                    //}
                }
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    eventResponse = JsonConvert.DeserializeObject<Event_Response>(json, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                }
            }

            return eventResponse;
        }
    }
}