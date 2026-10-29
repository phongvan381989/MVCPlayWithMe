using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Repository cho tbecommerceorder_check_invoice
    /// </summary>
    public static class ECommerceOrderCheckInvoiceMySql
    {
        /// <summary>
        /// Insert đơn hàng cần kiểm tra hóa đơn
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(string code, int ecommerce)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"INSERT INTO tbecommerceorder_check_invoice (Code, ECommmerce, IsDone)
                                   VALUES (@code, @ecommerce, 0)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@ecommerce", ecommerce);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = "Thêm đơn kiểm tra hóa đơn thành công";
                        }
                        else
                        {
                            result.State = EMySqlResultState.EXCEPTION;
                            result.Message = "Không thể thêm đơn kiểm tra hóa đơn";
                        }
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Duplicate entry
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.InsertAsync duplicate: {ex.Message}");
                result.State = EMySqlResultState.EXIST;
                result.Message = "Đơn hàng đã tồn tại trong danh sách kiểm tra";
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.InsertAsync error: {ex.Message}");
                Common.SetResultException(ex, result);
            }

            return result;
        }

        /// <summary>
        /// Lấy tất cả đơn cần kiểm tra hóa đơn
        /// </summary>
        public static async Task<List<ECommerceOrderCheckInvoice>> GetAllAsync()
        {
            List<ECommerceOrderCheckInvoice> list = new List<ECommerceOrderCheckInvoice>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "SELECT * FROM tbecommerceorder_check_invoice ORDER BY Id DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(new ECommerceOrderCheckInvoice
                                {
                                    Id = rdr.GetInt32("Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    IsDone = MyMySql.GetInt32(rdr, "IsDone"),
                                    UpdateTime = MyMySql.GetDateTime(rdr, "UpdateTime")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.GetAllAsync error: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Lấy đơn kiểm tra hóa đơn theo Code và ECommerce
        /// </summary>
        public static async Task<ECommerceOrderCheckInvoice> GetByCodeAndECommerceAsync(string code, int ecommerce)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"SELECT * FROM tbecommerceorder_check_invoice
                                   WHERE Code = @code AND ECommmerce = @ecommerce
                                   LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@ecommerce", ecommerce);

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                return new ECommerceOrderCheckInvoice
                                {
                                    Id = rdr.GetInt32("Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    IsDone = MyMySql.GetInt32(rdr, "IsDone"),
                                    UpdateTime = MyMySql.GetDateTime(rdr, "UpdateTime")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.GetByCodeAndECommerceAsync error: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Lấy danh sách đơn kiểm tra hóa đơn theo trạng thái IsDone
        /// </summary>
        public static async Task<List<ECommerceOrderCheckInvoice>> GetByIsDoneAsync(int isDone)
        {
            List<ECommerceOrderCheckInvoice> list = new List<ECommerceOrderCheckInvoice>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"SELECT * FROM tbecommerceorder_check_invoice
                                   WHERE IsDone = @isDone
                                   ORDER BY Id DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@isDone", isDone);

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(new ECommerceOrderCheckInvoice
                                {
                                    Id = rdr.GetInt32("Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    IsDone = MyMySql.GetInt32(rdr, "IsDone"),
                                    UpdateTime = MyMySql.GetDateTime(rdr, "UpdateTime")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.GetByIsDoneAsync error: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Cập nhật trạng thái IsDone = 1 theo Code và ECommerce
        /// </summary>
        public static async Task<MySqlResultState> UpdateIsDoneAsync(string code, int ecommerce)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE tbecommerceorder_check_invoice
                                   SET IsDone = 1
                                   WHERE Code = @code AND ECommmerce = @ecommerce";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@ecommerce", ecommerce);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = "Cập nhật trạng thái hoàn thành kiểm tra";
                        }
                        else
                        {
                            result.State = EMySqlResultState.EXCEPTION;
                            result.Message = "Không tìm thấy đơn hàng để cập nhật";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.UpdateIsDoneAsync error: {ex.Message}");
                Common.SetResultException(ex, result);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật trạng thái IsDone = 1 theo Id
        /// </summary>
        public static async Task<MySqlResultState> UpdateIsDoneByIdAsync(int id)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE tbecommerceorder_check_invoice
                                   SET IsDone = 1
                                   WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = "Cập nhật trạng thái hoàn thành kiểm tra";
                        }
                        else
                        {
                            result.State = EMySqlResultState.EXCEPTION;
                            result.Message = "Không tìm thấy đơn hàng để cập nhật";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.UpdateIsDoneByIdAsync error: {ex.Message}");
                Common.SetResultException(ex, result);
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra đơn hàng đã tồn tại chưa
        /// </summary>
        public static async Task<bool> ExistsAsync(string code, int ecommerce)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"SELECT COUNT(*) FROM tbecommerceorder_check_invoice
                                   WHERE Code = @code AND ECommmerce = @ecommerce";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@ecommerce", ecommerce);

                        long count = (long)await cmd.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.ExistsAsync error: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Insert hoặc bỏ qua nếu đã tồn tại (INSERT IGNORE)
        /// Bỏ trả về MySqlResultState cho nhanh
        /// </summary>
        public static async Task InsertOrIgnoreAsync(string code, int ecommerce)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"INSERT IGNORE INTO tbecommerceorder_check_invoice (Code, ECommmerce, IsDone)
                                   VALUES (@code, @ecommerce, 0)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@ecommerce", ecommerce);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderCheckInvoiceMySql.InsertOrIgnoreAsync error: {ex.Message}");
            }
        }
    }
}
