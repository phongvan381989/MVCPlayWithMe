﻿using Newtonsoft.Json;
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

namespace MVCPlayWithMe.OpenPlatform.API.TikiAPI
{
    public class CommonTikiAPI
    {
        static public TikiConfigApp tikiConfigApp;

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
            TikiMySql tikiMySql = new TikiMySql();

            tikiMySql.TikiSaveAccessToken(accessToken);

            // Cập nhật
            tikiConfigApp = tikiMySql.GetTikiConfigApp();

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
            if (CommonTikiAPI.tikiConfigApp == null)
            {
                // Thử lấy
                TikiMySql tikiMySql = new TikiMySql();
                CommonTikiAPI.tikiConfigApp = tikiMySql.GetTikiConfigApp();
                if (CommonTikiAPI.tikiConfigApp == null)
                    return null;
            }

            request.AddHeader("Authorization", "Bearer " + (string.IsNullOrEmpty(tikiConfigApp.tikiAu.access_token) ? string.Empty: tikiConfigApp.tikiAu.access_token));
            IRestResponse response = Common.client.Execute(request);
            MyLogger.InfoRestLog(Common.client, request, response);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
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
