using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class PublisherMySql : BasicMySql
    {
        public List<Publisher> GetListPublisher()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<Publisher> ls = new List<Publisher>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbPublisher_Select_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(new Publisher(MyMySql.GetInt32(rdr, "Id"), MyMySql.GetString(rdr, "Name"), MyMySql.GetString(rdr, "Detail")));
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

        public MySqlResultState CreateNewPublisher(string name, string detail)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;
            // Check publisherName exist
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@publisherName", name);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Count_With_Name", paras);
            if (result.State != EMySqlResultState.OK)
            {
                return result;
            }

            parasLength = 4;
            paras = new MySqlParameter[parasLength];

            paras[0] = new MySqlParameter("@publisherName", name);
            paras[1] = new MySqlParameter("@detail", detail);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Insert", paras);

            return result;
        }

        public MySqlResultState DeletePublisher(int id)
        {

            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;
            // Check publisherName exist
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@publisherId", id);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Delete_From_Id", paras);

            return result;
        }

        public MySqlResultState DeletePublisher(string name)
        {

            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;
            // Check publisherName exist
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@publisherName", name);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Delete_From_Name", paras);

            return result;
        }

        public MySqlResultState UpdatePublisher(string name, string detail)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 4;
            // Check publisherName exist
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@publisherName", name);
            paras[1] = new MySqlParameter("@detail", detail);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Update", paras);

            return result;
        }

        /// <summary>
        /// Từ name lấy được id. Nếu name chưa có, ta thêm vào bảng
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetPublisherIdFromName(string name)
        {
            MySqlParameter[] paras = null;
            int parasLength = 3;

            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@publisherName", name);
            MyMySql.AddOutParameters(paras);
            return MyMySql.ExcuteGetIdStoreProceduce("st_tbPublisher_GetIdFromName", paras);
        }
    }
}