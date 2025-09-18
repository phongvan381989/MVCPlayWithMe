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
using MySql.Data.MySqlClient;
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
        private void ShopeeHandleOrderStatusPush(OrderStatusPush orderStatusPush)
        {
            if (orderStatusPush == null)
            {
                return;
            }

            // Check status: UNPAID, READY_TO_SHIP, CANCELLED, IN_CANCEL đề phòng miss thông báo.
            // UNPAID, READY_TO_SHIP: thông báo này đến trước sự kiện đóng đơn
            // Những trạng thái khác không xử lý. Ví dụ trạng thái SHIPPED. Khi có 100 đơn hàng, shipper lấy hàng quét liên tục 
            // sẽ có 100 thông báo gửi về server cùng lúc, nếu ta không return ở đây có thể gây treo server.
            // Trạng thái PROCESSED khi ta xác nhận 100 đơn cùng lúc cũng có thể gây treo server nên ta cũng không xử lý.
            if (orderStatusPush.status != "UNPAID" &&
                orderStatusPush.status != "READY_TO_SHIP" &&// Phục vụ nhắc âm thanh khi có đơn hỏa tốc
                //orderStatusPush.status != "PROCESSED" &&
                orderStatusPush.status != "IN_CANCEL" &&
                //orderStatusPush.status != "TO_RETURN" && // Khách nhận, và trả hàng
                orderStatusPush.status != "CANCELLED") // Hủy đơn
            {
                return;
            }
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            TikiMySql tikiSqler = new TikiMySql();

            TbEcommerceOrder tbEcommerceOrderLastest = tikiSqler.GetLastestStatusOfECommerceOrder(
                orderStatusPush.ordersn,
                EECommerceType.SHOPEE, conn);

            ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceOrderLastest.status;

            ECommerceOrderStatus status = ECommerceOrderStatus.BOOKED;
            if (orderStatusPush.status == "CANCELLED" || orderStatusPush.status == "IN_CANCEL")
            {
                status = ECommerceOrderStatus.UNBOOKED;
                //if (orderStatusPush.status == "CANCELLED")
                //{
                    // Khách hủy và đã có mã vận đơn nhưng chưa được lưu db do chưa xác nhận đã đóng,
                    // ta sẽ lưu vào db ở đây

                    string trackingNumber = tikiSqler.GetTrackingNumberFromSNConnectOut(
                    orderStatusPush.ordersn, EECommerceType.SHOPEE, conn);

                    if (string.IsNullOrEmpty(trackingNumber))
                    {
                        trackingNumber = ShopeeGetTrackingNumber.ShopeeGetShipCode(orderStatusPush.ordersn, string.Empty);
                        shopeeMySql.UpdateTrackingNumberToDBConnectOut(orderStatusPush.ordersn, trackingNumber, conn);
                    }
                //}
            }

            if (orderStatusPush.status == "READY_TO_SHIP")
            {
                // Nếu là hỏa tốc và đã được lưu thì cập nhật trạng thái sang ready to ship
                tikiSqler.UpdateStatusToReadyToShipTbExpressOrder(orderStatusPush.ordersn, EECommerceType.SHOPEE, conn);
            }

            if (!tikiSqler.IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(status, oldStatus))
            {
                return;
            }

            // Có trường hợp nhận được event: UNPAID, CANCELLED rồi nhận lại event: UNPAID.
            // Hoặc UNPAID không nhân được do lỗi, nhận được READY_TO_SHIP rồi lại nhận được UNPAID
            // Ta cần check xem có nhận lại event cũ không bởi thời gian event được sàn ghi nhận.
            if (orderStatusPush.update_time < tbEcommerceOrderLastest.updateTime)
            {
                return;
            }

            try
            {
                // Giữ chỗ nếu đơn hàng vừa sinh ra.
                // Hủy giữ chỗ nếu đơn hàng bị khách hủy và đang ở trạng thái giữ chỗ.
                ShopeeOrderDetail shopeeOrderDetail = ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailFromOrderSN(orderStatusPush.ordersn);

                if (shopeeOrderDetail != null)
                {
                    // Nếu đơn hàng là hỏa tốc, ta thêm vào bảng đơn hỏa tốc
                    if(CommonOpenPlatform.IsShopeeExpress(shopeeOrderDetail.checkout_shipping_carrier))
                    {
                        if(orderStatusPush.status == "UNPAID")
                        {
                            tikiSqler.InsertTbExpressOrder(shopeeOrderDetail.order_sn, EECommerceType.SHOPEE, conn);
                        }
                        else if(orderStatusPush.status == "READY_TO_SHIP")
                        {
                            tikiSqler.InsertTbExpressOrder(shopeeOrderDetail.order_sn, EECommerceType.SHOPEE, conn);
                            tikiSqler.UpdateStatusToReadyToShipTbExpressOrder(shopeeOrderDetail.order_sn, EECommerceType.SHOPEE, conn);
                        }
                    }

                    CommonOrder commonOrder = new CommonOrder(shopeeOrderDetail);
                    shopeeMySql.ShopeeGetMappingOfCommonOrderConnectOut(commonOrder, conn);

                    MySqlResultState resultState = tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOut(
                        commonOrder, status, orderStatusPush.update_time, oldStatus,
                        EECommerceType.SHOPEE, conn);

                    if (resultState != null && resultState.myAnything == 1)
                    {
                        // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                        ProductController.GetListNeedUpdateQuantityAndUpdate_Core();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
        }

        // Cập nhật trạng thái Item vào db khi có thay đổi
        private void ShopeeHandleViolationItemPush(ViolationItemPush violationItemPush)
        {
            if (violationItemPush == null)
            {
                return;
            }
            int status = CommonOpenPlatform.ShopeeGetEnumValueFromString(violationItemPush.item_status);
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            shopeeMySql.UpdateStatusOfItemFromTMDTItemId(violationItemPush.item_id, status);
        }

        // Giữ chỗ trong kho, nên cập nhật tồn kho và số lượng lên các sản phẩm khác trên sàn 
        // TMDT
        private void ShopeeHandleBookingStatusPush(ShopeeBookingSatusPushData data)
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
                    conn.Open();
                    TikiMySql tikiSqler = new TikiMySql();
                    ECommerceOrderStatus status = ECommerceOrderStatus.BOOKED;
                    ECommerceOrderStatus oldStatus = ECommerceOrderStatus.DONT_EXIST;
                    if (data.booking_status == "READY_TO_SHIP" || data.booking_status == "CANCELLED")
                    {
                        TbEcommerceOrder tbEcommerceBookingLastest = tikiSqler.GetLastestStatusOfECommerceBooking(
                        data.booking_sn,
                        EECommerceType.SHOPEE, conn);

                        oldStatus = (ECommerceOrderStatus)tbEcommerceBookingLastest.status;

                        status = ECommerceOrderStatus.BOOKED;
                        if (data.booking_status == "CANCELLED")
                        {
                            status = ECommerceOrderStatus.UNBOOKED;
                        }

                        if (oldStatus == status ||
                            (status == ECommerceOrderStatus.BOOKED && oldStatus == ECommerceOrderStatus.PACKED) ||
                            (status == ECommerceOrderStatus.UNBOOKED && oldStatus == ECommerceOrderStatus.PACKED))
                        {
                                return;
                        }
                    }
                    // Lấy chi tiết booking
                    ShopeeBookingDetail detail = 
                        ShopeeGetBookingDetail.ShopeeOrderGetBookingDetailFromBookingSN(data.booking_sn);
                    if (detail == null)
                    {
                        return;
                    }

                    ShopeeMySql shopeeMySql = new ShopeeMySql();
                    CommonOrder commonOrder = new CommonOrder(detail);

                    if (data.booking_status == "MATCHED")
                    {
                        // Đã có đơn hàng được matched, vì khi đơn hàng sinh ra,
                        // (log ở trạng thái UNPAID chưa có booking được matched)
                        // thì đã trừ kho nên giờ cần trả lại hàng về kho

                        // Ngủ 120 giây vì có thể shopee vừa gửi push thay đổi trạng thái đơn hàng
                        Thread.Sleep(120000);
                        // HOàn lại số lượng về kho
                        tikiSqler.ReturnQuantityOfOrderMatchedBooking(commonOrder,
                            EECommerceType.SHOPEE, conn);
                        ProductController.GetListNeedUpdateQuantityAndUpdate_Core();
                        return;
                    }

                    shopeeMySql.ShopeeGetMappingOfCommonOrderConnectOut(commonOrder, conn);

                    MySqlResultState resultState = tikiSqler.UpdateQuantityOfProductInWarehouseFromBookingConnectOut(
                        commonOrder, status, data.update_time, oldStatus,
                        EECommerceType.SHOPEE, conn);

                    if (resultState != null && resultState.myAnything == 1)
                    {
                        // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                        ProductController.GetListNeedUpdateQuantityAndUpdate_Core();
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public void ThreadShopeeNotifications(string requestBody)
        {
            try
            {
                MyLogger.GetInstance().Info("ThreadShopeeNotifications get notification");
                MyLogger.GetInstance().Info(requestBody);

                if (string.IsNullOrEmpty(requestBody))
                {
                    return;
                }

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
                    ShopeeHandleOrderStatusPush(orderStatusPush);
                }
                else if (code == 16)
                {
                    // violation_item_push
                    // Get notified when item status becomes BANNED or SHOPEE_DELETE, or marked as deboost,
                    // including the violation type, violation reason, suggestion and fix deadline time.
                    ViolationItemPush violationItemPush = JsonConvert.DeserializeObject<ViolationItemPush>(obj["data"].ToString());
                    ShopeeHandleViolationItemPush(violationItemPush);
                }
                else if(code == 23)
                {
                    // booking_status_push
                    // Get notified immediately on all booking status updates.
                    // This includes booking cancellations that occur before shipping,
                    // so that you can take the necessary steps in time.
                    ShopeeBookingSatusPushData data = JsonConvert.DeserializeObject<ShopeeBookingSatusPushData>(obj["data"].ToString());
                    ShopeeHandleBookingStatusPush(data);
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
                // Đọc request body
                string requestBody;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }

                // Đẩy công việc vào Task.Run
                Task.Run(() =>
                {
                    // Xử lý công việc dài hạn trong nền
                    ThreadShopeeNotifications(requestBody);
                });
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn("ShopeeNotifications calling " + ex.ToString());
            }

            Response.StatusCode = 200;
            return string.Empty;
        }

        private void LazadaHandleOrderStatusPush(LazadaOrderStatusPush orderStatusPush)
        {
            if (orderStatusPush == null)
            {
                return;
            }

            // Possible values are unpaid, pending, canceled, ready_to_ship, 
            // delivered, returned, shipped , failed, topack,toship,shipping and lost
            // Check status: unpaid, pending, topack, canceled đề phòng miss thông báo.
            if (orderStatusPush.order_status != "unpaid" &&
                orderStatusPush.order_status != "pending" &&
                orderStatusPush.order_status != "canceled") // Hủy đơn
            {
                return;
            }
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            TikiMySql tikiSqler = new TikiMySql();
            LazadaMySql lazadaMySql = new LazadaMySql();

            TbEcommerceOrder tbEcommerceOrderLastest = tikiSqler.GetLastestStatusOfECommerceOrder(
                orderStatusPush.trade_order_id,
                EECommerceType.LAZADA, conn);

            ECommerceOrderStatus oldStatus = (ECommerceOrderStatus)tbEcommerceOrderLastest.status;

            ECommerceOrderStatus status = ECommerceOrderStatus.BOOKED;
            if (orderStatusPush.order_status == "canceled")
            {
                status = ECommerceOrderStatus.UNBOOKED;
            }

            if (!tikiSqler.IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(status, oldStatus))
            {
                return;
            }

            // Ta cần check xem có nhận lại event cũ không bởi thời gian event được sàn ghi nhận.
            if (orderStatusPush.status_update_time < tbEcommerceOrderLastest.updateTime)
            {
                return;
            }

            try
            {
                // Giữ chỗ nếu đơn hàng vừa sinh ra.
                // Hủy giữ chỗ nếu đơn hàng bị khách hủy và đang ở trạng thái giữ chỗ.
                long order_id = Common.ConvertStringToInt64(orderStatusPush.trade_order_id);
                if(order_id < 0)
                {
                    return;
                }

                List<LazadaOrderItem> orderItems = LazadaOrderAPI.LazadaGetOrderItems(order_id);
                if (orderItems.Count != 0)
                {
                    LazadaOrder order = new LazadaOrder();
                    order.order_id = orderItems[0].order_id;
                    order.orderItems = orderItems;

                    CommonOrder commonOrder = new CommonOrder(order);
                    lazadaMySql.LazadaGetMappingOfCommonOrderConnectOut(commonOrder, conn);

                    MySqlResultState resultState = tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOut(
                        commonOrder, status, orderStatusPush.status_update_time, oldStatus,
                        EECommerceType.LAZADA, conn);

                    if (resultState != null && resultState.myAnything == 1)
                    {
                        // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                        ProductController.GetListNeedUpdateQuantityAndUpdate_Core();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
        }

        public void ThreadLazadaNotifications(string requestBody)
        {
            try
            {
                MyLogger.GetInstance().Info("ThreadLazadaNotifications get notification");
                MyLogger.GetInstance().Info(requestBody);

                if (string.IsNullOrEmpty(requestBody))
                {
                    return;
                }

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
                    LazadaOrderStatusPush orderStatusPush = JsonConvert.DeserializeObject<LazadaOrderStatusPush>(obj["data"].ToString());
                    LazadaHandleOrderStatusPush(orderStatusPush);
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
                // Đọc request body
                string requestBody;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }

                // Đẩy công việc vào Task.Run
                Task.Run(() =>
                {
                    // Xử lý công việc dài hạn trong nền
                    ThreadLazadaNotifications(requestBody);
                });
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