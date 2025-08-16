using MVCPlayWithMe.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MVCPlayWithMe.General
{
    public class MyMySql
    {
        // Đây là admin connect với nhiều quyền hơn
        public static string connStr;

        public static string customerConnStr;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stName"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static MySqlResultState ExcuteNonQueryStoreProceduce(string stName, List<MySqlParameter> paras)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(stName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (paras != null)
                {
                    foreach (var para in paras)
                    {
                        cmd.Parameters.Add(para);
                    }
                }

                cmd.ExecuteNonQuery();
                int lengthPara = cmd.Parameters.Count;
                {
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.Message = (string)cmd.Parameters[lengthPara - 1].Value;
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public static MySqlResultState ExcuteNonQueryStoreProceduce(string stName, MySqlParameter[] paras)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlResultState result = new MySqlResultState();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(stName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                cmd.ExecuteNonQuery();
                int lengthPara = cmd.Parameters.Count;
                //if ((EMySqlResultState)cmd.Parameters[lengthPara - 2].Value != EMySqlResultState.OK)
                {
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.Message = (string)cmd.Parameters[lengthPara - 1].Value;
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public static MySqlResultState ExcuteNonQuery(string stName, MySqlParameter[] paras)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(stName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();
            return result;
        }

        public static int ExcuteGetIdStoreProceduce(string stName, MySqlParameter[] paras)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            int id = 0;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(stName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                cmd.ExecuteNonQuery();
                int lengthPara = cmd.Parameters.Count;
                id = (int)cmd.Parameters[lengthPara - 2].Value;
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return id;
        }

        public static MySqlResultState ExcuteNonQueryStoreProceduce(string stName, MySqlParameterCollection paras)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlResultState result = new MySqlResultState();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(stName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Insert(0, paras);

                cmd.ExecuteNonQuery();
                int lengthPara = cmd.Parameters.Count;
                //if ((EMySqlResultState)cmd.Parameters[lengthPara - 2].Value != EMySqlResultState.OK)
                {
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.Message = (string)cmd.Parameters[lengthPara - 1].Value;
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

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
            catch(Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static DateTime GetDateTime(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return DateTime.MinValue;

            try
            {
                return rdr.GetDateTime(columnName);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static MySqlResultState Login(string userName, string password, string storePro)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            MySqlResultState result = new MySqlResultState();
            result.Message = "Đăng nhập thành công.";
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(storePro, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inUserName", userName);

                MySqlDataReader rdr = cmd.ExecuteReader();
                byte[] salt = null;
                byte[] hash = null;
                while (rdr.Read())
                {
                    salt = MyMySql.GetByteArray(rdr, "Salt");
                    hash = MyMySql.GetByteArray(rdr, "Hash");
                }
                rdr.Close();

                // Generate hash from login password with hash in db
                if (salt == null || !Common.ByteArrayCompare(Common.GenerateSaltedHash(password, salt), hash))
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Mật khẩu không đúng.";
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public static MySqlResultState ChangePassword(int id, string oldPassWord,
            string newPassWord, string renewPassWord,
            string storeGetSaltHash, string storeChagePassword)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            MySqlResultState result = new MySqlResultState();
            result.Message = "Thay đổi mật khẩu thành công.";
            try
            {
                conn.Open();

                // Kiểm tra mật khẩu cũ chính xác không
                MySqlCommand cmd = new MySqlCommand(storeGetSaltHash, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                byte[] salt = null;
                byte[] hash = null;
                while (rdr.Read())
                {
                    salt = MyMySql.GetByteArray(rdr, "Salt");
                    hash = MyMySql.GetByteArray(rdr, "Hash");
                }
                rdr.Close();

                // Generate hash from login password with hash in db
                if (!Common.ByteArrayCompare(Common.GenerateSaltedHash(oldPassWord, salt), hash))
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Mật khẩu cũ không đúng.";
                }

                if (result.State == EMySqlResultState.OK)
                {
                    // Thay đổi mật khẩu cũ
                    MySqlCommand cmdChange = new MySqlCommand(storeChagePassword, conn);
                    cmdChange.CommandType = CommandType.StoredProcedure;
                    cmdChange.Parameters.AddWithValue("@InId", id);

                    salt = Common.CreateSalt();
                    hash = Common.GenerateSaltedHash(newPassWord, salt);

                    MySqlParameter paSalt = new MySqlParameter();
                    paSalt.ParameterName = @"inSalt";
                    paSalt.Size = Common.SHA512Size;
                    paSalt.MySqlDbType = MySqlDbType.Binary;
                    paSalt.Value = salt;
                    cmdChange.Parameters.Add(paSalt);

                    MySqlParameter paHash = new MySqlParameter();
                    paHash.ParameterName = @"inHash";
                    paHash.Size = Common.SHA512Size;
                    paHash.MySqlDbType = MySqlDbType.Binary;
                    paHash.Value = hash;
                    cmdChange.Parameters.Add(paHash);

                    cmdChange.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public static MySqlResultState MyExcuteNonQuery(MySqlCommand cmd)
        {
            MySqlResultState resultState = new MySqlResultState();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            return resultState;
        }
    }
}