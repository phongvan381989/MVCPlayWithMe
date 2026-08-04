using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.BankAccount
{
    public class BankAccountMySql
    {
        #region Private Helper Methods

        /// <summary>
        /// Map dữ liệu từ MySqlDataReader sang BankAccount object
        /// </summary>
        private static BankAccount MapFromReader(MySqlDataReader rdr)
        {
            return new BankAccount
            {
                Id = MyMySql.GetInt32(rdr, "Id"),
                BankName = MyMySql.GetString(rdr, "BankName"),
                BankCode = MyMySql.GetString(rdr, "BankCode"),
                AccountNumber = MyMySql.GetString(rdr, "AccountNumber"),
                AccountHolder = MyMySql.GetString(rdr, "AccountHolder"),
                Branch = MyMySql.GetString(rdr, "Branch"),
                QRCodeTemplate = MyMySql.GetString(rdr, "QRCodeTemplate"),
                IsActive = MyMySql.GetInt32(rdr, "IsActive"),
                Note = MyMySql.GetString(rdr, "Note"),
                CreatedAt = MyMySql.GetDateTime(rdr, "CreatedAt"),
                UpdatedAt = MyMySql.GetDateTime(rdr, "UpdatedAt")
            };
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Lấy tài khoản ngân hàng đang active để hiển thị cho khách chuyển khoản
        /// </summary>
        public static async Task<BankAccount> GetActiveBankAccountAsync()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM tb_bank_accounts WHERE IsActive = 1 LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            return MapFromReader(rdr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetActiveBankAccountAsync error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Lấy tài khoản ngân hàng theo ID
        /// </summary>
        public static async Task<BankAccount> GetByIdAsync(int id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM tb_bank_accounts WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                return MapFromReader(rdr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetByIdAsync error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Lấy tất cả tài khoản ngân hàng
        /// </summary>
        public static async Task<List<BankAccount>> GetAllAsync()
        {
            List<BankAccount> list = new List<BankAccount>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM tb_bank_accounts ORDER BY IsActive DESC, Id DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            list.Add(MapFromReader(rdr));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetAllAsync error: {ex.Message}");
            }
            return list;
        }

        #endregion

        #region Insert/Update/Delete

        /// <summary>
        /// Thêm mới tài khoản ngân hàng
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(BankAccount bankAccount)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"
                        INSERT INTO tb_bank_accounts
                        (BankName, BankCode, AccountNumber, AccountHolder, Branch, QRCodeTemplate, IsActive, Note)
                        VALUES
                        (@bankName, @bankCode, @accountNumber, @accountHolder, @branch, @qrCodeTemplate, @isActive, @note)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@bankName", MySqlDbType.VarChar).Value = bankAccount.BankName;
                        cmd.Parameters.Add("@bankCode", MySqlDbType.VarChar).Value = bankAccount.BankCode;
                        cmd.Parameters.Add("@accountNumber", MySqlDbType.VarChar).Value = bankAccount.AccountNumber;
                        cmd.Parameters.Add("@accountHolder", MySqlDbType.VarChar).Value = bankAccount.AccountHolder;
                        cmd.Parameters.Add("@branch", MySqlDbType.VarChar).Value = (object)bankAccount.Branch ?? DBNull.Value;
                        cmd.Parameters.Add("@qrCodeTemplate", MySqlDbType.Text).Value = (object)bankAccount.QRCodeTemplate ?? DBNull.Value;
                        cmd.Parameters.Add("@isActive", MySqlDbType.Int32).Value = bankAccount.IsActive;
                        cmd.Parameters.Add("@note", MySqlDbType.Text).Value = (object)bankAccount.Note ?? DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Thêm tài khoản ngân hàng thành công";
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản ngân hàng
        /// </summary>
        public static async Task<MySqlResultState> UpdateAsync(BankAccount bankAccount)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"
                        UPDATE tb_bank_accounts SET
                            BankName = @bankName,
                            BankCode = @bankCode,
                            AccountNumber = @accountNumber,
                            AccountHolder = @accountHolder,
                            Branch = @branch,
                            QRCodeTemplate = @qrCodeTemplate,
                            IsActive = @isActive,
                            Note = @note
                        WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = bankAccount.Id;
                        cmd.Parameters.Add("@bankName", MySqlDbType.VarChar).Value = bankAccount.BankName;
                        cmd.Parameters.Add("@bankCode", MySqlDbType.VarChar).Value = bankAccount.BankCode;
                        cmd.Parameters.Add("@accountNumber", MySqlDbType.VarChar).Value = bankAccount.AccountNumber;
                        cmd.Parameters.Add("@accountHolder", MySqlDbType.VarChar).Value = bankAccount.AccountHolder;
                        cmd.Parameters.Add("@branch", MySqlDbType.VarChar).Value = (object)bankAccount.Branch ?? DBNull.Value;
                        cmd.Parameters.Add("@qrCodeTemplate", MySqlDbType.Text).Value = (object)bankAccount.QRCodeTemplate ?? DBNull.Value;
                        cmd.Parameters.Add("@isActive", MySqlDbType.Int32).Value = bankAccount.IsActive;
                        cmd.Parameters.Add("@note", MySqlDbType.Text).Value = (object)bankAccount.Note ?? DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Cập nhật tài khoản ngân hàng thành công";
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        /// <summary>
        /// Bật/tắt tài khoản ngân hàng
        /// </summary>
        public static async Task<MySqlResultState> UpdateIsActiveAsync(int id, int isActive)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tb_bank_accounts SET IsActive = @isActive WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                        cmd.Parameters.Add("@isActive", MySqlDbType.Int32).Value = isActive;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = isActive == 1 ? "Đã bật tài khoản" : "Đã tắt tài khoản";
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        /// <summary>
        /// Xóa tài khoản ngân hàng
        /// </summary>
        public static async Task<MySqlResultState> DeleteAsync(int id)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM tb_bank_accounts WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Xóa tài khoản ngân hàng thành công";
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        #endregion
    }
}
