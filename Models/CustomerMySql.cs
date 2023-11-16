using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class CustomerMySql : BasicMySql
    {
        /// <summary>
        /// Chỉ lấy Id của customer
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public Customer GetCustomerFromCookie(string userCookieIdentify)
        {
            Customer customer = new Customer();

            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);

            MySqlParameter paraoutId = new MySqlParameter();
            paraoutId.ParameterName = @"outId";
            paraoutId.Value = -1;
            paraoutId.Direction = ParameterDirection.Output;
            paras[1] = paraoutId;

            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCookie_Get_CustomerId", paras);

            if (result.State != EMySqlResultState.OK)
                return null;

            customer.id = (int)paras[1].Value;
            return customer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">SDT</param>
        /// <param name="userNameType">1: email, 2: SDT, 3: user name. Hard code 2</param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public MySqlResultState AddNewCustomer(string userName, int userNameType, string passWord)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            byte[] salt = Common.CreateSalt();
            byte[] hash = Common.GenerateSaltedHash(passWord, salt);
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
                paSalt.Size = Common.SHA512Size;
                paSalt.MySqlDbType = MySqlDbType.Binary;
                paSalt.Value = salt;
                cmd.Parameters.Add(paSalt);

                MySqlParameter paHash = new MySqlParameter();
                paHash.ParameterName = @"inHash";
                paHash.Size = Common.SHA512Size;
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
        /// Customer logout tài khoản
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public MySqlResultState CustomerLogout(string userCookieIdentify)
        {
            MySqlParameter[] paras = new MySqlParameter[3];

            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);

            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCookie_Logout", paras);
            return result;
        }


        public MySqlResultState LoginCustomer(string userName, string password)
        {
            return Login(userName, password, "st_tbCustomer_Get_Salt_Hash");
        }

        /// <summary>
        /// Lấy customer
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Customer GetCustomerFromUserName(string userName)
        {
            Customer customer = new Customer();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
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
        public MySqlResultState CookieCustomerLogin(string userCookieIdentify, int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);
            paras[1] = new MySqlParameter("@inCustomerId", customerId);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCookie_Login", paras);
            return result;
        }

        /// <summary>
        /// Thêm vào bảng tbCookie
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public MySqlResultState AddNewCookie(string userCookieIdentify, int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);
            paras[1] = new MySqlParameter("@inCustomerId", customerId);

            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCookie_Insert", paras);
            return result;
        }
    }
}