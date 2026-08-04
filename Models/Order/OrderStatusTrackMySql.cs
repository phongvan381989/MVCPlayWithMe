using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models.Order
{
    /// <summary>
    /// Repository cho bảng tborder_track
    /// </summary>
    public class OrderStatusTrackMySql
    {
        #region Get Methods

        /// <summary>
        /// Lấy toàn bộ lịch sử thay đổi trạng thái của 1 đơn hàng
        /// </summary>
        public static async Task<List<OrderStatusTrack>> GetByOrderIdAsync(int orderId)
        {
            List<OrderStatusTrack> list = new List<OrderStatusTrack>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = @"
                        SELECT Id, OrderId, Status, Time
                        FROM tborder_track
                        WHERE OrderId = @orderId
                        ORDER BY Time ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;

                        using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(MapToOrderStatusTrack(rdr));
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
        /// Lấy trạng thái hiện tại (mới nhất) của 1 đơn hàng
        /// </summary>
        public static async Task<OrderStatusTrack> GetLatestByOrderIdAsync(int orderId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = @"
                        SELECT Id, OrderId, Status, Time
                        FROM tborder_track
                        WHERE OrderId = @orderId
                        ORDER BY Time DESC
                        LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;

                        using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                return MapToOrderStatusTrack(rdr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetLatestByOrderIdAsync error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Đếm số đơn hàng theo từng trạng thái (cho dashboard)
        /// </summary>
        public static async Task<Dictionary<int, int>> GetOrderCountByStatusAsync()
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // Lấy trạng thái mới nhất của mỗi đơn
                    string query = @"
                        SELECT Status, COUNT(*) as Count
                        FROM (
                            SELECT OrderId, Status
                            FROM tborder_track t1
                            WHERE Time = (
                                SELECT MAX(Time)
                                FROM tborder_track t2
                                WHERE t2.OrderId = t1.OrderId
                            )
                            GROUP BY OrderId, Status
                        ) AS latest_status
                        GROUP BY Status";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            int status = MyMySql.GetInt32(rdr, "Status");
                            int count = MyMySql.GetInt32(rdr, "Count");
                            result[status] = count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetOrderCountByStatusAsync error: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng có trạng thái cụ thể (dựa trên status mới nhất)
        /// </summary>
        public static async Task<List<int>> GetOrderIdsByStatusAsync(int status)
        {
            List<int> orderIds = new List<int>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT DISTINCT OrderId
                        FROM tborder_track t1
                        WHERE Status = @status
                          AND Time = (
                              SELECT MAX(Time)
                              FROM tborder_track t2
                              WHERE t2.OrderId = t1.OrderId
                          )";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@status", MySqlDbType.Int32).Value = status;

                        using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                orderIds.Add(MyMySql.GetInt32(rdr, "OrderId"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetOrderIdsByStatusAsync error: {ex.Message}");
            }
            return orderIds;
        }

        #endregion

        #region Insert (Manual tracking - nếu cần)

        /// <summary>
        /// Thêm record tracking thủ công (thường trigger tự động làm)
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(int orderId, int status)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"
                        INSERT INTO tborder_track (OrderId, Status, Time)
                        VALUES (@orderId, @status, @time)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("@status", MySqlDbType.Int32).Value = status;
                        cmd.Parameters.Add("@time", MySqlDbType.DateTime).Value = DateTime.Now;

                        await cmd.ExecuteNonQueryAsync();
                        result.State = EMySqlResultState.OK;
                        result.Message = "Đã thêm tracking record";
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
        /// Thêm tracking với OrderStatus string
        /// </summary>
        public static async Task<MySqlResultState> InsertAsync(int orderId, string orderStatus)
        {
            int status = OrderStatusMapper.ToInt(orderStatus);
            return await InsertAsync(orderId, status);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Map MySqlDataReader sang OrderStatusTrack object
        /// </summary>
        private static OrderStatusTrack MapToOrderStatusTrack(MySqlDataReader rdr)
        {
            return new OrderStatusTrack
            {
                Id = MyMySql.GetInt32(rdr, "Id"),
                OrderId = MyMySql.GetInt32(rdr, "OrderId"),
                Status = MyMySql.GetInt32(rdr, "Status"),
                Time = MyMySql.GetDateTime(rdr, "Time")
            };
        }

        #endregion
    }
}
