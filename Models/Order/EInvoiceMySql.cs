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
        /// Insert hóa đơn mới
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(EInvoice invoice)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = @"INSERT INTO `tb_einvoice`
                        (`Code`, `ECommmerce`, `OrderStatus`, `MoneySum`, `InvoiceStatus`, `InvoiceDate`, `InvoiceCode`, `Note`)
                        VALUES (@code, @ecommerce, @orderStatus, @moneySum, @invoiceStatus, @invoiceDate, @invoiceCode, @note)
                        ON DUPLICATE KEY UPDATE
                        `OrderStatus` = @orderStatus,
                        `MoneySum` = @moneySum,
                        `InvoiceStatus` = @invoiceStatus,
                        `Note` = @note";

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

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Thêm hóa đơn thành công";
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
        /// Lấy tất cả hóa đơn, sắp xếp theo ngày mới nhất
        /// </summary>
        public static async Task<List<EInvoice>> GetAllAsync()
        {
            List<EInvoice> list = new List<EInvoice>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM `tb_einvoice` ORDER BY `InvoiceDate` DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                EInvoice invoice = new EInvoice
                                {
                                    Id = MyMySql.GetInt32(rdr, "Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    OrderStatus = MyMySql.GetString(rdr, "OrderStatus"),
                                    MoneySum = MyMySql.GetFloat(rdr, "MoneySum"),
                                    InvoiceStatus = MyMySql.GetInt32(rdr, "InvoiceStatus"),
                                    InvoiceDate = MyMySql.GetDateTime(rdr, "InvoiceDate"),
                                    InvoiceCode = MyMySql.GetString(rdr, "InvoiceCode"),
                                    Note = MyMySql.GetString(rdr, "Note")
                                };
                                list.Add(invoice);
                            }
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
                            if (await rdr.ReadAsync())
                            {
                                invoice = new EInvoice
                                {
                                    Id = MyMySql.GetInt32(rdr, "Id"),
                                    Code = MyMySql.GetString(rdr, "Code"),
                                    ECommmerce = MyMySql.GetInt32(rdr, "ECommmerce"),
                                    OrderStatus = MyMySql.GetString(rdr, "OrderStatus"),
                                    MoneySum = MyMySql.GetFloat(rdr, "MoneySum"),
                                    InvoiceStatus = MyMySql.GetInt32(rdr, "InvoiceStatus"),
                                    InvoiceDate = MyMySql.GetDateTime(rdr, "InvoiceDate"),
                                    InvoiceCode = MyMySql.GetString(rdr, "InvoiceCode"),
                                    Note = MyMySql.GetString(rdr, "Note")
                                };
                            }
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
        /// Cập nhật OrderStatus từ Code và ECommmerce.
        /// Nếu chưa tồn tại (Code, ECommmerce) thì insert mới với OrderStatus, các field khác để default.
        /// Nếu đã tồn tại thì chỉ update OrderStatus.
        /// </summary>
        public static async Task UpdateOrInsertOrderStatusBackgroundAsync(string code,
            int ecommerce,
            string orderStatus)
        {
            if(string.IsNullOrEmpty(code) || string.IsNullOrEmpty(orderStatus))
            {
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Insert nếu chưa tồn tại, Update OrderStatus nếu đã có
                    // ON DUPLICATE KEY UPDATE dựa vào UNIQUE KEY `UC_CombinedKeys` (`Code`,`ECommmerce`)
                    string query = @"INSERT INTO `tb_einvoice`
                        (`Code`, `ECommmerce`, `OrderStatus`, `OrderStatusUpdateTime`)
                        VALUES (@code, @ecommerce, @orderStatus, NOW())
                        ON DUPLICATE KEY UPDATE
                        `OrderStatus` = @orderStatus, `OrderStatusUpdateTime` = NOW()";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                        cmd.Parameters.Add("@ecommerce", MySqlDbType.Int32).Value = ecommerce;
                        cmd.Parameters.Add("@orderStatus", MySqlDbType.VarChar).Value = orderStatus ?? string.Empty;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }
    }
}
