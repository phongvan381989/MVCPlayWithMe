using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeOrder;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct
{
    public class ShopeeUpdateStock
    {
        //{
        //"item_id": 1000,
        //"stock_list": [{
        //"model_id": 3456,
        //"normal_stock": 100
        //}]
        //}
        // Sản phẩm không có model
        // model_id : 0
        public static async Task<ShopeeUpdateStockResponseHTTP> ShopeeProductUpdateStockAsync(MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock st)
        {
            string path = "/api/v2/product/update_stock";
            string body = JsonConvert.SerializeObject(st, Formatting.Indented);
            MyLogger.GetInstance().Info(body);
            ShopeeUpdateStockResponseHTTP objResponse = null;
            IRestResponse response = await CommonShopeeAPI.ShopeePostMethodAsync(path, body);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                MyLogger.GetInstance().Info("/api/v2/product/update_stock response.StatusCode: " + response.StatusCode
                     + ", response.Content" + response.Content);
                return null;
            }

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                objResponse = JsonConvert.DeserializeObject<ShopeeUpdateStockResponseHTTP>(response.Content, settings);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return null;
            }

            return objResponse;
        }

        /// <summary>
        /// Từ item_id, model_id sản phẩm cập nhật số lượng
        /// </summary>
        /// <param name="shopeeItemId"></param>
        /// <returns></returns>
        public static async Task<ShopeeUpdateStockResponseHTTP> ShopeeProductUpdateStockAsync(ShopeeItemId shopeeItemId)
        {
            OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock stock =
                new OpenPlatform.Model.ShopeeApp.ShopeeProduct.ShopeeUpdateStock();

            stock.item_id = shopeeItemId.item_id;
            if (shopeeItemId.model_id == -1)// Không có model
                stock.stock_list.Add(new ShopeeUpdateStockStock(0, shopeeItemId.quantity));
            else
                stock.stock_list.Add(new ShopeeUpdateStockStock(shopeeItemId.model_id, shopeeItemId.quantity));

            ShopeeUpdateStockResponseHTTP rs =
                await MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct.ShopeeUpdateStock.ShopeeProductUpdateStockAsync(stock);
            return rs;
        }

        ///// <summary>
        ///// Từ đơn hàng mới trong khoảng thời gian, ta lấy được số lượng hàng cần xuất
        ///// <param name="time_from"></param>
        ///// <param name="time_to"></param>
        ///// <returns></returns>
        //static public Dictionary<string, int> ShopeeGetProductQuantityPairToTake(DateTime time_from, DateTime time_to)
        //{
        //    List<ShopeeOrderDetail> lsOrderShopeeFullInfo;
        //    lsOrderShopeeFullInfo = ShopeeGetOrderDetail.ShopeeOrderGetOrderDetailAll(time_from, time_to,
        //        new ShopeeOrderStatus(ShopeeOrderStatus.EnumShopeeOrderStatus.READY_TO_SHIP));
        //    Dictionary<string, int> dic = ShopeeGetOrderDetail.ShopeeGetDictionaryOfProductQuantityFromListDetailOrder(lsOrderShopeeFullInfo);

        //    return dic;
        //}
    }
}
