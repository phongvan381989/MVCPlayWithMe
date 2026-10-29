using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Repository cho tbecommerceorder_return_shop
    /// </summary>
    public static class ECommerceOrderReturnShopMySql
    {
        /// <summary>
        /// Insert đơn hàng cần theo dõi hoàn trả
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(string code, int ecommerce)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"INSERT INTO tbecommerceorder_return_shop (Code, ECommmerce, IsReturn)
                                   VALUES (@code, @ecommerce, 0)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@ecommerce", ecommerce);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = "Thêm đơn hoàn trả thành công";
                        }
                        else
                        {
                            result.State = EMySqlResultState.EXCEPTION;
                            result.Message = "Không thể thêm đơn hoàn trả";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.InsertAsync error: {ex.Message}");
                Common.SetResultException(ex, result);
            }

            return result;
        }

        /// <summary>
        /// Lấy tất cả đơn hoàn trả
        /// </summary>
        public static async Task<List<ECommerceOrderReturnShop>> GetAllAsync()
        {
            List<ECommerceOrderReturnShop> list = new List<ECommerceOrderReturnShop>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "SELECT * FROM tbecommerceorder_return_shop ORDER BY Id DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(new ECommerceOrderReturnShop
                                {
                                    Id = rdr.GetInt32("Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    IsReturn = MyMySql.GetInt32(rdr, "IsReturn")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.GetAllAsync error: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Lấy đơn hoàn trả theo Code và ECommerce
        /// </summary>
        public static async Task<ECommerceOrderReturnShop> GetByCodeAndECommerceAsync(string code, int ecommerce)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"SELECT * FROM tbecommerceorder_return_shop
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
                                return new ECommerceOrderReturnShop
                                {
                                    Id = rdr.GetInt32("Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    IsReturn = MyMySql.GetInt32(rdr, "IsReturn")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.GetByCodeAndECommerceAsync error: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Lấy danh sách đơn hoàn trả theo trạng thái IsReturn
        /// </summary>
        public static async Task<List<ECommerceOrderReturnShop>> GetByIsReturnAsync(int isReturn)
        {
            List<ECommerceOrderReturnShop> list = new List<ECommerceOrderReturnShop>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"SELECT * FROM tbecommerceorder_return_shop
                                   WHERE IsReturn = @isReturn
                                   ORDER BY Id DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@isReturn", isReturn);

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(new ECommerceOrderReturnShop
                                {
                                    Id = rdr.GetInt32("Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    IsReturn = MyMySql.GetInt32(rdr, "IsReturn")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.GetByIsReturnAsync error: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Cập nhật trạng thái IsReturn theo Code và ECommerce
        /// </summary>
        public static async Task<MySqlResultState> UpdateIsReturnAsync(string code,
            int ecommerce,
            int isReturn) // 0: chưa hoàn hàng về shop, 1: đã hoàn về shop, 2: thất lạc
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE tbecommerceorder_return_shop
                                   SET IsReturn = @isReturn
                                   WHERE Code = @code AND ECommmerce = @ecommerce";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@ecommerce", ecommerce);
                        cmd.Parameters.AddWithValue("@isReturn", isReturn);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = "Cập nhật trạng thái hoàn trả thành công";
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
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.UpdateIsReturnAsync error: {ex.Message}");
                Common.SetResultException(ex, result);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật trạng thái IsReturn theo Id
        /// </summary>
        public static async Task<MySqlResultState> UpdateIsReturnByIdAsync(int id, int isReturn)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE tbecommerceorder_return_shop
                                   SET IsReturn = @isReturn
                                   WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@isReturn", isReturn);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = "Cập nhật trạng thái hoàn trả thành công";
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
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.UpdateIsReturnByIdAsync error: {ex.Message}");
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

                    string query = @"SELECT COUNT(*) FROM tbecommerceorder_return_shop
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
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.ExistsAsync error: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Insert đơn hoàn trả nếu Status = 0 (PACKED) trong tbecommerceorder nhưng đơn bị hủy
        /// Gọi stored procedure st_InsertOrderReturnShop_IfPacked
        /// </summary>
        public static async Task<MySqlResultState> InsertIfPackedAsync(string code,
            int ecommerce,
            MySqlConnection conn
            )
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbEcommerceorder_Return_Shop_IfPacked", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // IN parameters
                    cmd.Parameters.AddWithValue("@inCode", code);
                    cmd.Parameters.AddWithValue("@inECommmerce", ecommerce);

                    // Execute
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"ECommerceOrderReturnShopMySql.InsertIfReturnedAsync error: {ex.Message}");
                Common.SetResultException(ex, result);
            }

            return result;
        }
    }
}
