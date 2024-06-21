using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Order;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Config;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product
{
    public class TikiUpdateStock
    {
        public static Boolean TikiProductUpdate(TikiUpdate st)
        {
            string http = TikiConstValues.cstrProductUpdate;
            IRestResponse response = CommonTikiAPI.PutExcuteRequest(http, st);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    TikiUpdateQuantityResponse updateResponse = JsonConvert.DeserializeObject<TikiUpdateQuantityResponse>(response.Content, settings);
                    Common.CommonErrorMessage = string.Empty;
                    foreach (var s in updateResponse.errors)
                    {
                        Common.CommonErrorMessage = Common.CommonErrorMessage + s + ". ";
                    }
                    MyLogger.GetInstance().Info(Common.CommonErrorMessage);
                    return false;
                }
                catch (Exception ex)
                {
                    Common.CommonErrorMessage = ex.Message;
                    MyLogger.GetInstance().Warn(ex.Message);
                    return false;
                }
            }
            return true;
        }
        public static Boolean TikiProductUpdateQuantity(TikiUpdateQuantity st)
        {
            return TikiProductUpdate(st);
        }

        public static Boolean TikiProductUpdatePrice(TikiUpdatePrice st)
        {
            return TikiProductUpdate(st);
        }

        public static Boolean TikiProductUpdateStatus(TikiUpdateStatus st)
        {
            return TikiProductUpdate(st);
        }

        public static int GetQuantityFromTikiProduct(TikiProduct product)
        {
            int quantity_sellable = 0;
            if(product == null)
                return quantity_sellable;
            if (product.inventory != null && product.inventory.warehouse_stocks != null)
            {
                foreach (var e in product.inventory.warehouse_stocks)
                {
                    if (e.warehouse_id == TikiConstValues.intIdKho28Ngo3TTDL)
                    {
                        quantity_sellable = e.quantity_sellable;
                        break;
                    }
                }
            }

            return quantity_sellable;
        }

        /// <summary>
        /// Set số lượng sản phẩm sau khi update số lượng lên sàn
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static void SetQuantityToTikiProduct(TikiProduct product, int quantity)
        {
            if (product == null)
                return ;
            if (product.inventory != null && product.inventory.warehouse_stocks != null)
            {
                foreach (var e in product.inventory.warehouse_stocks)
                {
                    if (e.warehouse_id == TikiConstValues.intIdKho28Ngo3TTDL)
                    {
                        e.quantity_sellable = quantity;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Kiểm tra tồn kho thực tế và trên sàn có bằng nhau không.
        /// Nếu bằng nhau thì không cần cập nhật số lượng
        /// </summary>
        /// <param name="proId"></param>
        /// <param name="quantity">Số lượng trên sàn TMDT</param>
        /// <returns>True: Nếu cần cập nhật số lượng. NGược lại trả về false</returns>
        public static Boolean NeedUpdateQuantityOfProTMDT(string proId, long quantity)
        {
            int quantityInWarehouse = 0;
            //quantityInWarehouse = ModelThongTinChiTiet.GetQuantityForProTMDT(Common.commerceNameTiki, proId);
            if (quantity == quantityInWarehouse)// ||  // Số lượng trên sàn và trong kho đã đúng
                //(quantityInWarehouse == -1 && quantity == 0)) // Không lấy được số lượng trong kho và số lượng trên sàn đang là 0
                return false;

            return true;
        }

        /// <summary>
        /// Set giá bán thực tế cho id sản phẩm
        /// </summary>
        /// <param name="proId"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static Boolean TikiProductUpdatePrice(int proId, int price)
        {

            TikiUpdatePrice st = new TikiUpdatePrice(proId);
            // Cập nhật tồn kho cho tham số

            st.UpdatePrice(price);
            Boolean isOk = TikiProductUpdatePrice(st);
            if (!isOk)
            {
                MyLogger.GetInstance().Info("Tiki sản phẩm cập nhật giá bán lỗi: " + proId.ToString());
                return false;
            }

            Thread.Sleep(200);
            return true;
        }

        /// <summary>
        /// Set giá bán thực tế cho id sản phẩm
        /// </summary>
        /// <param name="proId"></param>
        /// <param name="status">enum {1: enabled, 2: disabled, 3: hided}</param>
        /// <returns></returns>
        public static Boolean TikiProductUpdateStatus(int proId, int status)
        {

            TikiUpdateStatus st = new TikiUpdateStatus(proId);
            // Cập nhật tồn kho cho tham số

            st.UpdateStatus(status);
            Boolean isOk = TikiProductUpdateStatus(st);
            if (!isOk)
            {
                MyLogger.GetInstance().Info("Tiki sản phẩm cập nhật trạng thái lỗi: " + proId.ToString());
                return false;
            }

            Thread.Sleep(200);
            return true;
        }


        /// <summary>
        /// Từ đơn hàng mới trong khoảng thời gian, ta lấy được số lượng hàng cần xuất
        /// </summary>
        /// <param name="time_from"></param>
        /// <param name="time_to"></param>
        /// <returns></returns>
        static public Dictionary<string, int> TikiGetProductQuantityPairToTake(DateTime time_from, DateTime time_to)
        {
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrder> lsOrderTikiFullInfo;
            lsOrderTikiFullInfo = MVCPlayWithMe.OpenPlatform.API.TikiAPI.Order.TikiGetListOrders.GetListOrderAShop(
                MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrderItemFilterByDate.EnumOrderItemFilterByDate.last7days);
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrder> lsTem = new List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Order.TikiOrder>();
            foreach(var e in lsOrderTikiFullInfo)
            {
                if(e.created_at >= time_from && e.created_at <= time_to)
                {
                    lsTem.Add(e);
                }
            }
            lsOrderTikiFullInfo = lsTem;
            Dictionary<string, int> dic = TikiGetListOrders.TikiGetDictionaryOfProductQuantityFromListDetailOrder(lsOrderTikiFullInfo);

            return dic;
        }
    }
}
