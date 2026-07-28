using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.OpenPlatform.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Models.ProductModel
{
    public class ProductMySql
    {
        /// <summary>
        /// T? d? li?u select db, ta tr? v? d?i tu?ng Product
        /// </summary>
        /// <param name="rdr">Tr? v? ngay t? câu select</param>
        /// <returns></returns>
        private static Product ConvertRowFromDataMySql(MySqlDataReader rdr)
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
            product.pageNumber = MyMySql.GetInt32(rdr, "PageNumber");
            product.discount = rdr.GetFloat("Discount");
            product.language = MyMySql.GetString(rdr, "Language");
            product.SetSrcImageVideo();

            return product;
        }

        /// <summary>
        /// T? d? li?u select db, ta tr? v? d?i tu?ng Product v?i 1 vài thu?c tính c?n thi?t
        /// </summary>
        /// <param name="rdr">Tr? v? ngay t? câu select</param>
        /// <returns></returns>
        private static async Task ConvertQuicklyRowFromDataMySql(MySqlDataReader rdr, List<Product> ls)
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

            while (await rdr.ReadAsync())
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

        private static async Task ConvertQuicklyRowFromDataMySqlForMapping(MySqlDataReader rdr, List<Product> ls)
        {
            int idIndex = rdr.GetOrdinal("Id");
            int nameIndex = rdr.GetOrdinal("Name");
            int statusIndex = rdr.GetOrdinal("Status");

            while (await rdr.ReadAsync())
            {
                Product product = new Product();
                product.id = rdr.GetInt32(idIndex);
                product.name = rdr.GetString(nameIndex);
                product.status = rdr.GetInt32(statusIndex);
                product.SetFirstSrcImage();

                ls.Add(product);
            }
        }

        //AddNewProsFromCSVPromise
        public static async Task<MySqlResultState> AddNewProsFromCSVPromise(List<ProductFromCsv> products, int publisherId)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    // cache comboName -> comboId
                    var comboCache =
                        new Dictionary<string, int>(
                            StringComparer.OrdinalIgnoreCase);

                    foreach (var item in products)
                    {
                        int comboId;

                        if (string.IsNullOrEmpty(item.comboName))
                        {
                            comboId = -1;// m?c d?nh
                        }
                        // dã có cache
                        else if (comboCache.ContainsKey(item.comboName))
                        {
                            comboId = comboCache[item.comboName];
                        }
                        else
                        {
                            object comboIdResult;
                            // tìm combo trong DB
                            using (MySqlCommand getComboCmd = new MySqlCommand(
                                @"SELECT id
                                FROM tbCombo
                                WHERE Name = @name
                                LIMIT 1",
                                conn))
                            {
                                getComboCmd.Parameters.AddWithValue(
                                    "@name",
                                    item.comboName);

                                comboIdResult =
                                    await getComboCmd.ExecuteScalarAsync();
                            }
                            // chua có -> insert
                            if (comboIdResult == null)
                            {
                                using (MySqlCommand insertComboCmd = new MySqlCommand(
                                    @"INSERT INTO tbCombo(name)
                                VALUES(@name);
                                SELECT LAST_INSERT_ID();",
                                    conn))
                                {
                                    insertComboCmd.Parameters.AddWithValue(
                                        "@name",
                                        item.comboName);

                                    comboId = Convert.ToInt32(
                                        await insertComboCmd.ExecuteScalarAsync());
                                }
                            }
                            else
                            {
                                comboId = Convert.ToInt32(comboIdResult);
                            }

                            comboCache[item.comboName] = comboId;
                        }

                        // insert product
                        using (MySqlCommand insertProductCmd = new MySqlCommand(
                            @"INSERT INTO tbproducts
                            (
                                Code,
                                Name,
                                ComboId,
                                BookCoverPrice,
                                PublisherId,
                                Quantity,
                                Discount,
                                Date
                            )
                            VALUES
                            (
                                @code,
                                @name,
                                @comboId,
                                @bookCoverPrice,
                                @publisherId,
                                @quantity,
                                @discount,
                                CURDATE()
                            );
                            SELECT LAST_INSERT_ID();",
                            conn))
                        {
                            insertProductCmd.Parameters.AddWithValue(
                                "@name",
                                item.productName);

                            insertProductCmd.Parameters.AddWithValue(
                                "@comboId",
                                comboId);

                            insertProductCmd.Parameters.AddWithValue(
                                "@bookCoverPrice",
                                item.bookCoverPrice);

                            insertProductCmd.Parameters.AddWithValue(
                                "@publisherId",
                                publisherId);

                            insertProductCmd.Parameters.AddWithValue(
                                "@quantity",
                                item.quantity);

                            insertProductCmd.Parameters.AddWithValue(
                                "@discount",
                                item.discount);
                            insertProductCmd.Parameters.AddWithValue(
                                "@code",
                                item.code);

                            int productId = Convert.ToInt32(await insertProductCmd.ExecuteScalarAsync());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        //public List<Import> GetImportList(string fromDate, string toDate, string publisher)
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    List<Import> ls = new List<Import>();
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbImport_Select", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inFromDate", fromDate);
        //        cmd.Parameters.AddWithValue("@inToDate", toDate);
        //        cmd.Parameters.AddWithValue("@inPublisher", publisher);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        int idIndex = rdr.GetOrdinal("Id");
        //        int productIdIndex = rdr.GetOrdinal("ProductId");
        //        int productNameIndex = rdr.GetOrdinal("Name");
        //        int priceIndex = rdr.GetOrdinal("Price");
        //        int quantityIndex = rdr.GetOrdinal("Quantity");
        //        int bookCoverPriceIndex = rdr.GetOrdinal("BookCoverPrice");
        //        int dateImportIndex = rdr.GetOrdinal("Date");
        //        int discountIndex = rdr.GetOrdinal("Discount");
        //        while (rdr.Read())
        //        {
        //            Import imp = new Import();
        //            imp.id = rdr.GetInt32(idIndex);
        //            imp.productId = rdr.GetInt32(productIdIndex);
        //            imp.productName = rdr.GetString(productNameIndex);
        //            imp.price = rdr.GetInt32(priceIndex);
        //            imp.quantity = rdr.GetInt32(quantityIndex);
        //            imp.bookCoverPrice = rdr.GetInt32(bookCoverPriceIndex);
        //            imp.dateImport = rdr.GetString(dateImportIndex);
        //            imp.discount = rdr.GetFloat(discountIndex);

        //            ls.Add(imp);
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

        //public MySqlResultState UpdateListImport(List<Import> ls)
        //{
        //    MySqlResultState result = new MySqlResultState();
        //    EMySqlResultState rsTemp;
        //    MySqlParameter[] paras = null;
        //    int lengthPara = 5;
        //    paras = new MySqlParameter[lengthPara];
        //    paras[0] = new MySqlParameter("@inId", (object)0);
        //    paras[1] = new MySqlParameter("@inPrice", (object)0);
        //    paras[2] = new MySqlParameter("@inDiscount", (object)0);
        //    MyMySql.AddOutParameters(paras);

        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbImport_Update", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddRange(paras);

        //        foreach (var obj in ls)
        //        {
        //            paras[0].Value = obj.id;
        //            paras[1].Value = obj.price;
        //            paras[2].Value = obj.discount;
        //            cmd.ExecuteNonQuery();
        //            rsTemp = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
        //            result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
        //            if (rsTemp != EMySqlResultState.OK) // Có l?i v?n th?c hi?n ti?p check l?i th? công sau
        //            {
        //                result.State = rsTemp;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Common.SetResultException(ex, result);
        //    }

        //    conn.Close();

        //    return result;
        //}

        // Tìm ki?m không phân trang
        public static async Task<List<Product>> SearchProduct(ProductSearchParameter searchParameter)
        {
            List<Product> ls = new List<Product>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_Product", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inPublisher", searchParameter.publisher);
                        cmd.Parameters.AddWithValue("@inCodeOrBarcode", searchParameter.codeOrBarcode);
                        cmd.Parameters.AddWithValue("@inName", searchParameter.name);
                        cmd.Parameters.AddWithValue("@inCombo", searchParameter.combo);
                        //cmd.Parameters.AddWithValue("@inStatus", searchParameter.status);

                        using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                        {
                            await ConvertQuicklyRowFromDataMySql(rdr, ls);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return ls;
        }

        public static async Task<List<Product>> SearchDontSellOnECommerce( Boolean isSingle,
            string eType,
            MySqlConnection conn)
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
            try
            {
                if (!string.IsNullOrEmpty(store))
                {
                    using (MySqlCommand cmd = new MySqlCommand(store, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                        {
                            await ConvertQuicklyRowFromDataMySql(rdr, ls);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return ls;
        }

        public static async Task<List<Product>> GetActiveProductHasComboActiveAll(MySqlConnection conn)
        {
            List<Product> ls = new List<Product>();

            using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_Has_Combo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                {
                    await ConvertQuicklyRowFromDataMySql(rdr, ls);
                }
            }

            return ls;
        }


        // Ki?m tra xem combo dã du?c bán full và riêng l? ? cùng 1 s?n ph?m cha trên sàn
        // True: N?u dã th?a mã, ngu?c l?i false
        public static async Task<Boolean> TikiDontSellFullComboAndSigleConnectOut(Combo combo, MySqlConnection conn)
        {
            try
            {
                Dictionary<int, Dictionary<int, List<int>>> dictionary =
                    new Dictionary<int, Dictionary<int, List<int>>>();

                using (MySqlCommand cmd = new MySqlCommand("st_tbTikiMapping_Get_From_Combo_ProductId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCount", combo.products.Count);
                    cmd.Parameters.AddWithValue("@inProductId", combo.products[0].id);
                    using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = rdr.GetOrdinal("Id");// index c?a row trong db
                        int productIdMappingIndex = rdr.GetOrdinal("ProductIdMapping");
                        int superIdIndex = rdr.GetOrdinal("superId");
                        int id = 0, productIdMapping = 0, superId = 0;
                        while (await rdr.ReadAsync())
                        {
                            superId = rdr.GetInt32(superIdIndex);
                            id = rdr.GetInt32(idIndex);
                            productIdMapping = rdr.IsDBNull(productIdMappingIndex) ? -1 : rdr.GetInt32(productIdMappingIndex);
                            if (dictionary.ContainsKey(superId))
                            {
                                if (dictionary[superId].ContainsKey(id))
                                {
                                    dictionary[superId][id].Add(productIdMapping);
                                }
                                else
                                {
                                    dictionary[superId].Add(id, new List<int>() { productIdMapping });
                                }
                            }
                            else
                            {
                                dictionary.Add(superId, new Dictionary<int, List<int>>
                            {{ id, new List<int>() { productIdMapping } }});
                            }
                        }
                    }
                }
                if(dictionary.Count == 0)
                {
                    return false;
                }

                List<int> lsProIdInCombo = new List<int>();
                foreach (var pro in combo.products)
                {
                    lsProIdInCombo.Add(pro.id);
                }
                Boolean isCombo = false; // check combo dã du?c mapping d?
                Boolean isSigle = false; // s?n ph?m riêng l? du?c mapping d?

                foreach (var proId in lsProIdInCombo)
                {
                    isSigle = false;
                    // Duy?t qua t?t c? các value
                    foreach (var dic1 in dictionary.Values)
                    {
                        foreach (var ls in dic1.Values)
                        {
                            // Chuy?n d?i thành HashSet và so sánh
                            if (ls.Count > 1)
                            {
                                if (!isCombo && ls.Count == lsProIdInCombo.Count)
                                {
                                    isCombo = new HashSet<int>(ls).SetEquals(lsProIdInCombo);
                                    if (isSigle && isCombo)
                                        break;
                                }
                            }
                            else
                            {
                                if (proId == ls[0])
                                {
                                    isSigle = true;
                                    if (isCombo)
                                        break;
                                }
                            }
                        }
                        if (isSigle && isCombo)
                        {
                            break;
                        }
                    }

                    if (!isSigle)
                    {
                        break;
                    }
                }

                return isCombo && isSigle;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return false;
        }

        private static Boolean CheckDontSellSigleWithParrent(Combo combo, List<CommonItem> lsCommonItem)
        {
            Boolean isSigle = false; // s?n ph?m riêng l? du?c mapping d?
            foreach (var commonItem in lsCommonItem)
            {
                foreach(var pro in combo.products)
                {
                    isSigle = false;
                    foreach (var model in commonItem.models)
                    {
                        if (model.mapping.Count == 1 &&
                                    model.mapping[0].product.id == pro.id)
                        {
                            isSigle = true;
                            break;
                        }
                    }
                    if(!isSigle)
                    {
                        break;
                    }
                }
                if(isSigle)
                {
                    break;
                }
            }
            return isSigle;
        }

        // Ki?m tra xem combo dã du?c bán riêng l? ? cùng 1 s?n ph?m cha trên sàn
        // True: N?u dã th?a mã, ngu?c l?i false
        public static async Task<Boolean> TikiDontSellSigleWithParrentConnectOut(Combo combo,
            MySqlConnection conn)
        {
            try
            {
                List<CommonItem> lsCommonItem = await TikiMySql.TikiGetListMappingOfComboAsync(combo.id, conn);

                Dictionary<int, List<CommonItem>> dictionary =
                    new Dictionary<int, List<CommonItem>>();

                foreach (var commonItem in lsCommonItem)
                {
                    if (commonItem.tikiSuperId != 0)
                    {
                        if (dictionary.ContainsKey(commonItem.tikiSuperId))
                        {
                            dictionary[commonItem.tikiSuperId].Add(commonItem);
                        }
                        else
                        {
                            dictionary.Add(commonItem.tikiSuperId, new List<CommonItem> { commonItem });
                        }
                    }
                }
                if (dictionary.Count == 0)
                {
                    return false;
                }

                List<int> lsProIdInCombo = new List<int>();
                foreach (var pro in combo.products)
                {
                    lsProIdInCombo.Add(pro.id);
                }
                Boolean isSigle = false; // s?n ph?m riêng l? du?c mapping d?

                // Duy?t qua t?t c? các value
                foreach (var lsCI in dictionary.Values)
                {
                    foreach (var proId in lsProIdInCombo)
                    {
                        isSigle = false;
                        foreach (var cI in lsCI)
                        {
                            if (cI.models[0].mapping.Count == 1 &&
                                cI.models[0].mapping[0].product.id == proId)
                            {
                                isSigle = true;
                                break;
                            }
                        }
                        if (!isSigle)
                        {
                            break;
                        }
                    }
                    if (isSigle)
                    {
                        break;
                    }
                }
                return isSigle;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return false;
        }

        // Ki?m tra xem combo dã du?c bán riêng l? ? cùng 1 s?n ph?m cha trên sàn
        // True: N?u dã th?a mã, ngu?c l?i false
        public static async Task<Boolean> Shopee_LazadaDontSellSigleWithParrentConnectOut(
            string eType, Combo combo,
            MySqlConnection conn)
        {
            try
            {
                List<CommonItem> lsCommonItem = null;
                if (eType == Common.eLazada)
                {
                    lsCommonItem = await LazadaMySql.LazadaGetListMappingOfComboAsync(combo.id, conn);
                }
                if(lsCommonItem == null || lsCommonItem.Count == 0)
                {
                    return false;
                }

                return CheckDontSellSigleWithParrent(combo, lsCommonItem);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return false;
        }

        public static async Task<List<Product>> SearchDontSellSigleWithNoParrentOnECommerceConnectOut(
            string eType,
            MySqlConnection conn)
        {
            List<Product> ls = new List<Product>();
            try
            {
                string store = "";
                if(eType == eTiki)
                {
                    store = "st_tbProducts_Search_Dont_Sell_On_Tiki_Signle_No_Parrent";
                }
                else if(eType == eShopee)
                {
                    store = "st_tbProducts_Search_Dont_Sell_On_Shopee_Signle_No_Parrent";
                }
                else if(eType == eLazada)
                {
                    store = "st_tbProducts_Search_Dont_Sell_On_Lazada_Signle_No_Parrent";
                }
                if (!string.IsNullOrEmpty(store))
                {
                    using (MySqlCommand cmd = new MySqlCommand(store, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                        {
                            await ConvertQuicklyRowFromDataMySql(rdr, ls);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return ls;
        }

        public static async Task<List<int>> SearchDontSellSigleWithNoParrentOnECommerce_GetIdListOnly_ConnectOutAsync(
            string eType,
            MySqlConnection conn)
        {
            List<int> ls = new List<int>();
            try
            {
                string store = "";
                if (eType == eTiki)
                {
                    store = "st_tbProducts_Search_Dont_Sell_On_Tiki_Signle_No_Parrent";
                }
                else if (eType == eShopee)
                {
                    store = "st_tbProducts_Search_Dont_Sell_On_Shopee_Signle_No_Parrent";
                }
                else if(eType == eLazada)
                {
                    store = "st_tbProducts_Search_Dont_Sell_On_Lazada_Signle_No_Parrent";
                }
                if (!string.IsNullOrEmpty(store))
                {
                    using (MySqlCommand cmd = new MySqlCommand(store, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            while (await rdr.ReadAsync())
                            {
                                ls.Add(rdr.GetInt32(idIndex));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return ls;
        }

        public static async Task<List<Product>> ShopeeDontSellSigleWithNoParrentConnectOut(
            MySqlConnection conn)
        {
            List<Product> ls = new List<Product>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_Dont_Sell_On_Tiki_Signle_No_Parrent", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                    {
                        await ConvertQuicklyRowFromDataMySql(rdr, ls);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return ls;
        }

        //Tìm ki?m có phân trang
        public static async Task<List<Product>> SearchProductChangePage(ProductSearchParameter searchParameter)
        {
            List<Product> ls = new List<Product>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inPublisher", searchParameter.publisher);
                        cmd.Parameters.AddWithValue("@inCodeOrBarcode", searchParameter.codeOrBarcode);
                        cmd.Parameters.AddWithValue("@inName", searchParameter.name);
                        cmd.Parameters.AddWithValue("@inCombo", searchParameter.combo);
                        cmd.Parameters.AddWithValue("@inStart", searchParameter.start);
                        cmd.Parameters.AddWithValue("@inOffset", searchParameter.offset);

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            await ConvertQuicklyRowFromDataMySql(rdr, ls);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return ls;
        }

        // -- Async versions ----------------------------------------------------

        public static async Task<Product> GetProductFromBarcodeAsync(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;
            Product product = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Barcode", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inBarcode", barcode);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync()) product = ConvertRowFromDataMySql(rdr);
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); product = null; }
            }
            return product;
        }

        public static async Task<Product> GetProductFromCodeAsync(string code)
        {
            Product product = null;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Code", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCode", code);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync()) product = ConvertRowFromDataMySql(rdr);
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); product = null; }
            }
            return product;
        }

        public static async Task<Product> GetProductFromIdAsync(int id, MySqlConnection conn)
        {
            Product product = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Product_From_Id", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inId", id);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync()) product = ConvertRowFromDataMySql(rdr);
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); product = null; }
            return product;
        }

        public static async Task<int> GetTikiCategoryIdFromProductCategoryIdAsync(int categoryId, MySqlConnection conn)
        {
            int id = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT TikiCategoryId FROM webplaywithme.tbcategory WHERE Id = @in_Id LIMIT 1;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@in_Id", categoryId);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync()) id = MyMySql.GetInt32(rdr, "TikiCategoryId");
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); id = 0; }
            return id;
        }

        public static async Task<MySqlResultState> UpdateProductBarcodeAsync(int id, string newBarcode)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", newBarcode);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbProducts_Update_Barcode", paras);
        }

        public static async Task<MySqlResultState> AddNewProAsync(Product pro)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = new MySqlParameter[27];
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
            paras[26] = new MySqlParameter("@inLanguage", pro.language);
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(paras);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                result.myAnything = MyMySql.GetInt32(rdr, "LastId");
                                if (result.myAnything == -2) { result.State = EMySqlResultState.EXIST; result.Message = "Code is exist"; }
                                else if (result.myAnything == -3) { result.State = EMySqlResultState.EXIST; result.Message = "Barcode is exist"; }
                            }
                        }
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> DeleteProductAsync(int id)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inId", id);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbProducts_Delete_Product_From_Id", paras);
        }

        public static async Task<MySqlResultState> UpdateCommonInfoWithComboAsync(ProductCommonInfoWithCombo pro)
        {
            MySqlParameter[] paras = new MySqlParameter[23];
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
            paras[20] = new MySqlParameter("@inLanguage", pro.language);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbProducts_Update_Common_Info_With_Combo", paras);
        }

        public static async Task<MySqlResultState> UpdateCommonHardCoverWithComboAsync(int comboId, int hardCover, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `HardCover` = @inHardCover WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inHardCover", hardCover);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCommonAgeWithComboAsync(int comboId, int minAge, int maxAge, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `MinAge` = @inMinAge, `MaxAge` = @inMaxAge WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inMinAge", minAge);
                    cmd.Parameters.AddWithValue("@inMaxAge", maxAge);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCommonLanguageWithComboAsync(int comboId, string language, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `Language` = @inLanguage WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inLanguage", language);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCommonDimensionWithComboAsync(int comboId, int productLong, int productWide, int productHigh, int productWeight, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `ProductLong` = @inProductLong, `ProductWide` = @inProductWide, `ProductHigh` = @inProductHigh, `ProductWeight` = @inProductWeight WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inProductLong", productLong);
                    cmd.Parameters.AddWithValue("@inProductWide", productWide);
                    cmd.Parameters.AddWithValue("@inProductHigh", productHigh);
                    cmd.Parameters.AddWithValue("@inProductWeight", productWeight);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCommonCategoryWithComboAsync(int comboId, int categoryId, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `CategoryId` = @inCategoryId WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inCategoryId", categoryId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCommonPageNumberWithComboAsync(int comboId, int pageNumber, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `PageNumber` = @inPageNumber WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inPageNumber", pageNumber);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCommonPublishingTimeWithComboAsync(int comboId, int publishingTime, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `PublishingTime` = @inPublishingTime WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inPublishingTime", publishingTime);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCommonBookCoverPriceWithComboAsync(int comboId, int bookCoverPrice, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts` SET `BookCoverPrice` = @inBookCoverPrice WHERE `ComboId` = @inComboId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.Parameters.AddWithValue("@inBookCoverPrice", bookCoverPrice);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateProductAsync(Product pro)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = new MySqlParameter[28];
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
            paras[27] = new MySqlParameter("@inLanguage", pro.language);
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(paras);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                result.myAnything = MyMySql.GetInt32(rdr, "LastId");
                                if (result.myAnything == -2) { result.State = EMySqlResultState.EXIST; result.Message = "Code is exist"; }
                                else if (result.myAnything == -3) { result.State = EMySqlResultState.EXIST; result.Message = "Barcode is exist"; }
                            }
                        }
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateProductFromFahasaAsync(int productId, string author, int publishingTime, int productLong, int productWide, int productHigh, int productWeight, int hardCover, int minAge, int maxAge, string detail, int pageNumber)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlParameter[] paras = new MySqlParameter[12];
            paras[0] = new MySqlParameter("@inproductId", productId);
            paras[1] = new MySqlParameter("@inauthor", author);
            paras[2] = new MySqlParameter("@inpublishingTime", publishingTime);
            paras[3] = new MySqlParameter("@inproductLong", productLong);
            paras[4] = new MySqlParameter("@inproductWide", productWide);
            paras[5] = new MySqlParameter("@inproductHigh", productHigh);
            paras[6] = new MySqlParameter("@inproductWeight", productWeight);
            paras[7] = new MySqlParameter("@inhardCover", hardCover);
            paras[8] = new MySqlParameter("@inminAge", minAge);
            paras[9] = new MySqlParameter("@inmaxAge", maxAge);
            paras[10] = new MySqlParameter("@indetail", detail);
            paras[11] = new MySqlParameter("@inpageNumber", pageNumber);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_From_Fahasa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(paras);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<List<ProductIdName>> GetListProductNameAsync()
        {
            List<ProductIdName> ls = new List<ProductIdName>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Name", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            int nameIndex = rdr.GetOrdinal("Name");
                            while (await rdr.ReadAsync())
                                ls.Add(new ProductIdName(rdr.GetInt32(idIndex), rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex)));
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<int> GetProductIdFromNameAsync(string productName)
        {
            int id = -1;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_With_Name", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@productName", productName);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync()) { id = MyMySql.GetInt32(rdr, "Id"); break; }
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return id;
        }

        public static async Task<List<string>> GetListBarcodeAsync()
        {
            List<string> ls = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Barcode", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int barcodeIndex = rdr.GetOrdinal("Barcode");
                            while (await rdr.ReadAsync()) ls.Add(rdr.IsDBNull(barcodeIndex) ? string.Empty : rdr.GetString(barcodeIndex));
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<List<string>> GetListComboNameAsync()
        {
            List<string> ls = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_ComboName", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int comboNameIndex = rdr.GetOrdinal("ComboName");
                            while (await rdr.ReadAsync()) ls.Add(rdr.GetString(comboNameIndex));
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<List<string>> GetListAuthorAsync()
        {
            List<string> ls = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Author", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int authorIndex = rdr.GetOrdinal("Author");
                            while (await rdr.ReadAsync()) ls.Add(rdr.IsDBNull(authorIndex) ? string.Empty : rdr.GetString(authorIndex));
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<List<string>> GetListTranslatorAsync()
        {
            List<string> ls = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_Translator", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int translatorIndex = rdr.GetOrdinal("Translator");
                            while (await rdr.ReadAsync()) ls.Add(rdr.IsDBNull(translatorIndex) ? string.Empty : rdr.GetString(translatorIndex));
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<List<string>> GetListPublishingCompanyAsync()
        {
            List<string> ls = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_All_PublishingCompany", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int publishingCompanyIndex = rdr.GetOrdinal("PublishingCompany");
                            while (await rdr.ReadAsync()) ls.Add(rdr.GetString(publishingCompanyIndex));
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<List<int>> GetListDifferenceIntValueAsync(int inType)
        {
            List<int> ls = new List<int>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Difference_INT_Value", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inType", inType);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int difValueIndex = rdr.GetOrdinal("difValue");
                            while (await rdr.ReadAsync()) ls.Add(rdr.IsDBNull(difValueIndex) ? 0 : rdr.GetInt32(difValueIndex));
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<List<Product>> GetProductIdCodeBarcodeNameBookCoverPriceAsync(int publisherId)
        {
            List<Product> ls = new List<Product>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Select_Id_Code_Barcode_Name_BookCoverPrice_All", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inPublisherId", publisherId);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            int codeIndex = rdr.GetOrdinal("Code");
                            int barcodeIndex = rdr.GetOrdinal("Barcode");
                            int nameIndex = rdr.GetOrdinal("Name");
                            int bookCoverPriceIndex = rdr.GetOrdinal("BookCoverPrice");
                            int comboIdIndex = rdr.GetOrdinal("ComboId");
                            int comboCodeIndex = rdr.GetOrdinal("ComboCode");
                            int comboNameIndex = rdr.GetOrdinal("ComboName");
                            while (await rdr.ReadAsync())
                            {
                                Product product = new Product();
                                product.id = rdr.GetInt32(idIndex);
                                product.code = rdr.IsDBNull(codeIndex) ? string.Empty : rdr.GetString(codeIndex);
                                product.barcode = rdr.IsDBNull(barcodeIndex) ? string.Empty : rdr.GetString(barcodeIndex);
                                product.name = rdr.GetString(nameIndex);
                                product.bookCoverPrice = rdr.GetInt32(bookCoverPriceIndex);
                                product.comboId = rdr.GetInt32(comboIdIndex);
                                product.comboCode = rdr.IsDBNull(comboCodeIndex) ? string.Empty : rdr.GetString(comboCodeIndex);
                                product.comboName = rdr.IsDBNull(comboNameIndex) ? string.Empty : rdr.GetString(comboNameIndex);
                                product.SetFirstSrcImage();
                                ls.Add(product);
                            }
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<MySqlResultState> AddListImportAsync(List<Import> ls)
        {
            MySqlResultState result = new MySqlResultState();
            bool isOK = true;
            MySqlParameter[] paras = new MySqlParameter[7];
            paras[0] = new MySqlParameter("@inProductId", 0);
            paras[1] = new MySqlParameter("@inPrice", 0);
            paras[2] = new MySqlParameter("@inQuantity", 0);
            paras[3] = new MySqlParameter("@inBookCoverPrice", 0);
            paras[4] = new MySqlParameter("@inDiscount", 0f);
            MyMySql.AddOutParameters(paras);
            foreach (var obj in ls)
            {
                paras[0].Value = obj.productId;
                paras[1].Value = obj.price;
                paras[2].Value = obj.quantity;
                paras[3].Value = obj.bookCoverPrice;
                paras[4].Value = obj.discount;
                result = await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbImport_Insert", paras);
                if (result.State != EMySqlResultState.OK) isOK = false;
            }
            result.State = isOK ? EMySqlResultState.OK : EMySqlResultState.ERROR;
            return result;
        }

        public static async Task<List<Import>> GetImportListAsync(string fromDate, string toDate, string publisher)
        {
            List<Import> ls = new List<Import>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbImport_Select", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inFromDate", fromDate);
                        cmd.Parameters.AddWithValue("@inToDate", toDate);
                        cmd.Parameters.AddWithValue("@inPublisher", publisher);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            int productIdIndex = rdr.GetOrdinal("ProductId");
                            int productNameIndex = rdr.GetOrdinal("Name");
                            int priceIndex = rdr.GetOrdinal("Price");
                            int quantityIndex = rdr.GetOrdinal("Quantity");
                            int bookCoverPriceIndex = rdr.GetOrdinal("BookCoverPrice");
                            int dateImportIndex = rdr.GetOrdinal("Date");
                            int discountIndex = rdr.GetOrdinal("Discount");
                            while (await rdr.ReadAsync())
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
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
            return ls;
        }

        public static async Task<MySqlResultState> UpdateListImportAsync(List<Import> ls)
        {
            MySqlResultState result = new MySqlResultState();
            int lengthPara = 5;
            MySqlParameter[] paras = new MySqlParameter[lengthPara];
            paras[0] = new MySqlParameter("@inId", (object)0);
            paras[1] = new MySqlParameter("@inPrice", (object)0);
            paras[2] = new MySqlParameter("@inDiscount", (object)0);
            MyMySql.AddOutParameters(paras);
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbImport_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(paras);
                        foreach (var obj in ls)
                        {
                            paras[0].Value = obj.id;
                            paras[1].Value = obj.price;
                            paras[2].Value = obj.discount;
                            await cmd.ExecuteNonQueryAsync();
                            result.State = (EMySqlResultState)cmd.Parameters[lengthPara - 2].Value;
                        }
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateOutputAndProductTableFromFromListImportAsync(int orderId, List<Import> ls, ECommerceOrderStatus status, EECommerceType eCommerceType)
        {
            MySqlResultState resultState = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOutput_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCode", orderId);
                        cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                        cmd.Parameters.AddWithValue("@inProductId", 0);
                        cmd.Parameters.AddWithValue("@inQuantity", 0);
                        foreach (var im in ls)
                        {
                            if (im.quantity == 0) continue;
                            cmd.Parameters[2].Value = im.productId;
                            cmd.Parameters[3].Value = status == ECommerceOrderStatus.RETURNED ? im.quantity * -1 : im.quantity;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, resultState); }
            }
            return resultState;
        }

        public static async Task<MySqlResultState> CreateOrderManuallyAsync(List<Import> ls, int sumPay)
        {
            int orderId = await OrderMySql.AddOrderAsync(-1, "", 1, null);
            MySqlResultState result = await UpdateOutputAndProductTableFromFromListImportAsync(orderId, ls, ECommerceOrderStatus.PACKED, EECommerceType.PLAY_WITH_ME);
            if (result.State != EMySqlResultState.OK) return result;
            OrderPay orderPay = new OrderPay();
            orderPay.type = EPayType.SUM;
            orderPay.value = sumPay;

            await OrderMySql.AddPayOrderAsync(orderId, new List<OrderPay> { orderPay });
            return result;
        }

        public static async Task<List<Product>> SearchProductForMappingAsync(ProductSearchParameter searchParameter, MySqlConnection conn)
        {
            List<Product> ls = new List<Product>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_For_Mapping", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCodeOrBarcode", searchParameter.codeOrBarcode);
                    cmd.Parameters.AddWithValue("@inName", searchParameter.name);
                    cmd.Parameters.AddWithValue("@inCombo", searchParameter.combo);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = rdr.GetOrdinal("Id");
                        int nameIndex = rdr.GetOrdinal("Name");
                        int statusIndex = rdr.GetOrdinal("Status");
                        while (await rdr.ReadAsync())
                        {
                            Product product = new Product();
                            product.id = rdr.GetInt32(idIndex);
                            product.name = rdr.GetString(nameIndex);
                            product.status = rdr.GetInt32(statusIndex);
                            product.SetFirstSrcImage();
                            ls.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            return ls;
        }

        public static async Task<List<Product>> GetSimpleComboCategoryAllConnectOutAsync(MySqlConnection conn)
        {
            List<Product> ls = new List<Product>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Get_Simple_Combo_All", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = rdr.GetOrdinal("Id");
                        int nameIndex = rdr.GetOrdinal("Name");
                        int comboIdIndex = rdr.GetOrdinal("ComboId");
                        int comboNameIndex = rdr.GetOrdinal("ComboName");
                        int categoryIdIndex = rdr.GetOrdinal("CategoryId");
                        int categoryNameIndex = rdr.GetOrdinal("CategoryName");
                        while (await rdr.ReadAsync())
                        {
                            Product product = new Product();
                            product.id = rdr.GetInt32(idIndex);
                            product.name = rdr.GetString(nameIndex);
                            product.comboId = rdr.GetInt32(comboIdIndex);
                            product.comboName = rdr.IsDBNull(comboNameIndex) ? string.Empty : rdr.GetString(comboNameIndex);
                            product.categoryId = rdr.GetInt32(categoryIdIndex);
                            product.categoryName = rdr.IsDBNull(categoryNameIndex) ? string.Empty : rdr.GetString(categoryNameIndex);
                            ls.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            return ls;
        }

        public static async Task<List<Product>> SearchProductFromTMDTNameForMappingAsync(string tmdtItemName, string tmdtModelName, MySqlConnection conn)
        {
            List<Product> ls = new List<Product>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Search_From_TMDT_Name_For_Mapping", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTItemName", tmdtItemName);
                    cmd.Parameters.AddWithValue("@inTMDTModelName", tmdtModelName);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int idIndex = rdr.GetOrdinal("Id");
                        int nameIndex = rdr.GetOrdinal("Name");
                        int statusIndex = rdr.GetOrdinal("Status");
                        while (await rdr.ReadAsync())
                        {
                            Product product = new Product();
                            product.id = rdr.GetInt32(idIndex);
                            product.name = rdr.GetString(nameIndex);
                            product.status = rdr.GetInt32(statusIndex);
                            product.SetFirstSrcImage();
                            ls.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            return ls;
        }

        public static async Task<MySqlResultState> UpdateNameAsync(int id, string name)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inProductName", name);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbProducts_Update_Name", paras);
        }

        public static async Task<MySqlResultState> UpdateCodeAsync(int id, string code)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inCode", code);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbProducts_Update_Code", paras);
        }

        // C?p nh?t c? ? tbNeedUpdateQuantity n?u có thay d?i t?n kho
        public static async Task<MySqlResultState> UpdateQuantityAsync(int id, int quantity)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_Quantity", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inQuantity", quantity);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        // // C?p nh?t c? ? tbNeedUpdateQuantity n?u có thay d?i t?n kho
        public static async Task<MySqlResultState> UpdateQuantityFromListAsync(List<int> lsId, List<int> lsQuantity)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_Quantity", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", 0);
                        cmd.Parameters.AddWithValue("@inQuantity", 0);
                        for (int i = 0; i < lsId.Count; i++)
                        {
                            cmd.Parameters["@inId"].Value = lsId[i];
                            cmd.Parameters["@inQuantity"].Value = lsQuantity[i];
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateISBNAsync(int id, string isbn)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inId", id);
            paras[1] = new MySqlParameter("@inBarcode", isbn);
            MyMySql.AddOutParameters(paras);
            return await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbProducts_Update_Barcode", paras);
        }

        public static async Task<MySqlResultState> UpdateDetailAsync(int id, string detail)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE webplaywithme.tbproducts SET Detail = @inDetail WHERE Id = @inId;", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inDetail", detail);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex) { Common.SetResultException(ex, result); }
            return result;
        }

        public static async Task<MySqlResultState> UpdateBookCoverPriceAsync(int id, int bookCoverPrice)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_BookeCoverPrice", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inBookCoverPrice", bookCoverPrice);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateDiscountWhenImportAsync(int id, float discount)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts`SET `Discount` = @inDiscount WHERE `Id` = @inId;", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inDiscount", discount);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdatePositionInWarehouseAsync(int id, string positionInWarehouse)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_PositionInWarehouse", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inpositionInWarehouse", positionInWarehouse);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateStatusOfProductAsync(int id, int statusOfProduct)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_Status", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inStatus", statusOfProduct);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateComboIdAsync(int id, int comboId)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_ComboId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inComboId", comboId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateCategoryIdAsync(int id, int categoryId)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_CategoryId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inCategoryId", categoryId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdatePublisherIdAsync(int id, int publisherId)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbProducts_Update_PublisherId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inPublisherId", publisherId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdatePublishingCompanyAsync(int id, string publishingCompany)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts`SET `PublishingCompany` = @inPublishingCompany WHERE `Id` = @inId;", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inPublishingCompany", publishingCompany);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<MySqlResultState> UpdateLanguageAsync(int id, string language)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE `tbProducts`SET `Language` = @inLanguage WHERE `Id` = @inId;", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inId", id);
                        cmd.Parameters.AddWithValue("@inLanguage", language);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { Common.SetResultException(ex, result); }
            }
            return result;
        }

        public static async Task<List<Product>> GetListProductInWarehouseChangedQuantityAsync()
        {
            List<Product> ls = new List<Product>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbNeedUpdateQuantity_Get_List", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            int nameIndex = rdr.GetOrdinal("Name");
                            int quantityIndex = rdr.GetOrdinal("Quantity");
                            while (await rdr.ReadAsync())
                            {
                                Product pro = new Product();
                                pro.id = rdr.GetInt32(idIndex);
                                pro.name = rdr.GetString(nameIndex);
                                pro.quantity = rdr.GetInt32(quantityIndex);
                                pro.SetFirstSrcImage();
                                ls.Add(pro);
                            }
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); ls.Clear(); }
            }
            return ls;
        }

        public static async Task UpdateStatusOfNeedUpdateQuantityAsync(List<int> listProId)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE tbNeedUpdateQuantity SET Status = 0 WHERE ProductId=@inProductId;", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inProductId", 0);
                        foreach (int id in listProId)
                        {
                            cmd.Parameters[0].Value = id;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
            }
        }

        public static async Task UpdateStatusOfNeedUpdateQuantityConnectOutAsync(List<int> listProId, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE tbNeedUpdateQuantity SET Status = 0 WHERE ProductId=@inProductId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inProductId", 0);
                    foreach (int id in listProId)
                    {
                        cmd.Parameters[0].Value = id;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
        }

        public static async Task<List<int>> GetListProductOfNeedUpdateQuantityConnectOutAsync(MySqlConnection conn)
        {
            List<int> listProductId = new List<int>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `ProductId` FROM `tbNeedUpdateQuantity` WHERE `Status`=1;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int productIdIndex = rdr.GetOrdinal("ProductId");
                        while (await rdr.ReadAsync())
                        {
                            listProductId.Add(rdr.GetInt32(productIdIndex));
                        }
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); listProductId.Clear(); }
            return listProductId;
        }

        public static async Task UpdateImageSrcShopeeItemAsync(long TMDTShopeeItemId, string imageSrc, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE tbShopeeItem SET Image = @inImage WHERE TMDTShopeeItemId=@inTMDTShopeeItemId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inImage", imageSrc);
                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", TMDTShopeeItemId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
        }

        public static async Task UpdateImageSrcShopeeModelAsync(long TMDTShopeeModelId, string imageSrc, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE tbShopeeModel SET Image = @inImage WHERE TMDTShopeeModelId=@inTMDTShopeeModelId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inImage", imageSrc);
                    cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", TMDTShopeeModelId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
        }

        public static async Task UpdateImageSrcTikiItemAsync(int TikiId, string imageSrc, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE tbTikiItem SET Image = @inImage WHERE TikiId=@inTikiId;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inImage", imageSrc);
                    cmd.Parameters.AddWithValue("@inTikiId", TikiId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); }
        }

        public static async Task<int> ShopeeGetQuantityOfOneItemModelConnectOutAsync(long itemId, long modelId, MySqlConnection conn)
        {
            int quantity = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Get_Quantity", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inItemId", itemId);
                    cmd.Parameters.AddWithValue("@inModelId", modelId);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int quantityIndex = rdr.GetOrdinal("Quantity");
                        while (await rdr.ReadAsync()) quantity = rdr.GetInt32(quantityIndex);
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); quantity = 0; }
            return quantity;
        }

        public static async Task<int> LazadaGetQuantityOfOneItemModelConnectOutAsync(long itemId, long modelId, MySqlConnection conn)
        {
            int quantity = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaModel_Get_Quantity", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inItemId", itemId);
                    cmd.Parameters.AddWithValue("@inModelId", modelId);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int quantityIndex = rdr.GetOrdinal("Quantity");
                        while (await rdr.ReadAsync()) quantity = rdr.GetInt32(quantityIndex);
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); quantity = 0; }
            return quantity;
        }

        public static async Task<int> TikiGetQuantityOfOneItemModelConnectOutAsync(int itemId, MySqlConnection conn)
        {
            int quantity = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_Quantity", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTTikiItemId", itemId);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int quantityIndex = rdr.GetOrdinal("Quantity");
                        while (await rdr.ReadAsync()) quantity = rdr.GetInt32(quantityIndex);
                    }
                }
            }
            catch (Exception ex) { MyLogger.GetInstance().Warn(ex.ToString()); quantity = 0; }
            return quantity;
        }

        public static async Task<List<ProductSellingStatistics>> GetProductSellingStatisticsAsync(int eCommmerce, int intervalDay, int publisherId, MySqlConnection conn)
        {
            var statisticsList = new List<ProductSellingStatistics>();
            using (var command = new MySqlCommand("st_tbOutput_Selling_Statistics", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("inECommmerce", eCommmerce);
                command.Parameters.AddWithValue("inIntervalDay", intervalDay);
                command.Parameters.AddWithValue("inPublisherId", publisherId);
                using (MySqlDataReader rdr = (MySqlDataReader)await command.ExecuteReaderAsync())
                {
                    int productIdIndex = rdr.GetOrdinal("Id");
                    int nameIndex = rdr.GetOrdinal("Name");
                    int publisherIdIndex = rdr.GetOrdinal("PublisherId");
                    int soldQuantityIndex = rdr.GetOrdinal("SoldQuantity");
                    int quantityIndex = rdr.GetOrdinal("Quantity");
                    int newImportedQuantityIndex = rdr.GetOrdinal("NewImportedQuantity");
                    int comboIdIndex = rdr.GetOrdinal("ComboId");
                    int daysDifferenceIndex = rdr.GetOrdinal("DaysDifference");
                    while (await rdr.ReadAsync())
                    {
                        statisticsList.Add(new ProductSellingStatistics
                        {
                            id = rdr.GetInt32(productIdIndex),
                            name = rdr.GetString(nameIndex),
                            publisherId = rdr.GetInt32(publisherIdIndex),
                            soldQuantity = rdr.IsDBNull(soldQuantityIndex) ? 0 : rdr.GetInt32(soldQuantityIndex),
                            quantityInWarehouse = rdr.GetInt32(quantityIndex),
                            newImportedQuantity = rdr.IsDBNull(newImportedQuantityIndex) ? 0 : rdr.GetInt32(newImportedQuantityIndex),
                            comboId = rdr.GetInt32(comboIdIndex),
                            daysDifference = rdr.IsDBNull(daysDifferenceIndex) ? intervalDay + 1 : rdr.GetInt32(daysDifferenceIndex)
                        });
                    }
                }
            }
            return statisticsList;
        }

        public static async Task<List<Output>> GetOutputOfProductAsync(int productId, MySqlConnection conn)
        {
            var list = new List<Output>();
            using (var command = new MySqlCommand("st_tbOutput_Get_Of_Product", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("inProductId", productId);
                using (MySqlDataReader rdr = (MySqlDataReader)await command.ExecuteReaderAsync())
                {
                    int idIndex = rdr.GetOrdinal("Id");
                    int codeIndex = rdr.GetOrdinal("Code");
                    int eCommmerceIndex = rdr.GetOrdinal("ECommmerce");
                    int productIdIndex = rdr.GetOrdinal("ProductId");
                    int quantityIndex = rdr.GetOrdinal("Quantity");
                    int timeIndex = rdr.GetOrdinal("Time");
                    int eCommerceOrderIndex = rdr.GetOrdinal("ECommerceOrder");
                    while (await rdr.ReadAsync())
                    {
                        list.Add(new Output
                        {
                            id = rdr.GetInt32(idIndex),
                            code = rdr.GetString(codeIndex),
                            eCommmerce = rdr.GetInt32(eCommmerceIndex),
                            productId = rdr.GetInt32(productIdIndex),
                            quantity = rdr.GetInt32(quantityIndex),
                            time = rdr.GetDateTime(timeIndex),
                            isCancel = rdr.IsDBNull(eCommerceOrderIndex) ? false : true
                        });
                    }
                }
            }
            return list;
        }
    }
}
