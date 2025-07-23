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

namespace QuanLyKho.ViewModel.Dev.ShopeeAPI.ShopeeCreateProduct
{
    class ShopeeCategory
    {
        // Lấy tất cả category
        public static ShopeeGetCategoryResponseHTTP ShopeeGetCategoryAll()
        {
            string path = "/api/v2/product/get_category";
            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, null);
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

        // Lấy category theo gợi ý tên sản phẩm
        public static ShopeeGetCategoryRecommendResponseHTTP ShopeeGetCategoryRecommend(string item_name)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("item_name", item_name));

            string path = "/api/v2/product/category_recommend";
            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
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

        // Get the attribute tree for categories
        public static ShopeeGetAttributeTreeResponseHTTP ShopeeGetAttributeTreeOfCategory(int categoryId)
        {
            List<DevNameValuePair> ls = new List<DevNameValuePair>();
            ls.Add(new DevNameValuePair("category_id_list", categoryId.ToString()));
            ls.Add(new DevNameValuePair("language", "vn"));

            string path = "/api/v2/product/get_attribute_tree";
            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, ls);
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
