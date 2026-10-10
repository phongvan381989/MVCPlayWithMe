using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product
{
    public class TikiCreateProductTrackingResponse
    {
        public string track_id { get; set; }
        public string request_id { get; set; }
        public string state { get; set; }
        public string reason { get; set; }
    }

    public class TikiCreateProduct
    {
        public static async Task<TikiCreateProductTrackingResponse> CreateProduct(TikiCreatingProduct createPro,
            MySqlResultState result)
        {
            TikiCreateProductTrackingResponse trackObj = null;
            string http = TikiConstValues.cstrCreateProduct;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            IRestResponse response = await CommonTikiAPI.PostExcuteRequest(http,
                JsonConvert.SerializeObject(createPro, settings));
            try
            {
                //TikiUpdateQuantityResponse updateResponse = JsonConvert.DeserializeObject<TikiUpdateQuantityResponse>(response.Content, settings);
                //return updateResponse;
                JObject obj = JObject.Parse(response.Content);
                if (obj["track_id"] != null)
                {
                    //track_id = (string)obj["track_id"];
                    trackObj = JsonConvert.DeserializeObject<TikiCreateProductTrackingResponse>(response.Content, settings);
                }
                else
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Create product failed: " + response.Content;
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);

                trackObj = null;
            }
            return trackObj;
        }

        // https://api.tiki.vn/integration/v2/tracking/db57745eb036422e92d14d656fa3187c
        public static async Task<TikiCreateProductTrackingResponse> TrackingRequestCreateProduct(string track_id)
        {
            TikiCreateProductTrackingResponse trackObj = null;
            string http = TikiConstValues.cstrTrackingRequestCreateProduct + track_id;


            IRestResponse response = await CommonTikiAPI.GetExcuteRequest(http);
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                trackObj = JsonConvert.DeserializeObject<TikiCreateProductTrackingResponse>(response.Content, settings);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                trackObj = null;
            }
            return trackObj;
        }

        public static string TikiGetStringCover(int hardCover)
        {
            if (hardCover == 1)
            {
                return "Bìa cứng";
            }

            return "Bìa mềm";
        }

        public static string TikiGetStringOneDimension(int deme)
        {
            return (deme / 10.0 + 1).ToString("0.0", CultureInfo.InvariantCulture);
        }

        public static string TikiGetStringDimensions(int length, int width, int height)
        {
            if (length > 0 && width > 0)
            {
                string strTemp =
                    (length / 10.0).ToString("0.0", CultureInfo.InvariantCulture)
                    + " x " +
                    (width / 10.0).ToString("0.0", CultureInfo.InvariantCulture); ;
                if (height > 0)
                {
                    strTemp = strTemp + " x " + (height / 10.0).ToString("0.0", CultureInfo.InvariantCulture); ;
                }
                strTemp = strTemp + " cm";
                return strTemp;
            }

            return null;
        }

        public static string TikiGetStringWeight(int weight)
        {
            return (weight / 1000.0 + 0.1).ToString("0.0", CultureInfo.InvariantCulture);
        }

        // Bán sách nên chỉ có age_group - Phù hợp với độ tuổi là nhiều lựa chọn
        // Từ khoảng tuổi sản phẩm chọn ra những khoảng tuổi thích hơp gồm:
        // "Người lớn", "Từ 0 - 3 tuổi", "Từ 10 - 12 tuổi", "Từ 13 - 18 tuổi", "Từ 4 - 6 tuổi", "Từ 7 - 9 tuổi"
        public static List<string> TikiGetAgeGroups(int minAge, int maxAge)
        {
            minAge = minAge <= 0 ? 0 : minAge;
            maxAge = maxAge <= 0 ? 10000 : maxAge;
            // Danh sách mức độ tuổi trên Tiki
            var tikiAgeGroups = new Dictionary<string, (int min, int max)>
            {
                { "Từ 0 - 3 tuổi", (0, 36) },
                { "Từ 4 - 6 tuổi", (37, 72) },
                { "Từ 7 - 9 tuổi", (73, 108) },
                { "Từ 10 - 12 tuổi", (109, 144) },
                { "Từ 13 - 18 tuổi", (145, 215) },
                { "Người lớn", (216, int.MaxValue) }, // Từ 18 tuổi trở lên
            };

            // Kết quả phù hợp
            var matchingGroups = new List<string>();

            foreach (var group in tikiAgeGroups)
            {
                var range = group.Value;
                // Kiểm tra nếu khoảng tuổi này giao nhau với (minAge, maxAge)
                if (range.max > minAge && range.min < maxAge)
                {
                    matchingGroups.Add(group.Key);
                }
            }

            return matchingGroups;
        }
    }
}
