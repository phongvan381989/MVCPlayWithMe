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
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

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
    }
}
