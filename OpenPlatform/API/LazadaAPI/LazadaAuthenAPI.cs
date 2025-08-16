using Lazop.Api;
using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaConfig;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.LazadaAPI
{
    public class LazadaAuthenAPI
    {
        public static LazadaAuth lazadaAuthen;
        public static string serverUrl = "https://api.lazada.com/rest";

        // Sau khi shopee ủy quyền, ta có được code từ url call back.
        // Từ code ta lấy được access, refresh token lần đầu
        public static Boolean LazadaAuthTokenCreate(string code)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                conn.Open();

                try
                {
                    LazopClient client = new LazopClient(serverUrl,
                        lazadaAuthen.appKey, lazadaAuthen.appSecret);
                    LazopRequest request = new LazopRequest("/auth/token/create");
                    request.AddApiParameter("code", code);
                    LazopResponse response = client.Execute(request);
                    MyLogger.LazadaRestLog(request, response);

                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    LazadaAuthTokenCreateResponseBody token =
                        JsonConvert.DeserializeObject<LazadaAuthTokenCreateResponseBody>(response.Body, settings);

                    LazadaSaveToken(token, conn);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return false;
                }
            }
            return true;
        }

        // Khi access token hết hạn, ta làm mới
        public static Boolean LazadaAuthTokenRefresh()
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                conn.Open();

                try
                {
                    LazopClient client = new LazopClient(serverUrl,
                        lazadaAuthen.appKey, lazadaAuthen.appSecret);
                    LazopRequest request = new LazopRequest("/auth/token/refresh");
                    request.AddApiParameter("refresh_token", lazadaAuthen.refreshToken);
                    LazopResponse response = client.Execute(request);
                    MyLogger.LazadaRestLog(request, response);

                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    LazadaAuthTokenCreateResponseBody token =
                        JsonConvert.DeserializeObject<LazadaAuthTokenCreateResponseBody>(response.Body, settings);

                    LazadaSaveToken(token, conn);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.Message);
                    return false;
                }
            }
            return true;
        }

        public static void LazadaSaveToken(LazadaAuthTokenCreateResponseBody lazadaToken,
    MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd =
                    new MySqlCommand(@"UPDATE `tb_lazada_authen` SET `AccessToken`=@AccessToken,
                    `ExpiresIn`=@ExpiresIn,
                    `RefreshToken`=@RefreshToken,
                    `RefreshExpiresIn`=@RefreshExpiresIn,
                    `RefreshDatetime` = NOW()
                    WHERE `Id` = 1", conn);
                cmd.Parameters.AddWithValue("@AccessToken", lazadaToken.access_token);
                cmd.Parameters.AddWithValue("@ExpiresIn", lazadaToken.expires_in);
                cmd.Parameters.AddWithValue("@RefreshToken", lazadaToken.refresh_token);
                cmd.Parameters.AddWithValue("@RefreshExpiresIn", lazadaToken.refresh_expires_in);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                lazadaAuthen = LazadaGetAuthFromDB(conn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public static LazadaAuth LazadaGetAuthFromDB(MySqlConnection conn)
        {
            LazadaAuth lazadaAuth = new LazadaAuth();
            try
            {
                MySqlCommand cmd =
                    new MySqlCommand(@"SELECT * FROM tb_lazada_authen
                    WHERE `Id` = 1", conn);
                cmd.CommandType = CommandType.Text;
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lazadaAuth.appKey = MyMySql.GetString(rdr, "AppKey");
                        lazadaAuth.appSecret = MyMySql.GetString(rdr, "AppSecret");

                        lazadaAuth.accessToken = MyMySql.GetString(rdr, "AccessToken");
                        lazadaAuth.expiresIn = MyMySql.GetInt32(rdr, "ExpiresIn");

                        lazadaAuth.refreshToken = MyMySql.GetString(rdr, "RefreshToken");
                        lazadaAuth.refreshExpiresIn = MyMySql.GetInt32(rdr, "RefreshExpiresIn");

                        lazadaAuth.refreshDatetime = MyMySql.GetDateTime(rdr, "RefreshDatetime");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                lazadaAuth = null;
            }
            return lazadaAuth;
        }

        public static Boolean LazadaRefreshAccessTokenIfNeed()
        {
            if (lazadaAuthen.refreshDatetime.AddSeconds(lazadaAuthen.expiresIn - 300)
                < DateTime.Now)
            {
                return LazadaAuthTokenRefresh();
            }
            return true;
        }
    }
}
