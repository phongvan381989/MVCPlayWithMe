using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI.Category
{
    public class TikiCategoryAction
    {
        // Từ id của cha, lấy được những category con
        public static List<TikiCategory> GetChildrenCategory(int parrentId)
        {
            TikiCategoryData responseData = new TikiCategoryData();
            //List<TikiCategory> ls = new List<TikiCategory>();

            // https://api.tiki.vn/integration/v2/categories?parent=320
            string http = TikiConstValues.cstrCategory + "?parent=" + parrentId.ToString();

                IRestResponse response = CommonTikiAPI.GetExcuteRequest(http);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = response.Content;

                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    responseData = JsonConvert.DeserializeObject<TikiCategoryData>(json);

                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                }
            }
            if (responseData != null)
            {
                return responseData.data;
            }
            return null;
        }

        // Từ id của category lấy được danh sách những attribute có thể
        public static List<TikiAttribute> GetAttributeOfCategory(int categoryId)
        {
            TikiAttributeData responseData = new TikiAttributeData();

            // https://api.tiki.vn/integration/v2/categories/852/attributes
            string http = TikiConstValues.cstrCategory + "/" + categoryId.ToString() + "/attributes";

            IRestResponse response = CommonTikiAPI.GetExcuteRequest(http);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = response.Content;

                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    responseData = JsonConvert.DeserializeObject<TikiAttributeData>(json);

                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                }
            }
            if (responseData != null)
            {
                return responseData.data;
            }
            return null;
        }
    }
}