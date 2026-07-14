using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.SanPhamModel;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.Customer
{
    /// <summary>
    /// Khách vãng lai không lưu userCookieIdentify vào db
    /// </summary>
    public class CustomerMySql
    {
        // Giữ sync vì dùng bởi BasicController.AuthentCustomer()
        public async Task<Customer> GetCustomerFromCookie(string userCookieIdentify)
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

            MySqlResultState result = await MyMySql.ExcuteNonQueryAsync("st_tbCookie_Get_CustomerId", paras);

            if (result.State != EMySqlResultState.OK)
                return null;

            Customer customer = new Customer();
            customer.id = (int)paras[1].Value;
            return customer;
        }

        private async Task GetCustomerFromDataReaderAsync(MySqlDataReader rdr, Customer customer)
        {
            while (await rdr.ReadAsync())
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

        public async Task<Customer> GetCustomerFromCookieAsync(string userCookieIdentify)
        {
            if (string.IsNullOrEmpty(userCookieIdentify)) return null;

            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);

            MySqlParameter paraoutId = new MySqlParameter();
            paraoutId.ParameterName = @"outId";
            paraoutId.Value = -1;
            paraoutId.Direction = ParameterDirection.Output;
            paras[1] = paraoutId;

            MyMySql.AddOutParameters(paras);

            MySqlResultState result = await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCookie_Get_CustomerId", paras);
            if (result.State != EMySqlResultState.OK) return null;

            Customer customer = new Customer();
            customer.id = (int)paras[1].Value;
            return customer;
        }

        public async Task<MySqlResultState> AddNewCustomerAsync(string userName, string passWord)
        {
            byte[] salt = Common.CreateSalt();
            byte[] hash = Common.GenerateSaltedHash(passWord, salt);
            MySqlResultState result = new MySqlResultState();
            result.Message = "Tạo tài khoản thành công.";
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
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
                    cmd.Parameters.AddWithValue("@inBirthday", "2020-01-01");
                    cmd.Parameters.AddWithValue("@inSex", 4);
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
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public async Task<MySqlResultState> CustomerLogoutAsync(string userCookieIdentify)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCookie_Logout", paras);
        }

        public async Task<MySqlResultState> LoginCustomerAsync(string userName, string password)
        {
            return await MyMySql.LoginAsync(userName, password, "st_tbCustomer_Get_Salt_Hash");
        }

        public async Task<Customer> GetCustomerFromUserNameAsync(string userName)
        {
            Customer customer = new Customer();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCustomer_Get_Customer_From_UserName", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inUserName", userName);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        await GetCustomerFromDataReaderAsync(rdr, customer);
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    customer = null;
                }
            }
            return customer;
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            Customer customer = new Customer();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCustomer_Get_Customer", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inId", id);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        await GetCustomerFromDataReaderAsync(rdr, customer);
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    customer = null;
                }
            }
            return customer;
        }

        public async Task<MySqlResultState> AddNewCookieAsync(string userCookieIdentify, int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inUserCookieIdentify", userCookieIdentify);
            paras[1] = new MySqlParameter("@inCustomerId", customerId);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCookie_Insert", paras);
        }

        public async Task<MySqlResultState> CookieCustomerLoginAsync(string userCookieIdentify, int customerId)
        {
            return await AddNewCookieAsync(userCookieIdentify, customerId);
        }

        public async Task<MySqlResultState> AddCartLoginAsync(int customerId, List<Cart> ls)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCart_Insert_And_Update", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                    cmd.Parameters.AddWithValue("@inSanPhamId", 0);
                    cmd.Parameters.AddWithValue("@inQuantity", 0);
                    cmd.Parameters.AddWithValue("@inReal", 0);
                    cmd.Parameters.AddWithValue("@inTime", DateTime.Now);
                    foreach (var cart in ls)
                    {
                        cmd.Parameters[1].Value = cart.sanPhamId;
                        cmd.Parameters[2].Value = cart.quantity;
                        cmd.Parameters[3].Value = cart.real;
                        if (cart.time.HasValue)
                        {
                            cmd.Parameters[4].Value = cart.time;
                        }
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }

            return result;
        }

        public async Task<MySqlResultState> AddCartAsync(int customerId, Cart cart)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCart_Insert_And_Update", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                    cmd.Parameters.AddWithValue("@inSanPhamId", cart.sanPhamId);
                    cmd.Parameters.AddWithValue("@inQuantity", cart.quantity);
                    cmd.Parameters.AddWithValue("@inReal", cart.real);
                    cmd.Parameters.AddWithValue("@inTime", DateTime.Now);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                    }
                    else
                    {
                        result.State = EMySqlResultState.EXCEPTION;
                        result.Message = $"Không thêm sản phẩm vào giỏ hàng thành công";
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public async Task<MySqlResultState> UpdateAddressAsync(Address add)
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
            return await MyMySql.ExcuteNonQueryAsync("st_tbAddress_Update", paras);
        }

        public async Task<MySqlResultState> DeleteAddressAsync(Address add)
        {
            MySqlParameter[] paras = new MySqlParameter[1];
            paras[0] = new MySqlParameter("@inId", add.id);
            return await MyMySql.ExcuteNonQueryAsync("st_tbAddress_Delete", paras);
        }

        public async Task<MySqlResultState> InsertAddressAsync(int customerId, Address add)
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
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbAddress_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(paras);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            result.myAnything = MyMySql.GetInt32(rdr, "LastId");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public async Task<MySqlResultState> UpdateInforAsync(Customer cus)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
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
                    await cmd.ExecuteNonQueryAsync();
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
            }
            return result;
        }

        public async Task<MySqlResultState> ChangePasswordCustomerAsync(int id, string oldPassWord,
            string newPassWord, string renewPassWord)
        {
            return await MyMySql.ChangePasswordAsync(id, oldPassWord, newPassWord, renewPassWord,
                "st_tbCustomer_Get_Salt_Hash_From_Id",
                "st_tbCustomer_ChangePassword");
        }

        public async Task<MySqlResultState> DeleteDefaultAddressAsync(int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[1];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            return await MyMySql.ExcuteNonQueryAsync("st_tbAddress_Delete_Default", paras);
        }

        public async Task<List<Address>> GetListAddressAsync(int customerId)
        {
            List<Address> lsAddress = new List<Address>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbAddress_Get", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
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
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    lsAddress.Clear();
                }
            }
            return lsAddress;
        }
    }
}
