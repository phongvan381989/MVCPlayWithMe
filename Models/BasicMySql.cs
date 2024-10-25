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
                if (!Common.ByteArrayCompare(Common.GenerateSaltedHash(password, salt), hash))
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

        public MySqlResultState ChangePassword(int id, string oldPassWord,
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

                if(result.State == EMySqlResultState.OK)
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
    }
}