using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class CategoryMySql
    {
        public List<Category> GetListCategory()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<Category> ls = new List<Category>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCategory_Select_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                int idIndex = rdr.GetOrdinal("Id");
                int nameIndex = rdr.GetOrdinal("Name");
                int tikiIndex = rdr.GetOrdinal("TikiCategoryId");
                int shopeeIndex = rdr.GetOrdinal("ShopeeCategoryId");
                int lazadaIndex = rdr.GetOrdinal("LazadaCategoryId");
                while (rdr.Read())
                {
                    Category cate = new Category(rdr.GetInt32(idIndex),
                        rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex));
                    cate.tikiCategoryId = rdr.IsDBNull(tikiIndex) ? -1 : rdr.GetInt32(tikiIndex);
                    cate.shopeeCategoryId = rdr.IsDBNull(shopeeIndex) ? -1 : rdr.GetInt64(shopeeIndex);
                    cate.lazadaCategoryId = rdr.IsDBNull(lazadaIndex) ? -1 : rdr.GetInt64(lazadaIndex);
                    ls.Add(cate);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                ls.Clear();
            }

            conn.Close();
            return ls;
        }

        public Category GetCategory(int id, MySqlConnection conn)
        {
            Category category = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbCategory WHERE `Id` = @inId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inId", id);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        category = new Category(MyMySql.GetInt32(rdr, "Id"),
                            MyMySql.GetString(rdr, "Name"));
                        category.tikiCategoryId = MyMySql.GetInt32(rdr, "TikiCategoryId");
                        category.shopeeCategoryId = MyMySql.GetInt64(rdr, "ShopeeCategoryId");
                        category.lazadaCategoryId = MyMySql.GetInt64(rdr, "LazadaCategoryId");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                category = null;
            }

            return category;
        }

        public MySqlResultState CreateNewCategory(string name)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;
            paras = new MySqlParameter[parasLength];

            paras[0] = new MySqlParameter("@categoryName", name);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCategory_Insert", paras);

            return result;
        }

        public MySqlResultState DeleteCategory(int id)
        {

            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 3;
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@categoryId", id);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCategory_Delete_From_Id", paras);

            return result;
        }

        //public MySqlResultState DeleteCategory(string name)
        //{
        //    MySqlResultState result = null;
        //    MySqlParameter[] paras = null;

        //    int parasLength = 3;

        //    paras = new MySqlParameter[parasLength];
        //    paras[0] = new MySqlParameter("@categoryName", name);
        //    MyMySql.AddOutParameters(paras);

        //    result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCategory_Delete_From_Name", paras);

        //    return result;
        //}

        /// <summary>
        /// Từ name lấy được id. Nếu name chưa có, ta thêm vào bảng
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetCategoryIdFromName(string name)
        {
            MySqlParameter[] paras = null;
            int parasLength = 3;

            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@categoryName", name);
            MyMySql.AddOutParameters(paras);
            return MyMySql.ExcuteGetIdStoreProceduce("st_tbCategory_GetIdFromName", paras);
        }

        public MySqlResultState UpdateCategory(int id, string name)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 4;
            // Check publisherName exist
            paras = new MySqlParameter[parasLength];
            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inCategoryName", name);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCategory_Update", paras);

            return result;
        }
    }
}