using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.OpenPlatform;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeLogistic;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model;
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
        /// và số lượng nên các sản phẩm khác trên các nền tảng SHOPEE, TIKI, LAZADA,...
        /// </summary>
        private void HandleOrderStatusPush(OrderStatusPush orderStatusPush)
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

                    string trackingNumber = shopeeMySql.GetTrackingNumberFromSNConnectOut(
                    orderStatusPush.ordersn, conn);

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

                    // Nếu đơn hàng vừa sinh ra, ta lưu id của item, model trong đơn.
                    if (orderStatusPush.status == "UNPAID")
                    {
                        tikiSqler.InsertTbItemOfEcommerceOder(commonOrder, EECommerceType.SHOPEE, conn);
                    }
                    else
                    {
                        tikiSqler.UpdateCancelledStatusTbItemOfEcommerceOder(commonOrder, EECommerceType.SHOPEE, conn);
                    }

                    MySqlResultState resultState = tikiSqler.UpdateQuantityOfProductInWarehouseFromOrderConnectOut(
                        commonOrder, status, orderStatusPush.update_time, oldStatus,
                        EECommerceType.SHOPEE, conn);

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
            conn.Close();
        }

        // Cập nhật trạng thái Item vào db khi có thay đổi
        private void HandleViolationItemPush(ViolationItemPush violationItemPush)
        {
            if (violationItemPush == null)
            {
                return;
            }
            int status = CommonOpenPlatform.ShopeeGetEnumValueFromString(violationItemPush.item_status);
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            shopeeMySql.UpdateStatusOfItemFromTMDTItemId(violationItemPush.item_id, status);
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
                    HandleOrderStatusPush(orderStatusPush);
                }
                else if (code == 16)
                {
                    ViolationItemPush violationItemPush = JsonConvert.DeserializeObject<ViolationItemPush>(obj["data"].ToString());
                    HandleViolationItemPush(violationItemPush);
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
    }
}