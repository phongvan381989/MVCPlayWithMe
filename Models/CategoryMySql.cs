using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class CategoryMySql : BasicMySql
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
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(new Category(MyMySql.GetInt32(rdr, "Id"), MyMySql.GetString(rdr, "Name")));
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                ls.Clear();
            }

            conn.Close();
            return ls;
        }

        public Category GetCategory(int id)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Category category = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbCategory WHERE `Id` = @inId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    category = new Category(MyMySql.GetInt32(rdr, "Id"),
                        MyMySql.GetString(rdr, "Name"));
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                category = null;
            }

            conn.Close();
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