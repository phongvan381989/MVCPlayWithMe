using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;
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
    }
}