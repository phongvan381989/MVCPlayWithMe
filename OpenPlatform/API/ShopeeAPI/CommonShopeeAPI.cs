using Newtonsoft.Json;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeConfig;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI
{
    public class CommonShopeeAPI
    {
        public const string cShopeeHost = "https://partner.shopeemobile.com";

        static public ShopeeAuthen shopeeAuthen = null;
        /// <summary>
        /// Generate Authorization Token
        /// </summary>
        /// <param name="redirectURL"></param>
        /// <param name="partnerKey"></param>
        /// <returns></returns>
        public static string ShopeeCallToken(String redirectURL, String partnerKey)
        {
            String str = string.Empty;
            String baseStr = partnerKey + redirectURL;
            SHA256 mySHA256 = SHA256.Create();
            byte[] hashValue = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(baseStr));
            str = BitConverter.ToString(hashValue);
            return str.Replace("-", "").ToLower();
        }

        /// <summary>
        /// Generate fixed authorization URL:
        /// </summary>
        /// <returns></returns>
        public static string ShopeeGenerateAuthPartnerUrl()
        {
            DateTime start = DateTime.Now;
            long timest = ((DateTimeOffset)start).ToUnixTimeSeconds();
            string host = "https://partner.shopeemobile.com";
            string path = "/api/v2/shop/auth_partner";
            string redirect = "https://vnexpress.net/";
            long partner_id = 2002851;
            ShopeeMySql shopeeMySql = new ShopeeMySql();
            string tmp_partner_key = shopeeAuthen.partnerKey;
            string tmp_base_string = String.Format("{0}{1}{2}", partner_id, path, timest);
            byte[] partner_key = Encoding.UTF8.GetBytes(tmp_partner_key);
            byte[] base_string = Encoding.UTF8.GetBytes(tmp_base_string);
            var hash = new HMACSHA256(partner_key);
            byte[] tmp_sign = hash.ComputeHash(base_string);
            string sign = BitConverter.ToString(tmp_sign).Replace("-", "").ToLower();
            string url = String.Format(host + path + "?partner_id={0}&timestamp={1}&sign={2}&redirect={3}", partner_id, timest, sign, redirect);
            MyLogger.GetInstance().Info(url);
            return url;
        }

        /// <summary>
        /// Nhận được access token sau khi được chủ shop ủy quyền
        /// </summary>
        /// <returns></returns>
        public static ShopeeToken ShopeeGetTokenShopLevel()
        {
            string shop_id = shopeeAuthen.shopId;
            string partner_id = shopeeAuthen.partnerId;
            string partner_key = shopeeAuthen.partnerKey;
            string code = shopeeAuthen.code;

            long timest = Common.GetTimestampNow();

            string path = "/api/v2/auth/token/get";
            string tmp_base_string = String.Format("{0}{1}{2}", partner_id, path, timest);
            byte[] byte_partner_key = Encoding.UTF8.GetBytes(partner_key);
            byte[] byte_base_string = Encoding.UTF8.GetBytes(tmp_base_string);
            var hash = new HMACSHA256(byte_partner_key);
            byte[] tmp_sign = hash.ComputeHash(byte_base_string);
            string sign = BitConverter.ToString(tmp_sign).Replace("-", "").ToLower();

            string url = String.Format(cShopeeHost + path + "?partner_id={0}&timestamp={1}&sign={2}", partner_id, timest, sign);

            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
            " + "\n" +
            @"    ""code"":""" + code + @""",
            " + "\n" +
            @"    ""shop_id"":" + shop_id + @",
            " + "\n" +
            @"    ""partner_id"":" + partner_id + @"
            " + "\n" +
            @"}";

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = Common.client.Execute(request);
            MyLogger.InfoRestLog(Common.client, request, response);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            ShopeeToken token = null;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                token = JsonConvert.DeserializeObject<ShopeeToken>(response.Content, settings);

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return null;
            }
            if (token != null)
            {
                ShopeeMySql shopeeMySql = new ShopeeMySql();
                shopeeMySql.ShopeeSaveToken(token);
                // Cập nhật lại shopee authen
                shopeeAuthen = shopeeMySql.ShopeeGetAuthen();
            }

             return token;
        }

        /// <summary>
        /// Làm mới access token khi bị hết hạn
        /// </summary>
        /// <param name="shop_id"></param>
        /// <param name="partner_id"></param>
        /// <param name="partner_key"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static ShopeeToken ShopeeGetRefreshTokenShopLevel()
        {
            long timest = Common.GetTimestampNow();

            string shop_id = shopeeAuthen.shopId;
            string partner_id = shopeeAuthen.partnerId;
            string partner_key = shopeeAuthen.partnerKey;
            string refreh_token = shopeeAuthen.shopeeToken.refresh_token;

            string path = "/api/v2/auth/access_token/get";
            string tmp_base_string = String.Format("{0}{1}{2}", partner_id, path, timest);
            byte[] byte_partner_key = Encoding.UTF8.GetBytes(partner_key);
            byte[] byte_base_string = Encoding.UTF8.GetBytes(tmp_base_string);
            var hash = new HMACSHA256(byte_partner_key);
            byte[] tmp_sign = hash.ComputeHash(byte_base_string);
            string sign = BitConverter.ToString(tmp_sign).Replace("-", "").ToLower();
            string url = String.Format(cShopeeHost + path + "?partner_id={0}&timestamp={1}&sign={2}", partner_id, timest, sign);

            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
            " + "\n" +
                        @"    ""refresh_token"":""" + refreh_token + @""",
            " + "\n" +
                        @"    ""shop_id"":" + shop_id.ToString() + @",
            " + "\n" +
                        @"    ""partner_id"":" + partner_id + @"
            " + "\n" +
            @"}";
            //MyLogger.GetInstance().Info(body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = Common.client.Execute(request);
            MyLogger.InfoRestLog(Common.client, request, response);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            //MyLogger.GetInstance().Info(response.Content);
            ShopeeToken token = null;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                token = JsonConvert.DeserializeObject<ShopeeToken>(response.Content, settings);

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                return null;
            }
            // Lưu giá trị cũ vào log
            MyLogger.GetInstance().Info("old access_token: " + shopeeAuthen.shopeeToken.access_token);
            MyLogger.GetInstance().Info("old refresh_token: " + shopeeAuthen.shopeeToken.refresh_token);
            if (token != null)
            {
                MyLogger.GetInstance().Info("new access_token from token: " + token.access_token);
                MyLogger.GetInstance().Info("new refresh_token from token: " + token.refresh_token);

                ShopeeMySql shopeeMySql = new ShopeeMySql();
                shopeeMySql.ShopeeSaveToken(token);
                // Cập nhật lại shopee authen
                shopeeAuthen = shopeeMySql.ShopeeGetAuthen();
            }
            MyLogger.GetInstance().Info("new access_token from db: " + shopeeAuthen.shopeeToken.access_token);
            MyLogger.GetInstance().Info("new refresh_token from db: " + shopeeAuthen.shopeeToken.refresh_token);
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partner_id"></param>
        /// <param name="access_token"></param>
        /// <param name="shop_id"></param>
        /// <param name="partner_key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GenerateURLShopeeAPI(string path, List<DevNameValuePair> ls)
        {
            long timest = Common.GetTimestampNow();
            string shop_id = shopeeAuthen.shopId;
            string partner_id = shopeeAuthen.partnerId;
            string partner_key = shopeeAuthen.partnerKey;
            string access_token = shopeeAuthen.shopeeToken.access_token;

            string tmp_base_string = String.Format("{0}{1}{2}{3}{4}", partner_id, path, timest, access_token, shop_id);
            byte[] byte_partner_key = Encoding.UTF8.GetBytes(partner_key);
            byte[] byte_base_string = Encoding.UTF8.GetBytes(tmp_base_string);
            var hash = new HMACSHA256(byte_partner_key);
            byte[] tmp_sign = hash.ComputeHash(byte_base_string);
            string sign = BitConverter.ToString(tmp_sign).Replace("-", "").ToLower();
            string url = String.Format(CommonShopeeAPI.cShopeeHost + path + "?partner_id={0}&timestamp={1}&sign={2}&shop_id={3}&access_token={4}" + DevNameValuePair.GetQueryStringWithAndPrefix(ls), partner_id, timest, sign, shop_id, access_token);
            return url;
        }

        public static IRestResponse ShopeeGetMethod(string path, List<DevNameValuePair> ls)
        {
            string url = GenerateURLShopeeAPI(path, ls);
            IRestResponse response = null;
            try
            {
                RestRequest request = new RestRequest(url, Method.GET);

                response = Common.client.Execute(request);
                MyLogger.InfoRestLog(Common.client, request, response);

                if (response.StatusCode == HttpStatusCode.Forbidden) // Làm mới access token và thử lại
                {
                    if (ShopeeGetRefreshTokenShopLevel() == null)
                    {
                        response = null;
                    }
                    else
                    {
                        url = GenerateURLShopeeAPI(path, ls);

                        request = new RestRequest(url, Method.GET);

                        response = Common.client.Execute(request);
                        MyLogger.InfoRestLog(Common.client, request, response);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                response = null;
            }
            return response;
        }

        public static string GetResponseErrorMessage(CommonResponseHTTP obj)
        {
            return "error: " + obj.error + ", message: " + obj.message;
        }

        public static IRestResponse ShopeePostMethod(string path, string body)
        {
            string url = GenerateURLShopeeAPI(path, null);

            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = null;
            try
            {
                response = Common.client.Execute(request);
                MyLogger.InfoRestLog(Common.client, request, response);

                if (response.StatusCode == HttpStatusCode.Forbidden) // Làm mới access token và thử lại
                {
                    if (ShopeeGetRefreshTokenShopLevel() == null)
                    {
                        response = null;
                    }
                    else
                    {
                        url = GenerateURLShopeeAPI(path, null);

                        request = new RestRequest(url, Method.POST);
                        request.AddHeader("Content-Type", "application/json");
                        request.AddParameter("application/json", body, ParameterType.RequestBody);

                        response = Common.client.Execute(request);
                        MyLogger.InfoRestLog(Common.client, request, response);
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.Message);
                response = null;
            }
            return response;
        }

        public static string ShopeeGetShopInfo()
        {
            string json = string.Empty;
            string path = "/api/v2/shop/get_shop_info";

            IRestResponse response = ShopeeGetMethod(path, null);
            if (response == null)
                return json;

            if (response.StatusCode == HttpStatusCode.OK)
                json = response.Content;

            return json;
        }
        #region Product

        #endregion

    }
}
