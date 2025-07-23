using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using Newtonsoft.Json;
using QuanLyKho.Model.Dev.ShopeeApp.ShopeeCreateProduct;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.ViewModel.Dev.ShopeeAPI.ShopeeCreateProduct
{
    class ShopeeMediaSpace
    {
        static public readonly int  maxImageOnItem = 9;

        static public async Task<ShopeeUploadImageResponseHTTP> ShopeeUpload_Image(string filePath)
        {
            string path = "/api/v2/media_space/upload_image";
            string url = CommonShopeeAPI.GenerateURLShopeeAPI(path, null);
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;

            // File truyền đúng tên là "file"
            request.AddFile("image", filePath);

            IRestResponse response = await client.ExecuteAsync(request);
            MyLogger.InfoRestLog(client, request, response);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            ShopeeUploadImageResponseHTTP objResponse = null;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                objResponse = JsonConvert.DeserializeObject<ShopeeUploadImageResponseHTTP>(response.Content, settings);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return null;
            }

            return objResponse;
        }
    }
}
