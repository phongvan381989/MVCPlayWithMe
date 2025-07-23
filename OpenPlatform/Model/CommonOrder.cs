using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Thông tin order
    /// </summary>
    public class CommonOrder
    {
        /// <summary>
        /// Tên sàn thương mại điện tử
        /// </summary>
        public string ecommerceName { get; set; }

        /// <summary>
        /// id đơn hàng
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Mã vận đơn
        /// </summary>
        public string shipCode { get; set; }

        /// <summary>
        /// complete	The status where the order is in the process to customer hands
        /// </summary>
        public string status { get; set; }

        // true: Nếu là đơn hỏa tốc, ngược lại false. Hỏa tốc theo phong cách SHOPEE
        public Boolean isExpress { get; set; }

        // Tin nhắn của khách
        public string messageToSeller { get; set; }

        public List<int> listQuantity { get; set; }

        // Trạng thái:
        //"Đã Đóng", "Đã Hoàn", "Giữ Chỗ", "Hủy Giữ Chỗ", hoặc trống
        public string orderStatusInWarehoue { get; set; }

        /// <summary>
        /// Danh sách item id và model id tương ứng,
        /// Shopee trả về model id = 0 nếu item không có model
        /// Tiki set mặc định model id = -1
        /// </summary>
        public List<long> listItemId { get; set; }
        public List<long> listModelId { get; set; }

        // Tiki có thông tin super id
        public List<int> listItemSuperId { get; set; }

        /// <summary>
        /// Danh sách item name và model name tương ứng
        /// Không có model ta lấy tên theo item name
        /// </summary>
        public List<string> listItemName { get; set; } // Tên của item
        public List<string> listModelName { get; set; } // Tên của model

        /// <summary>
        /// 
        /// </summary>
        public List<List<Mapping>> listMapping { get; set; }

        /// <summary>
        /// 2020-08-10 18:50:17	When the order is created
        /// </summary>
        public DateTime created_at { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> listThumbnail { get; set; }

        public CommonOrder()
        {
            ecommerceName = "";
            id = -1;
            code = string.Empty;
            status = string.Empty;
            messageToSeller = string.Empty;
            isExpress = false;
            listItemId = new List<long>();
            listModelId = new List<long>();
            created_at = new DateTime();
            listThumbnail = new List<string>();
            listQuantity = new List<int>();

            listItemName = new List<string>();
            listModelName = new List<string>();
            listMapping = new List<List<Mapping>>();
        }

        /// <summary>
        /// </summary>
        /// <param name="order"></param>
        public CommonOrder(TikiOrder order)
        {
            ecommerceName = Common.eTiki;

            id = order.id;
            code = order.code;
            status = order.status;
            messageToSeller = string.Empty;
            isExpress = false;
            listItemId = new List<long>();
            listModelId = new List<long>();
            listItemSuperId = new List<int>();
            listThumbnail = new List<string>();
            listQuantity = new List<int>();

            listItemName = new List<string>();
            listModelName = new List<string>();
            listMapping = new List<List<Mapping>>();

            foreach (TikiOrderItemV2 e in order.items)
            {
                listItemId.Add(e.product.id);
                listItemSuperId.Add(e.product.super_id);
                listModelId.Add(-1);
                listQuantity.Add(e.qty);
                listThumbnail.Add(e.product.thumbnail);
                listItemName.Add(e.product.name);
                listModelName.Add("");
            }
            created_at = order.created_at;

            //TikiMySql tikiMySql = new TikiMySql();
            //orderStatusInWarehoue = tikiMySql.TikiGetOrderStatusInWarehoue(code, (int)Common.EECommerceType.TIKI);
        }

        /// <summary>
        /// </summary>
        /// <param name="order"></param>
        public CommonOrder(ShopeeOrderDetail order)
        {
            ecommerceName = Common.eShopee;

            id = -1;
            code = order.order_sn;
            // Lấy mã vận đơn
            shipCode = order.shipCode;
            status = order.order_status;
            messageToSeller = order.message_to_seller;

            isExpress = CommonOpenPlatform.IsShopeeExpress(order.checkout_shipping_carrier);
            created_at = Common.GetDateFromTimestamp(order.create_time);
            listItemId = new List<long>();
            listModelId = new List<long>();
            listThumbnail = new List<string>();
            listQuantity = new List<int>();

            listItemName = new List<string>();
            listModelName = new List<string>();
            listMapping = new List<List<Mapping>>();

            foreach (ShopeeGetOrderDetailItem e in order.item_list)
            {
                listItemId.Add(e.item_id);
                listModelId.Add(e.model_id);
                listThumbnail.Add(e.image_info.image_url);
                listQuantity.Add(e.model_quantity_purchased);

                listItemName.Add(e.item_name);
                listModelName.Add(e.model_name);
            }
            //TikiMySql tikiMySql = new TikiMySql();
            //orderStatusInWarehoue = tikiMySql.TikiGetOrderStatusInWarehoue(code, (int)Common.EECommerceType.SHOPEE);
        }
    }
}