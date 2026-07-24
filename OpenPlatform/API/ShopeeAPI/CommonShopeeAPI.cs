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
using MySqlConnector;
using System.Data;

namespace MVCPlayWithMe.OpenPlatform.API.ShopeeAPI
{
    public class CommonShopeeAPI
    {
        public const string cShopeeHost = "https://partner.shopeemobile.com";

        static public ShopeeAuthen shopeeAuthen = null;

        static public async Task<ShopeeAuthen> ShopeeGetAuthen(MySqlConnection conn)
        {
            ShopeeAuthen shopeeAuthen = new ShopeeAuthen();
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tbShopeeAuthen WHERE Id = 1", conn);
                cmd.CommandType = CommandType.Text;
                using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        shopeeAuthen.shopId = MyMySql.GetString(rdr, "ShopId");
                        shopeeAuthen.partnerId = MyMySql.GetString(rdr, "PartnerId");
                        shopeeAuthen.partnerKey = MyMySql.GetString(rdr, "PartnerKey");
                        shopeeAuthen.code = MyMySql.GetString(rdr, "Code");
                        shopeeAuthen.validAccessTokenTime = MyMySql.GetDateTime(rdr, "ValidAccessTokenTime");
                        shopeeAuthen.shopeeToken.access_token = MyMySql.GetString(rdr, "AccessToken");
                        shopeeAuthen.shopeeToken.refresh_token = MyMySql.GetString(rdr, "RefreshToken");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                shopeeAuthen = null;
            }
            return shopeeAuthen;
        }

        /// <summary>
        /// Generate Authorization Token
        /// </summary>
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
        /// Generate fixed authorization URL
        /// </summary>
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
        public static async Task<ShopeeToken> ShopeeGetTokenShopLevelAsync()
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
            IRestResponse response = await Common.client.ExecuteAsync(request);
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
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    await shopeeMySql.ShopeeSaveTokenAsync(token, conn);
                    shopeeAuthen = await ShopeeGetAuthen(conn);
                }
            }

             return token;
        }

        public static async Task<MySqlResultState> ShopeeSaveLivePartnerKeyAsync(string key)
        {
            MySqlResultState result = null;
            try
            {
                ShopeeMySql shopeeMySql = new ShopeeMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    result = await shopeeMySql.ShopeeSaveLivePartnerKeyAsync(key, conn);
                    if (result.State == EMySqlResultState.OK)
                    {
                        shopeeAuthen = await ShopeeGetAuthen(conn);
                    }
                }
            }
            catch(Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        // Lưu code mới trong Redirect URL sau khi được chủ shop ủy quyền
        public static async Task<MySqlResultState> ShopeeSaveCode(string code)
        {
            MySqlResultState result = null;
            try
            {
                ShopeeMySql shopeeMySql = new ShopeeMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    result = await shopeeMySql.ShopeeSaveCodeAsync(code, conn);
                    if (result.State == EMySqlResultState.OK)
                    {
                        shopeeAuthen = await ShopeeGetAuthen(conn);
                    }
                }
            }
            catch(Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        /// <summary>
        /// Làm mới access token khi bị hết hạn
        /// </summary>
        public static async Task<ShopeeToken> ShopeeGetRefreshTokenShopLevelAsync()
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

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = await Common.client.ExecuteAsync(request);
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
            MyLogger.GetInstance().Info("old access_token: " + shopeeAuthen.shopeeToken.access_token);
            MyLogger.GetInstance().Info("old refresh_token: " + shopeeAuthen.shopeeToken.refresh_token);
            if (token != null)
            {
                MyLogger.GetInstance().Info("new access_token from token: " + token.access_token);
                MyLogger.GetInstance().Info("new refresh_token from token: " + token.refresh_token);

                ShopeeMySql shopeeMySql = new ShopeeMySql();
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    await shopeeMySql.ShopeeSaveTokenAsync(token, conn);
                    shopeeAuthen = await ShopeeGetAuthen(conn);
                }
            }
            else
            {
                MyLogger.GetInstance().Error("ShopeeGetRefreshTokenShopLevel get null token ");
            }
            return token;
        }

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
            string url = String.Format(CommonShopeeAPI.cShopeeHost + path +
                "?partner_id={0}&timestamp={1}&sign={2}&shop_id={3}&access_token={4}" +
                DevNameValuePair.GetQueryStringWithAndPrefix(ls), partner_id, timest, sign, shop_id, access_token);
            return url;
        }

        public static async Task<IRestResponse> ShopeeGetMethodAsync(string path, List<DevNameValuePair> ls)
        {
            IRestResponse response = null;
            try
            {
                if (DateTime.Now > shopeeAuthen.validAccessTokenTime)
                {
                    if (await ShopeeGetRefreshTokenShopLevelAsync() == null)
                    {
                        return null;
                    }
                }

                string url = GenerateURLShopeeAPI(path, ls);

                RestRequest request = new RestRequest(url, Method.GET);

                response = await Common.client.ExecuteAsync(request);
                MyLogger.InfoRestLog(Common.client, request, response);

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MyLogger.GetInstance().Info("ShopeeGetMethodAsync call because response.StatusCode == HttpStatusCode.Forbidden");

                    if (await ShopeeGetRefreshTokenShopLevelAsync() == null)
                    {
                        response = null;
                    }
                    else
                    {
                        url = GenerateURLShopeeAPI(path, ls);

                        request = new RestRequest(url, Method.GET);

                        response = await Common.client.ExecuteAsync(request);
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

        public static async Task<IRestResponse> ShopeePostMethodAsync(string path, string body)
        {
            IRestResponse response = null;
            try
            {
                if (DateTime.Now > shopeeAuthen.validAccessTokenTime)
                {
                    if (await ShopeeGetRefreshTokenShopLevelAsync() == null)
                    {
                        return null;
                    }
                }

                string url = GenerateURLShopeeAPI(path, null);

                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                response = await Common.client.ExecuteAsync(request);
                MyLogger.InfoRestLog(Common.client, request, response);

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MyLogger.GetInstance().Info("ShopeePostMethodAsync call because response.StatusCode == HttpStatusCode.Forbidden");

                    if (await ShopeeGetRefreshTokenShopLevelAsync() == null)
                    {
                        response = null;
                    }
                    else
                    {
                        url = GenerateURLShopeeAPI(path, null);

                        request = new RestRequest(url, Method.POST);
                        request.AddHeader("Content-Type", "application/json");
                        request.AddParameter("application/json", body, ParameterType.RequestBody);

                        response = await Common.client.ExecuteAsync(request);
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

        public static async Task<string> ShopeeGetShopInfoAsync()
        {
            string json = string.Empty;
            string path = "/api/v2/shop/get_shop_info";

            IRestResponse response = await ShopeeGetMethodAsync(path, null);
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
