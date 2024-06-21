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

        private static string errMessage;

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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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
            //if (Convert.IsDBNull(rdr[columnName]))
            //    return -1;
            //else
            //    return rdr.GetInt32(columnName);
            try
            {
                return rdr.GetInt32(columnName);
            }
            catch(Exception)
            {
                return -1;
            }
        }

        public static long GetInt64(MySqlDataReader rdr, string columnName)
        {
            //if (Convert.IsDBNull(rdr[columnName]))
            //    return -1;
            //else
            //    return rdr.GetInt64(columnName);
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
            //if (Convert.IsDBNull(rdr[columnName]))
            //    return new DateTime();
            //else
            //    return rdr.GetDateTime(columnName);

            try
            {
                return rdr.GetDateTime(columnName);
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }
    }
}