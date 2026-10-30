using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.CassoInvoiceApp;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class CassoAuthenMySql
    {
        /// <summary>
        /// Lấy thông tin xác thực Casso (lấy record đầu tiên)
        /// </summary>
        public static async Task<CassoAuthen> GetAsync()
        {
            CassoAuthen authen = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM `tb_casso_authen` LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                authen = new CassoAuthen
                                {
                                    Id = MyMySql.GetInt32(rdr, "Id"),
                                    app_id = MyMySql.GetString(rdr, "app_id"),
                                    app_secret = MyMySql.GetString(rdr, "app_secret"),
                                    access_token = MyMySql.GetString(rdr, "access_token"),
                                    exp = MyMySql.GetInt64(rdr, "exp"),
                                    expires_in = MyMySql.GetInt32(rdr, "expires_in"),
                                    issued_at = MyMySql.GetInt64(rdr, "issued_at"),
                                    SkippingCharacters = MyMySql.GetString(rdr, "SkippingCharacters")
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return authen;
        }

        /// <summary>
        /// Lấy thông tin xác thực Casso (lấy record đầu tiên)
        /// </summary>
        public static async Task<string> GetSkippingCharactersAsync(MySqlConnection conn)
        {
            string skippingCharacters = null;
            try
            {
                string query = "SELECT SkippingCharacters FROM `tb_casso_authen` LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            skippingCharacters = MyMySql.GetString(rdr, "SkippingCharacters");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return skippingCharacters;
        }

        /// <summary>
        /// Lưu hoặc cập nhật chỉ thông tin thay đổi thường xuyên access_token
        /// </summary>
        public static async Task<MySqlResultState> SaveOrUpdateAsync(CassoAuthen authen)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Thử UPDATE trước (chỉ update token fields, KHÔNG update app_id, app_secret)
                    string updateQuery = @"UPDATE `tb_casso_authen`
                        SET `access_token` = @accessToken,
                            `exp` = @exp,
                            `expires_in` = @expiresIn,
                            `issued_at` = @issuedAt
                        LIMIT 1";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.CommandType = CommandType.Text;
                        updateCmd.Parameters.Add("@accessToken", MySqlDbType.VarChar).Value = authen.access_token ?? string.Empty;
                        updateCmd.Parameters.Add("@exp", MySqlDbType.Int64).Value = authen.exp;
                        updateCmd.Parameters.Add("@expiresIn", MySqlDbType.Int32).Value = authen.expires_in;
                        updateCmd.Parameters.Add("@issuedAt", MySqlDbType.Int64).Value = authen.issued_at;

                        await updateCmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }
    }
}
