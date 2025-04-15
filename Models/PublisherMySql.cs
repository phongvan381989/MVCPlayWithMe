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
        public List<Publisher> GetListPublisherConnectOut(MySqlConnection conn)
        {
            List<Publisher> ls = new List<Publisher>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbPublisher_Select_All", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                MySqlDataReader rdr = cmd.ExecuteReader();
                int idIndex = rdr.GetOrdinal("Id");
                int nameIndex = rdr.GetOrdinal("Name");
                int discountIndex = rdr.GetOrdinal("Discount");
                int detailIndex = rdr.GetOrdinal("Detail");
                while (rdr.Read())
                {
                    ls.Add(new Publisher(rdr.GetInt32(idIndex),
                        rdr.GetString(nameIndex),
                        rdr.GetInt32(discountIndex),
                        rdr.IsDBNull(detailIndex) ? string.Empty : rdr.GetString(detailIndex)));
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                ls.Clear();
            }
            return ls;
        }

        public MySqlResultState CreateNewPublisher(string name, float discount, string detail)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            //int parasLength = 3;
            //// Check publisherName exist
            //paras = new MySqlParameter[parasLength];
            //paras[0] = new MySqlParameter("@publisherName", name);
            //MyMySql.AddOutParameters(paras);

            //result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Count_With_Name", paras);
            //if (result.State != EMySqlResultState.OK)
            //{
            //    return result;
            //}

            int parasLength = 5;
            paras = new MySqlParameter[parasLength];

            paras[0] = new MySqlParameter("@publisherName", name);
            paras[1] = new MySqlParameter("@discount", discount);
            paras[2] = new MySqlParameter("@detail", detail);
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
            paras[0] = new MySqlParameter("@inPublisherId", id);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Delete_From_Id", paras);

            return result;
        }

        //public MySqlResultState DeletePublisher(string name)
        //{
        //    MySqlResultState result = null;
        //    MySqlParameter[] paras = null;

        //    int parasLength = 3;
        //    // Check publisherName exist
        //    paras = new MySqlParameter[parasLength];
        //    paras[0] = new MySqlParameter("@publisherName", name);
        //    MyMySql.AddOutParameters(paras);

        //    result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbPublisher_Delete_From_Name", paras);

        //    return result;
        //}

        public MySqlResultState UpdatePublisher(int id, string name, float discount, string detail)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 6;
            // Check publisherName exist
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inPublisherName", name);
            paras[2] = new MySqlParameter("@inDiscount", discount);
            paras[3] = new MySqlParameter("@inDetail", detail);
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

        public Publisher GetPublisher(int id)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Publisher publisher = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbpublisher WHERE `Id` = @inId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    publisher = new Publisher(MyMySql.GetInt32(rdr, "Id"),
                        MyMySql.GetString(rdr, "Name"),
                        rdr.GetFloat("Discount"),
                        MyMySql.GetString(rdr, "Detail")
                        );
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                publisher = null;
            }

            conn.Close();
            return publisher;
        }
    }
}