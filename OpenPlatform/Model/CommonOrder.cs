using MVCPlayWithMe.General;
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

        // Trạng thái đóng hàng đi từ, hoàn hàng về kho
        public string orderStatusInWarehoue { get; set; }

        /// <summary>
        /// Danh sách item id và model id tương ứng, model id = -1 nếu item không có model
        /// </summary>
        public List<long> listItemId { get; set; }
        public List<long> listModelId { get; set; }

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
            listItemId = new List<long>();
            listModelId = new List<long>();
            created_at = new DateTime();
            listThumbnail = new List<string>();
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
            listItemId = new List<long>();
            listModelId = new List<long>();
            listThumbnail = new List<string>();
            foreach (TikiOrderItemV2 e in order.items)
            {
                listItemId.Add(e.id);
                listModelId.Add(-1);
                listThumbnail.Add(e.product.thumbnail);
            }
            created_at = order.created_at;
            TikiMySql tikiMySql = new TikiMySql();
            orderStatusInWarehoue = tikiMySql.TikiGetOrderStatusInWarehoue(code, (int)Common.EECommerceType.TIKI);
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
            created_at = Common.GetDateFromTimestamp(order.create_time);
            listItemId = new List<long>();
            listModelId = new List<long>();
            listThumbnail = new List<string>();
            foreach (ShopeeGetOrderDetailItem e in order.item_list)
            {
                listItemId.Add(e.item_id);
                listModelId.Add(e.model_id);
                listThumbnail.Add(e.image_info.image_url);
            }
            TikiMySql tikiMySql = new TikiMySql();
            orderStatusInWarehoue = tikiMySql.TikiGetOrderStatusInWarehoue(code, (int)Common.EECommerceType.SHOPEE);
        }
    }
}