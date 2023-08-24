using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

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
        /// <param name="productName"></param>
        /// <returns>Có thể trả về null</returns>
        public Product GetProductFromProductName(string productName)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Product product = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inProductName", productName);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> combo id</param>
        /// <returns></returns>
        public Product GetProductFromFirstComboId(int id)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            Product product = null;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_First_Product_From_Combo_Id", conn);
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

        public MySqlResultState UpdateProductBarcode(int id, string newBarcode)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", newBarcode);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Barcode", paras);
            return result;
        }

        public MySqlResultState AddMoreProductBarcode(int id, string newBarcode)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", newBarcode);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Barcode_Add_More", paras);
            return result;
        }

        public MySqlResultState UpdateProductName(int id, string newProductName)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inProductName", newProductName);
            MyMySql.AddOutParameters(paras);

            MySqlResultState result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Name", paras);
            return result;
        }

        private void AddUpdateParameters(Product pro, MySqlParameter[] paras)
        {
            paras[0]  = new MySqlParameter("@proCode", pro.code);
            paras[1]  = new MySqlParameter("@barcode", pro.barcode);
            paras[2]  = new MySqlParameter("@productName", pro.name);
            paras[3]  = new MySqlParameter("@comboId", pro.comboId);
            paras[4]  = new MySqlParameter("@categoryId", pro.categoryId);
            paras[5]  = new MySqlParameter("@bookCoverPrice", pro.bookCoverPrice);
            paras[6]  = new MySqlParameter("@author", pro.author);
            paras[7]  = new MySqlParameter("@translator", pro.translator);
            paras[8]  = new MySqlParameter("@publisherId", pro.publisherId);
            paras[9]  = new MySqlParameter("@publishingCompany", pro.publishingCompany);
            paras[10] = new MySqlParameter("@publishingTime", pro.publishingTime);
            paras[11] = new MySqlParameter("@productLong", pro.productLong);
            paras[12] = new MySqlParameter("@productWide", pro.productWide);
            paras[13] = new MySqlParameter("@productHigh", pro.productHigh);
            paras[14] = new MySqlParameter("@productWeight", pro.productWeight);
            paras[15] = new MySqlParameter("@positionInWarehouse", pro.positionInWarehouse);
            paras[16] = new MySqlParameter("@hardCover", pro.hardCover);
            paras[17] = new MySqlParameter("@minAge", pro.minAge);
            paras[18] = new MySqlParameter("@maxAge", pro.maxAge);
            paras[19] = new MySqlParameter("@parentId", pro.parentId);
            paras[20] = new MySqlParameter("@republish", pro.republish);
            paras[21] = new MySqlParameter("@detail", pro.detail);
            paras[22] = new MySqlParameter("@proStatus", pro.status);

            MyMySql.AddOutParameters(paras);
        }

        public MySqlResultState AddNewPro(
            Product pro
        )
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[25];
            AddUpdateParameters(pro, paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Insert", paras);

            return result;
        }

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
            paras[0] = new MySqlParameter("@comboId", pro.comboId);
            paras[1] = new MySqlParameter("@categoryId", pro.categoryId);
            paras[2] = new MySqlParameter("@bookCoverPrice", pro.bookCoverPrice);
            paras[3] = new MySqlParameter("@author", pro.author);
            paras[4] = new MySqlParameter("@translator", pro.translator);
            paras[5] = new MySqlParameter("@publisherId", pro.publisherId);
            paras[6] = new MySqlParameter("@publishingCompany", pro.publishingCompany);
            paras[7] = new MySqlParameter("@publishingTime", pro.publishingTime);
            paras[8] = new MySqlParameter("@productLong", pro.productLong);
            paras[9] = new MySqlParameter("@productWide", pro.productWide);
            paras[10] = new MySqlParameter("@productHigh", pro.productHigh);
            paras[11] = new MySqlParameter("@productWeight", pro.productWeight);
            paras[12] = new MySqlParameter("@positionInWarehouse", pro.positionInWarehouse);
            paras[13] = new MySqlParameter("@hardCover", pro.hardCover);
            paras[14] = new MySqlParameter("@minAge", pro.minAge);
            paras[15] = new MySqlParameter("@maxAge", pro.maxAge);
            paras[16] = new MySqlParameter("@republish", pro.republish);
            paras[17] = new MySqlParameter("@proStatus", pro.status);

            MyMySql.AddOutParameters(paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update_Common_Info_With_Combo", paras);

            return result;
        }

        public MySqlResultState UpdateProduct(
            Product pro
        )
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[25];

            AddUpdateParameters(pro, paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbProducts_Update", paras);

            return result;
        }

        public List<ProductIdName> GetListParent()
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
                        ls.Add(rdr.GetString("Barcode"));
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
                        ls.Add(rdr.GetString("ComboName"));
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
                        ls.Add(rdr.GetString("Author"));
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
                        ls.Add(rdr.GetString("Translator"));
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
                        ls.Add(rdr.GetString("PublishingCompany"));
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

        public List<ProductIdCodeBarcodeNameBookCoverPrice> GetProductIdCodeBarcodeNameBookCoverPrice()
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<ProductIdCodeBarcodeNameBookCoverPrice> ls = new List<ProductIdCodeBarcodeNameBookCoverPrice>();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Id_Code_Barcode_Name_BookCoverPrice_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ProductIdCodeBarcodeNameBookCoverPrice product = new ProductIdCodeBarcodeNameBookCoverPrice();
                    product.id = MyMySql.GetInt32(rdr, "Id");
                    product.code = MyMySql.GetString(rdr, "Code");
                    product.barcode = MyMySql.GetString(rdr, "Barcode");
                    product.name = MyMySql.GetString(rdr, "Name");
                    product.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");

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
                paras[1] = new MySqlParameter("@inPrice", obj.priceImport);
                paras[2] = new MySqlParameter("@inQuantity", obj.quantity);
                paras[3] = new MySqlParameter("@inDate", DateTime.Now.ToString("yyy-MM-dd"));
                paras[4] = new MySqlParameter("@inBookCoverPrice", obj.bookCoverPrice);
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

        public List<Import> GetImportList(string fromDate, string toDate)
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

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Import imp = new Import();
                    imp.id = MyMySql.GetInt32(rdr, "Id");
                    imp.productId = MyMySql.GetInt32(rdr, "ProductId");
                    imp.productName = MyMySql.GetString(rdr, "Name");
                    imp.priceImport = MyMySql.GetInt32(rdr, "PriceImport");
                    imp.quantity = MyMySql.GetInt32(rdr, "Quantity");
                    imp.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
                    imp.dateImport = MyMySql.GetString(rdr, "DateImport");

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
            paras[2] = new MySqlParameter("@inQuantity", 0);
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
                    paras[1].Value = obj.priceImport;
                    paras[2].Value = obj.quantity;
                    cmd.ExecuteNonQuery();
                    rsTemp = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                    if (rsTemp != EMySqlResultState.OK) // Có lỗi vẫn thực hiện tiếp check lại thủ công sau
                    {
                        result.State = rsTemp;
                        //result.Message = "Chả có gì đâu";
                    }
                }

            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
            }

            conn.Close();

            return result;
        }

        public List<Product> SearProduct(string codeOrBarcode, string name, string combo)
        {
            List<Product> ls = new List<Product>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_Product", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCodeOrBarcode", codeOrBarcode);
                cmd.Parameters.AddWithValue("@inName", name);
                cmd.Parameters.AddWithValue("@inCombo", combo);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ls.Add(ConvertOneRowFromDataMySql(rdr));
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
    }
}