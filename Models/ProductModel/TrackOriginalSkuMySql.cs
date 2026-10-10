using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Models.ProductModel
{
    public class TrackOriginalSkuMySql
    {
        /// <summary>
        /// Đọc tất cả TrackOriginalSku objects từ MySqlDataReader
        /// Tối ưu: lấy ordinal 1 lần, dùng cho tất cả các row
        /// </summary>
        private static async Task<List<TrackOriginalSku>> ReadAllTrackOriginalSkuFromReaderAsync(MySqlDataReader rdr)
        {
            List<TrackOriginalSku> list = new List<TrackOriginalSku>();

            // Lấy index của các cột 1 lần duy nhất
            int idOrdinal = rdr.GetOrdinal("Id");
            int eCommerceOrdinal = rdr.GetOrdinal("ECommmerce");
            int originalSkuOrdinal = rdr.GetOrdinal("OriginalSku");
            int productIdOrdinal = rdr.GetOrdinal("ProductId");
            int quantityOrdinal = rdr.GetOrdinal("Quantity");
            int createdDateOrdinal = rdr.GetOrdinal("CreatedDate");
            int isVariantOrdinal = rdr.GetOrdinal("IsVariant");
            int trackIdOrdinal = rdr.GetOrdinal("TrackId");
            int proOnEcoIdOrdinal = rdr.GetOrdinal("ProOnEcoId");
            int isSaveProOnEcoToDbOrdinal = rdr.GetOrdinal("IsSaveProOnEcoToDb");

            // Đọc từng row và tạo object
            while (await rdr.ReadAsync())
            {
                list.Add(new TrackOriginalSku
                {
                    Id = rdr.GetInt32(idOrdinal),
                    ECommmerce = (EECommerceType)rdr.GetInt32(eCommerceOrdinal),
                    OriginalSku = rdr.IsDBNull(originalSkuOrdinal) ? string.Empty : rdr.GetString(originalSkuOrdinal),
                    ProductId = rdr.GetInt32(productIdOrdinal),
                    Quantity = rdr.GetInt32(quantityOrdinal),
                    CreatedDate = rdr.GetDateTime(createdDateOrdinal),
                    IsVariant = rdr.GetByte(isVariantOrdinal),
                    TrackId = rdr.IsDBNull(trackIdOrdinal) ? string.Empty : rdr.GetString(trackIdOrdinal),
                    ProOnEcoId = rdr.IsDBNull(proOnEcoIdOrdinal) ? -1 : rdr.GetInt64(proOnEcoIdOrdinal),
                    IsSaveProOnEcoToDb = rdr.IsDBNull(isSaveProOnEcoToDbOrdinal) ? (byte)0 : rdr.GetByte(isSaveProOnEcoToDbOrdinal)
                });
            }

            return list;
        }

        /// <summary>
        /// Insert tracking SKU gốc từ sàn TMĐT
        /// </summary>
        public static async Task<MySqlResultState> InsertTrackOriginalSkuAsync(
            EECommerceType eCommerce,
            string originalSku,
            int productId,
            int quantity,
            byte isVariant,
            MySqlConnection conn,
            string trackId = null,
            long proOnEcoId = -1)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"INSERT INTO tb_track_original_sku
                    (ECommmerce, OriginalSku, ProductId, Quantity, CreatedDate, IsVariant, TrackId, ProOnEcoId)
                    VALUES (@eCommerce, @originalSku, @productId, @quantity, @createdDate, @isVariant, @trackId, @proOnEcoId)",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@eCommerce", (int)eCommerce);
                    cmd.Parameters.AddWithValue("@originalSku", originalSku ?? string.Empty);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@isVariant", isVariant);
                    cmd.Parameters.AddWithValue("@trackId", trackId ?? string.Empty);
                    cmd.Parameters.AddWithValue("@proOnEcoId", proOnEcoId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        result.State = EMySqlResultState.OK;
                        result.Message = "Insert track original SKU thành công";
                    }
                    else
                    {
                        result.State = EMySqlResultState.ERROR;
                        result.Message = "Insert thất bại";
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"InsertTrackOriginalSkuAsync error: {ex.ToString()}");
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Insert nhiều tracking SKU cùng lúc (batch insert)
        /// </summary>
        public static async Task<MySqlResultState> InsertTrackOriginalSkuBatchAsync(
            List<TrackOriginalSku> trackOriginalSkus,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();

            if (trackOriginalSkus == null || trackOriginalSkus.Count == 0)
            {
                result.State = EMySqlResultState.INVALID;
                result.Message = "Danh sách rỗng, không có gì để insert";
                return result;
            }

            MySqlTransaction transaction = null;
            try
            {
                // Bắt đầu transaction
                transaction = await conn.BeginTransactionAsync();

                // Build câu INSERT với multiple VALUES
                // INSERT INTO table (col1, col2, ...) VALUES (val1, val2, ...), (val1, val2, ...), ...
                var values = new List<string>();
                var parameters = new List<MySqlParameter>();

                for (int i = 0; i < trackOriginalSkus.Count; i++)
                {
                    var item = trackOriginalSkus[i];

                    // Tạo placeholders cho row này
                    values.Add($"(@eCommerce{i}, @originalSku{i}, @productId{i}, @quantity{i}, @createdDate{i}, @isVariant{i}, @trackId{i}, @proOnEcoId{i})");

                    // Thêm parameters
                    parameters.Add(new MySqlParameter($"@eCommerce{i}", (int)item.ECommmerce));
                    parameters.Add(new MySqlParameter($"@originalSku{i}", item.OriginalSku ?? string.Empty));
                    parameters.Add(new MySqlParameter($"@productId{i}", item.ProductId));
                    parameters.Add(new MySqlParameter($"@quantity{i}", item.Quantity));
                    parameters.Add(new MySqlParameter($"@createdDate{i}", item.CreatedDate));
                    parameters.Add(new MySqlParameter($"@isVariant{i}", item.IsVariant));
                    parameters.Add(new MySqlParameter($"@trackId{i}", item.TrackId ?? string.Empty));
                    parameters.Add(new MySqlParameter($"@proOnEcoId{i}", item.ProOnEcoId));
                }

                string sql = $@"INSERT INTO tb_track_original_sku
                    (ECommmerce, OriginalSku, ProductId, Quantity, CreatedDate, IsVariant, TrackId, ProOnEcoId)
                    VALUES {string.Join(", ", values)}";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn, transaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(parameters.ToArray());

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    result.State = EMySqlResultState.OK;
                    result.Message = $"Insert thành công {rowsAffected} record(s)";
                }
            }
            catch (Exception ex)
            {
                // Rollback nếu có lỗi
                if (transaction != null)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        MyLogger.GetInstance().Warn($"Rollback error: {rollbackEx.ToString()}");
                    }
                }

                MyLogger.GetInstance().Warn($"InsertTrackOriginalSkuBatchAsync error: {ex.ToString()}");
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }
            finally
            {
                transaction?.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách tracking SKU theo ProductId
        /// </summary>
        public static async Task<List<TrackOriginalSku>> GetTrackOriginalSkuByProductIdAsync(
            int productId,
            MySqlConnection conn)
        {
            List<TrackOriginalSku> list = new List<TrackOriginalSku>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT *
                    FROM tb_track_original_sku
                    WHERE ProductId = @productId
                    ORDER BY CreatedDate DESC",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@productId", productId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        list = await ReadAllTrackOriginalSkuFromReaderAsync(rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetTrackOriginalSkuByProductIdAsync error: {ex.ToString()}");
            }

            return list;
        }

        /// <summary>
        /// Lấy danh sách tracking SKU theo OriginalSku và ECommerce
        /// </summary>
        public static async Task<List<TrackOriginalSku>> GetTrackOriginalSkuBySkuAsync(
            string originalSku,
            EECommerceType eCommerce,
            MySqlConnection conn)
        {
            List<TrackOriginalSku> list = new List<TrackOriginalSku>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT *
                    FROM tb_track_original_sku
                    WHERE OriginalSku = @originalSku AND ECommmerce = @eCommerce
                    ORDER BY CreatedDate DESC",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@originalSku", originalSku);
                    cmd.Parameters.AddWithValue("@eCommerce", (int)eCommerce);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        list = await ReadAllTrackOriginalSkuFromReaderAsync(rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetTrackOriginalSkuBySkuAsync error: {ex.ToString()}");
            }

            return list;
        }

        /// <summary>
        /// Lấy tất cả tracking SKU theo ECommerce
        /// </summary>
        public static async Task<List<TrackOriginalSku>> GetTrackOriginalSkuByECommerceAsync(
            EECommerceType eCommerce,
            MySqlConnection conn)
        {
            List<TrackOriginalSku> list = new List<TrackOriginalSku>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT *
                    FROM tb_track_original_sku
                    WHERE ECommmerce = @eCommerce
                    ORDER BY CreatedDate DESC",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@eCommerce", (int)eCommerce);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        list = await ReadAllTrackOriginalSkuFromReaderAsync(rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetTrackOriginalSkuByECommerceAsync error: {ex.ToString()}");
            }

            return list;
        }

        /// <summary>
        /// Xóa tracking SKU theo OriginalSku và ECommerce
        /// </summary>
        public static async Task<MySqlResultState> DeleteTrackOriginalSkuAsync(
            string originalSku,
            EECommerceType eCommerce,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"DELETE FROM tb_track_original_sku
                    WHERE OriginalSku = @originalSku AND ECommmerce = @eCommerce",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@originalSku", originalSku);
                    cmd.Parameters.AddWithValue("@eCommerce", (int)eCommerce);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        result.State = EMySqlResultState.OK;
                        result.Message = $"Đã xóa {rowsAffected} record(s)";
                    }
                    else
                    {
                        result.State = EMySqlResultState.INVALID;
                        result.Message = "Không tìm thấy record để xóa";
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"DeleteTrackOriginalSkuAsync error: {ex.ToString()}");
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Cập nhật ProOnEcoId theo OriginalSku và ECommerce
        /// </summary>
        public static async Task UpdateProOnEcoIdAsync(
            string originalSku,
            EECommerceType eCommerce,
            long proOnEcoId,
            MySqlResultState result,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE tb_track_original_sku
                    SET ProOnEcoId = @proOnEcoId
                    WHERE OriginalSku = @originalSku AND ECommmerce = @eCommerce",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@proOnEcoId", proOnEcoId);
                    cmd.Parameters.AddWithValue("@originalSku", originalSku);
                    cmd.Parameters.AddWithValue("@eCommerce", (int)eCommerce);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        result.State = EMySqlResultState.OK;
                        result.Message = $"Đã cập nhật ProOnEcoId cho {rowsAffected} record(s)";
                    }
                    else
                    {
                        result.State = EMySqlResultState.INVALID;
                        result.Message = "Không tìm thấy record để cập nhật";
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"UpdateProOnEcoIdAsync error: {ex.ToString()}");
                result.State = EMySqlResultState.ERROR;
                result.Message = ex.Message;
            }
        }
    }
}
