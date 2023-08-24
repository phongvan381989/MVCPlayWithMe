using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class ItemModelMySql : BasicMySql
    {
        public List<BasicIdName> GetListItemName()
        {
            List<BasicIdName> ls = new List<BasicIdName>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Select_All_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(new ProductIdName(MyMySql.GetInt32(rdr, "Id"), MyMySql.GetString(rdr, "Name")));
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return ls;
        }

        private void ItemParameters(Item item, MySqlParameter[] paras)
        {
            paras[0] = new MySqlParameter("@inId", item.id);
            paras[1] = new MySqlParameter("@inName", item.name);
            paras[2] = new MySqlParameter("@inStatus", item.status);
            paras[3] = new MySqlParameter("@inDetail", item.detail);
            paras[4] = new MySqlParameter("@inQuota", item.quota);

            MyMySql.AddOutParameters(paras);
        }

        public MySqlResultState AddItem(Item it)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[7];
            ItemParameters(it, paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbItem_Insert", paras);

            return result;
        }

        public MySqlResultState UpdateItem(Item it)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[7];
            ItemParameters(it, paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbItem_Update", paras);

            return result;
        }

        /// <summary>
        /// Lấy giá trị item id lớn nhất
        /// </summary>
        /// <returns> -1 nếu có lỗi </returns>
        public int GetMaxItemId()
        {
            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Get_Max_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            id = MyMySql.GetInt32(rdr, "Id");
                            break;
                        }
                    }
                    else
                    {
                        id = -1;
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                id = -1;
            }

            conn.Close();
            return id;
        }

        /// <summary>
        /// Lấy giá trị model id lớn nhất
        /// </summary>
        /// <returns> -1 nếu có lỗi </returns>
        public int GetMaxModelId()
        {
            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Get_Max_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            id = MyMySql.GetInt32(rdr, "Id");
                            break;
                        }
                    }
                    else
                    {
                        id = -1;
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                id = -1;
            }

            conn.Close();
            return id;
        }

        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product
        /// </summary>
        /// <returns></returns>
        private Model ConvertFromDataMySql(MySqlDataReader rdr)
        {
            Model model = new Model();
            model.id = MyMySql.GetInt32(rdr, "Id");
            model.itemId = MyMySql.GetInt32(rdr, "ItemId");
            model.name = MyMySql.GetString(rdr, "Name");
            model.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
            model.price = MyMySql.GetInt32(rdr, "Price");
            model.status = MyMySql.GetInt32(rdr, "Status");
            model.quota = MyMySql.GetInt32(rdr, "Quota");
            model.quantity = MyMySql.GetInt32(rdr, "Quantity");
            return model;
        }

        /// <summary>
        /// Từ list model hiện tại, xóa hết model cũ và trả về list model đã bị xóa
        /// </summary>
        /// <param name="lsNewModel"></param>
        public List<Model> DeleteOldModel(List<int> lsNewModel, int itemId)
        {
            List<Model> lsDeletedModel = new List<Model>();
            List<Model> lsOldModel = new List<Model>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            MySqlResultState result = new MySqlResultState();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Select_From_ItemId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inItemId", itemId);

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        lsOldModel.Add(ConvertFromDataMySql(rdr));
                    }
                }
                if (rdr != null)
                    rdr.Close();

                // Xóa model cũ
                MySqlParameter[] paras = null;
                int lengthPara = 3;
                paras = new MySqlParameter[lengthPara];
                paras[0] = new MySqlParameter("@inId", 0);
                MyMySql.AddOutParameters(paras);

                MySqlCommand cmdDel = new MySqlCommand("st_tbModel_Delete_From_Id", conn);
                cmdDel.CommandType = CommandType.StoredProcedure;
                cmdDel.Parameters.AddRange(paras);

                foreach (var old in lsOldModel)
                {
                    if (!lsNewModel.Contains(old.id))
                    {
                        cmdDel.Parameters[0].Value = old.id;
                        lsDeletedModel.Add(old);
                    }
                    cmdDel.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return lsDeletedModel;
        }

        private void ModelParameters(Model model, MySqlParameter[] paras)
        {
            paras[0] = new MySqlParameter("@inId", model.id);
            paras[1] = new MySqlParameter("@inItemId", model.itemId);
            paras[2] = new MySqlParameter("@inName", model.name);
            paras[3] = new MySqlParameter("@inBookCoverPrice", model.bookCoverPrice);
            paras[4] = new MySqlParameter("@inPrice", model.price);
            paras[5] = new MySqlParameter("@inStatus", model.status);
            paras[6] = new MySqlParameter("@inQuota", model.quota);
            paras[7] = new MySqlParameter("@inQuantity", model.quantity);

            MyMySql.AddOutParameters(paras);
        }

        public MySqlResultState AddModel(Model model)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[10];
            ModelParameters(model, paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbModel_Insert", paras);

            return result;
        }

        public MySqlResultState UpdateModel(Model model)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[10];
            ModelParameters(model, paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbModel_Update", paras);

            return result;
        }
    }
}