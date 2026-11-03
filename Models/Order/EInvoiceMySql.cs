using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVCPlayWithMe.Models.Order
{
    public class EInvoiceMySql
    {
        /// <summary>
        /// Đọc dữ liệu EInvoice từ MySqlDataReader
        /// </summary>
        private static async Task ReadEInvoicesAsync(MySqlDataReader rdr, List<EInvoice> list)
        {
            // Lấy ordinal của các cột một lần
            int idIndex = rdr.GetOrdinal("Id");
            int codeIndex = rdr.GetOrdinal("Code");
            int ecommerceIndex = rdr.GetOrdinal("ECommmerce");
            int orderStatusIndex = rdr.GetOrdinal("OrderStatus");
            int moneySumIndex = rdr.GetOrdinal("MoneySum");
            int invoiceStatusIndex = rdr.GetOrdinal("InvoiceStatus");
            int invoiceDateIndex = rdr.GetOrdinal("InvoiceDate");
            int invoiceCodeIndex = rdr.GetOrdinal("InvoiceCode");
            int noteIndex = rdr.GetOrdinal("Note");
            int orderStatusUpdateTimeIndex = rdr.GetOrdinal("OrderStatusUpdateTime");
            int needInvoiceIndex = rdr.GetOrdinal("NeedInvoice");
            int needInvoiceUpdateTimeIndex = rdr.GetOrdinal("NeedInvoiceUpdateTime");
            int invoiceLookupCodeIndex = rdr.GetOrdinal("InvoiceLookupCode");
            int invoiceIdIndex = rdr.GetOrdinal("InvoiceId");
            int invoiceNumberIndex = rdr.GetOrdinal("InvoiceNumber");

            while (await rdr.ReadAsync())
            {
                EInvoice invoice = new EInvoice
                {
                    Id = rdr.IsDBNull(idIndex) ? -1 : rdr.GetInt32(idIndex),
                    Code = rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex),
                    ECommmerce = rdr.IsDBNull(ecommerceIndex) ? -1 : rdr.GetInt32(ecommerceIndex),
                    OrderStatus = rdr.IsDBNull(orderStatusIndex) ? string.Empty : rdr.GetString(orderStatusIndex),
                    MoneySum = rdr.IsDBNull(moneySumIndex) ? 0f : rdr.GetFloat(moneySumIndex),
                    InvoiceStatus = rdr.IsDBNull(invoiceStatusIndex) ? 0 : rdr.GetInt32(invoiceStatusIndex),
                    InvoiceDate = rdr.IsDBNull(invoiceDateIndex) ? (DateTime?)null : rdr.GetDateTime(invoiceDateIndex),
                    InvoiceCode = rdr.IsDBNull(invoiceCodeIndex) ? string.Empty : rdr.GetString(invoiceCodeIndex),
                    Note = rdr.IsDBNull(noteIndex) ? string.Empty : rdr.GetString(noteIndex),
                    OrderStatusUpdateTime = rdr.IsDBNull(orderStatusUpdateTimeIndex) ? (DateTime?)null : rdr.GetDateTime(orderStatusUpdateTimeIndex),
                    NeedInvoice = rdr.IsDBNull(needInvoiceIndex) ? 0 : rdr.GetSByte(needInvoiceIndex),
                    NeedInvoiceUpdateTime = rdr.IsDBNull(needInvoiceUpdateTimeIndex) ? (DateTime?)null : rdr.GetDateTime(needInvoiceUpdateTimeIndex),
                    InvoiceLookupCode = rdr.IsDBNull(invoiceLookupCodeIndex) ? string.Empty : rdr.GetString(invoiceLookupCodeIndex),
                    InvoiceId = rdr.IsDBNull(invoiceIdIndex) ? string.Empty : rdr.GetString(invoiceIdIndex),
                    InvoiceNumber = rdr.IsDBNull(invoiceNumberIndex) ? 0 : rdr.GetInt32(invoiceNumberIndex)
                };
                list.Add(invoice);
            }
        }

        /// <summary>
        /// Insert hóa đơn mới
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(EInvoice invoice, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                string query = @"INSERT INTO `tb_einvoice`
                    (`Code`, `ECommmerce`, `OrderStatus`, `MoneySum`, `InvoiceStatus`, `InvoiceDate`, `InvoiceCode`, `Note`,
                        `NeedInvoice`, `NeedInvoiceUpdateTime`, `InvoiceLookupCode`, `InvoiceId`, `InvoiceNumber`)
                    VALUES (@code, @ecommerce, @orderStatus, @moneySum, @invoiceStatus, @invoiceDate, @invoiceCode, @note,
                        @needInvoice, NOW(), @invoiceLookupCode, @invoiceId, @invoiceNumber)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = invoice.Code;
                    cmd.Parameters.Add("@ecommerce", MySqlDbType.Int32).Value = invoice.ECommmerce;
                    cmd.Parameters.Add("@orderStatus", MySqlDbType.VarChar).Value = invoice.OrderStatus ?? string.Empty;
                    cmd.Parameters.Add("@moneySum", MySqlDbType.Float).Value = invoice.MoneySum;
                    cmd.Parameters.Add("@invoiceStatus", MySqlDbType.Byte).Value = invoice.InvoiceStatus;
                    cmd.Parameters.Add("@invoiceDate", MySqlDbType.DateTime).Value = invoice.InvoiceDate;
                    cmd.Parameters.Add("@invoiceCode", MySqlDbType.VarChar).Value = invoice.InvoiceCode ?? string.Empty;
                    cmd.Parameters.Add("@note", MySqlDbType.VarChar).Value = invoice.Note ?? string.Empty;
                    cmd.Parameters.Add("@needInvoice", MySqlDbType.Byte).Value = invoice.NeedInvoice;
                    cmd.Parameters.Add("@invoiceLookupCode", MySqlDbType.VarChar).Value = invoice.InvoiceLookupCode ?? string.Empty;
                    cmd.Parameters.Add("@invoiceId", MySqlDbType.VarChar).Value = invoice.InvoiceId ?? string.Empty;
                    cmd.Parameters.Add("@invoiceNumber", MySqlDbType.Int32).Value = invoice.InvoiceNumber;

                    await cmd.ExecuteNonQueryAsync();
                    result.State = EMySqlResultState.OK;
                    result.Message = "Thêm hóa đơn thành công";
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả hóa đơn, sắp xếp theo ngày mới nhất
        /// </summary>
        public static async Task<List<EInvoice>> GetAllAsync(DateTime? fromDate = null)
        {
            List<EInvoice> list = new List<EInvoice>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM `tb_einvoice`";

                    if (fromDate.HasValue)
                    {
                        query += " WHERE (`NeedInvoiceUpdateTime` IS NULL AND `OrderStatusUpdateTime` >= @fromDate) OR `NeedInvoiceUpdateTime` >= @fromDate";
                    }

                    query += " ORDER BY `NeedInvoiceUpdateTime` DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (fromDate.HasValue)
                        {
                            fromDate = fromDate.Value.Date; // Chỉ lấy phần ngày, bỏ phần giờ
                            cmd.Parameters.Add("@fromDate", MySqlDbType.DateTime).Value = fromDate.Value;
                        }
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            await ReadEInvoicesAsync(rdr, list);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    list.Clear();
                }
            }
            return list;
        }

        /// <summary>
        /// Lấy hóa đơn theo ID
        /// </summary>
        public static async Task<EInvoice> GetByIdAsync(int id)
        {
            EInvoice invoice = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM `tb_einvoice` WHERE `Id` = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            List<EInvoice> tempList = new List<EInvoice>();
                            await ReadEInvoicesAsync(rdr, tempList);
                            invoice = tempList.FirstOrDefault();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return invoice;
        }

        /// <summary>
        /// Cập nhật trạng thái hóa đơn
        /// </summary>
        public static async Task<MySqlResultState> UpdateInvoiceStatusAsync(int id, int invoiceStatus, string invoiceCode, string note)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = @"UPDATE `tb_einvoice`
                        SET `InvoiceStatus` = @invoiceStatus,
                            `InvoiceDate` = @invoiceDate,
                            `InvoiceCode` = @invoiceCode,
                            `Note` = @note
                        WHERE `Id` = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                        cmd.Parameters.Add("@invoiceStatus", MySqlDbType.Byte).Value = invoiceStatus;
                        cmd.Parameters.Add("@invoiceDate", MySqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@invoiceCode", MySqlDbType.VarChar).Value = invoiceCode ?? string.Empty;
                        cmd.Parameters.Add("@note", MySqlDbType.VarChar).Value = note ?? string.Empty;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Cập nhật trạng thái hóa đơn thành công";
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Xóa hóa đơn theo ID
        /// </summary>
        public static async Task<MySqlResultState> DeleteAsync(int id)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "DELETE FROM `tb_einvoice` WHERE `Id` = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Xóa hóa đơn thành công";
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng cần xuất hóa đơn theo ngày
        /// </summary>
        public static async Task<List<EInvoice>> GetOrdersForExportAsync(DateTime date)
        {
            List<EInvoice> list = new List<EInvoice>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = @"SELECT * FROM `tb_einvoice`
                        WHERE `NeedInvoice` = 1
                        AND DATE(`NeedInvoiceUpdateTime`) = DATE(@date)
                        ORDER BY `NeedInvoiceUpdateTime` DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@date", MySqlDbType.DateTime).Value = date;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            await ReadEInvoicesAsync(rdr, list);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return list;
        }

        /// <summary>
        /// Insert OrderStatus từ Code và ECommmerce.
        /// Chỉ insert nếu chưa tồn tại (Code, ECommmerce).
        /// </summary>
        /// <returns>true nếu insert thành công, false nếu đã tồn tại (duplicate key)</returns>
        public static async Task<bool> InsertOrderStatusBackgroundAsync(
            string code,
            int ecommerce,
            string orderStatus,
            MySqlConnection conn)
        {
            if(string.IsNullOrEmpty(code) || string.IsNullOrEmpty(orderStatus))
            {
                return false;
            }

            try
            {
                // Chỉ INSERT nếu chưa tồn tại, bỏ qua nếu duplicate
                // IGNORE dựa vào UNIQUE KEY `UC_CombinedKeys` (`Code`,`ECommmerce`)
                string query = @"INSERT IGNORE INTO `tb_einvoice`
                    (`Code`, `ECommmerce`, `OrderStatus`, `OrderStatusUpdateTime`, `NeedInvoice`, `NeedInvoiceUpdateTime`)
                    VALUES (@code, @ecommerce, @orderStatus, NOW(), 1, NOW())";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    cmd.Parameters.Add("@ecommerce", MySqlDbType.Int32).Value = ecommerce;
                    cmd.Parameters.Add("@orderStatus", MySqlDbType.VarChar).Value = orderStatus ?? string.Empty;

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    // rowsAffected == 1: insert thành công
                    // rowsAffected == 0: duplicate key, bỏ qua
                    return rowsAffected == 1;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra Code + ECommmerce đã có InvoiceId hoặc InvoiceLookupCode chưa
        /// </summary>
        /// <returns>true nếu đã có, false nếu chưa có hoặc không tồn tại record</returns>
        public static async Task<bool> HasCassoInvoiceLookUpInfoAsync(string code, int ecommerce,
            MySqlConnection conn)
        {
            try
            {
                string query = @"SELECT `InvoiceId`, `InvoiceLookupCode`
                    FROM `tb_einvoice`
                    WHERE `Code` = @code AND `ECommmerce` = @ecommerce
                    LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    cmd.Parameters.Add("@ecommerce", MySqlDbType.Int32).Value = ecommerce;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            string invoiceId = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                            string lookupCode = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);

                            // Có ít nhất 1 trong 2 trường có giá trị
                            return !string.IsNullOrEmpty(invoiceId) || !string.IsNullOrEmpty(lookupCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return false; // Không tìm thấy hoặc lỗi
        }

        /// <summary>
        /// Cập nhật InvoiceId và InvoiceLookupCode theo Code và ECommmerce
        /// </summary>
        public static async Task<MySqlResultState> UpdateCassoInvoiceLookUpInfoAsync(
            string code,
            int ecommerce,
            string invoiceId,
            string invoiceLookupCode,
            int invoiceStatus, // 0: Chưa xuất  / 1:  Đã xuất / 2:  Lỗi
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                string query = @"UPDATE `tb_einvoice`
                    SET `InvoiceId` = @invoiceId,
                        `InvoiceLookupCode` = @invoiceLookupCode
                    WHERE `Code` = @code AND `ECommmerce` = @ecommerce";

                if(invoiceStatus != 0)
                {
                    query = @"UPDATE `tb_einvoice`
                    SET `InvoiceId` = @invoiceId,
                        `InvoiceLookupCode` = @invoiceLookupCode,
                        `InvoiceStatus` = @invoiceStatus
                    WHERE `Code` = @code AND `ECommmerce` = @ecommerce";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    cmd.Parameters.Add("@ecommerce", MySqlDbType.Int32).Value = ecommerce;
                    cmd.Parameters.Add("@invoiceId", MySqlDbType.VarChar).Value = invoiceId ?? string.Empty;
                    cmd.Parameters.Add("@invoiceLookupCode", MySqlDbType.VarChar).Value = invoiceLookupCode ?? string.Empty;
                    if (invoiceStatus != 0)
                    {
                        cmd.Parameters.Add("@invoiceStatus", MySqlDbType.Byte).Value = invoiceStatus;
                    }

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        result.State = EMySqlResultState.OK;
                        result.Message = "Cập nhật thông tin hóa đơn thành công";
                    }
                    else
                    {
                        result.State = EMySqlResultState.INVALID;
                        result.Message = "Không tìm thấy đơn hàng để cập nhật";
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
        /// Lấy EInvoice theo Code và ECommmerce
        /// </summary>
        public static async Task<EInvoice> GetByCodeAndECommerceAsync(string code,
            int ecommerce,
            MySqlConnection conn)
        {
            EInvoice invoice = null;
            try
            {
                string query = @"SELECT * FROM `tb_einvoice`
                                WHERE `Code` = @code AND `ECommmerce` = @ecommerce
                                LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    cmd.Parameters.Add("@ecommerce", MySqlDbType.Int32).Value = ecommerce;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        List<EInvoice> tempList = new List<EInvoice>();
                        await ReadEInvoicesAsync(rdr, tempList);
                        invoice = tempList.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return invoice;
        }

        /// <summary>
        /// Cập nhật InvoiceNumber theo Code và ECommmerce
        /// </summary>
        public static async Task<MySqlResultState> UpdateInvoiceNumberAsync(
            string code,
            int ecommerce,
            int invoiceNumber,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                string query = @"UPDATE `tb_einvoice`
                                SET `InvoiceNumber` = @invoiceNumber
                                WHERE `Code` = @code AND `ECommmerce` = @ecommerce";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@invoiceNumber", MySqlDbType.Int32).Value = invoiceNumber;
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    cmd.Parameters.Add("@ecommerce", MySqlDbType.Int32).Value = ecommerce;

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        //result.State = EMySqlResultState.OK;
                        //result.Message = "Cập nhật số hóa đơn thành công";
                    }
                    else
                    {
                        result.State = EMySqlResultState.INVALID;
                        result.Message = "Không tìm thấy đơn hàng để cập nhật";
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }
    }
}
