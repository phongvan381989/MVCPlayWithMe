using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
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
        /// </summary>
        /// <param name="rdr">Trả về ngay từ câu select</param>
        /// <returns></returns>
        private Product ConvertRowFromDataMySql(MySqlDataReader rdr)
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
            product.discount = rdr.GetFloat("Discount");
            product.SetSrcImageVideo();

            return product;
        }

        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product với 1 vài thuộc tính cần thiết
        /// </summary>
        /// <param name="rdr">Trả về ngay từ câu select</param>
        /// <returns></returns>
        private void ConvertQuicklyRowFromDataMySql(MySqlDataReader rdr, List<Product> ls)
        {
            int idIndex = rdr.GetOrdinal("Id");
            int codeIndex = rdr.GetOrdinal("Code");
            int barcodeIndex = rdr.GetOrdinal("Barcode");
            int nameIndex = rdr.GetOrdinal("Name");
            int quantityIndex = rdr.GetOrdinal("Quantity");
            int bookCoverPriceIndex = rdr.GetOrdinal("BookCoverPrice");
            int statusIndex = rdr.GetOrdinal("Status");
            int comboIdIndex = rdr.GetOrdinal("ComboId");
            int comboNameIndex = rdr.GetOrdinal("ComboName");
            int publisherIdIndex = rdr.GetOrdinal("PublisherId");

            while (rdr.Read())
            {
                Product product = new Product();
                product.id = rdr.GetInt32(idIndex);
                product.code = rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex);
                product.barcode = rdr.IsDBNull(barcodeIndex) ? string.Empty : rdr.GetString(barcodeIndex);
                product.name = rdr.GetString(nameIndex);
                product.quantity = rdr.GetInt32(quantityIndex);
                product.bookCoverPrice = rdr.GetInt32(bookCoverPriceIndex);
                product.status = rdr.GetInt32(statusIndex);
                product.comboId = rdr.GetInt32(comboIdIndex);
                product.comboName = rdr.IsDBNull(comboNameIndex) ? string.Empty : rdr.GetString(comboNameIndex);
                product.publisherId = rdr.GetInt32(publisherIdIndex);
                product.SetFirstSrcImage();

                ls.Add(product);
            }
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
                    product = ConvertRowFromDataMySql(rdr);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
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
                    product = ConvertRowFromDataMySql(rdr);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
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
                    product = ConvertRowFromDataMySql(rdr);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
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
        //        
        //        MyLogger.GetInstance().Warn(ex.ToString());
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

        // Thêm mới sản phẩm
        // Thêm đồng thời vào bảng tbneedupdatequantity
        public MySqlResultState AddNewPro(
            Product pro
        )
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[26];
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
            paras[23] = new MySqlParameter("@inpageNumber", pro.pageNumber);
            paras[24] = new MySqlParameter("@inQuantity", pro.quantity);
            paras[25] = new MySqlParameter("@inDiscount", pro.discount);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                MySqlDataReader rdr = cmd.ExecuteReader();

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

            paras = new MySqlParameter[22];
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
            paras[18] = new MySqlParameter("@inpageNumber", pro.pageNumber);
            paras[19] = new MySqlParameter("@inDiscount", pro.discount);

            MyMySql.AddOutParameters(paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Common_Info_With_Combo", paras);

            return result;
        }

        public MySqlResultState UpdateProduct(
            Product pro
        )
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = new MySqlParameter[27];
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
            paras[24] = new MySqlParameter("@inpageNumber", pro.pageNumber);
            paras[25] = new MySqlParameter("@inQuantity", pro.quantity);
            paras[26] = new MySqlParameter("@inDiscount", pro.discount);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                MySqlDataReader rdr = cmd.ExecuteReader();

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
                int idIndex = rdr.GetOrdinal("Id");
                int nameIndex = rdr.GetOrdinal("Name");

                while (rdr.Read())
                {
                    ls.Add(new ProductIdName(rdr.GetInt32(idIndex),
                        rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex)));
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

                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "Id");
                    break;
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
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
                int barcodeIndex = rdr.GetOrdinal("Barcode");
                while (rdr.Read())
                {
                    ls.Add(rdr.IsDBNull(barcodeIndex) ? string.Empty : rdr.GetString(barcodeIndex));
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
                int comboNameIndex = rdr.GetOrdinal("ComboName");
                while (rdr.Read())
                {
                    ls.Add(rdr.GetString(comboNameIndex));
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
                int authorIndex = rdr.GetOrdinal("Author");
                while (rdr.Read())
                {
                    ls.Add(rdr.IsDBNull(authorIndex) ? string.Empty : rdr.GetString(authorIndex));
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
                int translatorIndex = rdr.GetOrdinal("Translator");
                while (rdr.Read())
                {
                    ls.Add(rdr.IsDBNull(translatorIndex) ? string.Empty : rdr.GetString(translatorIndex));
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
                int publishingCompanyIndex = rdr.GetOrdinal("PublishingCompany");
                while (rdr.Read())
                {
                    ls.Add(rdr.GetString(publishingCompanyIndex));
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
                int difValueIndex = rdr.GetOrdinal("difValue");
                while (rdr.Read())
                {
                    ls.Add(rdr.IsDBNull(difValueIndex) ? 0 : rdr.GetInt32(difValueIndex));
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

        public List<Product> GetProductIdCodeBarcodeNameBookCoverPrice(int publisherId)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<Product> ls = new List<Product>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Id_Code_Barcode_Name_BookCoverPrice_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisherId", publisherId);

                MySqlDataReader rdr = cmd.ExecuteReader();
                int idIndex = rdr.GetOrdinal("Id");
                int codeIndex = rdr.GetOrdinal("Code");
                int barcodeIndex = rdr.GetOrdinal("Barcode");
                int nameIndex = rdr.GetOrdinal("Name");
                int bookCoverPriceIndex = rdr.GetOrdinal("BookCoverPrice");
                int comboIdIndex = rdr.GetOrdinal("ComboId");
                int comboNameIndex = rdr.GetOrdinal("ComboName");
                while (rdr.Read())
                {
                    Product product = new Product();
                    product.id = rdr.GetInt32(idIndex);
                    product.code = rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex);
                    product.barcode = rdr.IsDBNull(barcodeIndex) ? string.Empty : rdr.GetString(barcodeIndex);
                    product.name = rdr.GetString(nameIndex);
                    product.bookCoverPrice = rdr.GetInt32(bookCoverPriceIndex);
                    product.comboId = rdr.GetInt32(comboIdIndex);
                    product.comboName = rdr.IsDBNull(comboNameIndex) ? string.Empty : rdr.GetString(comboNameIndex);
                    product.SetFirstSrcImage();
                    ls.Add(product);
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
                int idIndex = rdr.GetOrdinal("Id");
                int productIdIndex = rdr.GetOrdinal("ProductId");
                int productNameIndex = rdr.GetOrdinal("Name");
                int priceIndex = rdr.GetOrdinal("Price");
                int quantityIndex = rdr.GetOrdinal("Quantity");
                int bookCoverPriceIndex = rdr.GetOrdinal("BookCoverPrice");
                int dateImportIndex = rdr.GetOrdinal("Date");
                int discountIndex = rdr.GetOrdinal("Discount");
                while (rdr.Read())
                {
                    Import imp = new Import();
                    imp.id = rdr.GetInt32(idIndex);
                    imp.productId = rdr.GetInt32(productIdIndex);
                    imp.productName = rdr.GetString(productNameIndex);
                    imp.price = rdr.GetInt32(priceIndex);
                    imp.quantity = rdr.GetInt32(quantityIndex);
                    imp.bookCoverPrice = rdr.GetInt32(bookCoverPriceIndex);
                    imp.dateImport = rdr.GetString(dateImportIndex);
                    imp.discount = rdr.GetFloat(discountIndex);

                    ls.Add(imp);
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

        public MySqlResultState UpdateListImport(List<Import> ls)
        {
            MySqlResultState result = new MySqlResultState();
            EMySqlResultState rsTemp;
            MySqlParameter[] paras = null;
            int lengthPara = 5;
            paras = new MySqlParameter[lengthPara];
            paras[0] = new MySqlParameter("@inId", (object)0);
            paras[1] = new MySqlParameter("@inPrice", (object)0);
            paras[2] = new MySqlParameter("@inDiscount", (object)0);
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

        public MySqlResultState UpdateOutputAndProductTableFromFromListImport(int orderId,
            List<Import> ls, ECommerceOrderStatus status,
            EECommerceType eCommerceType)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            MySqlResultState resultState = new MySqlResultState();
            try
            {
                conn.Open();
                // Lưu vào bảng tbOutput, tbProducts, tbNeedUpdateQuantity
                MySqlCommand cmd = new MySqlCommand("st_tbOutput_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCode", orderId); // TH với đơn Shopee, tiki,... là mã đơn hàng, TH này là id của đơn hàng
                cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                cmd.Parameters.AddWithValue("@inProductId", 0);
                cmd.Parameters.AddWithValue("@inQuantity", 0);
                foreach(var im in ls)
                {
                    if (im.quantity == 0)
                    {
                        continue;
                    }

                    cmd.Parameters[2].Value = im.productId;
                    if (status == ECommerceOrderStatus.RETURNED)
                        cmd.Parameters[3].Value = im.quantity * -1;
                    else
                        cmd.Parameters[3].Value = im.quantity;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }
            conn.Close();
            return resultState;
        }

        public MySqlResultState CreateOrderManually(List<Import> ls, int sumPay)
        {
            MySqlResultState result = null;

            // Ta thêm dữ liệu vào tbOrder
            OrderMySql ordersqler = new OrderMySql();
            int orderId = ordersqler.AddOrder(-1, "", 1, null);

            // Ta thêm dữ liệu vào tbOutput
            result = UpdateOutputAndProductTableFromFromListImport(orderId, ls, ECommerceOrderStatus.PACKED,
                EECommerceType.PLAY_WITH_ME);

            if (result.State != EMySqlResultState.OK)
                return result;

            // Ta thêm dữ liệu vào tbPayOrder
            OrderPay orderPay = new OrderPay();
            orderPay.type = EPayType.SUM;
            orderPay.value = sumPay;

            List<OrderPay> lsOrderPay = new List<OrderPay>();
            lsOrderPay.Add(orderPay);

            OrderMySql orderMySql = new OrderMySql();
            orderMySql.AddPayOrder(orderId, lsOrderPay);

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
                ConvertQuicklyRowFromDataMySql(rdr, ls);


                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return ls;
        }

        public List<Product> SearchDontSellOnECommerce (Boolean isSingle, string eType)
        {
            string store = string.Empty;
            if (eType == Common.eTiki)
            {
                store = "st_tbProducts_Search_Dont_Sell_On_Tiki";
            }
            else if (eType == Common.eShopee)
            {
                store = "st_tbProducts_Search_Dont_Sell_On_Shopee";
            }
            else if (eType == Common.ePlayWithMe)
            {
                store = "st_tbProducts_Search_Dont_Sell_On_PlayWithMe";
            }

            if (isSingle)
            {
                store = store + "_Signle";
            }

            List<Product> ls = new List<Product>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(store, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                ConvertQuicklyRowFromDataMySql(rdr, ls);

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
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
        //        
        //        MyLogger.GetInstance().Warn(ex.ToString());
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
                ConvertQuicklyRowFromDataMySql(rdr, ls);

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
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

        // Cập nhật thẳng số lượng vào bảng tbProducts nên cần cập nhật ở bảng tbneedupdatequantity,
        // không cập nhật qua bảng tbImport
        public MySqlResultState UpdateQuantity(int id, int quantity)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inQuantity", quantity);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        // Cập nhật thẳng số lượng vào bảng tbProducts nên cần cập nhật ở bảng tbneedupdatequantity,
        // không cập nhật qua bảng tbImport
        public MySqlResultState UpdateQuantityFromList(List<int> lsId, List<int> lsQuantity)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", 0);
                cmd.Parameters.AddWithValue("@inQuantity", 0);

                for (int i = 0; i < lsId.Count; i++)
                {
                    cmd.Parameters["@inId"].Value = lsId[i];
                    cmd.Parameters["@inQuantity"].Value = lsQuantity[i];
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
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

        public MySqlResultState UpdateDiscountWhenImport(int id, float discount)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts`SET `Discount` = @inDiscount WHERE `Id` = @inId;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inDiscount", discount);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState UpdatePositionInWarehouse(int id, string positionInWarehouse)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_PositionInWarehouse", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inpositionInWarehouse", positionInWarehouse);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        // Trạng thái sản phẩm. 0: Đang kinh doanh bình thường,
        // 1:  Nhà phát hành tạm thời hết hàng , 2: Ngừng kinh doanh
        public MySqlResultState UpdateStatusOfProduct(int id, int statusOfProduct)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_Status", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);
                cmd.Parameters.AddWithValue("@inStatus", statusOfProduct);
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

                int status = MyMySql.GetInt32(rdr, "ShopeeItemStatus");
                if (status == 0)
                    commonItem.bActive = true;
                else
                    commonItem.bActive = false;
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
                // Đọc sang model mới vì kết quả sql đã được order by theo itemid, modelid
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
                int status = MyMySql.GetInt32(rdr, "ShopeeModelStatus");
                if (status == 0)
                    commonModel.bActive = true;
                else
                    commonModel.bActive = false;
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

                int status = MyMySql.GetInt32(rdr, "TikiItemStatus");
                if (status != 1)
                    commonItem.bActive = true;
                else
                    commonItem.bActive = false;
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
                int idIndex = rdr.GetOrdinal("Id");
                int nameIndex = rdr.GetOrdinal("Name");
                int quantityIndex = rdr.GetOrdinal("Quantity");

                while (rdr.Read())
                {
                    Product pro = new Product();
                    pro.id = rdr.GetInt32(idIndex);
                    pro.name = rdr.GetString(nameIndex);
                    pro.quantity = rdr.GetInt32(quantityIndex);
                    pro.SetFirstSrcImage();

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
                int productIdIndex = rdr.GetOrdinal("ProductId");
                while (rdr.Read())
                {
                    listProductId.Add(rdr.GetInt32(productIdIndex));
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

        //// Lấy được số lượng thực tế trong kho của sản phẩm sàn Shopee
        //public int ShopeeGetQuantityOfOneItemModel(long itemId, long modelId)
        //{
        //    int quantity = 0;
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Get_Quantity", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inItemId", itemId);
        //        cmd.Parameters.AddWithValue("@inModelId", modelId);
        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        int quantityIndex = rdr.GetOrdinal("Quantity");
        //        while (rdr.Read())
        //        {
        //            quantity = rdr.GetInt32(quantityIndex);
        //        }

        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        quantity = 0;
        //    }

        //    conn.Close();
        //    return quantity;
        //}

        //// Lấy được số lượng thực tế trong kho của sản phẩm sàn Tiki
        //public int TikiGetQuantityOfOneItemModel(int itemId)
        //{
        //    int quantity = 0;
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_Quantity", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inTMDTTikiItemId", itemId);
        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        int quantityIndex = rdr.GetOrdinal("Quantity");
        //        while (rdr.Read())
        //        {
        //            quantity = rdr.GetInt32(quantityIndex);
        //        }

        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        quantity = 0;
        //    }

        //    conn.Close();
        //    return quantity;
        //}

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
                int quantityIndex = rdr.GetOrdinal("Quantity");
                while (rdr.Read())
                {
                    quantity = rdr.GetInt32(quantityIndex);
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
                int quantityIndex = rdr.GetOrdinal("Quantity");
                while (rdr.Read())
                {
                    quantity = rdr.GetInt32(quantityIndex);
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