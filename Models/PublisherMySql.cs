using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models
{
    public class PublisherMySql
    {
        //public List<Publisher> GetListPublisherConnectOut(MySqlConnection conn)
        //{
        //    List<Publisher> ls = new List<Publisher>();
        //    try
        //    {
        //        MySqlCommand cmd = new MySqlCommand("st_tbPublisher_Select_All", conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        using (MySqlDataReader rdr = cmd.ExecuteReader())
        //        {
        //            int idIndex = rdr.GetOrdinal("Id");
        //            int nameIndex = rdr.GetOrdinal("Name");
        //            int discountIndex = rdr.GetOrdinal("Discount");
        //            int detailIndex = rdr.GetOrdinal("Detail");
        //            while (rdr.Read())
        //            {
        //                ls.Add(new Publisher(rdr.GetInt32(idIndex),
        //                    rdr.GetString(nameIndex),
        //                    rdr.GetFloat(discountIndex),
        //                    rdr.IsDBNull(detailIndex) ? string.Empty : rdr.GetString(detailIndex)));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        ls.Clear();
        //    }
        //    return ls;
        //}

        //public Publisher GetPublisher(int id)
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    Publisher publisher = null;
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbpublisher WHERE `Id` = @inId", conn);
        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@inId", id);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            publisher = new Publisher(MyMySql.GetInt32(rdr, "Id"),
        //                MyMySql.GetString(rdr, "Name"),
        //                rdr.GetFloat("Discount"),
        //                MyMySql.GetString(rdr, "Detail"));

        //            publisher.tikiCertificate = MyMySql.GetString(rdr, "TikiCertificate");
        //            publisher.tikiAttributeValue = MyMySql.GetString(rdr, "TikiAttributeValue");
        //        }
        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        publisher = null;
        //    }

        //    conn.Close();
        //    return publisher;
        //}

        public async Task<List<Publisher>> GetListPublisherConnectOutAsync(MySqlConnection conn)
        {
            List<Publisher> ls = new List<Publisher>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbPublisher_Select_All", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    int idIndex = rdr.GetOrdinal("Id");
                    int nameIndex = rdr.GetOrdinal("Name");
                    int discountIndex = rdr.GetOrdinal("Discount");
                    int detailIndex = rdr.GetOrdinal("Detail");
                    while (await rdr.ReadAsync())
                    {
                        ls.Add(new Publisher(rdr.GetInt32(idIndex),
                            rdr.GetString(nameIndex),
                            rdr.GetFloat(discountIndex),
                            rdr.IsDBNull(detailIndex) ? string.Empty : rdr.GetString(detailIndex)));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                ls.Clear();
            }
            return ls;
        }

        public async Task<Publisher> GetPublisherAsync(int id)
        {
            Publisher publisher = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbpublisher WHERE `Id` = @inId", conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inId", id);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            publisher = new Publisher(MyMySql.GetInt32(rdr, "Id"),
                                MyMySql.GetString(rdr, "Name"),
                                rdr.GetFloat("Discount"),
                                MyMySql.GetString(rdr, "Detail"));
                            publisher.tikiCertificate = MyMySql.GetString(rdr, "TikiCertificate");
                            publisher.tikiAttributeValue = MyMySql.GetString(rdr, "TikiAttributeValue");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    publisher = null;
                }
            }
            return publisher;
        }

        public async Task<MySqlResultState> CreateNewPublisherAsync(string name, float discount, string detail)
        {
            MySqlParameter[] paras = new MySqlParameter[5];
            paras[0] = new MySqlParameter("@publisherName", name);
            paras[1] = new MySqlParameter("@discount", discount);
            paras[2] = new MySqlParameter("@detail", detail);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbPublisher_Insert", paras);
        }

        public async Task<MySqlResultState> DeletePublisherAsync(int id)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inPublisherId", id);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbPublisher_Delete_From_Id", paras);
        }

        public async Task<MySqlResultState> UpdatePublisherAsync(int id, string name, float discount, string detail)
        {
            MySqlParameter[] paras = new MySqlParameter[6];
            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inPublisherName", name);
            paras[2] = new MySqlParameter("@inDiscount", discount);
            paras[3] = new MySqlParameter("@inDetail", detail);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbPublisher_Update", paras);
        }

        public async Task<int> GetPublisherIdFromNameAsync(string name)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@publisherName", name);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteGetIdStoreProcedureAsync("st_tbPublisher_GetIdFromName", paras);
        }
    }
}
