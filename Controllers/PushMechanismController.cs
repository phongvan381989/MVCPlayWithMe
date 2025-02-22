using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
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
        private void UpdateQuantityAfterShopeeHasOrder(OrderStatusPush orderStatusPush)
        {
            // Giữ chỗ nếu đơn hàng vừa sinh ra trên shopee.
            // Hủy giữ chỗ nếu đơn hàng bị khách hủy và đang ở trạng thái giữ chỗ.
            ShopeeOrderDetail shopeeOrderDetail = ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailFromOrderSN(orderStatusPush.ordersn);

            if (shopeeOrderDetail != null)
            {
                CommonOrder commonOrder = new CommonOrder(shopeeOrderDetail);
                ShopeeMySql shopeeMySql = new ShopeeMySql();
                shopeeMySql.ShopeeGetMappingOfCommonOrder(commonOrder);

                TikiMySql tikiSqler = new TikiMySql();
                Boolean isUpdate = false;
                //0: UNPAID, 1:  READY_TO_SHIP,
                if (orderStatusPush.status == "UNPAID" || orderStatusPush.status == "READY_TO_SHIP")
                {
                    tikiSqler.UpdateQuantityOfProductInWarehouseFromOrder(commonOrder, ECommerceOrderStatus.BOOKED, EECommerceType.SHOPEE);
                    isUpdate = true;
                }
                else if (orderStatusPush.status == "IN_CANCEL" || orderStatusPush.status == "CANCELLED")
                {
                    tikiSqler.UpdateQuantityOfProductInWarehouseFromOrder(commonOrder, ECommerceOrderStatus.UNBOOKED, EECommerceType.SHOPEE);
                    isUpdate = true;
                }

                if (isUpdate)
                {
                    // Cập nhật số lượng sản phẩm khác trên sàn SHOPEE, TIKI, LAZADA. Không quan tâm kết quả thành công hay không
                    ProductController productController = new ProductController();
                    productController.GetListNeedUpdateQuantityAndUpdate_Core();
                }
            }

        }

        public void ThreadShopeeNotifications(object json)
        {
            try
            {
                MyLogger.GetInstance().Info("ThreadShopeeNotifications get notification");
                MyLogger.GetInstance().Info((string)json);

                JObject obj = JObject.Parse((string)json);
                int code = Common.ConvertStringToInt32(obj["code"].ToString());
                if (code == System.Int32.MinValue)
                {
                    return;
                }
                // order_status_push: Lấy thay đổi trạng thái đơn hàng
                if (code == 3)
                {
                    OrderStatusPush orderStatusPush = JsonConvert.DeserializeObject<OrderStatusPush>(obj["data"].ToString());
                    UpdateQuantityAfterShopeeHasOrder(orderStatusPush);

                }
                //else if(code == 6)
                //{

                //}
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn("ThreadShopeeNotifications calling " + ex.ToString());
            }
        }

        [HttpPost]
        public string ShopeeNotifications()
        {
            try
            {
                MyLogger.GetInstance().Info("SHOPEE Get notification");
                var req = Request.InputStream;
                var json = new StreamReader(req).ReadToEnd();

                Thread thread = new Thread(ThreadShopeeNotifications);
                thread.Start(json); // Bắt đầu thread
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