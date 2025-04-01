using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI.DealDiscount
{
    // Tạo, tắt, cập nhật, lấy chương trình giảm giá
    public class DealAction
    {
        // Tạo chương trình giảm giá
        // Trả về null tức có mảng dữ liệu chi tiết về chương trình giảm giá tức thành công
        // Khác null là thông báo lỗi server tiki trả về
        static DealResponseStatus CreateDeal(CreatingRequestBody body)
        {
            DealResponseStatus dealResponseStatus = null;
            string http = TikiConstValues.cstrCreateDeal;
            IRestResponse response = CommonTikiAPI.PostExcuteRequest(http, body.GetJson());
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = response.Content;
                try
                {
                    JToken token = JToken.Parse(json);

                    if (token.Type == JTokenType.Array)
                    {
                        // Tạo deal giảm giá thành công
                        //JArray jsonArray = (JArray)token;
                        //if (jsonArray.Count > 0 && jsonArray[0]["id"] != null)
                        //{
                        //    //return (int)jsonArray[0]["id"];
                        //}
                        
                    }
                    else if (token.Type == JTokenType.Object)
                    {
                        // Tạo deal thất bại

                        //JObject obj = (JObject)token;
                        // 
                        //return obj["id"] != null ? (int)obj["id"] : -1;
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        dealResponseStatus = JsonConvert.DeserializeObject<DealResponseStatus>(json, settings);
                    }
                    //JsonSerializerSettings settings = new JsonSerializerSettings
                    //{
                    //    NullValueHandling = NullValueHandling.Ignore,
                    //    MissingMemberHandling = MissingMemberHandling.Ignore
                    //};
                    //eventResponse = JsonConvert.DeserializeObject<Event_Response>(json, settings);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                }
            }
            else
            {
                dealResponseStatus = new DealResponseStatus();
            }

            return dealResponseStatus;
        }

        // Từ sku lấy được các deal đã tạo cho sku
        // ls khởi tạo ngoài hàm
        // Trả về khác null là thông báo lỗi server tiki trả về
        static public DealResponseStatus SearchDeal(string sku, List<DealCreatedResponseDetail> ls)
        {
            DealResponseStatus dealResponseStatus = null;
            List<DevNameValuePair> listValuePair = new List<DevNameValuePair>();

            // Phần tử đầu tiên của listValuePair phải là "page"
            // Add page = 1
            // Mặc định chỉ lấy dữ liệu ở page = 1 vì chỉ thế là đủ
            listValuePair.Add(new DevNameValuePair("page", "1"));

            // Add limit=20
            listValuePair.Add(new DevNameValuePair("limit", TikiConstValues.cstrPerPage));

            // Hardcode khoảng thời gian 4 năm, bắt đầu 0.5 năm trước và kết thúc cách hiện tại 2 năm
            // Add special_from_date, Ex: 2022-01-13 02:59:59
            listValuePair.Add(new DevNameValuePair("special_from_date",
                DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "%20").Replace(":", "%3A")));

            // Add special_to_date, Ex: 2022-01-13 03:59:59
            listValuePair.Add(new DevNameValuePair("special_to_date",
                DateTime.Now.AddYears(2).ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "%20").Replace(":", "%3A")));

            // Add is_active status of deal, values: INACTIVE = 0 | ACTIVE = 1 | RUNNING = 2 | EXPIRED = 3 | HOT_SALE = 4 | CLOSED = 5 | PAUSED = 6
            listValuePair.Add(new DevNameValuePair("is_active", "0,1,2,3,4,5,6"));

            // Add danh sachs skus, ex: 12345678,87654321
            listValuePair.Add(new DevNameValuePair("skus", sku));
            string http = TikiConstValues.cstrSearchDeal + DevNameValuePair.GetQueryString(listValuePair);

            try
            {
                IRestResponse response = CommonTikiAPI.GetExcuteRequest(http);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string json = response.Content;

                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    // Có lỗi trả về json
                    //{
                    //    "statusCode": 400,
                    //    "message": {
                    //        "en": "Start time must be less than end time",
                    //        "vi": "Thời gian bắt chạy giảm giá phải nhỏ hơn thời gian kết thúc"
                    //    },
                    //    "error": "Bad Request"
                    //}
                    JObject obj = JObject.Parse(json);
                    if(obj["message"] != null)
                    {
                        dealResponseStatus = JsonConvert.DeserializeObject<DealResponseStatus>(json, settings);
                    }
                    else
                    {
                        DealSearchResponse dealSearchResponse =
                            JsonConvert.DeserializeObject<DealSearchResponse>(json, settings);
                        ls.AddRange(dealSearchResponse.data);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }

            return dealResponseStatus;
        }
    }
}