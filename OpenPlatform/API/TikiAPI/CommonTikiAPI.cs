using Newtonsoft.Json;
using MVCPlayWithMe.General;
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
using MVCPlayWithMe.OpenPlatform.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI
{
    public class CommonTikiAPI
    {
        static public TikiConfigApp tikiConfigApp;

        static public TikiConfigApp GetTikiConfigApp(MySqlConnection conn)
        {
            TikiConfigApp config = new TikiConfigApp();
            try
            {
                // Lưu vào bảng tbECommerceOrder
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tbTikiAuthen", conn);
                cmd.CommandType = CommandType.Text;
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        config.appID = MyMySql.GetString(rdr, "AppId");
                        config.homeAddress = MyMySql.GetString(rdr, "Home");
                        config.secretAppCode = MyMySql.GetString(rdr, "Secret");
                        config.tikiAu.access_token = MyMySql.GetString(rdr, "AccessToken");
                        config.tikiAu.expires_in = MyMySql.GetString(rdr, "ExpiresIn");
                        config.tikiAu.token_type = MyMySql.GetString(rdr, "TokenType");
                        config.tikiAu.scope = MyMySql.GetString(rdr, "Scope");
                        config.tikiAu.refreshAccessTokenTime = MyMySql.GetDateTime(rdr, "RefreshAccessTokenTime");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                config = null;
            }

            return config;
        }

        /// <summary>
        /// Khi Access token phục vụ authorization hết hạn, gọi hàm này lấy access token mới và lưu và db xml
        /// </summary>
        /// <param name="appID"></param>
        /// <returns>empty nếu thành công. Ngược lại trả về string mô tả lỗi</returns>
        static public string RefreshDataAuthorization()
        {
            RestRequest request = new RestRequest("https://api.tiki.vn/sc/oauth2/token", Method.POST);
            request.AddHeader("Authorization", "Basic " + tikiConfigApp.Tiki_GetAppCredentialBase64Format());
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", tikiConfigApp.appID);
            request.AddParameter("scope", "");
            IRestResponse response = null;
            try
            {
                response = Common.client.Execute(request);
                MyLogger.InfoRestLog(Common.client, request, response);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return "Có lỗi: " + ex.Message;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "Lấy quyền truy cập shop lỗi. Vui lòng thử lại.";
            }
            TikiAuthorization accessToken = JsonConvert.DeserializeObject<TikiAuthorization>(response.Content);
            TikiMySql tikiMySql = new TikiMySql();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();
                tikiMySql.TikiSaveAccessToken(accessToken, conn);

                // Cập nhật
                tikiConfigApp = GetTikiConfigApp(conn);
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            MyLogger.GetInstance().Info("New token: " + accessToken.access_token);
            return string.Empty;
        }

        /// <summary>
        /// Thực hiện 1 request HTTP, nếu access token hết hạn thì làm mới
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="configApp"></param>
        /// <returns></returns>
        static public IRestResponse ExcuteRequest(RestRequest request)
        {
            //if (CommonTikiAPI.tikiConfigApp == null)
            //{
            //    TikiMySql tikiMySql = new TikiMySql();
            //    CommonTikiAPI.tikiConfigApp = tikiMySql.GetTikiConfigApp();
            //    if (CommonTikiAPI.tikiConfigApp == null)
            //        return null;
            //}

            // Nếu access token hết hạn, ta làm mới
            if (Common.ConvertStringToInt32(tikiConfigApp.tikiAu.expires_in) == System.Int32.MinValue ||
                DateTime.Now > 
                tikiConfigApp.tikiAu.refreshAccessTokenTime.AddSeconds(
                Common.ConvertStringToInt32(tikiConfigApp.tikiAu.expires_in) - 600) // Trừ hao 600 giây
                )
            {
                string str = RefreshDataAuthorization();
                if (!string.IsNullOrEmpty(str))
                {
                    return null;
                }
            }

            request.AddHeader("Authorization", "Bearer " + (string.IsNullOrEmpty(tikiConfigApp.tikiAu.access_token) ? string.Empty: tikiConfigApp.tikiAu.access_token));
            IRestResponse response = Common.client.Execute(request);
            MyLogger.InfoRestLog(Common.client, request, response);

            // Phần code này dự là không bao giờ chạy vì đã được làm mới bên trên
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                MyLogger.GetInstance().Info("ExcuteRequest call because response.StatusCode == response.StatusCode == HttpStatusCode.Unauthorized");
                MyLogger.GetInstance().Info("Expried token:" + (string.IsNullOrEmpty(tikiConfigApp.tikiAu.access_token) ? string.Empty : tikiConfigApp.tikiAu.access_token));
                // Làm mới access token
                string str;
                str = CommonTikiAPI.RefreshDataAuthorization();

                if (!string.IsNullOrEmpty(str))
                {
                    MyLogger.GetInstance().Warn(str);
                    return response;
                }

                // Thực hiện request lại
                request.AddOrUpdateHeader("Authorization", "Bearer " + tikiConfigApp.tikiAu.access_token);
                response = Common.client.Execute(request);
                MyLogger.InfoRestLog(Common.client, request, response);
            }

            return response;
        }

        static public IRestResponse GetExcuteRequest(string http)
        {
            RestRequest request = new RestRequest(http, Method.GET);
            IRestResponse response = ExcuteRequest(request);
            return response;
        }

        static public IRestResponse PostExcuteRequest(string http, string body)
        {
            RestRequest request = new RestRequest(http, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = ExcuteRequest(request);
            return response;
        }

        static public IRestResponse PutExcuteRequest(string http, TikiUpdate st)
        {
            RestRequest request = new RestRequest(http, Method.PUT);

            request.AddHeader("Content-Type", "application/json");

            string body = JsonConvert.SerializeObject(st, Formatting.Indented);
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = ExcuteRequest(request);
            return response;
        }
    }
}
