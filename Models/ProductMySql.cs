using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Models
{
    public class ProductMySql : BasicMySql
    {
        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product
        /// Dùng trong trường hợp câu select chỉ trả về 1 row
        /// </summary>
        /// <param name="rdr">Trả về ngay từ câu select</param>
        /// <returns></returns>
        private Product ConvertOneRowFromDataMySql(MySqlDataReader rdr)
        {
            Product product = new Product();
            product.id = MyMySql.GetInt32(rdr, "Id");
            product.code = MyMySql.GetString(rdr, "Code");
            product.barcode = MyMySql.GetString(rdr, "Barcode");
            product.name = MyMySql.GetString(rdr, "Name");
            product.comboId = MyMySql.GetInt32(rdr, "ComboId");
            product.comboName = MyMySql.GetString(rdr, "ComboName");
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
            product.parentName = MyMySql.GetString(rdr, "ParentName");
            product.republish = MyMySql.GetInt32(rdr, "Republish");
            product.detail = MyMySql.GetString(rdr, "Detail");
            product.status = MyMySql.GetInt32(rdr, "Status");
            product.quantity = MyMySql.GetInt32(rdr, "Quantity");
            product.SetSrcImageVideo();

            return product;
        }

        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product với 1 vài thuộc tính cần thiết
        /// Dùng trong trường hợp câu select chỉ trả về 1 row
        /// </summary>
        /// <param name="rdr">Trả về ngay từ câu select</param>
        /// <returns></returns>
        private Product ConvertQuicklyOneRowFromDataMySql(MySqlDataReader rdr)
        {
            Product product = new Product();
            product.id = MyMySql.GetInt32(rdr, "Id");
            product.code = MyMySql.GetString(rdr, "Code");
            product.barcode = MyMySql.GetString(rdr, "Barcode");
            product.name = MyMySql.GetString(rdr, "Name");
            product.quantity = MyMySql.GetInt32(rdr, "Quantity");
            product.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
            product.status = MyMySql.GetInt32(rdr, "Status");
            product.comboName = MyMySql.GetString(rdr, "ComboName");
            product.SetSrcImageVideo();

            return product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns>Có thể trả về null</returns>
        public Product GetProductFromBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return null;

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Product product = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Barcode", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inBarcode", barcode);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    product = ConvertOneRowFromDataMySql(rdr);
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                product = null;
            }

            conn.Close();
            return product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Có thể trả về null</returns>
        public Product GetProductFromCode(string code)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Product product = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Code", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCode", code);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    product = ConvertOneRowFromDataMySql(rdr);
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                product = null;
            }

            conn.Close();
            return product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Có thể trả về null</returns>
        public Product GetProductFromId(int id)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Product product = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    product = ConvertOneRowFromDataMySql(rdr);
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                product = null;
            }

            conn.Close();
            return product;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"> combo id</param>
        ///// <returns></returns>
        //public Product GetProductFromFirstComboId(int id)
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    Product product = null;
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_First_Product_From_Combo_Id", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inId", id);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            product = ConvertOneRowFromDataMySql(rdr);
        //        }
        //        if (rdr != null)
        //            rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        errMessage = ex.ToString();
        //        MyLogger.GetInstance().Warn(errMessage);
        //        product = null;
        //    }

        //    conn.Close();
        //    return product;
        //}

        public MySqlResultState UpdateProductBarcode(int id, string newBarcode)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", newBarcode);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Barcode", paras);
            return result;
        }

        //private void AddUpdateParameters(Product pro, MySqlParameter[] paras)
        //{
        //    paras[0] = new MySqlParameter("@inproductId", pro.id);
        //    paras[1]  = new MySqlParameter("@proCode", pro.code);
        //    paras[2]  = new MySqlParameter("@barcode", pro.barcode);
        //    paras[3]  = new MySqlParameter("@productName", pro.name);
        //    paras[4]  = new MySqlParameter("@comboId", pro.comboId);
        //    paras[5]  = new MySqlParameter("@categoryId", pro.categoryId);
        //    paras[6]  = new MySqlParameter("@bookCoverPrice", pro.bookCoverPrice);
        //    paras[7]  = new MySqlParameter("@author", pro.author);
        //    paras[8]  = new MySqlParameter("@translator", pro.translator);
        //    paras[9]  = new MySqlParameter("@publisherId", pro.publisherId);
        //    paras[10]  = new MySqlParameter("@publishingCompany", pro.publishingCompany);
        //    paras[11] = new MySqlParameter("@publishingTime", pro.publishingTime);
        //    paras[12] = new MySqlParameter("@productLong", pro.productLong);
        //    paras[13] = new MySqlParameter("@productWide", pro.productWide);
        //    paras[14] = new MySqlParameter("@productHigh", pro.productHigh);
        //    paras[15] = new MySqlParameter("@productWeight", pro.productWeight);
        //    paras[16] = new MySqlParameter("@positionInWarehouse", pro.positionInWarehouse);
        //    paras[17] = new MySqlParameter("@hardCover", pro.hardCover);
        //    paras[18] = new MySqlParameter("@minAge", pro.minAge);
        //    paras[19] = new MySqlParameter("@maxAge", pro.maxAge);
        //    paras[20] = new MySqlParameter("@parentId", pro.parentId);
        //    paras[21] = new MySqlParameter("@republish", pro.republish);
        //    paras[22] = new MySqlParameter("@detail", pro.detail);
        //    paras[23] = new MySqlParameter("@proStatus", pro.status);

        //    MyMySql.AddOutParameters(paras);
        //}

        // Thêm mới sản phẩm và trả về id sản phẩm thêm mới như outResult
        public MySqlResultState AddNewPro(
            Product pro
        )
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[23];
            paras[0] = new MySqlParameter("@inproCode", pro.code);
            paras[1] = new MySqlParameter("@inbarcode", pro.barcode);
            paras[2] = new MySqlParameter("@inproductName", pro.name);
            paras[3] = new MySqlParameter("@incomboId", pro.comboId);
            paras[4] = new MySqlParameter("@incategoryId", pro.categoryId);
            paras[5] = new MySqlParameter("@inbookCoverPrice", pro.bookCoverPrice);
            paras[6] = new MySqlParameter("@inauthor", pro.author);
            paras[7] = new MySqlParameter("@intranslator", pro.translator);
            paras[8] = new MySqlParameter("@inpublisherId", pro.publisherId);
            paras[9] = new MySqlParameter("@inpublishingCompany", pro.publishingCompany);
            paras[10] = new MySqlParameter("@inpublishingTime", pro.publishingTime);
            paras[11] = new MySqlParameter("@inproductLong", pro.productLong);
            paras[12] = new MySqlParameter("@inproductWide", pro.productWide);
            paras[13] = new MySqlParameter("@inproductHigh", pro.productHigh);
            paras[14] = new MySqlParameter("@inproductWeight", pro.productWeight);
            paras[15] = new MySqlParameter("@inpositionInWarehouse", pro.positionInWarehouse);
            paras[16] = new MySqlParameter("@inhardCover", pro.hardCover);
            paras[17] = new MySqlParameter("@inminAge", pro.minAge);
            paras[18] = new MySqlParameter("@inmaxAge", pro.maxAge);
            paras[19] = new MySqlParameter("@inparentId", pro.parentId);
            paras[20] = new MySqlParameter("@inrepublish", pro.republish);
            paras[21] = new MySqlParameter("@indetail", pro.detail);
            paras[22] = new MySqlParameter("@inproStatus", pro.status);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        result.myAnything = MyMySql.GetInt32(rdr, "LastId");
                        if(result.myAnything == -2)
                        {
                            result.State = EMySqlResultState.EXIST;
                            result.Message = "Code is exist";
                        }
                        else if (result.myAnything == -3)
                        {
                            result.State = EMySqlResultState.EXIST;
                            result.Message = "Barcode is exist";
                        }
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();

            return result;
        }

        //
        public MySqlResultState DeleteProduct(int id)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[3];

            paras[0] = new MySqlParameter("@inId", id);

            MyMySql.AddOutParameters(paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Delete_Product_From_Id", paras);

            return result;
        }

        /// <summary>
        /// id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MySqlResultState UpdateCommonInfoWithCombo(ProductCommonInfoWithCombo pro)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[20];
            paras[0] = new MySqlParameter("@inComboId", pro.comboId);
            paras[1] = new MySqlParameter("@inCategoryId", pro.categoryId);
            paras[2] = new MySqlParameter("@inBookCoverPrice", pro.bookCoverPrice);
            paras[3] = new MySqlParameter("@inAuthor", pro.author);
            paras[4] = new MySqlParameter("@inTranslator", pro.translator);
            paras[5] = new MySqlParameter("@inPublisherId", pro.publisherId);
            paras[6] = new MySqlParameter("@inPublishingCompany", pro.publishingCompany);
            paras[7] = new MySqlParameter("@inPublishingTime", pro.publishingTime);
            paras[8] = new MySqlParameter("@inProductLong", pro.productLong);
            paras[9] = new MySqlParameter("@inProductWide", pro.productWide);
            paras[10] = new MySqlParameter("@inProductHigh", pro.productHigh);
            paras[11] = new MySqlParameter("@inProductWeight", pro.productWeight);
            paras[12] = new MySqlParameter("@inPositionInWarehouse", pro.positionInWarehouse);
            paras[13] = new MySqlParameter("@inHardCover", pro.hardCover);
            paras[14] = new MySqlParameter("@inMinAge", pro.minAge);
            paras[15] = new MySqlParameter("@inMaxAge", pro.maxAge);
            paras[16] = new MySqlParameter("@inRepublish", pro.republish);
            paras[17] = new MySqlParameter("@inProStatus", pro.status);

            MyMySql.AddOutParameters(paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Common_Info_With_Combo", paras);

            return result;
        }

        public MySqlResultState UpdateProduct(
            Product pro
        )
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = new MySqlParameter[24];
            paras[0] = new MySqlParameter("@inproductId", pro.id);
            paras[1] = new MySqlParameter("@inproCode", pro.code);
            paras[2] = new MySqlParameter("@inbarcode", pro.barcode);
            paras[3] = new MySqlParameter("@inproductName", pro.name);
            paras[4] = new MySqlParameter("@incomboId", pro.comboId);
            paras[5] = new MySqlParameter("@incategoryId", pro.categoryId);
            paras[6] = new MySqlParameter("@inbookCoverPrice", pro.bookCoverPrice);
            paras[7] = new MySqlParameter("@inauthor", pro.author);
            paras[8] = new MySqlParameter("@intranslator", pro.translator);
            paras[9] = new MySqlParameter("@inpublisherId", pro.publisherId);
            paras[10] = new MySqlParameter("@inpublishingCompany", pro.publishingCompany);
            paras[11] = new MySqlParameter("@inpublishingTime", pro.publishingTime);
            paras[12] = new MySqlParameter("@inproductLong", pro.productLong);
            paras[13] = new MySqlParameter("@inproductWide", pro.productWide);
            paras[14] = new MySqlParameter("@inproductHigh", pro.productHigh);
            paras[15] = new MySqlParameter("@inproductWeight", pro.productWeight);
            paras[16] = new MySqlParameter("@inpositionInWarehouse", pro.positionInWarehouse);
            paras[17] = new MySqlParameter("@inhardCover", pro.hardCover);
            paras[18] = new MySqlParameter("@inminAge", pro.minAge);
            paras[19] = new MySqlParameter("@inmaxAge", pro.maxAge);
            paras[20] = new MySqlParameter("@inparentId", pro.parentId);
            paras[21] = new MySqlParameter("@inrepublish", pro.republish);
            paras[22] = new MySqlParameter("@indetail", pro.detail);
            paras[23] = new MySqlParameter("@inproStatus", pro.status);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        result.myAnything = MyMySql.GetInt32(rdr, "LastId");
                        if (result.myAnything == -2)
                        {
                            result.State = EMySqlResultState.EXIST;
                            result.Message = "Code is exist";
                        }
                        else if (result.myAnything == -3)
                        {
                            result.State = EMySqlResultState.EXIST;
                            result.Message = "Barcode is exist";
                        }
                    }
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();

            return result;
        }

        public List<ProductIdName> GetListProductName()
        {
            List<ProductIdName> ls = new List<ProductIdName>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Name", conn);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productName"></param>
        /// <returns> -1 nếu tên sản phẩm không tồn tại hoặc có lỗi </returns>
        public int GetProductIdFromName(string productName)
        {
            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_With_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productName", productName);

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

        public List<string> GetListBarcode()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Barcode", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(MyMySql.GetString(rdr, "Barcode"));
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

        /// <summary>
        /// Lấy tất cả comboname trong db
        /// Comboname phải khác trống
        /// </summary>
        /// <returns></returns>
        public List<string> GetListComboName()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_ComboName", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(MyMySql.GetString(rdr, "ComboName"));
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

        /// <summary>
        /// Lấy tất cả tác giả
        /// Tác giả phải khác trống
        /// </summary>
        /// <returns></returns>
        public List<string> GetListAuthor()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Author", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(MyMySql.GetString(rdr, "Author"));
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

        /// <summary>
        /// Lấy tất cả người dịch
        /// Người dịch phải khác trống
        /// </summary>
        /// <returns></returns>
        public List<string> GetListTranslator()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Translator", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(MyMySql.GetString(rdr, "Translator"));
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

        /// <summary>
        /// Lấy tất cả nhà xuất bản
        /// Nhà xuất bản phải khác trống
        /// </summary>
        /// <returns></returns>
        public List<string> GetListPublishingCompany()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<string> ls = new List<string>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_PublishingCompany", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(MyMySql.GetString(rdr, "PublishingCompany"));
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inType"></param>
        /// inType: 1: ProductLong, 
        /// inType: 2: ProductWide, 
        /// inType: 3: ProductHigh, 
        /// inType: 4: ProductWeight,
        /// inType: 5: MinAge, 
        /// inType: 6: MaxAge, 
        /// inType: 7: PublishingTime, 
        /// <returns></returns>
        public List<int> GetListDifferenceIntValue(int inType)
        {
            List<int> ls = new List<int>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Difference_INT_Value", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inType", inType);

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ls.Add(MyMySql.GetInt32(rdr, "difValue"));
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

        public List<Product> GetProductIdCodeBarcodeNameBookCoverPrice(string publisher)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<Product> ls = new List<Product>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Id_Code_Barcode_Name_BookCoverPrice_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisher", publisher);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Product product = new Product();
                    product.id = MyMySql.GetInt32(rdr, "Id");
                    product.code = MyMySql.GetString(rdr, "Code");
                    product.barcode = MyMySql.GetString(rdr, "Barcode");
                    product.name = MyMySql.GetString(rdr, "Name");
                    product.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
                    product.SetSrcImageVideo();
                    ls.Add(product);
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

        public MySqlResultState AddListImport(List<Import> ls)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;
            Boolean isOK = true;
            paras = new MySqlParameter[7];
            foreach (var obj in ls)
            {
                paras[0] = new MySqlParameter("@inProductId", obj.productId);
                paras[1] = new MySqlParameter("@inPrice", obj.price);
                paras[2] = new MySqlParameter("@inQuantity", obj.quantity);
                paras[3] = new MySqlParameter("@inBookCoverPrice", obj.bookCoverPrice);
                paras[4] = new MySqlParameter("@inDiscount", obj.discount);
                MyMySql.AddOutParameters(paras);
                result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbImport_Insert", paras);
                if (result.State != EMySqlResultState.OK) // Có lỗi vẫn thực hiện tiếp check lại thủ công sau
                    isOK = false;
            }
            if (!isOK)
                result.State = EMySqlResultState.ERROR;
            else
                result.State = EMySqlResultState.OK;
            return result;
        }

        public List<Import> GetImportList(string fromDate, string toDate, string publisher)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<Import> ls = new List<Import>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbImport_Select", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inFromDate", fromDate);
                cmd.Parameters.AddWithValue("@inToDate", toDate);
                cmd.Parameters.AddWithValue("@inPublisher", publisher);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Import imp = new Import();
                    imp.id = MyMySql.GetInt32(rdr, "Id");
                    imp.productId = MyMySql.GetInt32(rdr, "ProductId");
                    imp.productName = MyMySql.GetString(rdr, "Name");
                    imp.price = MyMySql.GetInt32(rdr, "Price");
                    imp.quantity = MyMySql.GetInt32(rdr, "Quantity");
                    imp.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
                    imp.dateImport = MyMySql.GetString(rdr, "Date");
                    imp.discount = MyMySql.GetInt32(rdr, "Discount");

                    ls.Add(imp);
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

        public MySqlResultState UpdateListImport(List<Import> ls)
        {
            MySqlResultState result = new MySqlResultState();
            EMySqlResultState rsTemp;
            MySqlParameter[] paras = null;
            int lengthPara = 5;
            paras = new MySqlParameter[lengthPara];
            paras[0] = new MySqlParameter("@inId", 0);
            paras[1] = new MySqlParameter("@inPrice", 0);
            paras[2] = new MySqlParameter("@inDiscount", 0);
            MyMySql.AddOutParameters(paras);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbImport_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                foreach (var obj in ls)
                {
                    paras[0].Value = obj.id;
                    paras[1].Value = obj.price;
                    paras[2].Value = obj.discount;
                    cmd.ExecuteNonQuery();
                    rsTemp = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    if (rsTemp != EMySqlResultState.OK) // Có lỗi vẫn thực hiện tiếp check lại thủ công sau
                    {
                        result.State = rsTemp;
                    }
                }

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();

            return result;
        }

        // Tìm kiếm không phân trang
        public List<Product> SearchProduct(ProductSearchParameter searchParameter)
        {
            List<Product> ls = new List<Product>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_Product", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisher", searchParameter.publisher);
                cmd.Parameters.AddWithValue("@inCodeOrBarcode", searchParameter.codeOrBarcode);
                cmd.Parameters.AddWithValue("@inName", searchParameter.name);
                cmd.Parameters.AddWithValue("@inCombo", searchParameter.combo);
                //cmd.Parameters.AddWithValue("@inStatus", searchParameter.status);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ls.Add(ConvertQuicklyOneRowFromDataMySql(rdr));
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

        //// Đếm số record kết quả trả về, phục vụ phân trang
        //public int SearchProductCount(ProductSearchParameter searchParameter)
        //{
        //    int count = 0;
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_Count_Record", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inPublisher", searchParameter.publisher);
        //        cmd.Parameters.AddWithValue("@inCodeOrBarcode", searchParameter.codeOrBarcode);
        //        cmd.Parameters.AddWithValue("@inName", searchParameter.name);
        //        cmd.Parameters.AddWithValue("@inCombo", searchParameter.combo);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            count = MyMySql.GetInt32(rdr, "CountRecord");
        //        }

        //        if (rdr != null)
        //            rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        errMessage = ex.ToString();
        //        MyLogger.GetInstance().Warn(errMessage);
        //    }

        //    conn.Close();
        //    return count;
        //}

        //Tìm kiếm có phân trang
        public List<Product> SearchProductChangePage(ProductSearchParameter searchParameter)
        {
            List<Product> ls = new List<Product>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisher", searchParameter.publisher);
                cmd.Parameters.AddWithValue("@inCodeOrBarcode", searchParameter.codeOrBarcode);
                cmd.Parameters.AddWithValue("@inName", searchParameter.name);
                cmd.Parameters.AddWithValue("@inCombo", searchParameter.combo);
                cmd.Parameters.AddWithValue("@inStart", searchParameter.start);
                cmd.Parameters.AddWithValue("@inOffset", searchParameter.offset);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ls.Add(ConvertQuicklyOneRowFromDataMySql(rdr));
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

        // Cập nhật chỉ tên sản phẩm, có check tên đã tồn tại trong store
        public MySqlResultState UpdateName(int id, string name)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inProductName", name);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Name", paras);
            return result;
        }

        // Cập nhật chỉ code, có check tên đã tồn tại trong store
        public MySqlResultState UpdateCode(int id, string code)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inCode", code);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Code", paras);
            return result;
        }

        // Cập nhật chỉ code, có check tên đã tồn tại trong store
        // isbn chính là Barcode trong db
        public MySqlResultState UpdateISBN(int id, string isbn)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", isbn);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Barcode", paras);
            return result;
        }

        public MySqlResultState UpdateBookCoverPrice(int id, int bookCoverPrice)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_BookeCoverPrice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inBookCoverPrice", bookCoverPrice);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }
        
        public MySqlResultState UpdateComboId(int id, int comboId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_ComboId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inComboId", comboId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState UpdateCategoryId(int id, int categoryId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_CategoryId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inCategoryId", categoryId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState UpdatePublisherId(int id, int publisherId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_PublisherId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inPublisherId", publisherId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public void ShopeeReadCommonItem(List<CommonItem> list, MySqlDataReader rdr)
        {
            CommonItem commonItem = null;
            CommonModel commonModel = null;
            int dbItemId = MyMySql.GetInt32(rdr, "ShopeeItemId");
            int dbModelId = MyMySql.GetInt32(rdr, "ShopeeModelId");
            if (list.Count() == 0)
            {
                commonItem = new CommonItem();
                list.Add(commonItem);
            }
            else
            {
                // Đọc sang item mới vì kết quả sql đã được order by theo itemid, modelid
                if (list[list.Count() - 1].dbItemId != dbItemId)
                {
                    commonItem = new CommonItem();
                    list.Add(commonItem);
                }
            }
            if (commonItem != null)
            {
                commonItem.dbItemId = dbItemId;
                commonItem.itemId = MyMySql.GetInt64(rdr, "TMDTShopeeItemId");
                commonItem.name = MyMySql.GetString(rdr, "ShopeeItemName");
                commonItem.imageSrc = MyMySql.GetString(rdr, "ShopeeItemImage");

                // Tạm thời đóng lấy trạng thái từ db, sẽ lấy từ Shopee
                //int status = MyMySql.GetInt32(rdr, "ShopeeItemStatus");
                //if (status != 1)
                //    commonItem.bActive = true;
                //else
                //    commonItem.bActive = false;
            }
            else
            {
                commonItem = list[list.Count() - 1];
            }

            if (commonItem.models.Count() == 0)
            {
                commonModel = new CommonModel();
                commonItem.models.Add(commonModel);
            }
            else
            {
                // Đọc sang item mới vì kết quả sql đã được order by theo itemid, modelid
                if (commonItem.models[commonItem.models.Count() - 1].dbModelId != dbModelId)
                {
                    commonModel = new CommonModel();
                    commonItem.models.Add(commonModel);
                }
            }
            if (commonModel != null)
            {
                commonModel.dbModelId = dbModelId;
                commonModel.modelId = MyMySql.GetInt64(rdr, "TMDTShopeeModelId");
                commonModel.name = MyMySql.GetString(rdr, "ShopeeModelName");
                commonModel.imageSrc = MyMySql.GetString(rdr, "ShopeeModelImage");
                // Tạm thời đóng lấy trạng thái từ db, sẽ lấy từ Shopee
                //int status = MyMySql.GetInt32(rdr, "ShopeeModelStatus");
                //if (status != 1)
                //    commonModel.bActive = true;
                //else
                //    commonModel.bActive = false;
            }
            else
            {
                commonModel = commonItem.models[commonItem.models.Count() - 1];
            }
            // Thêm mapping
            Mapping mapping = new Mapping();
            mapping.quantity = MyMySql.GetInt32(rdr, "ShopeeMappingQuantity");

            Product product = new Product();
            product.id = MyMySql.GetInt32(rdr, "ProductId");
            product.name = MyMySql.GetString(rdr, "ProductName");
            product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
            mapping.product = product;

            commonModel.mapping.Add(mapping);
        }

        // Kết nối đóng mở bên ngoài
        // Từ bảng tbNeedUpdateQuantity lấy được danh sách sản phẩm shopee có thay đổi số lượng
        // cần cập nhật
        public List<CommonItem> ShopeeGetListNeedUpdateQuantityConnectOut(MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_Need_Update_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ShopeeReadCommonItem(listCI, rdr);
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
        // Lấy được Shopee Item mapping với sản phẩm trong kho
        public List<CommonItem> ShopeeGetListMappingOfProduct(int productId, MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_Mapping_Product_Id", conn);
                cmd.Parameters.AddWithValue("@inProductId", productId);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ShopeeReadCommonItem(listCI, rdr);
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

        public void TikiReadCommonItem(List<CommonItem> list, MySqlDataReader rdr)
        {
            CommonItem commonItem = null;
            CommonModel commonModel = null;
            int dbItemId = MyMySql.GetInt32(rdr, "TikiItemId");
            if (list.Count() == 0)
            {
                commonItem = new CommonItem(Common.eTiki);
                list.Add(commonItem);
            }
            else
            {
                // Đọc sang item mới vì kết quả sql đã được order by theo itemid, modelid
                if (list[list.Count() - 1].dbItemId != dbItemId)
                {
                    commonItem = new CommonItem(Common.eTiki);
                    list.Add(commonItem);
                }
            }
            if (commonItem != null)
            {
                commonItem.dbItemId = dbItemId;
                commonItem.itemId = MyMySql.GetInt32(rdr, "TMDTTikiItemId");
                commonItem.name = MyMySql.GetString(rdr, "TikiItemName");
                commonItem.imageSrc = MyMySql.GetString(rdr, "TikiItemImage");
                commonItem.tikiSuperId = MyMySql.GetInt32(rdr, "TMDTTikiItemSuperId");

                // Tạm thời đóng lấy trạng thái từ db, sẽ lấy từ Tiki
                //int status = MyMySql.GetInt32(rdr, "TikiItemStatus");
                //if (status != 1)
                //    commonItem.bActive = true;
                //else
                //    commonItem.bActive = false;
            }
            else
            {
                commonItem = list[list.Count() - 1];
            }

            if (commonItem.models.Count() == 0)
            {
                commonItem.models.Add(new CommonModel());
            }
            commonModel = commonItem.models[commonItem.models.Count() - 1];
            commonModel.modelId = -1;

            // Thêm mapping
            Mapping mapping = new Mapping();
            mapping.quantity = MyMySql.GetInt32(rdr, "TikiMappingQuantity");

            Product product = new Product();
            product.id = MyMySql.GetInt32(rdr, "ProductId");
            product.name = MyMySql.GetString(rdr, "ProductName");
            product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
            mapping.product = product;

            commonModel.mapping.Add(mapping);
        }

        // Kết nối đóng mở bên ngoài
        // Từ bảng tbNeedUpdateQuantity lấy được danh sách sản phẩm Tiki có thay đổi số lượng
        // cần cập nhật
        public List<CommonItem> TikiGetListNeedUpdateQuantityConnectOut(MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_Need_Update_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    TikiReadCommonItem(listCI, rdr);
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
        // Lấy được Tiki Item mapping với sản phẩm trong kho
        public List<CommonItem> TikiGetListMappingOfProduct(int productId, MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_From_Mapping_Product_Id", conn);
                cmd.Parameters.AddWithValue("@inProductId", productId);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader rdr = cmd.ExecuteReader();
                
                while (rdr.Read())
                {
                    TikiReadCommonItem(listCI, rdr);
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

        public List<Product> GetListProductInWarehoueChangedQuantity()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<Product> ls = new List<Product>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbNeedUpdateQuantity_Get_List", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Product pro = new Product();
                    pro.id = MyMySql.GetInt32(rdr, "Id");
                    pro.name  = MyMySql.GetString(rdr, "Name");
                    pro.quantity = MyMySql.GetInt32(rdr, "Quantity");
                    pro.SetSrcImageVideo();

                    ls.Add(pro);
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

        /// <summary>
        ///  Cập nhật trạng thái từ 1 sang 0 của tbNeedUpdateQuantity
        /// </summary>
        /// <param name="listProId"></param>
        public void UpdateStatusOfNeedUpdateQuantity(List<int> listProId)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("UPDATE tbNeedUpdateQuantity SET Status = 0 WHERE ProductId=@inProductId;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inProductId", 0);
                foreach(int id in listProId)
                {
                    cmd.Parameters[0].Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
        }

        //
        /// <summary>
        ///  Cập nhật trạng thái từ 1 sang 0 của tbNeedUpdateQuantity
        /// </summary>
        /// <param name="listProId"></param>
        public void UpdateStatusOfNeedUpdateQuantityConnectOut(List<int> listProId, MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE tbNeedUpdateQuantity SET Status = 0 WHERE ProductId=@inProductId;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inProductId", 0);
                foreach (int id in listProId)
                {
                    cmd.Parameters[0].Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public List<int> GetListProductOfNeedUpdateQuantityConnectOut(MySqlConnection conn)
        {
            List<int> listProductId = new List<int>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `ProductId` FROM `tbNeedUpdateQuantity` WHERE `Status`=1;", conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listProductId.Add( MyMySql.GetInt32(rdr, "ProductId"));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                listProductId.Clear();
            }
            return listProductId;
        }

        // Cập nhật url ảnh đại diện shopee item từ TMDTShopeeItemId
        // Mở đóng kết nối bên ngoài
        public void UpdateImageSrcShopeeItem(long TMDTShopeeItemId, string imageSrc, MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE tbShopeeItem SET Image = @inImage WHERE TMDTShopeeItemId=@inTMDTShopeeItemId;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inImage", imageSrc);
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", TMDTShopeeItemId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Cập nhật url ảnh đại diện shopee model từ inTMDTShopeeModelId
        // Mở đóng kết nối bên ngoài
        public void UpdateImageSrcShopeeModel(long TMDTShopeeModelId, string imageSrc, MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE tbShopeeModel SET Image = @inImage WHERE TMDTShopeeModelId=@inTMDTShopeeModelId;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inImage", imageSrc);
                cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", TMDTShopeeModelId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Cập nhật url ảnh đại diện Tiki item từ TikiId
        // Mở đóng kết nối bên ngoài
        public void UpdateImageSrcTikiItem(int TikiId, string imageSrc, MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE tbTikiItem SET Image = @inImage WHERE TikiId=@inTikiId;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inImage", imageSrc);
                cmd.Parameters.AddWithValue("@inTikiId", TikiId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Lấy được số lượng thực tế trong kho của sản phẩm sàn Shopee
        public int ShopeeGetQuantityOfOneItemModel(long itemId, long modelId)
        {
            int quantity = 0;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Get_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inItemId", itemId);
                cmd.Parameters.AddWithValue("@inModelId", modelId);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    quantity = MyMySql.GetInt32(rdr, "Quantity");
                    if (quantity == -1)
                        quantity = 0;
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                quantity = 0;
            }

            conn.Close();
            return quantity;
        }

        // Lấy được số lượng thực tế trong kho của sản phẩm sàn Tiki
        public int TikiGetQuantityOfOneItemModel(int itemId)
        {
            int quantity = 0;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTTikiItemId", itemId);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    quantity = MyMySql.GetInt32(rdr, "Quantity");
                    if (quantity == -1)
                        quantity = 0;
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                quantity = 0;
            }

            conn.Close();
            return quantity;
        }

        // Kết nối đóng mở bên ngoài
        // Lấy được số lượng thực tế trong kho của sản phẩm sàn Shopee
        public int ShopeeGetQuantityOfOneItemModelConnectOut(long itemId, long modelId, MySqlConnection conn)
        {
            int quantity = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Get_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inItemId", itemId);
                cmd.Parameters.AddWithValue("@inModelId", modelId);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    quantity = MyMySql.GetInt32(rdr, "Quantity");
                    if (quantity == -1)
                        quantity = 0;
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                quantity = 0;
            }

            return quantity;
        }

        // Kết nối đóng mở bên ngoài
        // Lấy được số lượng thực tế trong kho của sản phẩm sàn Tiki
        public int TikiGetQuantityOfOneItemModelConnectOut(int itemId, MySqlConnection conn)
        {
            int quantity = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTTikiItemId", itemId);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    quantity = MyMySql.GetInt32(rdr, "Quantity");
                    if (quantity == -1)
                        quantity = 0;
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                quantity = 0;
            }

            return quantity;
        }
    }
}