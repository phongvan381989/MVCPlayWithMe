using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.LazadaAPI;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeLogistic;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaNotification;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeNotification;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Controllers
{
    /// <summary>
    /// Nhận thông báo từ các sàn thương mại điện tử
    /// </summary>
    public class PushMechanismController : Controller
    {
        /// <summary>
        /// Giữ chỗ trong kho khi phát sinh đơn, nên cập nhật lại tồn kho
        /// và số lượng lên các sản phẩm khác trên các nền tảng SHOPEE, TIKI, LAZADA,...
        /// </summary>
        private async Task ShopeeHandleOrderStatusPushAsync(OrderStatusPush orderStatusPush)
        {
            if (orderStatusPush == null) return;

            // Check status: UNPAID, READY_TO_SHIP, CANCELLED, IN_CANCEL đề phòng miss thông báo.
            // UNPAID, READY_TO_SHIP: thông báo này đến trước sự kiện đóng đơn
            // Những trạng thái khác không xử lý. Ví dụ trạng thái SHIPPED. Khi có 100 đơn hàng, shipper lấy hàng quét liên tục
            // sẽ có 100 thông báo gửi về server cùng lúc, nếu ta không return ở đây có thể gây treo server.
            // Trạng thái PROCESSED khi ta xác nhận 100 đơn cùng lúc cũng có thể gây treo server nên ta cũng không xử lý.
            if (orderStatusPush.status != "UNPAID" &&
                orderStatusPush.status != "READY_TO_SHIP" &&// Phục vụ nhắc âm thanh khi có đơn hỏa tốc
                //orderStatusPush.status != "PROCESSED" &&
                //orderStatusPush.status != "IN_CANCEL" &&
                orderStatusPush.status != "TO_RETURN" && // Shop đưa đơn cho shiper nhưng chưa giao cho khách, khách không còn nhu cầu, trả lại nhà bán
                orderStatusPush.status != "CANCELLED") // Hủy đơn
            {
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                ShopeeMySql shopeeMySql = new ShopeeMySql();
                TikiMySql tikiSqler = new TikiMySql();

                TbEcommerceOrder tbEcommerceOrderLastest = await tikiSqler.GetLastestStatusOfECommerceOrderAsync(
                    orderStatusPush.ordersn,
                    EECommerceType.SHOPEE, conn);

                ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceOrderLastest.status;

                ECommerceOrderStatus status = ECommerceOrderStatus.BOOKED;
                if (orderStatusPush.status == "CANCELLED" ||
                    orderStatusPush.status == "TO_RETURN")
                {
                    status = ECommerceOrderStatus.UNBOOKED;

                    string trackingNumber = await tikiSqler.GetTrackingNumberFromSNConnectOutAsync(
                        orderStatusPush.ordersn, EECommerceType.SHOPEE, conn);

                    if (string.IsNullOrEmpty(trackingNumber))
                    {
                        trackingNumber = await ShopeeGetTrackingNumber.ShopeeOrderGetTrackingNumberAsync(orderStatusPush.ordersn, string.Empty);
                        await shopeeMySql.UpdateTrackingNumberToDBConnectOutAsync(orderStatusPush.ordersn, trackingNumber, conn);
                    }
                }

                if (orderStatusPush.status == "READY_TO_SHIP")
                {
                    // Nếu là hỏa tốc và đã được lưu thì cập nhật trạng thái sang ready to ship
                    await tikiSqler.UpdateStatusToReadyToShipTbExpressOrderAsync(orderStatusPush.ordersn, EECommerceType.SHOPEE, conn);
                }

                if (!tikiSqler.IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(status, oldStatus))
                    return;

                // Có trường hợp nhận được event: UNPAID, CANCELLED rồi nhận lại event: UNPAID.
                // Hoặc UNPAID không nhân được do lỗi, nhận được READY_TO_SHIP rồi lại nhận được UNPAID
                // Ta cần check xem có nhận lại event cũ không bởi thời gian event được sàn ghi nhận.
                if (orderStatusPush.update_time < tbEcommerceOrderLastest.updateTime)
                    return;

                try
                {
                    ShopeeOrderDetail shopeeOrderDetail = await ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailFromOrderSNAsync(orderStatusPush.ordersn);

                    if (shopeeOrderDetail != null)
                    {
                        if (CommonOpenPlatform.IsShopeeExpress(shopeeOrderDetail.checkout_shipping_carrier))
                        {
                            if (orderStatusPush.status == "UNPAID")
                            {
                                await tikiSqler.InsertTbExpressOrderAsync(shopeeOrderDetail.order_sn, EECommerceType.SHOPEE, conn);
                            }
                            else if (orderStatusPush.status == "READY_TO_SHIP")
                            {
                                await tikiSqler.InsertTbExpressOrderAsync(shopeeOrderDetail.order_sn, EECommerceType.SHOPEE, conn);
                                await tikiSqler.UpdateStatusToReadyToShipTbExpressOrderAsync(shopeeOrderDetail.order_sn, EECommerceType.SHOPEE, conn);
                            }
                        }

                        CommonOrder commonOrder = new CommonOrder(shopeeOrderDetail);
                        await shopeeMySql.ShopeeGetMappingOfCommonOrderConnectOutAsync(commonOrder, conn);

                        MySqlResultState resultState = await tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOutAsync(
                            commonOrder, status, orderStatusPush.update_time, oldStatus,
                            EECommerceType.SHOPEE, conn);

                        if (resultState != null && resultState.myAnything == 1)
                        {
                            // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                            await ProductController.GetListNeedUpdateQuantityAndUpdate_CoreAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
        }

        // Cập nhật trạng thái Item vào db khi có thay đổi
        private async Task ShopeeHandleViolationItemPushAsync(ViolationItemPush violationItemPush)
        {
            if (violationItemPush != null)
            {
                int status = CommonOpenPlatform.ShopeeGetEnumValueFromString(violationItemPush.item_status);
                ShopeeMySql shopeeMySql = new ShopeeMySql();
                await shopeeMySql.UpdateStatusOfItemFromTMDTItemIdAsync(violationItemPush.item_id, status);
            }
        }

        // Giữ chỗ trong kho, nên cập nhật tồn kho và số lượng lên các sản phẩm khác trên sàn TMDT
        private async Task ShopeeHandleBookingStatusPushAsync(ShopeeBookingSatusPushData data)
        {
            try
            {
                if (data.booking_status != "READY_TO_SHIP" &&
                    data.booking_status != "MATCHED" &&
                    data.booking_status != "CANCELLED")
                {
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    TikiMySql tikiSqler = new TikiMySql();
                    ShopeeMySql shopeeMySql = new ShopeeMySql();
                    ECommerceOrderStatus status = ECommerceOrderStatus.BOOKED;
                    ECommerceOrderStatus oldStatus = ECommerceOrderStatus.DONT_EXIST;
                    if (data.booking_status == "READY_TO_SHIP"
                        || data.booking_status == "CANCELLED")
                    {
                        TbEcommerceOrder tbEcommerceBookingLastest = await tikiSqler.GetLastestStatusOfECommerceBookingAsync(
                            data.booking_sn, EECommerceType.SHOPEE, conn);

                        oldStatus = (ECommerceOrderStatus)tbEcommerceBookingLastest.status;

                        status = ECommerceOrderStatus.BOOKED;
                        if (data.booking_status == "CANCELLED")
                        {
                            status = ECommerceOrderStatus.UNBOOKED;

                            string trackingNumber = await tikiSqler.GetBookingTrackingNumberFromSNConnectOutAsync(
                                data.booking_sn, EECommerceType.SHOPEE, conn);

                            if (string.IsNullOrEmpty(trackingNumber))
                            {
                                trackingNumber = await ShopeeGetTrackingNumber.ShopeeGetBookingTrackingNumberAsync(data.booking_sn);
                                await shopeeMySql.UpdateBookingTrackingNumberToDBConnectOutAsync(data.booking_sn, trackingNumber, conn);
                            }
                        }

                        if (oldStatus == status ||
                            (status == ECommerceOrderStatus.BOOKED && oldStatus == ECommerceOrderStatus.PACKED) ||
                            (status == ECommerceOrderStatus.UNBOOKED && oldStatus == ECommerceOrderStatus.PACKED))
                        {
                            return;
                        }
                    }

                    ShopeeBookingDetail detail =
                        await ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailFromBookingSNAsync(data.booking_sn);
                    if (detail == null) return;

                    CommonOrder commonOrder = new CommonOrder(detail);

                    if (data.booking_status == "MATCHED")
                    {
                        // Đã có đơn hàng được matched, vì khi đơn hàng sinh ra,
                        // (log ở trạng thái UNPAID chưa có booking được matched)
                        // thì đã trừ kho nên giờ cần trả lại hàng về kho

                        // Chờ 120 giây vì có thể shopee vừa gửi push thay đổi trạng thái đơn hàng
                        await Task.Delay(120000);
                        await tikiSqler.ReturnQuantityOfOrderMatchedBookingAsync(commonOrder, EECommerceType.SHOPEE, conn);
                        await ProductController.GetListNeedUpdateQuantityAndUpdate_CoreAsync();
                        return;
                    }

                    await shopeeMySql.ShopeeGetMappingOfCommonOrderConnectOutAsync(commonOrder, conn);

                    MySqlResultState resultState = await tikiSqler.UpdateQuantityOfProductInWarehouseFromBookingConnectOutAsync(
                        commonOrder, status, data.update_time, oldStatus,
                        EECommerceType.SHOPEE, conn);

                    if (resultState != null && resultState.myAnything == 1)
                    {
                        // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                        await ProductController.GetListNeedUpdateQuantityAndUpdate_CoreAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task ThreadShopeeNotificationsAsync(string requestBody)
        {
            try
            {
                MyLogger.GetInstance().Info("ThreadShopeeNotifications get notification");
                MyLogger.GetInstance().Info(requestBody);

                if (string.IsNullOrEmpty(requestBody)) return;

                JObject obj = JObject.Parse(requestBody);
                int code = obj["code"] != null ? (int)obj["code"] : -1;
                if (code == -1)
                {
                    MyLogger.GetInstance().Info("Cant get code from requestBody");
                    return;
                }

                // order_status_push: Lấy thay đổi trạng thái đơn hàng
                if (code == 3)
                {
                    OrderStatusPush orderStatusPush = JsonConvert.DeserializeObject<OrderStatusPush>(obj["data"].ToString());
                    await ShopeeHandleOrderStatusPushAsync(orderStatusPush);
                }
                else if (code == 16)
                {
                    // violation_item_push
                    ViolationItemPush violationItemPush = JsonConvert.DeserializeObject<ViolationItemPush>(obj["data"].ToString());
                    await ShopeeHandleViolationItemPushAsync(violationItemPush);
                }
                else if (code == 23)
                {
                    // booking_status_push
                    ShopeeBookingSatusPushData data = JsonConvert.DeserializeObject<ShopeeBookingSatusPushData>(obj["data"].ToString());
                    await ShopeeHandleBookingStatusPushAsync(data);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn("ThreadShopeeNotifications calling " + ex.ToString());
            }
        }

        // Test với:
        // POST https://voibenho.com/PushMechanism/ShopeeNotifications
        // {"msg_id":"275d49be9b484e63a5cba42115815b4a","data":{"completed_scenario":"","items":[],"ordersn":"2503119YA2XB8B","status":"PROCESSED","update_time":1741645463},"shop_id":137637267,"code":3,"timestamp":1741645463}
        [HttpPost]
        public string ShopeeNotifications()
        {
            try
            {
                string requestBody;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }

                // Đẩy công việc async vào nền, trả 200 ngay lập tức
                Task.Run(async () => await ThreadShopeeNotificationsAsync(requestBody));
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn("ShopeeNotifications calling " + ex.ToString());
            }

            Response.StatusCode = 200;
            return string.Empty;
        }

        private async Task LazadaHandleOrderStatusPushAsync(LazadaOrderStatusPush orderStatusPush)
        {
            if (orderStatusPush == null) return;

            // Possible values are unpaid, pending, canceled, ready_to_ship,
            // delivered, returned, shipped , failed, topack,toship,shipping and lost
            // Check status: unpaid, pending, topack, canceled đề phòng miss thông báo.
            if (orderStatusPush.order_status != "unpaid" &&
                orderStatusPush.order_status != "pending" &&
                orderStatusPush.order_status != "canceled")
            {
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                TikiMySql tikiSqler = new TikiMySql();
                LazadaMySql lazadaMySql = new LazadaMySql();

                TbEcommerceOrder tbEcommerceOrderLastest = await tikiSqler.GetLastestStatusOfECommerceOrderAsync(
                    orderStatusPush.trade_order_id,
                    EECommerceType.LAZADA, conn);

                ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceOrderLastest.status;

                ECommerceOrderStatus status = ECommerceOrderStatus.BOOKED;
                if (orderStatusPush.order_status == "canceled")
                {
                    status = ECommerceOrderStatus.UNBOOKED;
                }

                if (!tikiSqler.IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(status, oldStatus))
                    return;

                // Ta cần check xem có nhận lại event cũ, duplicate không bởi thời gian event được sàn ghi nhận.
                if (orderStatusPush.status_update_time <= tbEcommerceOrderLastest.updateTime)
                    return;

                try
                {
                    long order_id = Common.ConvertStringToInt64(orderStatusPush.trade_order_id);
                    if (order_id < 0) return;

                    List<LazadaOrderItem> orderItems = await LazadaOrderAPI.LazadaGetOrderItemsAsync(order_id);
                    if (orderItems.Count != 0)
                    {
                        LazadaOrder order = new LazadaOrder();
                        order.order_id = orderItems[0].order_id;
                        order.orderItems = orderItems;

                        CommonOrder commonOrder = new CommonOrder(order);
                        await lazadaMySql.LazadaGetMappingOfCommonOrderConnectOutAsync(commonOrder, conn);

                        MySqlResultState resultState = await tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOutAsync(
                            commonOrder, status, orderStatusPush.status_update_time, oldStatus,
                            EECommerceType.LAZADA, conn);

                        if (resultState != null && resultState.myAnything == 1)
                        {
                            // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                            await ProductController.GetListNeedUpdateQuantityAndUpdate_CoreAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
        }

        public async Task ThreadLazadaNotificationsAsync(string requestBody)
        {
            try
            {
                MyLogger.GetInstance().Info("ThreadLazadaNotifications get notification");
                MyLogger.GetInstance().Info(requestBody);

                if (string.IsNullOrEmpty(requestBody)) return;

                JObject obj = JObject.Parse(requestBody);
                int message_type = obj["message_type"] != null ? (int)obj["message_type"] : -1;
                if (message_type == -1)
                {
                    MyLogger.GetInstance().Info("Lazada cant get message_type from requestBody");
                    return;
                }

                // Webhook API Trade Order Notifications: Lấy thay đổi trạng thái đơn hàng
                if (message_type == 0)
                {
                    // Thực tế ghi log thấy lazada đang duplicate thông báo cùng thời điểm(bắn quá nhiều notification giống nhau ở cùng thời điểm)
                    // Vì cùng thời điểm nên chưa kịp lưu thông tin vào db, làm trừ kho hơn một lần
                    // Giải quyết: Ngay lập tức lấy trade_order_id id, status_update_time và so sánh với biến cũ
                    LazadaOrderStatusPush orderStatusPush = JsonConvert.DeserializeObject<LazadaOrderStatusPush>(obj["data"].ToString());
                    await LazadaHandleOrderStatusPushAsync(orderStatusPush);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn("ThreadLazadaNotifications calling " + ex.ToString());
            }
        }

        // Test với:
        // POST https://voibenho.com/PushMechanism/LazadaNotifications
        //
        [HttpPost]
        public string LazadaNotifications()
        {
            try
            {
                string requestBody;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }

                // Đẩy công việc async vào nền, trả 200 ngay lập tức
                Task.Run(async () => await ThreadLazadaNotificationsAsync(requestBody));
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn("LazadaNotifications calling " + ex.ToString());
            }

            Response.StatusCode = 200;
            return string.Empty;
        }
    }
}