using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Repository cho tbTaxAndFee
    /// </summary>
    public static class TaxAndFeeMySql
    {
        /// <summary>
        /// Lấy TaxAndFee theo tên sàn
        /// </summary>
        public static async Task<TaxAndFee> GetByNameAsync(string name)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "SELECT * FROM tbtaxesandfees WHERE Name = @name LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await rdr.ReadAsync())
                            {
                                return new TaxAndFee
                                {
                                    Id = rdr.GetInt32("Id"),
                                    name = rdr.GetString("Name"),
                                    tax = rdr.GetFloat("Tax"),
                                    fee = rdr.GetFloat("Fee"),
                                    minProfit = rdr.GetInt32("MinProfit"),
                                    packingCost = rdr.GetInt32("PackingCost"),
                                    expectedPercentProfit = rdr.GetFloat("ExpectedPercentProfit"),
                                    minPercentProfit = rdr.GetFloat("MinPercentProfit")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetByNameAsync error: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Lấy tất cả TaxAndFee
        /// </summary>
        public static async Task<List<TaxAndFee>> GetAllAsync()
        {
            List<TaxAndFee> list = new List<TaxAndFee>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "SELECT * FROM tbTaxAndFee ORDER BY Name";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                list.Add(new TaxAndFee
                                {
                                    Id = rdr.GetInt32("Id"),
                                    name = rdr.GetString("Name"),
                                    tax = rdr.GetFloat("Tax"),
                                    fee = rdr.GetFloat("Fee"),
                                    minProfit = rdr.GetInt32("MinProfit"),
                                    packingCost = rdr.GetInt32("PackingCost"),
                                    expectedPercentProfit = rdr.GetFloat("ExpectedPercentProfit"),
                                    minPercentProfit = rdr.GetFloat("MinPercentProfit")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"GetAllAsync error: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Update TaxAndFee
        /// </summary>
        public static async Task<MySqlResultState> UpdateAsync(TaxAndFee taxAndFee)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE tbTaxAndFee
                                   SET Tax = @tax,
                                       Fee = @fee,
                                       MinProfit = @minProfit,
                                       PackingCost = @packingCost,
                                       ExpectedPercentProfit = @expectedProfit,
                                       MinPercentProfit = @minProfit
                                   WHERE Id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", taxAndFee.Id);
                        cmd.Parameters.AddWithValue("@tax", taxAndFee.tax);
                        cmd.Parameters.AddWithValue("@fee", taxAndFee.fee);
                        cmd.Parameters.AddWithValue("@minProfit", taxAndFee.minProfit);
                        cmd.Parameters.AddWithValue("@packingCost", taxAndFee.packingCost);
                        cmd.Parameters.AddWithValue("@expectedProfit", taxAndFee.expectedPercentProfit);
                        cmd.Parameters.AddWithValue("@minPercent", taxAndFee.minPercentProfit);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            result.State = EMySqlResultState.OK;
                            result.Message = "Cập nhật TaxAndFee thành công";
                        }
                        else
                        {
                            result.State = EMySqlResultState.EXCEPTION;
                            result.Message = "Không tìm thấy TaxAndFee để cập nhật";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn($"UpdateAsync error: {ex.Message}");
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
