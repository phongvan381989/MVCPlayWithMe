using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI.Category
{
    public class TikiCategoryAction
    {
        // Từ id của cha, lấy được những category con
        public static async Task<List<TikiCategory>> GetChildrenCategory(int parrentId)
        {
            TikiCategoryData responseData = new TikiCategoryData();
            //List<TikiCategory> ls = new List<TikiCategory>();

            // https://api.tiki.vn/integration/v2/categories?parent=320
            string http = TikiConstValues.cstrCategory + "?parent=" + parrentId.ToString();

                IRestResponse response = await CommonTikiAPI.GetExcuteRequest(http);
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
        public static async Task< List<TikiAttribute>> GetAttributeOfCategory(int categoryId)
        {
            TikiAttributeData responseData = new TikiAttributeData();

            // https://api.tiki.vn/integration/v2/categories/852/attributes
            string http = TikiConstValues.cstrCategory + "/" + categoryId.ToString() + "/attributes";

            IRestResponse response = await CommonTikiAPI.GetExcuteRequest(http);
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

        public static async Task<MySqlResultState> CheckGetAttributeValues(int attributeId, string values)
        {
            // https://api.tiki.vn/integration/v2/attributes/540/values?q=phong
            string http = TikiConstValues.cstrGetAttributeValues + attributeId.ToString() + "/values?q=" + values;
            IRestResponse response = await CommonTikiAPI.GetExcuteRequest(http);

            MySqlResultState result = new MySqlResultState();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = response.Content;
                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        JsonElement root = doc.RootElement;

                        // Kiểm tra xem trường 'data' có tồn tại và có phải là mảng không
                        if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                        {
                            int count = dataElement.GetArrayLength();
                            if (count > 0)
                            {
                            }
                            else
                            {
                                result.State = EMySqlResultState.EMPTY;
                                result.Message = "Data rỗng ([]).";
                            }
                        }
                        else
                        {
                            result.State = EMySqlResultState.DONT_EXIST;
                            result.Message = "Không tìm thấy trường data hoặc không phải mảng.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            else
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "response.StatusCode" + response.StatusCode.ToString();
            }
            return result;
        }
    }
}
