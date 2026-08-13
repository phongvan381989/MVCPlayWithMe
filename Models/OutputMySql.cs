using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models
{
    public class OutputMySql
    {
        /// <summary>
        /// Convert rows từ MySqlDataReader sang List<Output>
        /// Tối ưu: lấy ordinal 1 lần, đọc theo index
        /// </summary>
        private static async Task ConvertRowsFromDataMySql(MySqlDataReader rdr, List<Output> outputs)
        {
            // Lấy index các cột 1 lần duy nhất
            int idIndex = rdr.GetOrdinal("Id");
            int codeIndex = rdr.GetOrdinal("Code");
            int eCommerceIndex = rdr.GetOrdinal("ECommmerce");
            int productIdIndex = rdr.GetOrdinal("ProductId");
            int quantityIndex = rdr.GetOrdinal("Quantity");
            int bookingCodeIndex = rdr.GetOrdinal("BookingCode");
            int createTimeIndex = rdr.GetOrdinal("Time");

            // Đọc tất cả rows
            while (await rdr.ReadAsync())
            {
                Output output = new Output();
                output.id = rdr.GetInt32(idIndex);
                output.code = rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex);
                output.eCommmerce = rdr.GetInt32(eCommerceIndex);
                output.productId = rdr.GetInt32(productIdIndex);
                output.quantity = rdr.GetInt32(quantityIndex);
                output.bookingCode = rdr.IsDBNull(bookingCodeIndex) ? string.Empty : rdr.GetString(bookingCodeIndex);
                output.time = rdr.GetDateTime(createTimeIndex);
                output.isCancel = false; // Default, không có trong DB

                outputs.Add(output);
            }
        }

        /// <summary>
        /// Lấy danh sách Output từ Code và ECommerce
        /// </summary>
        /// <param name="conn">MySQL connection (đã mở)</param>
        /// <param name="code">Mã đơn hàng</param>
        /// <param name="ecommerce">Loại sàn (1=Tiki, 2=Shopee, 3=Lazada)</param>
        /// <returns>Danh sách Output</returns>
        public static async Task<List<Output>> GetOutputsByCodeAndECommerceAsync(
            MySqlConnection conn,
            string code,
            int ecommerce)
        {
            List<Output> outputs = new List<Output>();

            try
            {
                string sql = @"
                    SELECT *
                    FROM tboutput
                    WHERE Code = @code AND ECommmerce = @ecommerce
                    ORDER BY Id ASC
                ";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    cmd.Parameters.Add("@ecommerce", MySqlDbType.Byte).Value = ecommerce;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        await ConvertRowsFromDataMySql(rdr, outputs);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetOutputsByCodeAndECommerceAsync failed: Code={code}, ECommerce={ecommerce}. Error: {ex.Message}");
            }

            return outputs;
        }
    }
}
