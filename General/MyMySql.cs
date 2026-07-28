using MVCPlayWithMe.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MVCPlayWithMe.General
{
    public class MyMySql
    {
        // Đây là admin connect với nhiều quyền hơn
        public static string connStr;

        public static string customerConnStr;

        /// <summary>
        /// Thêm 2 parameters outResult và outMessage
        /// </summary>
        /// <param name="parameters"></param>
        public static void AddOutParameters(MySqlParameterCollection parameters)
        {
            MySqlParameter paraoutResult = new MySqlParameter();
            paraoutResult.ParameterName = @"outResult";
            paraoutResult.Value = -1;
            paraoutResult.Direction = ParameterDirection.Output;
            parameters.Add(paraoutResult);

            MySqlParameter paraoutMessage = new MySqlParameter();
            paraoutMessage.ParameterName = @"outMessage";
            paraoutMessage.Value = "";
            paraoutMessage.Direction = ParameterDirection.Output;
            parameters.Add(paraoutMessage);
        }

        /// <summary>
        /// Thêm 2 parameters outResult và outMessage
        /// </summary>
        /// <param name="parameters"> Mảng tham số đầu vào</param>
        /// <param name="lengthParas">Chiều dài mảng tham số</param>
        public static void AddOutParameters(MySqlParameter[] paras)
        {
            int lengthParas = paras.Length;
            MySqlParameter paraoutResult = new MySqlParameter();
            paraoutResult.ParameterName = @"outResult";
            paraoutResult.Value = -1;
            paraoutResult.Direction = ParameterDirection.Output;
            paras[lengthParas - 2] = paraoutResult;

            MySqlParameter paraoutMessage = new MySqlParameter();
            paraoutMessage.ParameterName = @"outMessage";
            paraoutMessage.Value = "";
            paraoutMessage.Direction = ParameterDirection.Output;
            paras[lengthParas - 1] = paraoutMessage;
        }

        public static int GetInt32(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return -1;

            try
            {
                return rdr.GetInt32(columnName);
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetInt32 failed for column '{columnName}': {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Overload: Đọc Int32 từ column index (nhanh hơn khi đọc nhiều rows)
        /// </summary>
        /// <param name="rdr">MySqlDataReader</param>
        /// <param name="indexColumn">Column index (lấy từ rdr.GetOrdinal(columnName))</param>
        /// <returns>Int32 value, hoặc -1 nếu NULL/error</returns>
        public static int GetInt32(MySqlDataReader rdr, int indexColumn)
        {
            if (Convert.IsDBNull(rdr[indexColumn]))
                return -1;

            try
            {
                return rdr.GetInt32(indexColumn);
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetInt32 failed for column index {indexColumn}: {ex.Message}");
                return -1;
            }
        }

        public static float GetFloat(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return -1;

            try
            {
                return rdr.GetFloat(columnName);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetFloat failed for column '{columnName}': {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Overload: Đọc Float từ column index (nhanh hơn khi đọc nhiều rows)
        /// </summary>
        public static float GetFloat(MySqlDataReader rdr, int indexColumn)
        {
            if (Convert.IsDBNull(rdr[indexColumn]))
                return -1;

            try
            {
                return rdr.GetFloat(indexColumn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetFloat failed for column index {indexColumn}: {ex.Message}");
                return -1;
            }
        }

        public static byte[] GetByteArray(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return null;

            try
            {
                return (byte[])rdr[columnName];
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetByteArray failed for column '{columnName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Overload: Đọc byte[] từ column index (nhanh hơn khi đọc nhiều rows)
        /// </summary>
        public static byte[] GetByteArray(MySqlDataReader rdr, int indexColumn)
        {
            if (Convert.IsDBNull(rdr[indexColumn]))
                return null;

            try
            {
                return (byte[])rdr[indexColumn];
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetByteArray failed for column index {indexColumn}: {ex.Message}");
                return null;
            }
        }

        public static long GetInt64(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return -1;

            try
            {
                return rdr.GetInt64(columnName);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetInt64 failed for column '{columnName}': {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Overload: Đọc Int64 từ column index (nhanh hơn khi đọc nhiều rows)
        /// </summary>
        public static long GetInt64(MySqlDataReader rdr, int indexColumn)
        {
            if (Convert.IsDBNull(rdr[indexColumn]))
                return -1;

            try
            {
                return rdr.GetInt64(indexColumn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetInt64 failed for column index {indexColumn}: {ex.Message}");
                return -1;
            }
        }

        public static string GetString(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return string.Empty;

            try
            {
                return rdr.GetString(columnName);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetString failed for column '{columnName}': {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Overload: Đọc String từ column index (nhanh hơn khi đọc nhiều rows)
        /// </summary>
        public static string GetString(MySqlDataReader rdr, int indexColumn)
        {
            if (Convert.IsDBNull(rdr[indexColumn]))
                return string.Empty;

            try
            {
                return rdr.GetString(indexColumn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetString failed for column index {indexColumn}: {ex.Message}");
                return string.Empty;
            }
        }

        public static DateTime? GetDateTime(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return null;

            try
            {
                return rdr.GetDateTime(columnName);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetDateTime failed for column '{columnName}': {ex.Message}");
                return DateTime.MinValue;
            }
        }

        public static async Task<MySqlResultState> LoginAsync(string userName, string password, string storePro)
        {
            MySqlResultState result = new MySqlResultState();
            result.Message = "Đăng nhập thành công.";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(storePro, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inUserName", userName);

                        byte[] salt = null;
                        byte[] hash = null;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                salt = GetByteArray(rdr, "Salt");
                                hash = GetByteArray(rdr, "Hash");
                            }
                        }

                        if (salt == null || !Common.ByteArrayCompare(Common.GenerateSaltedHash(password, salt), hash))
                        {
                            result.State = EMySqlResultState.INVALID;
                            result.Message = "Mật khẩu không đúng.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public static async Task<MySqlResultState> ChangePasswordAsync(int id, string oldPassWord,
            string newPassWord, string renewPassWord,
            string storeGetSaltHash, string storeChangePassword)
        {
            MySqlResultState result = new MySqlResultState();
            result.Message = "Thay đổi mật khẩu thành công.";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(storeGetSaltHash, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@InId", id);

                        byte[] salt = null;
                        byte[] hash = null;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                salt = GetByteArray(rdr, "Salt");
                                hash = GetByteArray(rdr, "Hash");
                            }
                        }

                        if (!Common.ByteArrayCompare(Common.GenerateSaltedHash(oldPassWord, salt), hash))
                        {
                            result.State = EMySqlResultState.INVALID;
                            result.Message = "Mật khẩu cũ không đúng.";
                        }

                        if (result.State == EMySqlResultState.OK)
                        {
                            using (MySqlCommand cmdChange = new MySqlCommand(storeChangePassword, conn))
                            {
                                cmdChange.CommandType = CommandType.StoredProcedure;
                                cmdChange.Parameters.AddWithValue("@InId", id);

                                salt = Common.CreateSalt();
                                hash = Common.GenerateSaltedHash(newPassWord, salt);

                                MySqlParameter paSalt = new MySqlParameter();
                                paSalt.ParameterName = @"inSalt";
                                paSalt.Size = Common.SHA256Size;
                                paSalt.MySqlDbType = MySqlDbType.Binary;
                                paSalt.Value = salt;
                                cmdChange.Parameters.Add(paSalt);

                                MySqlParameter paHash = new MySqlParameter();
                                paHash.ParameterName = @"inHash";
                                paHash.Size = Common.SHA256Size;
                                paHash.MySqlDbType = MySqlDbType.Binary;
                                paHash.Value = hash;
                                cmdChange.Parameters.Add(paHash);

                                await cmdChange.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public static async Task<MySqlResultState> ExcuteNonQueryAsync(string stName, MySqlParameter[] paras)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(stName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(paras);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public static async Task<MySqlResultState> ExcuteNonQueryStoreProcedureAsync(string stName, MySqlParameter[] paras)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(stName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(paras);
                        await cmd.ExecuteNonQueryAsync();
                        int lengthPara = cmd.Parameters.Count;
                        result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                        result.Message = (string)cmd.Parameters[lengthPara - 1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public static async Task<int> ExcuteGetIdStoreProcedureAsync(string stName, MySqlParameter[] paras)
        {
            int id = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(stName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(paras);
                        await cmd.ExecuteNonQueryAsync();
                        int lengthPara = cmd.Parameters.Count;
                        id = (int)cmd.Parameters[lengthPara - 2].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return id;
        }
    }
}
