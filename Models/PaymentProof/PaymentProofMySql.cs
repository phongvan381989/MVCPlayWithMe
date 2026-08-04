using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.PaymentProof
{
    public class PaymentProofMySql
    {
        #region Get Methods

        /// <summary>
        /// Lấy chứng từ thanh toán theo ID
        /// </summary>
        public static async Task<PaymentProof> GetByIdAsync(int id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM tb_payment_proofs WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                return MapToPaymentProof(rdr);
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
        /// Lấy tất cả chứng từ của 1 đơn hàng
        /// </summary>
        public static async Task<List<PaymentProof>> GetByOrderIdAsync(int orderId)
        {
            List<PaymentProof> list = new List<PaymentProof>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM tb_payment_proofs WHERE OrderId = @orderId ORDER BY CreatedAt DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;

                        using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(MapToPaymentProof(rdr));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetByOrderIdAsync error: {ex.Message}");
            }
            return list;
        }

        /// <summary>
        /// Lấy tất cả chứng từ đang chờ duyệt (cho Admin)
        /// </summary>
        public static async Task<List<PaymentProof>> GetPendingProofsAsync()
        {
            List<PaymentProof> list = new List<PaymentProof>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM tb_payment_proofs WHERE Status = 'pending' ORDER BY CreatedAt ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            list.Add(MapToPaymentProof(rdr));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetPendingProofsAsync error: {ex.Message}");
            }
            return list;
        }

        /// <summary>
        /// Lấy tất cả chứng từ theo status
        /// </summary>
        public static async Task<List<PaymentProof>> GetByStatusAsync(string status)
        {
            List<PaymentProof> list = new List<PaymentProof>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM tb_payment_proofs WHERE Status = @status ORDER BY CreatedAt DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;

                        using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(MapToPaymentProof(rdr));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetByStatusAsync error: {ex.Message}");
            }
            return list;
        }

        #endregion

        #region Insert/Update/Delete

        /// <summary>
        /// Thêm mới chứng từ thanh toán
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(PaymentProof proof)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"
                        INSERT INTO tb_payment_proofs
                        (OrderId, TransactionCode, TransferAmount, TransferNote, ImageUrl, Status)
                        VALUES
                        (@orderId, @transactionCode, @transferAmount, @transferNote, @imageUrl, @status)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = proof.OrderId;
                        cmd.Parameters.Add("@transactionCode", MySqlDbType.VarChar).Value = (object)proof.TransactionCode ?? DBNull.Value;
                        cmd.Parameters.Add("@transferAmount", MySqlDbType.Int32).Value = proof.TransferAmount;
                        cmd.Parameters.Add("@transferNote", MySqlDbType.VarChar).Value = (object)proof.TransferNote ?? DBNull.Value;
                        cmd.Parameters.Add("@imageUrl", MySqlDbType.VarChar).Value = (object)proof.ImageUrl ?? DBNull.Value;
                        cmd.Parameters.Add("@status", MySqlDbType.VarChar).Value = proof.Status ?? "pending";

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Đã gửi chứng từ thanh toán thành công";
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
        /// Xác nhận hoặc từ chối chứng từ (standalone version)
        /// </summary>
        public static async Task<MySqlResultState> VerifyProofAsync(int proofId, int adminId, bool approve, string rejectReason = null)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string status = approve ? "verified" : "rejected";
                    string query = @"
                        UPDATE tb_payment_proofs
                        SET Status = @status,
                            VerifiedBy = @verifiedBy,
                            VerifiedAt = @verifiedAt,
                            RejectReason = @rejectReason
                        WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = proofId;
                        cmd.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;
                        cmd.Parameters.Add("@verifiedBy", MySqlDbType.Int32).Value = adminId;
                        cmd.Parameters.Add("@verifiedAt", MySqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@rejectReason", MySqlDbType.VarChar).Value = (object)rejectReason ?? DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = approve ? "Đã xác nhận thanh toán" : "Đã từ chối chứng từ";
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
        /// Xác nhận hoặc từ chối chứng từ (transaction version)
        /// </summary>
        public static async Task<MySqlResultState> VerifyProofTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            int proofId,
            int adminId,
            bool approve,
            string rejectReason = null)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                string status = approve ? "verified" : "rejected";
                string query = @"
                    UPDATE tb_payment_proofs
                    SET Status = @status,
                        VerifiedBy = @verifiedBy,
                        VerifiedAt = @verifiedAt,
                        RejectReason = @rejectReason
                    WHERE Id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = proofId;
                    cmd.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;
                    cmd.Parameters.Add("@verifiedBy", MySqlDbType.Int32).Value = adminId;
                    cmd.Parameters.Add("@verifiedAt", MySqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@rejectReason", MySqlDbType.VarChar).Value = (object)rejectReason ?? DBNull.Value;

                    await cmd.ExecuteNonQueryAsync();
                    result.State = EMySqlResultState.OK;
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật status
        /// </summary>
        public static async Task<MySqlResultState> UpdateStatusAsync(int proofId, string status)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tb_payment_proofs SET Status = @status WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = proofId;
                        cmd.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Cập nhật trạng thái thành công";
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
        /// Insert payment proof đã verified (admin tự tạo khi xác nhận thanh toán)
        /// </summary>
        public static async Task<MySqlResultState> InsertVerifiedProofAsync(PaymentProof proof)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"
                        INSERT INTO tb_payment_proofs
                        (OrderId, TransactionCode, TransferAmount, TransferNote, Status, VerifiedBy, VerifiedAt)
                        VALUES
                        (@orderId, @transactionCode, @transferAmount, @transferNote, @status, @verifiedBy, @verifiedAt)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = proof.OrderId;
                        cmd.Parameters.Add("@transactionCode", MySqlDbType.VarChar).Value = (object)proof.TransactionCode ?? DBNull.Value;
                        cmd.Parameters.Add("@transferAmount", MySqlDbType.Int32).Value = proof.TransferAmount;
                        cmd.Parameters.Add("@transferNote", MySqlDbType.VarChar).Value = (object)proof.TransferNote ?? DBNull.Value;
                        cmd.Parameters.Add("@status", MySqlDbType.VarChar).Value = proof.Status;
                        cmd.Parameters.Add("@verifiedBy", MySqlDbType.Int32).Value = proof.VerifiedBy;
                        cmd.Parameters.Add("@verifiedAt", MySqlDbType.DateTime).Value = proof.VerifiedAt;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Đã tạo chứng từ thanh toán";
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
        /// Xóa chứng từ
        /// </summary>
        public static async Task<MySqlResultState> DeleteAsync(int id)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM tb_payment_proofs WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Xóa chứng từ thành công";
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

        #region Helper Methods

        /// <summary>
        /// Map MySqlDataReader sang PaymentProof object
        /// </summary>
        private static PaymentProof MapToPaymentProof(MySqlDataReader rdr)
        {
            return new PaymentProof
            {
                Id = MyMySql.GetInt32(rdr, "Id"),
                OrderId = MyMySql.GetInt32(rdr, "OrderId"),
                TransactionCode = MyMySql.GetString(rdr, "TransactionCode"),
                TransferAmount = MyMySql.GetInt32(rdr, "TransferAmount"),
                TransferNote = MyMySql.GetString(rdr, "TransferNote"),
                ImageUrl = MyMySql.GetString(rdr, "ImageUrl"),
                VerifiedBy = rdr.IsDBNull(rdr.GetOrdinal("VerifiedBy")) ? (int?)null : MyMySql.GetInt32(rdr, "VerifiedBy"),
                VerifiedAt = MyMySql.GetDateTime(rdr, "VerifiedAt"),
                Status = MyMySql.GetString(rdr, "Status"),
                RejectReason = MyMySql.GetString(rdr, "RejectReason"),
                CreatedAt = MyMySql.GetDateTime(rdr, "CreatedAt"),
                UpdatedAt = MyMySql.GetDateTime(rdr, "UpdatedAt")
            };
        }

        #endregion
    }
}
