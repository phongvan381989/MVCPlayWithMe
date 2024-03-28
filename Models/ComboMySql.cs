using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class ComboMySql : BasicMySql
    {
        public List<Combo> GetListCombo()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<Combo> ls = new List<Combo>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCombo_Select_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(new Combo(MyMySql.GetInt32(rdr, "Id"), MyMySql.GetString(rdr, "Name")));
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

        public MySqlResultState CreateNewCombo(string name)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;
            paras = new MySqlParameter[parasLength];

            paras[0] = new MySqlParameter("@comboName", name);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCombo_Insert", paras);

            return result;
        }

        public MySqlResultState DeleteCombo(int id)
        {

            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@comboId", id);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCombo_Delete_From_Id", paras);

            return result;
        }

        public MySqlResultState DeleteCombo(string name)
        {

            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;

            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@comboName", name);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCombo_Delete_From_Name", paras);

            return result;
        }

        /// <summary>
        /// Từ name lấy được id. Nếu name chưa có, ta thêm vào bảng
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetComboIdFromName(string name)
        {
            MySqlParameter[] paras = null;
            int parasLength = 3;

            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@comboName", name);
            MyMySql.AddOutParameters(paras);
            return MyMySql.ExcuteGetIdStoreProceduce("st_tbCombo_GetComboIdFromName", paras);
        }
    }
}