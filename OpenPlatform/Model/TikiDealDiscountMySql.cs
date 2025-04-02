using MVCPlayWithMe.General;
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
                    MySqlCommand cmd = new MySqlCommand("sp_tbTikiDealDiscount_Insert", conn);
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

        public List<SimpleTikiProduct> GetItemsNoDealDiscountRunning(MySqlConnection conn)
        {
            List<SimpleTikiProduct> simpleTikiProducts = new List<SimpleTikiProduct>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbTikiDealDiscount_Get_Item_No_Deal_Running", conn))
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
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return simpleTikiProducts;
        }
    }
}