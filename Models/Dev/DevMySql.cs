using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.Dev
{
    public class DevMySql
    {
        public MySqlResultState DeleteDuplicateDataOftbShopeeModel()
        {
            MySqlResultState result = new MySqlResultState();
            //MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            //try
            //{
            //    conn.Open();

            //    List<Tuple<int, int>> lsIdItemId = new List<Tuple<int, int>>();
            //    // Lấy dữ liệu của tbshopeemodel TMDTShopeeModelId = -1 đang bị trùng lặp
            //    {
            //        MySqlCommand cmdTem = new MySqlCommand(
            //            "SELECT Id, ItemId FROM tbshopeemodel WHERE  tbshopeemodel.ItemId IN(SELECT ItemId FROM(SELECT ItemId, count(Id) AS aaa FROM tbshopeemodel WHERE TMDTShopeeModelId = -1 GROUP BY ItemId ORDER BY aaa) A WHERE A.aaa > 1)ORDER BY ItemId;",
            //            conn);
            //        cmdTem.CommandType = CommandType.Text;
            //        MySqlDataReader rdr = cmdTem.ExecuteReader();
            //        while (rdr.Read())
            //        {
            //            lsIdItemId.Add(new Tuple<int, int>(
            //                MyMySql.GetInt32(rdr, "Id"),
            //                MyMySql.GetInt32(rdr, "ItemId")
            //                ));
            //        }
            //        if (rdr != null)
            //            rdr.Close();
            //    }

            //    // Nếu nhiều id có chung ItemId, giữ lại id bé nhất
            //    List<int> lsNeedDelete = new List<int>();
            //    int le = lsIdItemId.Count();
            //    for (int i = 1; i < le; i = i + 2)
            //    {
            //        lsNeedDelete.Add(lsIdItemId[i].Item1);
            //    }
            // Xóa trên tbshopeemapping, tbpwmmappingother, tbshopeemodel
            //{
            //    MySqlCommand cmdTem = new MySqlCommand("st_tbShopeeModel_Delete_From_Id", conn);
            //    cmdTem.CommandType = CommandType.StoredProcedure;
            //    cmdTem.Parameters.AddWithValue("@inShopeeModelId", 0);
            //    foreach (var id in lsNeedDelete)
            //    {
            //        cmdTem.Parameters[0].Value = id;
            //        cmdTem.ExecuteNonQuery();
            //    }
            //}

            //}
            //catch (Exception ex)
            //{
            //    Common.SetResultException(ex, result);
            //}
            //conn.Close();
            return result;
        }

        public MySqlResultState ShopeeSaveLivePartnerKey(string key)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE webplaywithme.tbshopeeauthen SET `PartnerKey` = @inPartnerKey  WHERE `Id` = 1", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inPartnerKey", key);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState ShopeeSaveCode(string code)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE webplaywithme.tbshopeeauthen SET `Code` = @inCode  WHERE `Id` = 1", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inCode", code);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }
    }
}