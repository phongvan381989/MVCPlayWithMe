using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.TikTokShopAPI;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikTokApp.TikTokAuth
{
    /// <summary>
    /// TikTok Shop Authentication Database Operations
    /// Data access layer for tb_tiktok_authen table
    /// </summary>
    public class TikTokAuthMySql
    {
        /// <summary>
        /// Lấy thông tin authentication từ database
        /// </summary>
        public static async Task<TikTokAuth> GetAuthFromDBAsync(MySqlConnection conn)
        {
            TikTokAuth auth = new TikTokAuth();

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT Id, ServiceId, AppKey, AppSecret, AccessToken, AccessTokenExpireIn, RefreshToken, RefreshTokenExpireIn, ShopId, ShopCipher
                      FROM tb_tiktok_authen
                      WHERE Id = 1", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            auth.Id = MyMySql.GetInt32(rdr, "Id");
                            auth.ServiceId = MyMySql.GetString(rdr, "ServiceId");
                            auth.AppKey = MyMySql.GetString(rdr, "AppKey");
                            auth.AppSecret = MyMySql.GetString(rdr, "AppSecret");
                            auth.AccessToken = MyMySql.GetString(rdr, "AccessToken");
                            auth.AccessTokenExpireIn = MyMySql.GetInt64(rdr, "AccessTokenExpireIn");
                            auth.RefreshToken = MyMySql.GetString(rdr, "RefreshToken");
                            auth.RefreshTokenExpireIn = MyMySql.GetInt64(rdr, "RefreshTokenExpireIn");
                            auth.ShopId = MyMySql.GetString(rdr, "ShopId");
                            auth.ShopCipher = MyMySql.GetString(rdr, "ShopCipher");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok GetAuthFromDB error: {ex}");
                return null;
            }

            return auth;
        }

        /// <summary>
        /// Lưu hoặc cập nhật token vào database và lấy lại thông tin auth mới nhất từ database
        /// </summary>
        public static async Task<bool> SaveTokenAsync(
            string accessToken,
            long accessTokenExpireIn,
            string refreshToken,
            long refreshTokenExpireIn,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE tb_tiktok_authen
                      SET AccessToken = @AccessToken,
                          AccessTokenExpireIn = @AccessTokenExpireIn,
                          RefreshToken = @RefreshToken,
                          RefreshTokenExpireIn = @RefreshTokenExpireIn
                      WHERE Id = 1", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@AccessToken", accessToken ?? string.Empty);
                    cmd.Parameters.AddWithValue("@AccessTokenExpireIn", accessTokenExpireIn);
                    cmd.Parameters.AddWithValue("@RefreshToken", refreshToken ?? string.Empty);
                    cmd.Parameters.AddWithValue("@RefreshTokenExpireIn", refreshTokenExpireIn);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        MyLogger.GetInstance().Info($"TikTok auth tokens saved. AccessToken: {accessToken}, {accessTokenExpireIn}, RefreshToken: {refreshToken}, {refreshTokenExpireIn}");

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok SaveToken error: {ex}");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Save shop information (ShopId and ShopCipher) to database
        /// </summary>
        public static async Task<bool> SaveShopInfoAsync(string shopId, string shopCipher, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE tb_tiktok_authen
                      SET ShopId = @ShopId,
                          ShopCipher = @ShopCipher
                      WHERE Id = 1", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ShopId", shopId ?? string.Empty);
                    cmd.Parameters.AddWithValue("@ShopCipher", shopCipher ?? string.Empty);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        MyLogger.GetInstance().Info($"TikTok shop info saved. ShopId: {shopId}, ShopCipher: {shopCipher}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok SaveShopInfo error: {ex}");
                return false;
            }

            return false;
        }


        /// <summary>
        /// Save shop information (ShopId and ShopCipher) to database
        /// Typically called after GetAuthorizedShopsAsync
        /// </summary>
        /// <param name="shopId">Shop ID from authorized shops</param>
        /// <param name="shopCipher">Shop cipher from authorized shops</param>
        /// <returns>True if successful</returns>
        public static async Task<bool> SaveShopInfoAsync(string shopId, string shopCipher)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    bool result = await TikTokAuthMySql.SaveShopInfoAsync(shopId, shopCipher, conn);

                    if (result)
                    {
                        // Reload auth config
                        TikTokAuthAPI.tikTokAuth = await TikTokAuthMySql.GetAuthFromDBAsync(conn);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"TikTok SaveShopInfo error: {ex}");
                return false;
            }
        }
    }
}
