using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models
{
    public class AdministratorMySql
    {
        //// Giữ sync vì dùng bởi BasicController.AuthentAdministrator()
        //public Administrator GetAdministratorFromCookie(string userCookieIdentify)
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    int id = -1;
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbCookie_Administrator_Get_From_CookieIdentify", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inAdministratorCookieIdentify", userCookieIdentify);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        int idIndex = rdr.GetOrdinal("AdministratorId");
        //        while (rdr.Read())
        //        {
        //            id = rdr.GetInt32(idIndex);
        //        }

        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        id = -1;
        //    }

        //    conn.Close();
        //    if (id == -1)
        //        return null;

        //    Administrator administrator = new Administrator();
        //    administrator.id = id;
        //    return administrator;
        //}

        //// Giữ sync vì dùng bởi BasicController.AuthentAdministrator() overload có connection
        //public Administrator GetAdministratorFromCookieConnectOut(string userCookieIdentify,
        //    MySqlConnection conn)
        //{
        //    int id = -1;
        //    try
        //    {
        //        MySqlCommand cmd = new MySqlCommand("st_tbCookie_Administrator_Get_From_CookieIdentify", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inAdministratorCookieIdentify", userCookieIdentify);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        int idIndex = rdr.GetOrdinal("AdministratorId");
        //        while (rdr.Read())
        //        {
        //            id = rdr.GetInt32(idIndex);
        //        }

        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        id = -1;
        //    }

        //    if (id == -1)
        //    {
        //        return null;
        //    }

        //    Administrator administrator = new Administrator();
        //    administrator.id = id;
        //    return administrator;
        //}

        public async Task<Administrator> GetAdministratorFromCookieConnectOutAsync(string userCookieIdentify, MySqlConnection conn)
        {
            int id = -1;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbCookie_Administrator_Get_From_CookieIdentify", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inAdministratorCookieIdentify", userCookieIdentify);

                using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    int idIndex = rdr.GetOrdinal("AdministratorId");
                    while (await rdr.ReadAsync())
                    {
                        id = rdr.GetInt32(idIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                id = -1;
            }
            if (id == -1) return null;
            Administrator administrator = new Administrator();
            administrator.id = id;
            return administrator;
        }

        public async Task<Administrator> GetAdministratorFromCookieAsync(string userCookieIdentify)
        {
            int id = -1;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCookie_Administrator_Get_From_CookieIdentify", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inAdministratorCookieIdentify", userCookieIdentify);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = rdr.GetOrdinal("AdministratorId");
                        while (await rdr.ReadAsync())
                        {
                            id = rdr.GetInt32(idIndex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    id = -1;
                }
            }
            if (id == -1) return null;
            Administrator administrator = new Administrator();
            administrator.id = id;
            return administrator;
        }

        public async Task<Administrator> GetAdministratorFromUserNameAsync(string userName)
        {
            Administrator administrator = new Administrator();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbAdministrator_Get_Admin_From_UserName", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inUserName", userName);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = rdr.GetOrdinal("Id");
                        int emailIndex = rdr.GetOrdinal("Email");
                        int sdtIndex = rdr.GetOrdinal("SDT");
                        int privilegeIndex = rdr.GetOrdinal("Privilege");

                        while (await rdr.ReadAsync())
                        {
                            administrator.id = rdr.GetInt32(idIndex);
                            administrator.email = rdr.IsDBNull(emailIndex) ? string.Empty : rdr.GetString(emailIndex);
                            administrator.sdt = rdr.IsDBNull(sdtIndex) ? string.Empty : rdr.GetString(sdtIndex);
                            administrator.userName = rdr.IsDBNull(sdtIndex) ? string.Empty : rdr.GetString(sdtIndex);
                            administrator.privilege = rdr.IsDBNull(privilegeIndex) ? -1 : rdr.GetInt32(privilegeIndex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return administrator;
        }

        public async Task<MySqlResultState> AddNewAdministratorAsync(string userName, int userNameType, string passWord, int privilege)
        {
            byte[] salt = Common.CreateSalt();
            byte[] hash = Common.GenerateSaltedHash(passWord, salt);
            MySqlResultState result = new MySqlResultState();
            result.Message = "Thêm mới thành công.";
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbAdministrator_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@inEmail", userNameType == 1 ? userName : "");

                    MySqlParameter paSalt = new MySqlParameter();
                    paSalt.ParameterName = @"inSalt";
                    paSalt.Size = Common.SHA256Size;
                    paSalt.MySqlDbType = MySqlDbType.Binary;
                    paSalt.Value = salt;
                    cmd.Parameters.Add(paSalt);

                    MySqlParameter paHash = new MySqlParameter();
                    paHash.ParameterName = @"inHash";
                    paHash.Size = Common.SHA256Size;
                    paHash.MySqlDbType = MySqlDbType.Binary;
                    paHash.Value = hash;
                    cmd.Parameters.Add(paHash);

                    cmd.Parameters.AddWithValue("@inSDT", userNameType == 2 ? userName : "");
                    cmd.Parameters.AddWithValue("@inUserName", userNameType == 3 ? userName : "");
                    cmd.Parameters.AddWithValue("@inPrivilege", privilege);

                    MyMySql.AddOutParameters(cmd.Parameters);

                    int lengthPara = cmd.Parameters.Count;
                    await cmd.ExecuteNonQueryAsync();
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
            }
            return result;
        }

        public async Task<MySqlResultState> AdministratorLogoutAsync(string administratorCookieIdentify)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inAdministratorCookieIdentify", administratorCookieIdentify);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCookie_Administrator_Logout", paras);
        }

        public async Task<MySqlResultState> LoginAdministratorAsync(string userName, string password)
        {
            return await MyMySql.LoginAsync(userName, password, "st_tbAdministrator_Get_Salt_Hash");
        }

        public async Task<MySqlResultState> AddNewCookieAdministratorAsync(string administratorCookieIdentify, int administratorId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inAdministratorCookieIdentify", administratorCookieIdentify);
            paras[1] = new MySqlParameter("@inAdministratorId", administratorId);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCookie_Administrator_Login", paras);
        }

        public async Task<MySqlResultState> ChangePasswordAdministratorAsync(int id, string oldPassWord,
            string newPassWord, string renewPassWord)
        {
            return await MyMySql.ChangePasswordAsync(id, oldPassWord, newPassWord, renewPassWord,
                "st_tbAdministrator_Get_Salt_Hash_From_Id",
                "st_tbAdministrator_ChangePassword");
        }
    }
}
