using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct
{
    //    {
    //      "item_id": 1000,
    //      "price_list":
    //          [{
//                   "model_id": 3456,
//                    "original_price": 11.11
//              }]
    //      }
    public class ShopeeUpdatePrice
    {
        public static ShopeeUpdatePrice_Response ShopeeProductUpdatePrice(ShopeeUpdatePrice_Request st)
        {
            string path = "/api/v2/product/update_price";
            string body = JsonConvert.SerializeObject(st, Formatting.Indented);
            MyLogger.GetInstance().Info(body);

            IRestResponse response = CommonShopeeAPI.ShopeePostMethod(path, body);
            if (response == null)
                return null;
            ShopeeUpdatePrice_Response_HTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeUpdatePrice_Response_HTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }

            if (objResponse == null || objResponse.response == null)
                return null;

            return objResponse.response;
        }


        /// <summary>
        /// Cập nhật giá.
        /// </summary>
        /// <param name="item_id"></param>
        /// <param name="model_id"> 0 for no model item</param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static Boolean ShopeeProductUpdatePrice(long item_id, long model_id, int price)
        {
            ShopeeUpdatePrice_Request st = new ShopeeUpdatePrice_Request();
            st.item_id = item_id;
            st.price_list.Add(new ShopeeUpdatePrice_Request_Price_List(model_id, price));

            ShopeeUpdatePrice_Response rs = MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeProduct.ShopeeUpdatePrice.ShopeeProductUpdatePrice(st);
            if (rs == null)
            {
                MyLogger.GetInstance().Info("Shopee sản phẩm cập nhật giá lỗi item_id: " + item_id.ToString() + ", model_id: " + model_id.ToString());
                return false;
            }

            Boolean isOk = true;
            Common.CommonErrorMessage = string.Empty;
            foreach (var eFailed in rs.failure_list)
            {
                isOk = false;
                Common.CommonErrorMessage = Common.CommonErrorMessage + eFailed.model_id.ToString() + ":" + eFailed.failed_reason + "; ";
            }
            if (!isOk)
            {
                MyLogger.GetInstance().Info(Common.CommonErrorMessage);
                return false;
            }

            return true;
        }
    }
}
