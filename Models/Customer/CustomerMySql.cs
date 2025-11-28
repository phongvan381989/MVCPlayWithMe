using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Customer
{
    /// <summary>
    /// Khách vãng lai không lưu userCookieIdentify vào db
    /// </summary>
    public class CustomerMySql
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

            Customer customer = new Customer();
            customer.id = (int)paras[1].Value;
            return customer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">SDT</param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public MySqlResultState AddNewCustomer(string userName, string passWord)
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
                cmd.Parameters.AddWithValue("@inEmail", "");

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

                cmd.Parameters.AddWithValue("@inSDT", "");

                cmd.Parameters.AddWithValue("@inUserName", userName);

                cmd.Parameters.AddWithValue("@inFullName", "");

                cmd.Parameters.AddWithValue("@inBirthday", "2020-01-01"); // Mặc định

                cmd.Parameters.AddWithValue("@inSex", 4); // Mặc định

                MyMySql.AddOutParameters(cmd.Parameters);

                int lengthPara = cmd.Parameters.Count;
                cmd.ExecuteNonQuery();
                if ((EMySqlResultState)cmd.Parameters[lengthPara - 2].Value != EMySqlResultState.OK)
                {
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.Message = (string)cmd.Parameters[lengthPara - 1].Value;
                }
                //}
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();
            return result;
        }

        /// <summary>
        /// Customer logout tài khoản ở tất cả các thiết bị đã đăng nhập
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
            return MyMySql.Login(userName, password, "st_tbCustomer_Get_Salt_Hash");
        }

        private void GetCustomerFromDataReader(MySqlDataReader rdr, Customer customer)
        {
            while (rdr.Read())
            {
                if (customer.id == -1)
                {
                    customer.id = MyMySql.GetInt32(rdr, "Id");
                    customer.email = MyMySql.GetString(rdr, "Email");
                    customer.sdt = MyMySql.GetString(rdr, "SDT");
                    customer.userName = MyMySql.GetString(rdr, "UserName");
                    customer.fullName = MyMySql.GetString(rdr, "FullName");
                    customer.birthday = MyMySql.GetDateTime(rdr, "Birthday");
                    customer.sex = MyMySql.GetInt32(rdr, "Sex");
                }
                // Thêm address
                if (!Convert.IsDBNull(rdr["AddressId"]))
                {
                    Address add = new Address();
                    add.id = MyMySql.GetInt32(rdr, "AddressId");
                    add.name = MyMySql.GetString(rdr, "Name");
                    add.phone = MyMySql.GetString(rdr, "Phone");
                    add.province = MyMySql.GetString(rdr, "Province");
                    add.district = MyMySql.GetString(rdr, "District");
                    add.subdistrict = MyMySql.GetString(rdr, "SubDistrict");
                    add.detail = MyMySql.GetString(rdr, "Detail");
                    add.defaultAdd = MyMySql.GetInt32(rdr, "DefaultAdd");
                    customer.lsAddress.Add(add);
                }
            }
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
                GetCustomerFromDataReader(rdr, customer);
                rdr.Close();
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
                customer = null;
            }

            conn.Close();
            return customer;
        }

        public Customer GetCustomer(int id)
        {
            Customer customer = new Customer();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCustomer_Get_Customer", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                GetCustomerFromDataReader(rdr, customer);
                rdr.Close();
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
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

                MySqlCommand cmd = new MySqlCommand("st_tbCart_Insert_And_Update_Login", conn);
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
                
                MyLogger.GetInstance().Warn(ex.ToString());
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

                MySqlCommand cmd = new MySqlCommand("st_tbCart_Insert_And_Update", conn);
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
                int lengthPara = cmd.Parameters.Count;
                //if ((EMySqlResultState)cmd.Parameters[lengthPara - 2].Value != EMySqlResultState.OK)
                {
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.Message = (string)cmd.Parameters[lengthPara - 1].Value;
                }

            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            return result;
        }

        //public void AddCustomerInforAddress(int customerId, List<Address> ls)
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbAddress_Insert_And_Update", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inCustomerId", customerId);
        //        cmd.Parameters.AddWithValue("@inName", "");
        //        cmd.Parameters.AddWithValue("@inPhone", "");
        //        cmd.Parameters.AddWithValue("@inProvince", "");
        //        cmd.Parameters.AddWithValue("@inDistrict", "");
        //        cmd.Parameters.AddWithValue("@inSubDistrict", "");
        //        cmd.Parameters.AddWithValue("@inDetail", "");
        //        cmd.Parameters.AddWithValue("@inDefaultAdd", 0);

        //        foreach (var cus in ls)
        //        {
        //            cmd.Parameters[1].Value = cus.name;
        //            cmd.Parameters[2].Value = cus.phone;
        //            cmd.Parameters[3].Value = cus.province;
        //            cmd.Parameters[4].Value = cus.district;
        //            cmd.Parameters[5].Value = cus.subdistrict;
        //            cmd.Parameters[6].Value = cus.detail;
        //            cmd.Parameters[7].Value = cus.defaultAdd;
        //            cmd.ExecuteNonQuery();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //    }
        //    conn.Close();
        //}

        public MySqlResultState UpdateAddress(Address add)
        {
            MySqlParameter[] paras = new MySqlParameter[8];

            paras[0] = new MySqlParameter("@inId", add.id);
            paras[1] = new MySqlParameter("@inName", add.name);
            paras[2] = new MySqlParameter("@inPhone", add.phone);
            paras[3] = new MySqlParameter("@inProvince", add.province);
            paras[4] = new MySqlParameter("@inDistrict", add.district);
            paras[5] = new MySqlParameter("@inSubDistrict", add.subdistrict);
            paras[6] = new MySqlParameter("@inDetail", add.detail);
            paras[7] = new MySqlParameter("@inDefaultAdd", add.defaultAdd);

            return MyMySql.ExcuteNonQuery("st_tbAddress_Update", paras);
        }

        public MySqlResultState DeleteAddress(Address add)
        {
            MySqlParameter[] paras = new MySqlParameter[1];

            paras[0] = new MySqlParameter("@inId", add.id);

            return MyMySql.ExcuteNonQuery("st_tbAddress_Delete", paras);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="add"></param>
        /// <returns>Trả về id của row vừa insert</returns>
        public MySqlResultState InsertAddress(int customerId, Address add)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = new MySqlParameter[8];

            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inName", add.name);
            paras[2] = new MySqlParameter("@inPhone", add.phone);
            paras[3] = new MySqlParameter("@inProvince", add.province);
            paras[4] = new MySqlParameter("@inDistrict", add.district);
            paras[5] = new MySqlParameter("@inSubDistrict", add.subdistrict);
            paras[6] = new MySqlParameter("@inDetail", add.detail);
            paras[7] = new MySqlParameter("@inDefaultAdd", add.defaultAdd);

            //return MyMySql.ExcuteNonQuery("st_tbAddress_Insert_And_Update", paras);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbAddress_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.myAnything = MyMySql.GetInt32(rdr, "LastId");
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();

            return result;
        }

        public MySqlResultState UpdateInfor(Customer cus)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCustomer_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", cus.id);
                cmd.Parameters.AddWithValue("@inEmail", cus.email);
                cmd.Parameters.AddWithValue("@inSDT", cus.sdt);
                cmd.Parameters.AddWithValue("@inFullName", cus.fullName);
                cmd.Parameters.AddWithValue("@inBirthday", cus.birthday);
                cmd.Parameters.AddWithValue("@inSex", cus.sex);

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
                Common.SetResultException(ex, result);
            }
            conn.Close();

            return result;
        }

        public MySqlResultState ChangePasswordCustomer(int id, string oldPassWord, 
            string newPassWord, string renewPassWord)
        {
            return MyMySql.ChangePassword(id, oldPassWord, newPassWord, renewPassWord,
                "st_tbCustomer_Get_Salt_Hash_From_Id",
                "st_tbCustomer_ChangePassword");
        }

        // Set default = 0
        public MySqlResultState DeleteDefaultAddress(int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[1];

            paras[0] = new MySqlParameter("@inCustomerId", customerId);

            return MyMySql.ExcuteNonQuery("st_tbAddress_Delete_Default", paras);
        }

        /// <summary>
        /// Từ customerId lấy danh sách địa chỉ nhận hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<Address> GetListAddress(int customerId)
        {
            List<Address> lsAddress = new List<Address>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbAddress_Get", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Address add = new Address();
                    add.id = MyMySql.GetInt32(rdr, "Id");
                    add.name = MyMySql.GetString(rdr, "Name");
                    add.phone = MyMySql.GetString(rdr, "Phone");
                    add.province = MyMySql.GetString(rdr, "Province");
                    add.district = MyMySql.GetString(rdr, "District");
                    add.subdistrict = MyMySql.GetString(rdr, "SubDistrict");
                    add.detail = MyMySql.GetString(rdr, "Detail");
                    add.defaultAdd = MyMySql.GetInt32(rdr, "DefaultAdd");
                    lsAddress.Add(add);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                lsAddress.Clear();
            }

            conn.Close();
            return lsAddress;
        }

        ///// <summary>
        ///// Check userName, SDT hoặc email đã tồn tại hay chưa?
        ///// </summary>
        ///// <param name="userName"></param>
        ///// <returns></returns>
        //public MySqlResultState CheckValidUserName(string userName)
        //{
        //    MySqlParameter[] paras = new MySqlParameter[4];

        //    paras[0] = new MySqlParameter("@inId", -1);
        //    paras[1] = new MySqlParameter("@inValue", userName);
        //    MyMySql.AddOutParameters(paras);
        //    MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCustomer_CheckExist", paras);

        //    return result;
        //}
    }
}