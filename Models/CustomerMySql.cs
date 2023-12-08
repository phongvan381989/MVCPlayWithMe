using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    /// <summary>
    /// Khách vãng lai không lưu userCookieIdentify vào db
    /// </summary>
    public class CustomerMySql : BasicMySql
    {
        /// <summary>
        /// Chỉ lấy Id của customer
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public Customer GetCustomerFromCookie(string userCookieIdentify)
        {
            if(string.IsNullOrEmpty(userCookieIdentify))
            {
                return null;
            }

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
                customer = null;
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
            //MySqlParameter[] paras = new MySqlParameter[4];

            //paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);
            //paras[1] = new MySqlParameter("@inCustomerId", customerId);
            //MyMySql.AddOutParameters(paras);

            //MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCookie_Login", paras);
            //return result;

            return AddNewCookie(userCookieIdentify, customerId);
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

        // Khi đăng nhập, cần mere cart cookie với db
        // Check trong db cart đã tồn tại, nếu đã tồn tại chỉ cần cập nhật trạng thái.
        // Ta không theo dõi chi tiết. Thực hiện trên store proceduce
        // Với số lượng ta không cộng dồn trong cart và db, ta lấy chỉ số lượng cho cart
        public void AddCartLogin(int customerId, List<Cart> ls)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_Cart_Insert_And_Update_Login", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                cmd.Parameters.AddWithValue("@inModelId", 0);
                cmd.Parameters.AddWithValue("@inQuantity", 0);
                cmd.Parameters.AddWithValue("@inReal", 0);

                foreach (var cart in ls)
                {
                    cmd.Parameters[1].Value = cart.id;
                    cmd.Parameters[2].Value = cart.q;
                    cmd.Parameters[3].Value = cart.real;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }
            conn.Close();
        }

        // Check trong db cart đã tồn tại, nếu đã tồn tại chỉ cần cập nhật trạng thái.
        // Ta không theo dõi chi tiết. Thực hiện trên store proceduce
        // Với số lượng ta không cộng dồn trong cart và db, ta lấy chỉ số lượng cho cart
        public MySqlResultState AddCart(int customerId, Cart cart, int maxQuantity)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_Cart_Insert_And_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                cmd.Parameters.AddWithValue("@inModelId", cart.id);
                cmd.Parameters.AddWithValue("@inQuantity", cart.q);
                cmd.Parameters.AddWithValue("@inReal", cart.real);
                cmd.Parameters.AddWithValue("@inMaxQuantity", maxQuantity);
                cmd.Parameters.AddWithValue("@outResult", 0);
                cmd.Parameters.AddWithValue("@outMessage", "");
                cmd.Parameters[5].Direction = ParameterDirection.Output;
                cmd.Parameters[6].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
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
            }
            conn.Close();
            return result;
        }

        public void AddCustomerInforAddress(int customerId, List<CustomerInforCookie> ls)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbAddress_Insert_And_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                cmd.Parameters.AddWithValue("@inName", "");
                cmd.Parameters.AddWithValue("@inPhone", "");
                cmd.Parameters.AddWithValue("@inProvince", "");
                cmd.Parameters.AddWithValue("@inDistrict", "");
                cmd.Parameters.AddWithValue("@inSubDistrict", "");
                cmd.Parameters.AddWithValue("@inDetail", "");
                cmd.Parameters.AddWithValue("@inDefaultAdd", 0);

                foreach (var cus in ls)
                {
                    cmd.Parameters[1].Value = cus.name;
                    cmd.Parameters[2].Value = cus.phone;
                    cmd.Parameters[3].Value = cus.province;
                    cmd.Parameters[4].Value = cus.district;
                    cmd.Parameters[5].Value = cus.subdistrict;
                    cmd.Parameters[6].Value = cus.detail;
                    cmd.Parameters[7].Value = cus.defaultAdd;
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }
            conn.Close();
        }
    }
}