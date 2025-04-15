﻿using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class TikiDealDiscountMySql
    {
        public void InsertTikiDealDiscountConnectOut(List<DealCreatedResponseDetail> listDeal,
            MySqlConnection conn)
        {
            try
            {
                foreach (var deal in listDeal)
                {
                    // Nếu tồn tại thì cập nhật trạng thái Active ngược lại thêm mới
                    MySqlCommand cmd = new MySqlCommand("st_tbTikiDealDiscount_Insert_Check_Exist", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thêm tham số cho Stored Procedure
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

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public List<string> GetListSkuOfActiveItemConnectOut(MySqlConnection conn)
        {
            List<string> skuList = new List<string>();
            string query = "SELECT Sku FROM tbtikiitem WHERE Status = 0";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skuList.Add(MyMySql.GetString(reader, "Sku"));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return skuList;
        }

        public List<SimpleTikiProduct> GetItemsWithDealDiscount(string store, 
            MySqlConnection conn)
        {
            List<SimpleTikiProduct> simpleTikiProducts = new List<SimpleTikiProduct>();
            List<SimpleTikiProduct> simpleTikiProductsTemp = new List<SimpleTikiProduct>();
            try
            {
                // Cập nhật lại trạng thái
                using (MySqlCommand cmd = new MySqlCommand("st_tbTikiDealDiscount_Update_IsActive", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                using (MySqlCommand cmd = new MySqlCommand(store, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Lấy index của cột một lần duy nhất (tối ưu hơn reader["column"])
                        int idIndex = reader.GetOrdinal("TikiId");
                        int skuIndex = reader.GetOrdinal("Sku");
                        int nameIndex = reader.GetOrdinal("Name");
                        int imageSrcIndex = reader.GetOrdinal("Image");

                        while (reader.Read())
                        {
                            // Kiểm tra NULL trước khi đọc dữ liệu
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

                // Loại bỏ Item đã mapping nhưng tồn kho mapping bằng 0
                TikiMySql tikiSqler = new TikiMySql();
                foreach (var simpleTiki in simpleTikiProducts)
                {
                    CommonItem item = new CommonItem();
                    item.models.Add(new CommonModel());
                    tikiSqler.TikiGetItemFromIdConnectOut(simpleTiki.id, item, conn);
                    if (item.models[0].mapping.Count > 0 && item.models[0].GetQuatityFromListMapping() > 0) // Đã mapping
                    {
                        simpleTikiProductsTemp.Add(simpleTiki);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return simpleTikiProductsTemp;
        }

        public List<SimpleTikiProduct> GetItemsNoDealDiscountRunning(MySqlConnection conn)
        {
            return GetItemsWithDealDiscount("st_tbTikiDealDiscount_Get_Item_No_Deal_Running", conn);
        }

        public List<SimpleTikiProduct> GetItemsHasDealDiscountRunning(MySqlConnection conn)
        {
            return GetItemsWithDealDiscount("st_tbTikiDealDiscount_Get_Item_Deal_Running", conn);
        }

        public TaxAndFee GetTaxAndFee(string eEcommerceName, MySqlConnection conn)
        {
            TaxAndFee taxAndFee = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbtaxesandfees WHERE Name = @inName;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inName", eEcommerceName);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Lấy index của cột một lần duy nhất (tối ưu hơn reader["column"])
                        int nameIndex = rdr.GetOrdinal("Name");
                        int taxIndex = rdr.GetOrdinal("Tax");
                        int feeIndex = rdr.GetOrdinal("Fee");
                        int minProfitIndex = rdr.GetOrdinal("MinProfit");
                        int packingCostIndex = rdr.GetOrdinal("PackingCost");
                        int expectedPercentProfitIndex = rdr.GetOrdinal("ExpectedPercentProfit");
                        int minPercentProfitIndex = rdr.GetOrdinal("MinPercentProfit");

                        while (rdr.Read())
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

        public MySqlResultState UpdateIsActiveOff(List<int> lsDealId, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE tbtikidealdiscount SET IsActive = 5 WHERE DealDiscount_Id = @inDealDiscount_Id;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inDealDiscount_Id", 0);
                    foreach( var id in lsDealId)
                    {
                        cmd.Parameters[0].Value = id;
                        cmd.ExecuteNonQuery();
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
    }
}