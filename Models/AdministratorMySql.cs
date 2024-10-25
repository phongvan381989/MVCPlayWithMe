using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class AdministratorMySql : BasicMySql
    {
        /// <summary>
        ///  Chỉ lấy Id của administrator
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <returns></returns>
        public Administrator GetAdministratorFromCookie(string userCookieIdentify)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            int id = -1;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCookie_Administrator_Get_From_CookieIdentify", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inAdministratorCookieIdentify", userCookieIdentify);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "AdministratorId");
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                id = -1;
            }

            conn.Close();
            if (id == -1)
                return null;

            Administrator administrator = new Administrator();
            administrator.id = id;
            return administrator;
        }

        /// <summary>
        /// Insert.new user to db.
        /// </summary>
        /// <param name="userName">Email / SDT/ UserName.</param>
        /// <param name="passWord">Password of user.</param>
        /// <param name="userNameType">1: email, 2: SDT, 3: user name</param>
        /// <param name="privilege">1: full quyền, tạo mới/ cập nhật sản phẩm, nhà phát hành</param>
        /// <returns>A result state.</returns>
        public MySqlResultState AddNewAdministrator(string userName, int userNameType, string passWord, int privilege)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            byte[] salt = Common.CreateSalt();
            byte[] hash = Common.GenerateSaltedHash(passWord, salt);
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
                MyMySql.AddOutParameters(cmd.Parameters);

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
                string err = ex.ToString();
                MyLogger.GetInstance().Warn(err);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = err;
            }

            return result;
        }

        /// <summary>
        /// Cập nhật thời gian khi logout tài khoản
        /// </summary>
        /// <param name="administratorCookieIdentify"></param>
        /// <returns></returns>
        public MySqlResultState AdministratorLogout(string administratorCookieIdentify)
        {
            MySqlParameter[] paras = new MySqlParameter[3];

            paras[0] = new MySqlParameter("@inAdministratorCookieIdentify", administratorCookieIdentify);

            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCookie_Administrator_Logout", paras);
            return result;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="email">Email address as user name.</param>
        /// <param name="password">Password.</param>
        /// <returns>A result state.</returns>
        public MySqlResultState LoginAdministrator(string userName, string password)
        {
            return Login(userName, password, "st_tbAdministrator_Get_Salt_Hash");
        }

        /// <summary>
        /// Lấy administrator
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Administrator GetAdministratorFromUserName(string userName)
        {
            Administrator administrator = new Administrator();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
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
                        administrator.id = MyMySql.GetInt32(rdr,"Id");
                        administrator.email = MyMySql.GetString(rdr, "Email");
                        administrator.sdt = MyMySql.GetString(rdr, "SDT");
                        administrator.userName = MyMySql.GetString(rdr, "SDT");
                        administrator.privilege = MyMySql.GetInt32(rdr, "Privilege");
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return administrator;
        }

        /// <summary>
        /// Thêm vào bảng tbCookie_Administrator khi đăng nhập tài khoản quản trị
        /// </summary>
        /// <param name="userCookieIdentify"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public MySqlResultState AddNewCookieAdministrator(string administratorCookieIdentify, int administratorId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inAdministratorCookieIdentify", administratorCookieIdentify);
            paras[1] = new MySqlParameter("@inAdministratorId", administratorId);

            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCookie_Administrator_Login", paras);
            return result;
        }

        public MySqlResultState ChangePasswordAdministrator(int id, string oldPassWord,
            string newPassWord, string renewPassWord)
        {
            return ChangePassword(id, oldPassWord, newPassWord, renewPassWord,
                "st_tbAdministrator_Get_Salt_Hash_From_Id",
                "st_tbAdministrator_ChangePassword");
        }
    }
}