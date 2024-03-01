using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.Config;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Config;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI
{
    public class CommonTikiAPI
    {
        /// <summary>
        /// Chứa danh sách các app đang sử dụng
        /// </summary>
        //static public List<TikiConfigApp> listTikiConfigAppUsing = new List<TikiConfigApp>();

        static public TikiConfigApp tikiCongifApp;

        /// <summary>
        /// Khi Access token phục vụ authorization hết hạn, gọi hàm này lấy access token mới và lưu và db xml
        /// </summary>
        /// <param name="appID"></param>
        /// <returns>empty nếu thành công. Ngược lại trả về string mô tả lỗi</returns>
        static public string RefreshDataAuthorization(string appID)
        {
            var client = new RestClient("https://api.tiki.vn/sc/oauth2/token");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic " + ModelThongTinBaoMat.Tiki_GetAppCredentialBase64Format(appID));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", appID);
            request.AddParameter("scope", "");
            IRestResponse response = null;
            try
            {
                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Lấy quyền truy cập shop lỗi. Vui lòng thử lại.";
            }
            TikiAuthorization accessToken = JsonConvert.DeserializeObject<TikiAuthorization>(response.Content);
            ModelThongTinBaoMat.Tiki_InhouseAppSaveAccessToken(appID, accessToken);
            MyLogger.GetInstance().Info("New token: " + accessToken.access_token);
            return string.Empty;
        }

        ///// <summary>
        ///// Cập nhật danh sách ứng dụng đang sử dụng
        ///// </summary>
        //static public void GetListTikiConfigAppUsing()
        //{
        //    listTikiConfigAppUsing.Clear();
        //    List<TikiConfigApp> l = ModelThongTinBaoMat.Tiki_InhouseAppGetListUsingApp();
        //    if (l != null)
        //        listTikiConfigAppUsing = l;
        //}

        ///// <summary>
        ///// Lấy danh sách URL của shop đang sử dụng
        ///// </summary>
        ///// <returns>Nếu không có shop nào đang sử dụng, trả về list rỗng</returns>
        //static public ObservableCollection<String> GetListHomeAddressUsing()
        //{
        //    ObservableCollection<String> lStr = new ObservableCollection<string>();
        //    foreach(TikiConfigApp e in listTikiConfigAppUsing)
        //    {
        //        if(e.usingApp == TikiConfigApp.constUsingApp)
        //            lStr.Add(e.homeAddress);
        //    }
        //    return lStr;
        //}

        ///// <summary>
        ///// Từ địa chỉ web shop lấy được đối tượng config
        ///// </summary>
        ///// <param name="homeAddress"></param>
        ///// <returns>Trả về null nếu không có đối tượng thỏa mãn</returns>
        //static public TikiConfigApp GetTikiConfigAppFromHomeAddress(string homeAddress)
        //{
        //    if (string.IsNullOrEmpty(homeAddress))
        //        return null;

        //    TikiConfigApp tikiConfigApp = null;
        //    foreach (TikiConfigApp e in listTikiConfigAppUsing)
        //    {
        //        if (e.usingApp == TikiConfigApp.constUsingApp && e.homeAddress == homeAddress)
        //        {
        //            tikiConfigApp = e;
        //            break;
        //        }
        //    }
        //    return tikiConfigApp;
        //}

        ///// <summary>
        ///// Từ địa chỉ web shop lấy được đối tượng config
        ///// </summary>
        ///// <param name="homeAddress"></param>
        ///// <returns>Trả về null nếu không có đối tượng thỏa mãn</returns>
        //static public TikiConfigApp GetTikiConfigAppDefault()
        //{
        //    TikiConfigApp tikiConfigApp = null;
        //    foreach (TikiConfigApp e in listTikiConfigAppUsing)
        //    {
        //        if (e.usingApp == TikiConfigApp.constUsingApp && e.homeAddress == TikiConstValues.cstrHomeAdress)
        //        {
        //            tikiConfigApp = e;
        //            break;
        //        }
        //    }
        //    return tikiConfigApp;
        //}

        /// <summary>
        /// Thực hiện 1 request HTTP, nếu access token hết hạn thì làm mới
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="configApp"></param>
        /// <returns></returns>
        static public IRestResponse ExcuteRequest(RestClient client, RestRequest request, TikiConfigApp configApp)
        {
            request.AddHeader("Authorization", "Bearer " + (string.IsNullOrEmpty(configApp.tikiAu.access_token) ? string.Empty: configApp.tikiAu.access_token));
            IRestResponse response = client.Execute(request);
            MyLogger.InfoRestLog(client, request, response);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                MyLogger.GetInstance().Info("Expried token:" + (string.IsNullOrEmpty(configApp.tikiAu.access_token) ? string.Empty : configApp.tikiAu.access_token));
                // Làm mới access token
                string str;
                str = CommonTikiAPI.RefreshDataAuthorization(configApp.appID);

                if (!string.IsNullOrEmpty(str))
                {
                    MyLogger.GetInstance().Warn(str);
                    return response;
                }
                // Cập nhật configApp
                configApp.tikiAu.access_token = ModelThongTinBaoMat.Tiki_InhouseGetAccessToken(configApp.appID);

                // Thực hiện request lại
                request.AddOrUpdateHeader("Authorization", "Bearer " + configApp.tikiAu.access_token);
                response = client.Execute(request);
                MyLogger.InfoRestLog(client, request, response);
            }
            return response;
        }

        static public IRestResponse GetExcuteRequest(TikiConfigApp configApp, string http)
        {
            RestClient client = new RestClient(http);
            client.Timeout = -1;
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse response = ExcuteRequest(client, request, configApp);
            return response;
        }

        static public IRestResponse PutExcuteRequest(TikiConfigApp configApp, string http, TikiUpdate st)
        {
            RestClient client = new RestClient(http);
            client.Timeout = -1;
            RestRequest request = new RestRequest(Method.PUT);

            request.AddHeader("Content-Type", "application/json");

            string body = JsonConvert.SerializeObject(st, Formatting.Indented);
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = ExcuteRequest(client, request, configApp);
            return response;
        }
    }
}
