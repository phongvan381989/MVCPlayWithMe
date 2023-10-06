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
        public static string connStr;

        private static string errMessage;

        public static void Initialization()
        {
            connStr = ConfigurationManager.AppSettings["ConectMysql"];
        }

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

        ///// <summary>
        ///// Lấy tất cả thể loại
        ///// Thể loại phải khác trống
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> GetListCategory()
        //{
        //    MySqlConnection conn = new MySqlConnection(connStr);
        //    List<string> ls = new List<string>();
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Category", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr != null && rdr.HasRows)
        //        {
        //            while (rdr.Read())
        //            {
        //                ls.Add(rdr.GetString("Category"));
        //            }
        //        }
        //        if (rdr != null)
        //            rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        errMessage = ex.ToString();
        //        MyLogger.GetInstance().Warn(errMessage);
        //    }

        //    conn.Close();
        //    return ls;
        //}

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

        private const int SHA512Size = 64;

        /// <summary>
        /// Insert.new user to db.
        /// </summary>
        /// <param name="userName">Email / SDT/ UserName.</param>
        /// <param name="passWord">Password of user.</param>
        /// <param name="userNameType">1: email, 2: SDT, 3: user name</param>
        /// <param name="privilege">1: full quyền, tạo mới/ cập nhật sản phẩm, nhà phát hành</param>
        /// <returns>A result state.</returns>
        public static MySqlResultState AddNewAdministrator(string userName, int userNameType, string passWord, int privilege)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            byte[] salt = CreateSalt();
            byte[] hash = GenerateSaltedHash(passWord, salt);
            MySqlResultState result = new MySqlResultState();
            result.Message = "Thêm mới thành công.";
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbAdministrator_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (userNameType == 1)
                    cmd.Parameters.AddWithValue("@inEmail", userName);
                else
                    cmd.Parameters.AddWithValue("@inEmail", "");

                MySqlParameter paSalt = new MySqlParameter();
                paSalt.ParameterName = @"inSalt";
                paSalt.Size = SHA512Size;
                paSalt.MySqlDbType = MySqlDbType.Binary;
                paSalt.Value = salt;
                cmd.Parameters.Add(paSalt);

                MySqlParameter paHash = new MySqlParameter();
                paHash.ParameterName = @"inHash";
                paHash.Size = SHA512Size;
                paHash.MySqlDbType = MySqlDbType.Binary;
                paHash.Value = hash;
                cmd.Parameters.Add(paHash);

                if(userNameType == 2)
                    cmd.Parameters.AddWithValue("@inSDT", userName);
                else
                    cmd.Parameters.AddWithValue("@inSDT", "");

                if (userNameType == 3)
                    cmd.Parameters.AddWithValue("@inUserName", userName);
                else
                    cmd.Parameters.AddWithValue("@inUserName", "");

                cmd.Parameters.AddWithValue("@inPrivilege", privilege);

                //MySqlParameter paraoutResult = new MySqlParameter();
                //paraoutResult.ParameterName = @"outResult";
                //paraoutResult.Value = -1;
                //paraoutResult.Direction = ParameterDirection.Output;
                //cmd.Parameters.Add(paraoutResult);

                //MySqlParameter paraoutMessage = new MySqlParameter();
                //paraoutMessage.ParameterName = @"outMessage";
                //paraoutMessage.Value = "";
                //paraoutMessage.Direction = ParameterDirection.Output;
                //cmd.Parameters.Add(paraoutMessage);
                AddOutParameters(cmd.Parameters);

                int lengthPara = cmd.Parameters.Count;
                cmd.ExecuteNonQuery();
                if ((EMySqlResultState)cmd.Parameters[lengthPara - 2].Value != EMySqlResultState.OK)
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

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">SDT</param>
        /// <param name="userNameType">1: email, 2: SDT, 3: user name. Hard code 2</param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static MySqlResultState AddNewCostomer(string userName, int userNameType, string passWord)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            byte[] salt = CreateSalt();
            byte[] hash = GenerateSaltedHash(passWord, salt);
            MySqlResultState result = new MySqlResultState();
            result.Message = "Tạo tài khoản thành công.";
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCustomer_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (userNameType == 1)
                    cmd.Parameters.AddWithValue("@inEmail", userName);
                else
                    cmd.Parameters.AddWithValue("@inEmail", "");

                MySqlParameter paSalt = new MySqlParameter();
                paSalt.ParameterName = @"inSalt";
                paSalt.Size = SHA512Size;
                paSalt.MySqlDbType = MySqlDbType.Binary;
                paSalt.Value = salt;
                cmd.Parameters.Add(paSalt);

                MySqlParameter paHash = new MySqlParameter();
                paHash.ParameterName = @"inHash";
                paHash.Size = SHA512Size;
                paHash.MySqlDbType = MySqlDbType.Binary;
                paHash.Value = hash;
                cmd.Parameters.Add(paHash);

                if (userNameType == 2)
                    cmd.Parameters.AddWithValue("@inSDT", userName);
                else
                    cmd.Parameters.AddWithValue("@inSDT", "");

                if (userNameType == 3)
                    cmd.Parameters.AddWithValue("@inUserName", userName);
                else
                    cmd.Parameters.AddWithValue("@inUserName", "");

                cmd.Parameters.AddWithValue("@inFullName", "");

                cmd.Parameters.AddWithValue("@inBirthday", "1930-01-01"); // Mặc định

                cmd.Parameters.AddWithValue("@inSex", 4); // Mặc định

                MySqlParameter paraoutResult = new MySqlParameter();
                paraoutResult.ParameterName = @"outResult";
                paraoutResult.Value = -1;
                paraoutResult.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraoutResult);

                MySqlParameter paraoutMessage = new MySqlParameter();
                paraoutMessage.ParameterName = @"outMessage";
                paraoutMessage.Value = "";
                paraoutMessage.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraoutMessage);

                int lengthPara = cmd.Parameters.Count;
                cmd.ExecuteNonQuery();
                if ((EMySqlResultState)cmd.Parameters[lengthPara - 2].Value != EMySqlResultState.OK)
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

            return result;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="email">Email address as user name.</param>
        /// <param name="password">Password.</param>
        /// <returns>A result state.</returns>
        public static MySqlResultState LoginAdministrator(string userName, string password)
        {
            return Login(userName, password, "st_tbAdministrator_Get_Salt_Hash");
        }

        public static MySqlResultState LoginCustomer(string userName, string password)
        {
            return Login(userName, password, "st_tbCustomer_Get_Salt_Hash");
        }

        private static MySqlResultState Login(string userName, string password, string storePro)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlResultState result = new MySqlResultState();
            result.Message = "Đăng nhập thành công.";
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(storePro, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inUserName", userName);

                MySqlParameter paraoutSalt = new MySqlParameter();
                paraoutSalt.ParameterName = "@outSalt";
                paraoutSalt.MySqlDbType = MySqlDbType.String;
                paraoutSalt.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraoutSalt);

                MySqlParameter paraoutHash = new MySqlParameter();
                paraoutHash.ParameterName = "@outHash";
                paraoutHash.MySqlDbType = MySqlDbType.String;
                paraoutHash.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraoutHash);

                MySqlParameter paraoutResult = new MySqlParameter();
                paraoutResult.ParameterName = @"outResult";
                paraoutResult.Value = -1;
                paraoutResult.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraoutResult);

                MySqlParameter paraoutMessage = new MySqlParameter();
                paraoutMessage.ParameterName = @"outMessage";
                paraoutMessage.Value = "";
                paraoutMessage.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraoutMessage);

                int lengthPara = cmd.Parameters.Count;
                cmd.ExecuteNonQuery();
                if ((EMySqlResultState)cmd.Parameters[lengthPara - 2].Value != EMySqlResultState.OK)
                {
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.Message = (string)cmd.Parameters[lengthPara - 1].Value;
                    return result;
                }

                string str = Convert.ToString(paraoutSalt.Value);
                byte[] salt = Convert.FromBase64String(str);
                str = Convert.ToString(paraoutHash.Value);
                byte[] hash = Convert.FromBase64String(str);

                // Generate hash from login password with hash in db
                if (!ByteArrayCompare(GenerateSaltedHash(password, salt), hash))
                {
                    result.State = EMySqlResultState.INVALID;
                    result.Message = "Mật khẩu không đúng.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
            }

            return result;
        }

        private static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create a salt value.
        /// </summary>
        /// <returns>Salt value.</returns>
        private static byte[] CreateSalt()
        {
            // Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[SHA512Size];
            rng.GetBytes(buff);
            return buff;
        }

        /// <summary>
        /// Generate a hash value with a salt added to orignal input
        /// </summary>
        /// <param name="password"> Original input.</param>
        /// <param name="salt"> Salt value.</param>
        /// <returns>Byte array.</returns>
        private static byte[] GenerateSaltedHash(string password, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA512Managed();
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            byte[] plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];
            Array.Copy(plainText, plainTextWithSaltBytes, plainText.Length);
            Array.Copy(salt, 0, plainTextWithSaltBytes, plainText.Length, salt.Length);

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        /// <summary>
        /// Thêm vào bảng tbCookie mặc định khi có request mới
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public static MySqlResultState AddNewCookie(string userCookieIdentify, int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);
            paras[1] = new MySqlParameter("@inCustomerId", customerId);

            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbCookie_Insert", paras);
            return result;
        }

        /// <summary>
        /// Thêm vào bảng tbCookie_Administrator khi đăng nhập tài khoản quản trị
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public static MySqlResultState AddNewCookieAdministrator(string administratorCookieIdentify, int administratorId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inAdministratorCookieIdentify", administratorCookieIdentify);
            paras[1] = new MySqlParameter("@inAdministratorId", administratorId);

            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbCookie_Administrator_Login", paras);
            return result;
        }

        /// <summary>
        /// Cập nhật thời gian khi logout tài khoản
        /// </summary>
        /// <param name="administratorCookieIdentify"></param>
        /// <returns></returns>
        public static MySqlResultState AdministratorLogout(string administratorCookieIdentify)
        {
            MySqlParameter[] paras = new MySqlParameter[3];

            paras[0] = new MySqlParameter("@inAdministratorCookieIdentify", administratorCookieIdentify);

            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbCookie_Administrator_Logout", paras);
            return result;
        }
        /// <summary>
        ///  Chỉ lấy Id của administrator
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public static Administrator GetAdministratorFromCookie(string userCookieIdentify)
        {
            Administrator administrator = new Administrator();

            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inAdministratorCookieIdentify", userCookieIdentify);

            MySqlParameter paraoutId = new MySqlParameter();
            paraoutId.ParameterName = @"outId";
            paraoutId.Value = -1;
            paraoutId.Direction = ParameterDirection.Output;
            paras[1] = paraoutId;

            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbCookie_Administrator_Get_AdminId", paras);

            if (result.State != EMySqlResultState.OK)
                return null;

            administrator.id = (int)paras[1].Value;
            return administrator;
        }

        /// <summary>
        /// Chỉ lấy Id của customer
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public static Customer GetCustomerFromCookie(string userCookieIdentify)
        {
            Customer customer = new Customer();

            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);

            MySqlParameter paraoutId = new MySqlParameter();
            paraoutId.ParameterName = @"outId";
            paraoutId.Value = -1;
            paraoutId.Direction = ParameterDirection.Output;
            paras[1] = paraoutId;

            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbCookie_Get_CustomerId", paras);

            if (result.State != EMySqlResultState.OK)
                return null;

            customer.id = (int)paras[1].Value;
            return customer;
        }

        /// <summary>
        /// Lấy administrator
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static Administrator GetAdministratorFromUserName(string userName)
        {
            Administrator administrator = new Administrator();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbAdministrator_Get_Admin_From_UserName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inUserName", userName);

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        administrator.id = rdr.GetInt32("Id");
                        administrator.email = rdr.GetString("Email");
                        administrator.sdt = rdr.GetString("SDT");
                        administrator.userName = rdr.GetString("SDT");
                        administrator.privilege = rdr.GetInt32("Privilege");
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return administrator;
        }

        /// <summary>
        /// Lấy customer
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static Customer GetCustomerFromUserName(string userName)
        {
            Customer customer = new Customer();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCustomer_Get_Customer_From_UserName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inUserName", userName);

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        customer.id = rdr.GetInt32("Id");
                        customer.email = rdr.GetString("Email");
                        customer.sdt = rdr.GetString("SDT");
                        customer.userName = rdr.GetString("SDT");
                        customer.fullName = rdr.GetString("FullName");
                        customer.birthday = rdr.GetDateTime("Birthday"); 
                        customer.sex = rdr.GetInt32("Sex");
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return customer;
        }

        /// <summary>
        /// Customer login, update customerId
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public static MySqlResultState CookieCustomerLogin(string userCookieIdentify, int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);
            paras[1] = new MySqlParameter("@inCustomerId", customerId);
            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbCookie_Login", paras);
            return result;
        }

        /// <summary>
        /// Customer logout tài khoản
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public static MySqlResultState CustomerLogout(string userCookieIdentify)
        {
            MySqlParameter[] paras = new MySqlParameter[3];

            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);

            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbCookie_Logout", paras);
            return result;
        }

        public static int GetInt32(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return -1;
            else
                return rdr.GetInt32(columnName);
        }

        public static string GetString(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return string.Empty;
            else
                return rdr.GetString(columnName);
        }

        public static DateTime GetDateTime(MySqlDataReader rdr, string columnName)
        {
            if (Convert.IsDBNull(rdr[columnName]))
                return new DateTime();
            else
                return rdr.GetDateTime(columnName);
        }
    }
}