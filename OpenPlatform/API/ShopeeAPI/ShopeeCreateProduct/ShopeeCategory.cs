using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI.ShopeeCreateProduct
{
    class ShopeeCategory
    {
        public static async Task<ShopeeGetCategoryResponseHTTP> ShopeeGetCategoryAllAsync()
        {
            string path = "/api/v2/product/get_category";
            IRestResponse response = await CommonShopeeAPI.ShopeeGetMethodAsync(path, null);
            if (response == null)
                return null;

            ShopeeGetCategoryResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetCategoryResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }
            return objResponse;
        }

        public static async Task<ShopeeGetCategoryRecommendResponseHTTP> ShopeeGetCategoryRecommendAsync(string item_name)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("item_name", item_name));

            string path = "/api/v2/product/category_recommend";
            IRestResponse response = await CommonShopeeAPI.ShopeeGetMethodAsync(path, ls);
            if (response == null)
                return null;

            ShopeeGetCategoryRecommendResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetCategoryRecommendResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }
            return objResponse;
        }

        public static async Task<ShopeeGetAttributeTreeResponseHTTP> ShopeeGetAttributeTreeOfCategoryAsync(int categoryId)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("category_id_list", categoryId.ToString()));
            ls.Add(new DevNameValuePair("language", "vn"));

            string path = "/api/v2/product/get_attribute_tree";
            IRestResponse response = await CommonShopeeAPI.ShopeeGetMethodAsync(path, ls);
            if (response == null)
                return null;

            ShopeeGetAttributeTreeResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetAttributeTreeResponseHTTP>(response.Content, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return null;
                }
            }
            return objResponse;
        }
    }
}
