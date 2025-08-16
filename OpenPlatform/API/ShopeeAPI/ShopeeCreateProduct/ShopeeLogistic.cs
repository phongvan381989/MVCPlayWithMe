using MVCPlayWithMe.General;
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
    class ShopeeLogistic
    {
        static public List<ShopeeLogisticInfo> logistic_info;
        static public List<ShopeeLogisticInfo> GetLogisticInfo(Boolean isRefresh)
        {
            if(logistic_info == null ||
                logistic_info.Count == 0 ||
                isRefresh)
            {
                logistic_info = new List<ShopeeLogisticInfo>();

                try
                {
                    ShopeeGetChannelListResponseHTTP obj = ShopeeLogisticGetChannelList();
                    if (obj != null)
                    {
                        foreach (var lo in obj.response.logistics_channel_list)
                        {
                            if (lo.fee_type == "SIZE_INPUT" &&
                                lo.logistics_channel_id != 5002 && // "logistics_channel_name": "Tiết kiệm",
                                lo.logistics_channel_id != 50016 // "logistics_channel_name": "VNPost Tiết Kiệm"
                                )
                            {
                                logistic_info.Add(new ShopeeLogisticInfo(lo.logistics_channel_id));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    logistic_info.Clear();
                }
            }
            return logistic_info;
        }

        // Get the attribute tree for categories
        public static ShopeeGetChannelListResponseHTTP ShopeeLogisticGetChannelList()
        {
            string path = "/api/v2/logistics/get_channel_list";
            IRestResponse response = CommonShopeeAPI.ShopeeGetMethod(path, null);
            if (response == null)
                return null;

            ShopeeGetChannelListResponseHTTP objResponse = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    objResponse = JsonConvert.DeserializeObject<ShopeeGetChannelListResponseHTTP>(response.Content, Common.jsonSerializersettings);
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
