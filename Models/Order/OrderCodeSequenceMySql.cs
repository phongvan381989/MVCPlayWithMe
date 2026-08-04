using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.Order
{
    public class OrderCodeSequenceMySql
    {
        /// <summary>
        /// Sinh mã đơn hàng unique (retry nếu trùng - xác suất cực thấp) và luu vào tborder_code_sequence để tránh trùng trong tương lai
        /// </summary>
        /// <param name="conn">MySqlConnection đang mở</param>
        /// <returns>Mã đơn hàng unique</returns>
        public static async Task<string> GenerateUniqueOrderCodeAsync()
        {
            const int maxRetries = 10;

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                string code = string.Empty;
                for (int i = 0; i < maxRetries; i++)
                {
                    code = Common.GenerateOrderCode();

                    // Kiểm tra trùng trong DB
                    bool exists = await CheckOrderCodeExistsAsync(code, conn);

                    if (!exists)
                    {
                        // Nếu không trùng lưu vào tborder_code_sequence để tránh trùng trong tương lai
                        await SaveOrderCodeAsync(code, conn);
                        return code;
                    }

                    MyLogger.GetInstance().Warn($"OrderCode duplicate: {code} (retry {i + 1}/{maxRetries})");
                }
            }

            throw new Exception("Sinh mã đơn ngẫu nhiên bị trùng. Vui lòng thử lại sau.");
        }

        private static async Task<bool> SaveOrderCodeAsync(string orderCode, MySqlConnector.MySqlConnection conn)
        {
            string query = "INSERT INTO tborder_code_sequence (Code) VALUES (@orderCode)";

            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand(query, conn))
            {
                cmd.Parameters.Add("@orderCode", MySqlConnector.MySqlDbType.VarChar).Value = orderCode;
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
        }

        /// <summary>
        /// Kiểm tra mã đơn hàng đã tồn tại trong DB chưa
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng cần check</param>
        /// <param name="conn">MySqlConnection đang mở</param>
        /// <returns>true nếu đã tồn tại, false nếu chưa</returns>
        private static async Task<bool> CheckOrderCodeExistsAsync(string orderCode, MySqlConnector.MySqlConnection conn)
        {
            string query = "SELECT COUNT(*) FROM tborder_code_sequence WHERE Code = @orderCode";

            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand(query, conn))
            {
                cmd.Parameters.Add("@orderCode", MySqlConnector.MySqlDbType.VarChar).Value = orderCode;

                object scalarResult = await cmd.ExecuteScalarAsync();
                long count = Convert.ToInt64(scalarResult);
                return count > 0;
            }
        }

    }
}
