using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static TikiCreateProductTrackingResponse CreateProduct(TikiCreatingProduct createPro)
        {
            TikiCreateProductTrackingResponse trackObj = null;
            string http = TikiConstValues.cstrCreateProduct;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            IRestResponse response = CommonTikiAPI.PostExcuteRequest(http,
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
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                trackObj = null;
            }
            return trackObj;
        }

        // https://api.tiki.vn/integration/v2/tracking/db57745eb036422e92d14d656fa3187c
        public static TikiCreateProductTrackingResponse TrackingRequestCreateProduct(string track_id)
        {
            TikiCreateProductTrackingResponse trackObj = null;
            string http = TikiConstValues.cstrTrackingRequestCreateProduct + track_id;


            IRestResponse response = CommonTikiAPI.GetExcuteRequest(http);
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
    }
}