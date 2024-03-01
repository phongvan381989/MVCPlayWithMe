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
    public class ShopeeGetModelList
    {
        private static ShopeeGetModelListResponse ShopeeProductGetModelList(List<DevNameValuePair> ls)
        {
            string path = "/api/v2/product/get_model_list";

            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
            if (response == null)
                return null;
            ShopeeGetModelListResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetModelListResponseHTTP>(response.Content, settings);
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

        public static ShopeeGetModelListResponse ShopeeProductGetModelList(long productCode)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            // The ID of the item
            ls.Add(new DevNameValuePair("item_id", productCode.ToString()));

            return ShopeeProductGetModelList(ls);
        }
    }
}
