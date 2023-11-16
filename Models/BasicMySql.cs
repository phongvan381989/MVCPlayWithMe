using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class BasicMySql
    {
        public string errMessage;

        public MySqlResultState Login(string userName, string password, string storePro)
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
                if (!Common.ByteArrayCompare(Common.GenerateSaltedHash(password, salt), hash))
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

    }
}