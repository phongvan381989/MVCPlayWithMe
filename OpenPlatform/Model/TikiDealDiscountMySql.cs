using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class TikiDealDiscountMySql
    {
        public async Task InsertCheckExistTikiDealDiscountOfOneSkuConnectOutAsync(
            List<DealCreatedResponseDetail> listDeal,
            MySqlConnection conn)
        {
            try
            {
                DealCreatedResponseDetail deal = listDeal[0];
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbTikiDealDiscount_Insert_Check_Exist", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_DealDiscount_Id", deal.id);
                    cmd.Parameters.AddWithValue("@p_Sku", deal.sku);
                    cmd.Parameters.AddWithValue("@p_SpecialPrice", deal.special_price);
                    cmd.Parameters.AddWithValue("@p_SpecialFromDate", deal.special_from_date);
                    cmd.Parameters.AddWithValue("@p_SpecialToDate", deal.special_to_date);
                    cmd.Parameters.AddWithValue("@p_QtyMax", deal.qty_max);
                    cmd.Parameters.AddWithValue("@p_QtyLimit", deal.qty_limit);
                    cmd.Parameters.AddWithValue("@p_DiscountPercent", deal.discount_percent);
                    cmd.Parameters.AddWithValue("@p_IsActive", deal.is_active);
                    cmd.Parameters.AddWithValue("@p_Notes", deal.notes);
                    cmd.Parameters.AddWithValue("@p_CreatedAt", deal.created_at);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task InsertCheckExistTikiDealDiscountOfSkuListConnectOutAsync(
            List<DealCreatedResponseDetail> listDeal,
            MySqlConnection conn)
        {
            try
            {
                List<string> skuInsertedList = new List<string>();
                DealCreatedResponseDetail deal = null;
                int count = listDeal.Count();
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbTikiDealDiscount_Insert_Check_Exist", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_DealDiscount_Id", 0);
                    cmd.Parameters.AddWithValue("@p_Sku", "");
                    cmd.Parameters.AddWithValue("@p_SpecialPrice", 0);
                    cmd.Parameters.AddWithValue("@p_SpecialFromDate", "");
                    cmd.Parameters.AddWithValue("@p_SpecialToDate", "");
                    cmd.Parameters.AddWithValue("@p_QtyMax", 0);
                    cmd.Parameters.AddWithValue("@p_QtyLimit", 0);
                    cmd.Parameters.AddWithValue("@p_DiscountPercent", 0.0);
                    cmd.Parameters.AddWithValue("@p_IsActive", 0);
                    cmd.Parameters.AddWithValue("@p_Notes", "");
                    cmd.Parameters.AddWithValue("@p_CreatedAt", DateTime.Now);

                    for (int i = 0; i < count; i++)
                    {
                        deal = listDeal[i];
                        if (skuInsertedList.Contains(deal.sku))
                        {
                            continue;
                        }
                        skuInsertedList.Add(deal.sku);
                        cmd.Parameters[0].Value = deal.id;
                        cmd.Parameters[1].Value = deal.sku;
                        cmd.Parameters[2].Value = deal.special_price;
                        cmd.Parameters[3].Value = deal.special_from_date;
                        cmd.Parameters[4].Value = deal.special_to_date;
                        cmd.Parameters[5].Value = deal.qty_max;
                        cmd.Parameters[6].Value = deal.qty_limit;
                        cmd.Parameters[7].Value = deal.discount_percent;
                        cmd.Parameters[8].Value = deal.is_active;
                        cmd.Parameters[9].Value = deal.notes;
                        cmd.Parameters[10].Value = deal.created_at;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task<List<string>> GetListSkuOfActiveItemConnectOutAsync(MySqlConnection conn)
        {
            List<string> skuList = new List<string>();
            string query = "SELECT Sku FROM tbtikiitem WHERE Status = 0";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            skuList.Add(MyMySql.GetString(reader, "Sku"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return skuList;
        }

        public async Task<List<SimpleTikiProduct>> GetItemsWithDealDiscountAsync(string store,
            MySqlConnection conn)
        {
            List<SimpleTikiProduct> simpleTikiProducts = new List<SimpleTikiProduct>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbTikiDealDiscount_Update_IsActive", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }

                using (MySqlCommand cmd = new MySqlCommand(store, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = reader.GetOrdinal("TikiId");
                        int skuIndex = reader.GetOrdinal("Sku");
                        int nameIndex = reader.GetOrdinal("Name");
                        int imageSrcIndex = reader.GetOrdinal("Image");

                        while (await reader.ReadAsync())
                        {
                            SimpleTikiProduct product = new SimpleTikiProduct
                            {
                                id = reader.GetInt32(idIndex),
                                sku = reader.IsDBNull(skuIndex) ? null : reader.GetString(skuIndex),
                                name = reader.IsDBNull(nameIndex) ? null : reader.GetString(nameIndex),
                                imageSrc = reader.IsDBNull(imageSrcIndex) ? null : reader.GetString(imageSrcIndex)
                            };

                            simpleTikiProducts.Add(product);
                        }
                    }
                }

                TikiMySql tikiSqler = new TikiMySql();
                foreach (var simpleTiki in simpleTikiProducts)
                {
                    CommonItem item = new CommonItem();
                    item.models.Add(new CommonModel());
                    await tikiSqler.TikiGetItemFromIdConnectOutAsync(simpleTiki.id, item, conn);
                    simpleTiki.models = item.models;
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                simpleTikiProducts = new List<SimpleTikiProduct>();
            }
            return simpleTikiProducts;
        }

        public async Task<List<SimpleTikiProduct>> GetItemsNoDealDiscountRunningAsync(MySqlConnection conn)
        {
            return await GetItemsWithDealDiscountAsync("st_tbTikiDealDiscount_Get_Item_No_Deal_Running", conn);
        }

        public async Task<List<SimpleTikiProduct>> GetItemsHasDealDiscountRunningAsync(MySqlConnection conn)
        {
            return await GetItemsWithDealDiscountAsync("st_tbTikiDealDiscount_Get_Item_Deal_Running", conn);
        }

        public async Task<TaxAndFee> GetTaxAndFeeAsync(string eEcommerceName, MySqlConnection conn)
        {
            TaxAndFee taxAndFee = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbtaxesandfees WHERE Name = @inName;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inName", eEcommerceName);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int nameIndex = rdr.GetOrdinal("Name");
                        int taxIndex = rdr.GetOrdinal("Tax");
                        int feeIndex = rdr.GetOrdinal("Fee");
                        int minProfitIndex = rdr.GetOrdinal("MinProfit");
                        int packingCostIndex = rdr.GetOrdinal("PackingCost");
                        int expectedPercentProfitIndex = rdr.GetOrdinal("ExpectedPercentProfit");
                        int minPercentProfitIndex = rdr.GetOrdinal("MinPercentProfit");

                        while (await rdr.ReadAsync())
                        {
                            taxAndFee = new TaxAndFee();
                            taxAndFee.name = rdr.GetString(nameIndex);
                            taxAndFee.tax = rdr.GetFloat(taxIndex);
                            taxAndFee.fee = rdr.GetFloat(feeIndex);
                            taxAndFee.minProfit = rdr.GetInt32(minProfitIndex);
                            taxAndFee.packingCost = rdr.GetInt32(packingCostIndex);
                            taxAndFee.expectedPercentProfit = rdr.GetFloat(expectedPercentProfitIndex);
                            taxAndFee.minPercentProfit = rdr.GetFloat(minPercentProfitIndex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return taxAndFee;
        }

        public async Task<MySqlResultState> UpdateIsActiveCloseFromLsDealIdAsync(List<int> lsDealId, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE tbtikidealdiscount SET IsActive = 5 WHERE DealDiscount_Id = @inDealDiscount_Id;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inDealDiscount_Id", 0);
                    foreach (var id in lsDealId)
                    {
                        cmd.Parameters[0].Value = id;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public async Task<MySqlResultState> UpdateIsActiveCloseFromSkuAsync(List<string> skuList, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE tbtikidealdiscount SET IsActive = 5 WHERE Sku = @inSku AND IsActive = 2;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inSku", "");
                    foreach (var sku in skuList)
                    {
                        cmd.Parameters[0].Value = sku;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public async Task<int> GetTikiIdBySkuAsync(string sku, MySqlConnection conn)
        {
            int tikiId = 0;
            try
            {
                string query = "SELECT TikiId FROM tbtikiitem WHERE Sku = @p_Sku LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p_Sku", sku);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            tikiId = rdr.GetInt32("TikiId");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return tikiId;
        }

        public async Task UpdateIsActiveCloseFromItemIdAsync(int itemId, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbTikiDealDiscount_Update_IsActive_Close_From_ItemId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inItemId", itemId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }
    }
}
