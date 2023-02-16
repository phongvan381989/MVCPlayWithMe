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
        private static string connStr;

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

        private static MySqlResultState ExcuteNonQueryStoreProceduce(string stName, MySqlParameter[] paras)
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

        public static List<Publisher> GetListPublisher()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<Publisher> ls = new List<Publisher>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbPublisher_Select_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(new Publisher(rdr.GetInt32("Id"), rdr.GetString("PublisherName"), rdr.GetString("Detail")));
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
            return ls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productName"></param>
        /// <returns> -2 nếu tên sản phẩm không tồn tại; -1 nếu có lỗi </returns>
        public static int GetProductIdFromName(string productName)
        {
            int id = -1;
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_With_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productName", productName);

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            id = rdr.GetInt32("Id");
                            break;
                        }
                    }
                    else
                    {
                        id = -2;
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                id = -1;
            }

            conn.Close();
            return id;
        }

        /// <summary>
        /// Lấy tất cả barcode
        /// barcode phải khác trống
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListBarcode()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Barcode", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(rdr.GetString("Barcode"));
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
            return ls;
        }

        /// <summary>
        /// Lấy tất cả tên sản phẩm
        /// Tên trống bỏ qua
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListProductName()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_ProductName", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(rdr.GetString("ProductName"));
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
            return ls;
        }

        /// <summary>
        /// Lấy tất cả comboname trong db
        /// Comboname phải khác trống
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListComboName()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_ComboName", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(rdr.GetString("ComboName"));
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
            return ls;
        }

        /// <summary>
        /// Lấy tất cả tác giả
        /// Tác giả phải khác trống
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListAuthor()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Author", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(rdr.GetString("Author"));
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
            return ls;
        }

        /// <summary>
        /// Lấy tất cả người dịch
        /// Người dịch phải khác trống
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListTranslator()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Translator", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(rdr.GetString("Translator"));
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
            return ls;
        }

        /// <summary>
        /// Lấy tất cả nhà xuất bản
        /// Nhà xuất bản phải khác trống
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListPublishingCompany()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_PublishingCompany", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(rdr.GetString("PublishingCompany"));
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
            return ls;
        }

        /// <summary>
        /// Lấy tất cả thể loại
        /// Thể loại phải khác trống
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListCategory()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Category", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(rdr.GetString("Category"));
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
            return ls;
        }

        /// <summary>
        /// Từ tên combo lấy được thông tin chung đã có trên db
        /// Thông tin chung là thông tin của sản phẩm đầu tiên thuộc combo
        /// </summary>
        /// <param name="comboName"></param>
        /// <returns></returns>
        public static string GetJsonCommonInfoFromComboName(string comboName)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            StringBuilder sb = new StringBuilder();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_First_Product_From_ComboName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@comboName", comboName);

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    sb.Append("{");
                    while (rdr.Read())
                    {
                        sb.Append("\"bookCoverPrice\"");
                        sb.Append(":");
                        sb.Append(rdr.GetInt32("BookCoverPrice"));
                        sb.Append(",");

                        sb.Append("\"author\"");
                        sb.Append(":");
                        sb.Append("\"");
                        sb.Append(rdr.GetString("Author"));
                        sb.Append("\"");
                        sb.Append(",");

                        sb.Append("\"translator\"");
                        sb.Append(":");
                        sb.Append("\"");
                        sb.Append(rdr.GetString("Translator"));
                        sb.Append("\"");
                        sb.Append(",");

                        sb.Append("\"publisherId\"");
                        sb.Append(":");
                        sb.Append(rdr.GetInt32("PublisherId"));
                        sb.Append(",");

                        sb.Append("\"publishingCompany\"");
                        sb.Append(":");
                        sb.Append("\"");
                        sb.Append(rdr.GetString("PublishingCompany"));
                        sb.Append("\"");
                        sb.Append(",");

                        sb.Append("\"publishingTime\"");
                        sb.Append(":");
                        sb.Append("\"");
                        sb.Append(rdr.GetDateTime("PublishingTime").ToString("yyyy-MM-dd"));
                        sb.Append("\"");
                        sb.Append(",");

                        sb.Append("\"productLong\"");
                        sb.Append(":");
                        sb.Append(rdr.GetInt32("ProductLong"));
                        sb.Append(",");

                        sb.Append("\"productWide\"");
                        sb.Append(":");
                        sb.Append(rdr.GetInt32("ProductWide"));
                        sb.Append(",");

                        sb.Append("\"productHigh\"");
                        sb.Append(":");
                        sb.Append(rdr.GetInt32("ProductHigh"));
                        sb.Append(",");

                        sb.Append("\"productWeight\"");
                        sb.Append(":");
                        sb.Append(rdr.GetInt32("ProductWeight"));
                        sb.Append(",");

                        sb.Append("\"positionInWarehouse\"");
                        sb.Append(":");
                        sb.Append("\"");
                        sb.Append(rdr.GetString("PositionInWarehouse"));
                        sb.Append("\"");
                        sb.Append(",");

                        sb.Append("\"category\"");
                        sb.Append(":");
                        sb.Append("\"");
                        sb.Append(rdr.GetString("Category"));
                        sb.Append("\"");
                    }
                    sb.Append("}");
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
            return sb.ToString();
        }

        /// <summary>
        /// Thêm 2 parameters outResult và outMessage
        /// </summary>
        /// <param name="parameters"></param>
        private static void AddOutParameters(MySqlParameterCollection parameters)
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
        private static void AddOutParameters(MySqlParameter[] paras)
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

        public static MySqlResultState AddNewPro(string barcode,
                string productName,
                string comboName,
                int bookCoverPrice,
                string author,
                string translator,
                int publisherId,
                string publishingCompany,
                string publishingTime,
                int productLong,
                int productWide,
                int productHigh,
                int productWeight,
                string positionInWarehouse,
                string detail,
                string category,
                int productIdForUpdate //productIdForUpdate = -1: tạo mới sản phẩm, ngược lại là update sản phẩm
                )
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            // Update không cần check barcode, tên sản phẩm tồn tại chưa
            if (productIdForUpdate == -1)
            {
                paras = new MySqlParameter[3];
                paras[0] = new MySqlParameter("@productName", productName);
                AddOutParameters(paras);

                // Check tên phẩm đã tồn tại?
                result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Count_With_Name", paras);
                if ((int)paras[1].Value != 0)
                {
                    return result;
                }

                // Check barcode đã tồn tại?
                paras[0] = new MySqlParameter("@barcode", barcode);
                result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Count_With_Barcode", paras);
                if ((int)paras[1].Value != 0)
                {
                    return result;
                }
            }

            if (productIdForUpdate == -1) // Tạo mới
            {
                paras = new MySqlParameter[18];
            }
            else // Update
            {
                paras = new MySqlParameter[19];
            }

            paras[0] = new MySqlParameter("@barcode", barcode);
            paras[1] = new MySqlParameter("@productName", productName);
            paras[2] = new MySqlParameter("@comboName", comboName);
            paras[3] = new MySqlParameter("@bookCoverPrice", bookCoverPrice);
            paras[4] = new MySqlParameter("@author", author);
            paras[5] = new MySqlParameter("@translator", translator);
            paras[6] = new MySqlParameter("@publisherId", publisherId);
            paras[7] = new MySqlParameter("@publishingCompany", publishingCompany);
            paras[8] = new MySqlParameter("@publishingTime", publishingTime);
            paras[9] = new MySqlParameter("@productLong", productLong);
            paras[10] = new MySqlParameter("@productWide", productWide);
            paras[11] = new MySqlParameter("@productHigh", productHigh);
            paras[12] = new MySqlParameter("@productWeight", productWeight);
            paras[13] = new MySqlParameter("@positionInWarehouse", positionInWarehouse);
            paras[14] = new MySqlParameter("@detail", detail);
            paras[15] = new MySqlParameter("@category", category);
            if (productIdForUpdate != -1)
                paras[16]  = new MySqlParameter("@id", productIdForUpdate);

            AddOutParameters(paras);
            if (productIdForUpdate == -1)
                result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Insert", paras);
            else
                result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update", paras);

            return result;
        }

        public static MySqlResultState UpdateCommonInfoWithComboNae(
                        string comboName,
                        int bookCoverPrice,
                        string author,
                        string translator,
                        int publisherId,
                        string publishingCompany,
                        string publishingTime,
                        int productLong,
                        int productWide,
                        int productHigh,
                        int productWeight,
                        string positionInWarehouse,
                        string category)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            // Check combo name phải tồn tại
            paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inComboName", comboName);
            AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Count_With_ComboName", paras);
            if ((int)paras[1].Value == 0)
            {
                return result;
            }

            paras = new MySqlParameter[15];
            paras[0] = new MySqlParameter("@comboName", comboName);
            paras[1] = new MySqlParameter("@bookCoverPrice", bookCoverPrice);
            paras[2] = new MySqlParameter("@author", author);
            paras[3] = new MySqlParameter("@translator", translator);
            paras[4] = new MySqlParameter("@publisherId", publisherId);
            paras[5] = new MySqlParameter("@publishingCompany", publishingCompany);
            paras[6] = new MySqlParameter("@publishingTime", publishingTime);
            paras[7] = new MySqlParameter("@productLong", productLong);
            paras[8] = new MySqlParameter("@productWide", productWide);
            paras[9] = new MySqlParameter("@productHigh", productHigh);
            paras[10] = new MySqlParameter("@productWeight", productWeight);
            paras[11] = new MySqlParameter("@positionInWarehouse", positionInWarehouse);
            paras[12] = new MySqlParameter("@category", category);
            AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Common_Infor_ComboName", paras);
            return result;
        }

        public static MySqlResultState AddNewPublisher(string publisherName, string detail)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            // Check publisherName exist
            paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@publisherName", publisherName);
            AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Count_With_Name", paras);
            if ((int)paras[1].Value != 0)
            {
                return result;
            }

            paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@publisherName", publisherName);
            paras[1] = new MySqlParameter("@detail", detail);
            AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Insert", paras);

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

        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product
        /// </summary>
        /// <returns></returns>
        private static Product ConvertFromDataMySql(MySqlDataReader rdr)
        {
            if (rdr == null || !rdr.HasRows)
                return null;

            Product product = new Product();
            while (rdr.Read())
            {
                product.id = rdr.GetInt32("Id");
                product.barcode = rdr.GetString("Barcode");
                product.productName = rdr.GetString("ProductName");
                product.comboName = rdr.GetString("ComboName");
                product.bookCoverPrice = rdr.GetInt32("BookCoverPrice");
                product.author = rdr.GetString("Author");
                product.translator = rdr.GetString("Translator");
                product.publisherId = rdr.GetInt32("PublisherId");
                product.publishingCompany = rdr.GetString("PublishingCompany");
                product.publishingTime = rdr.GetDateTime("PublishingTime");
                product.publishingTimeyyyyMMdd = product.publishingTime.ToString("yyyy-MM-dd");
                product.productLong = rdr.GetInt32("ProductLong");
                product.productWide = rdr.GetInt32("ProductWide");
                product.productHigh = rdr.GetInt32("ProductHigh");
                product.productWeight = rdr.GetInt32("ProductWeight");
                product.positionInWarehouse = rdr.GetString("PositionInWarehouse");
                product.detail = rdr.GetString("Detail");
                product.category = rdr.GetString("Category");
                break;
            }
            return product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns>Có thể trả về null</returns>
        public static Product GetProductFromBarcode(string barcode)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            StringBuilder sb = new StringBuilder();
            Product product = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Barcode", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inBarcode", barcode);

                MySqlDataReader rdr = cmd.ExecuteReader();
                product = ConvertFromDataMySql(rdr);
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                product = null;
            }

            conn.Close();
            return product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns>Có thể trả về null</returns>
        public static Product GetProductFromProductName(string productName)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            StringBuilder sb = new StringBuilder();
            Product product = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inProductName", productName);

                MySqlDataReader rdr = cmd.ExecuteReader();
                product = ConvertFromDataMySql(rdr);
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                product = null;
            }

            conn.Close();
            return product;
        }

        public static MySqlResultState UpdateProductBarcode(int id, string newBarcode)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", newBarcode);
            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Barcode", paras);
            return result;
        }

        public static MySqlResultState AddMoreProductBarcode(int id, string newBarcode)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", newBarcode);
            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Barcode_Add_More", paras);
            return result;
        }

        public static MySqlResultState UpdateProductName(int id, string newProductName)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inProductName", newProductName);
            AddOutParameters(paras);

            MySqlResultState result = ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Name", paras);
            return result;
        }
    }
}