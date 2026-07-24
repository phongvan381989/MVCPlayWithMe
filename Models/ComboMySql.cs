using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.Models.ProductModel;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models
{
    public class ComboMySql
    {
        // Giữ sync vì dùng bởi BasicController.ViewDataGetListCombo()
        //public List<Combo> GetListCombo()
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    List<Combo> ls = new List<Combo>();
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbCombo_Select_All", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        int idIndex = rdr.GetOrdinal("Id");
        //        int nameIndex = rdr.GetOrdinal("Name");
        //        int codeIndex = rdr.GetOrdinal("Code");

        //        while (rdr.Read())
        //        {
        //            ls.Add(new Combo(rdr.GetInt32(idIndex),
        //                rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex),
        //                rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex)));
        //        }

        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //    }

        //    conn.Close();
        //    return ls;
        //}

        // Lấy danh sách id sản phẩm đang kinh doanh thuộc combo
        public async Task<List<int>> GetProductIdsOfComboAsync(int comboId)
        {
            List<int> productIds = new List<int>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(
                    "SELECT Id FROM tbproducts WHERE ComboId = @in_ComboId AND Status = 0;", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@in_ComboId", comboId);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            while (await rdr.ReadAsync())
                            {
                                productIds.Add(rdr.GetInt32(idIndex));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                productIds.Clear();
            }

            return productIds;
        }

        public async Task<List<Combo>> GetListComboAsync()
        {
            List<Combo> ls = new List<Combo>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCombo_Select_All", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = rdr.GetOrdinal("Id");
                        int nameIndex = rdr.GetOrdinal("Name");
                        int codeIndex = rdr.GetOrdinal("Code");
                        while (await rdr.ReadAsync())
                        {
                            ls.Add(new Combo(rdr.GetInt32(idIndex),
                                rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex),
                                rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return ls;
        }

        public async Task<List<Combo>> GetListComboConnectOutAsync(MySqlConnection conn)
        {
            List<Combo> ls = new List<Combo>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbCombo_Select_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    int idIndex = rdr.GetOrdinal("Id");
                    int nameIndex = rdr.GetOrdinal("Name");
                    int codeIndex = rdr.GetOrdinal("Code");
                    while (await rdr.ReadAsync())
                    {
                        ls.Add(new Combo(rdr.GetInt32(idIndex),
                            rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex),
                            rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex)));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return ls;
        }

        public async Task<List<Combo>> GetListComboIncludeSimpleProductsAsync(MySqlConnection conn)
        {
            List<Combo> ls = new List<Combo>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbCombo_Select_All_Include_Simple_Product", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    int idIndex = rdr.GetOrdinal("Id");
                    int nameIndex = rdr.GetOrdinal("Name");
                    int productIdIndex = rdr.GetOrdinal("ProductId");
                    int productNameIndex = rdr.GetOrdinal("ProductName");

                    Combo combo = null;
                    int comboIdTemp = 0;
                    while (await rdr.ReadAsync())
                    {
                        int comboId = rdr.GetInt32(idIndex);
                        if (comboId != comboIdTemp)
                        {
                            comboIdTemp = comboId;
                            string comboName = rdr.GetString(nameIndex);
                            if (comboName.TrimStart().StartsWith("combo", StringComparison.OrdinalIgnoreCase))
                                comboName = comboName.TrimStart().Substring(5).Trim();
                            else
                                comboName = comboName.Trim();
                            combo = new Combo(comboId, comboName);
                            ls.Add(combo);
                        }
                        combo.products.Add(new Product(rdr.GetInt32(productIdIndex), rdr.GetString(productNameIndex)));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return ls;
        }

        public async Task<Combo> GetComboAsync(int id)
        {
            Combo combo = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCombo_Select_From_Id", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inId", id);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            if (combo == null)
                            {
                                combo = new Combo(MyMySql.GetInt32(rdr, "TBComboId"),
                                    MyMySql.GetString(rdr, "TBComboName"),
                                    MyMySql.GetString(rdr, "TBComboCode"));
                            }
                            int proIdTem = MyMySql.GetInt32(rdr, "Id");
                            if (proIdTem != -1)
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
                                product.language = MyMySql.GetString(rdr, "Language");
                                product.SetFirstSrcImage();
                                combo.products.Add(product);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    combo = null;
                }
            }
            return combo;
        }

        public async Task<MySqlResultState> CreateNewComboAsync(string name, string code)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@comboName", name);
            paras[1] = new MySqlParameter("@comboCode", code);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCombo_Insert", paras);
        }

        public async Task<MySqlResultState> DeleteComboAsync(int id)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@comboId", id);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCombo_Delete_From_Id", paras);
        }

        public async Task<MySqlResultState> UpdateComboAsync(int id, string name, string code)
        {
            MySqlParameter[] paras = new MySqlParameter[5];
            paras[0] = new MySqlParameter("@comboId", id);
            paras[1] = new MySqlParameter("@comboName", name);
            paras[2] = new MySqlParameter("@comboCode", code);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbCombo_Update", paras);
        }

        public async Task<int> GetComboIdFromNameAsync(string name)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@comboName", name);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteGetIdStoreProcedureAsync("st_tbCombo_GetComboIdFromName", paras);
        }
    }
}
