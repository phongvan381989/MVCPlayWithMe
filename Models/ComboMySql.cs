using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using MVCPlayWithMe.Models.ProductModel;
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
                int idIndex = rdr.GetOrdinal("Id");
                int nameIndex = rdr.GetOrdinal("Name");
                int codeIndex = rdr.GetOrdinal("Code");

                while (rdr.Read())
                {
                    ls.Add(new Combo(rdr.GetInt32(idIndex),
                        rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex),
                        rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex)));
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return ls;
        }

        public Combo GetCombo(int id)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Combo combo = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCombo_Select_From_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                int proIdTem = 0;
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (combo == null)
                    {
                        combo = new Combo(MyMySql.GetInt32(rdr, "TBComboId"),
                            MyMySql.GetString(rdr, "TBComboName"),
                            MyMySql.GetString(rdr, "TBComboCode"));
                    }

                    proIdTem = MyMySql.GetInt32(rdr, "Id");
                    if(proIdTem != -1)
                    {
                        Product product = new Product();
                        product.id = proIdTem;

                        product.name = MyMySql.GetString(rdr, "Name");
                        product.categoryId = MyMySql.GetInt32(rdr, "CategoryId");
                        product.categoryName = MyMySql.GetString(rdr, "CategoryName");
                        product.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
                        product.author = MyMySql.GetString(rdr, "Author");
                        product.translator = MyMySql.GetString(rdr, "Translator");
                        product.publisherId = MyMySql.GetInt32(rdr, "PublisherId");
                        product.publisherName = MyMySql.GetString(rdr, "PublisherName");
                        product.publishingCompany = MyMySql.GetString(rdr, "PublishingCompany");
                        product.publishingTime = MyMySql.GetInt32(rdr, "PublishingTime");
                        product.productLong = MyMySql.GetInt32(rdr, "ProductLong");
                        product.productWide = MyMySql.GetInt32(rdr, "ProductWide");
                        product.productHigh = MyMySql.GetInt32(rdr, "ProductHigh");
                        product.productWeight = MyMySql.GetInt32(rdr, "ProductWeight");
                        product.positionInWarehouse = MyMySql.GetString(rdr, "PositionInWarehouse");
                        product.hardCover = MyMySql.GetInt32(rdr, "HardCover");
                        product.minAge = MyMySql.GetInt32(rdr, "MinAge");
                        product.maxAge = MyMySql.GetInt32(rdr, "MaxAge");
                        product.parentId = MyMySql.GetInt32(rdr, "ParentId");
                        product.republish = MyMySql.GetInt32(rdr, "Republish");

                        product.status = MyMySql.GetInt32(rdr, "Status");
                        product.quantity = MyMySql.GetInt32(rdr, "Quantity");
                        product.pageNumber = MyMySql.GetInt32(rdr, "PageNumber");
                        product.discount = rdr.IsDBNull(rdr.GetOrdinal("Discount")) ? 0 : rdr.GetFloat("Discount");

                        product.SetFirstSrcImage();
                        combo.products.Add(product);
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                combo = null;
            }

            conn.Close();
            return combo;
        }

        public MySqlResultState CreateNewCombo(string name, string code)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 4;
            paras = new MySqlParameter[parasLength];

            paras[0] = new MySqlParameter("@comboName", name);
            paras[1] = new MySqlParameter("@comboCode", code);
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

        //public MySqlResultState DeleteCombo(string name)
        //{

        //    MySqlResultState result = null;
        //    MySqlParameter[] paras = null;

        //    int parasLength = 3;

        //    paras = new MySqlParameter[parasLength];
        //    paras[0] = new MySqlParameter("@comboName", name);
        //    MyMySql.AddOutParameters(paras);

        //    result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCombo_Delete_From_Name", paras);

        //    return result;
        //}

        public MySqlResultState UpdateCombo(int id, string name, string code)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            int parasLength = 5;
            paras = new MySqlParameter[parasLength];

            paras[0] = new MySqlParameter("@comboId", id);
            paras[1] = new MySqlParameter("@comboName", name);
            paras[2] = new MySqlParameter("@comboCode", code);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbCombo_Update", paras);

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

        // Kết nối đóng mở bên ngoài
        // Lấy được Shopee Item mapping với sản phẩm trong kho thuộc 1 combo
        public List<CommonItem> ShopeeGetListMappingOfCombo(int comboId, MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_Mapping_Combo_Id", conn);
                cmd.Parameters.AddWithValue("@inComboId", comboId);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader rdr = cmd.ExecuteReader();
                ProductMySql productMySql = new ProductMySql();
                while (rdr.Read())
                {
                    productMySql.ShopeeReadCommonItem(listCI, rdr);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                listCI.Clear();
            }
            return listCI;
        }

        // Kết nối đóng mở bên ngoài
        // Lấy được Tiki Item mapping với sản phẩm trong kho thuộc 1 combo
        public List<CommonItem> TikiGetListMappingOfCombo(int comboId, MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_From_Mapping_Combo_Id", conn);
                cmd.Parameters.AddWithValue("@inComboId", comboId);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader rdr = cmd.ExecuteReader();
                ProductMySql productMySql = new ProductMySql();
                while (rdr.Read())
                {
                    productMySql.TikiReadCommonItem(listCI, rdr);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                listCI.Clear();
            }
            return listCI;
        }
    }
}