﻿using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.ProductModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ItemModel
{
    public class ItemModelMySql
    {
        public List<BasicIdName> GetListItemName()
        {
            List<BasicIdName> ls = new List<BasicIdName>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Select_All_Name", conn);
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

        private void ItemParameters(Item item, MySqlParameter[] paras)
        {
            paras[0] = new MySqlParameter("@inId", item.id);
            paras[1] = new MySqlParameter("@inName", item.name);
            paras[2] = new MySqlParameter("@inStatus", item.status);
            paras[3] = new MySqlParameter("@inDetail", item.detail);
            paras[4] = new MySqlParameter("@inQuota", item.quota);
            paras[5] = new MySqlParameter("@inCategoryId", item.categoryId);

            MyMySql.AddOutParameters(paras);
        }

        public int AddItem(Item item)
        {
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[5];
            paras[0] = new MySqlParameter("@inName", item.name);
            paras[1] = new MySqlParameter("@inStatus", item.status);
            paras[2] = new MySqlParameter("@inDetail", item.detail);
            paras[3] = new MySqlParameter("@inQuota", item.quota);
            paras[4] = new MySqlParameter("@inCategoryId", item.categoryId);

            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "LastId");
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

        public int AddItem(string itemName, int itemStatus, string itemDetail)
        {
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[5];
            paras[0] = new MySqlParameter("@inName", itemName);
            paras[1] = new MySqlParameter("@inStatus", itemStatus);
            paras[2] = new MySqlParameter("@inDetail", itemDetail);
            paras[3] = new MySqlParameter("@inQuota",5);
            paras[4] = new MySqlParameter("@inCategoryId", 0);

            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "LastId");
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

        public int GetItemIdFromName(string itemName)
        {
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[1];
            paras[0] = new MySqlParameter("@inName", itemName);

            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Get_Id_From_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "Id");
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

        public MySqlResultState UpdateItem(Item it)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[8];
            ItemParameters(it, paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbItem_Update", paras);

            return result;
        }

        /// <summary>
        /// Lấy giá trị item id lớn nhất
        /// </summary>
        /// <returns> -1 nếu có lỗi </returns>
        public int GetMaxItemId()
        {
            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Get_Max_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;

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
                id = -1;
            }

            conn.Close();
            return id;
        }

        /// <summary>
        /// Lấy giá trị model id lớn nhất
        /// </summary>
        /// <returns> -1 nếu có lỗi </returns>
        public int GetMaxModelId()
        {
            int id = -1;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Get_Max_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;

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
                id = -1;
            }

            conn.Close();
            return id;
        }

        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product
        /// </summary>
        /// <returns></returns>
        private Model ConvertFromDataMySql(MySqlDataReader rdr)
        {
            Model model = new Model();
            model.id = MyMySql.GetInt32(rdr, "Id");
            model.itemId = MyMySql.GetInt32(rdr, "ItemId");
            model.name = MyMySql.GetString(rdr, "Name");
            model.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
            model.price = MyMySql.GetInt32(rdr, "Price");
            model.status = MyMySql.GetInt32(rdr, "Status");
            model.quota = MyMySql.GetInt32(rdr, "Quota");
            model.quantity = MyMySql.GetInt32(rdr, "Quantity");
            return model;
        }

        private void ModelParameters(Model model, MySqlParameter[] paras)
        {
            paras[0] = new MySqlParameter("@inId", model.id);
            paras[1] = new MySqlParameter("@inItemId", model.itemId);
            paras[2] = new MySqlParameter("@inName", model.name);
            paras[3] = new MySqlParameter("@inQuota", model.quota);
            paras[4] = new MySqlParameter("@inDiscount", model.discount);

            MyMySql.AddOutParameters(paras);
        }

        public int AddModel(Model model)
        {
            int id = -1;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inItemId", model.itemId);
            paras[1] = new MySqlParameter("@inName", model.name);
            paras[2] = new MySqlParameter("@inQuota", model.quota);
            paras[3] = new MySqlParameter("@inDiscount", model.discount);
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "LastId");
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

        public MySqlResultState UpdateModel(Model model)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[7];
            ModelParameters(model, paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbModel_Update", paras);

            return result;
        }

        // Xóa model sản phẩm, mapping sản phẩm trong kho tương ứng,
        // mapping sản phẩm trên Shopee, Tiki, Lazada tương ứng
        public MySqlResultState DeleteModel(int id)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlParameter[] paras = null;
            int lengthPara = 3;
            paras = new MySqlParameter[lengthPara];
            paras[0] = new MySqlParameter("@inId", id);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbModel_Delete_From_Id", paras);

            return result;
        }

        // Xóa item, model thuộc item, mapping sản phẩm trong kho tương ứng,
        // mapping sản phẩm trên Shopee, Tiki, Lazada tương ứng
        public MySqlResultState DeleteItem(int id)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Delete_From_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();

            return result;
        }

        public MySqlResultState AddMapping(Model model)
        {
            MySqlResultState result = new MySqlResultState();
            if (model.mapping.Count() == 0)
            {
                return result;
            }

            MySqlParameter[] paras = null;

            paras = new MySqlParameter[5];
            paras[0] = new MySqlParameter("@inModelId", model.id);
            paras[1] = new MySqlParameter("@inProductId", (object)0);
            paras[2] = new MySqlParameter("@inQuantity", (object)0);
            MyMySql.AddOutParameters(paras);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbMapping_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                foreach (var map in model.mapping)
                {
                    paras[1].Value = map.product.id;
                    paras[2].Value = map.quantity;
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

        // Xóa tất cả mapping của model
        public MySqlResultState DeleteMapping( int modelId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlParameter[] paras = null;

            paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inModelId", modelId);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbMapping_Delete_From_ModelId", paras);

            return result;
        }

        ///// <summary>
        ///// TỪ dữ liệu select db, ta trả về đối tượng Product
        ///// </summary>
        ///// <param name="rdr">Trả về ngay từ câu select</param>
        ///// <returns></returns>
        //public static Product ConvertOneRowFromDataMySqlToProduct(MySqlDataReader rdr)
        //{
        //    Product product = new Product();
        //    product.id = MyMySql.GetInt32(rdr, "ProductId");
        //    product.code = MyMySql.GetString(rdr, "ProductCode");
        //    product.barcode = MyMySql.GetString(rdr, "ProductBarcode");
        //    product.name = MyMySql.GetString(rdr, "ProductName");
        //    product.comboId = MyMySql.GetInt32(rdr, "ComboId");
        //    product.comboName = MyMySql.GetString(rdr, "ComboName");
        //    product.categoryId = MyMySql.GetInt32(rdr, "CategoryId");
        //    product.categoryName = MyMySql.GetString(rdr, "CategoryName");
        //    product.bookCoverPrice = MyMySql.GetInt32(rdr, "ProductBookCoverPrice");
        //    product.author = MyMySql.GetString(rdr, "ProductAuthor");
        //    product.translator = MyMySql.GetString(rdr, "ProductTranslator");
        //    product.publisherId = MyMySql.GetInt32(rdr, "PublisherId");
        //    product.publisherName = MyMySql.GetString(rdr, "PublisherName");
        //    product.publishingCompany = MyMySql.GetString(rdr, "ProductPublishingCompany");
        //    product.publishingTime = MyMySql.GetInt32(rdr, "ProductPublishingTime");
        //    product.productLong = MyMySql.GetInt32(rdr, "ProductLong");
        //    product.productWide = MyMySql.GetInt32(rdr, "ProductWide");
        //    product.productHigh = MyMySql.GetInt32(rdr, "ProductHigh");
        //    product.productWeight = MyMySql.GetInt32(rdr, "ProductWeight");
        //    product.positionInWarehouse = MyMySql.GetString(rdr, "ProductPositionInWarehouse");
        //    product.hardCover = MyMySql.GetInt32(rdr, "ProductHardCover");
        //    product.minAge = MyMySql.GetInt32(rdr, "ProductMinAge");
        //    product.maxAge = MyMySql.GetInt32(rdr, "ProductMaxAge");
        //    product.parentId = MyMySql.GetInt32(rdr, "ParentId");
        //    product.republish = MyMySql.GetInt32(rdr, "ProductRepublish");
        //    product.detail = MyMySql.GetString(rdr, "ProductDetail");
        //    product.status = MyMySql.GetInt32(rdr, "ProductStatus");
        //    product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");

        //    product.SetSrcImageVideo();

        //    return product;
        //}

        private void ConvertOneRowFromDataMySqlToModel(MySqlDataReader rdr, List<Model> models)
        {
            int modelId = MyMySql.GetInt32(rdr, "ModelId");
            if (modelId != -1)// item có model
            {
                // check models đã có model này chưa? Chỉ cần check phần tử cuối cùng của models
                if (models.Count() == 0 || models[models.Count() - 1].id != modelId)
                {
                    Model model = new Model();
                    model.id = modelId;
                    model.itemId = MyMySql.GetInt32(rdr, "ItemId");
                    model.name = MyMySql.GetString(rdr, "ModelName");

                    model.quota = MyMySql.GetInt32(rdr, "ModelQuota");
                    model.discount = rdr.IsDBNull(rdr.GetOrdinal("ModelDiscount")) ? 0 : rdr.GetFloat("ModelDiscount");
                    model.price = MyMySql.GetInt32(rdr, "ModelPrice");
                    model.soldQuantity = MyMySql.GetInt32(rdr, "ModelSoldQuantity");
                    model.bookCoverPrice = MyMySql.GetInt32(rdr, "ModelBookCoverPrice");

                    model.SetSrcImage();

                    models.Add(model);
                }

                Product pro = new Product();
                pro.id = MyMySql.GetInt32(rdr, "ProductId");
                pro.name = MyMySql.GetString(rdr, "ProductName");
                pro.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
                pro.SetFirstSrcImage();

                int quan = MyMySql.GetInt32(rdr, "MappingQuantity");
                if (pro.id != -1)
                    models[models.Count() - 1].mapping.Add( new Mapping(pro, quan));
            }
        }

        private Item ConvertOneRowFromDataMySqlToItem(MySqlDataReader rdr)
        {
            Item item = new Item();
            item.id = MyMySql.GetInt32(rdr, "ItemId");
            item.name = MyMySql.GetString(rdr, "ItemName");
            item.quota = MyMySql.GetInt32(rdr, "ItemQuota");
            item.status = MyMySql.GetInt32(rdr, "ItemStatus");
            item.date = MyMySql.GetDateTime(rdr, "ItemDate");
            item.detail = MyMySql.GetString(rdr, "ItemDetail");
            item.categoryId = MyMySql.GetInt32(rdr, "ItemCategoryId");
            item.SetSrcImageVideo();
            //ite

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namePara"></param>
        /// <param name="start">Tính từ 0</param>
        /// <param name="offset">Số item max trên 1 page</param>
        /// <returns></returns>
        public List<Item> SearchItemPage(ItemModelSearchParameter searchParameter)
        {
            List<Item> ls = new List<Item>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItemModel_Search", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisherIdPara", searchParameter.publisherId);
                cmd.Parameters.AddWithValue("@inNamePara", searchParameter.name);
                cmd.Parameters.AddWithValue("@inStart", searchParameter.start);
                cmd.Parameters.AddWithValue("@inOffset", searchParameter.offset);
                int itemId = 0;
                Item itemTemp = null;
                MySqlDataReader rdr = cmd.ExecuteReader();

                int itemIdIndex = rdr.GetOrdinal("ItemId");
                int itemNameIndex = rdr.GetOrdinal("ItemName");

                int modelIdIndex = rdr.GetOrdinal("ModelId");
                int modelNameIndex = rdr.GetOrdinal("ModelName");
                int modelPriceIndex = rdr.GetOrdinal("ModelPrice");
                int modelBookCoverPriceIndex = rdr.GetOrdinal("ModelBookCoverPrice");
                int modelSoldQuantityIndex = rdr.GetOrdinal("ModelSoldQuantity");
                int modelDiscountIndex = rdr.GetOrdinal("ModelDiscount");
                while (rdr.Read())
                {
                    itemId = rdr.GetInt32(itemIdIndex);
                    if(ls.Count == 0 || ls[ls.Count - 1].id != itemId) // Thêm mới item
                    {
                        Item item = new Item();
                        item.id = itemId;
                        item.name = rdr.IsDBNull(itemNameIndex) ? string.Empty : rdr.GetString(itemNameIndex); ;
                        item.SetFirstSrcImage();
                        ls.Add(item);
                    }
                    itemTemp = ls[ls.Count - 1];
                    // Thêm model
                    {
                        Model model = new Model();
                        model.id = rdr.GetInt32(modelIdIndex);
                        model.itemId = itemTemp.id;
                        model.name = rdr.IsDBNull(modelNameIndex) ? string.Empty : rdr.GetString(modelNameIndex);

                        model.price = rdr.IsDBNull(modelPriceIndex) ? 0 : rdr.GetInt32(modelPriceIndex);
                        model.bookCoverPrice = rdr.IsDBNull(modelBookCoverPriceIndex) ? 0 : rdr.GetInt32(modelBookCoverPriceIndex);
                        model.soldQuantity = rdr.IsDBNull(modelSoldQuantityIndex) ? 0 : rdr.GetInt32(modelSoldQuantityIndex);
                        model.discount = rdr.IsDBNull(modelDiscountIndex) ? 0 : rdr.GetFloat(modelDiscountIndex);
                        itemTemp.models.Add(model);
                    }
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

        public List<Item> SearchItemPageConnectOut(ItemModelSearchParameter searchParameter,
            MySqlConnection conn)
        {
            List<Item> ls = new List<Item>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbItemModel_Search", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisherIdPara", searchParameter.publisherId);
                cmd.Parameters.AddWithValue("@inNamePara", searchParameter.name);
                cmd.Parameters.AddWithValue("@inStart", searchParameter.start);
                cmd.Parameters.AddWithValue("@inOffset", searchParameter.offset);
                int itemId = 0;
                Item itemTemp = null;
                MySqlDataReader rdr = cmd.ExecuteReader();
                int itemIdIndex = rdr.GetOrdinal("ItemId");
                int itemNameIndex = rdr.GetOrdinal("ItemName");

                int modelIdIndex = rdr.GetOrdinal("ModelId");
                int modelNameIndex = rdr.GetOrdinal("ModelName");
                int modelPriceIndex = rdr.GetOrdinal("ModelPrice");
                int modelBookCoverPriceIndex = rdr.GetOrdinal("ModelBookCoverPrice");
                int modelSoldQuantityIndex = rdr.GetOrdinal("ModelSoldQuantity");
                int modelDiscountIndex = rdr.GetOrdinal("ModelDiscount");
                while (rdr.Read())
                {
                    itemId = rdr.GetInt32(itemIdIndex);
                    if (ls.Count == 0 || ls[ls.Count - 1].id != itemId) // Thêm mới item
                    {
                        Item item = new Item();
                        item.id = itemId;
                        item.name = rdr.IsDBNull(itemNameIndex) ? string.Empty : rdr.GetString(itemNameIndex); ;
                        item.SetFirstSrcImage();
                        ls.Add(item);
                    }
                    itemTemp = ls[ls.Count - 1];
                    // Thêm model
                    {
                        Model model = new Model();
                        model.id = rdr.GetInt32(modelIdIndex);
                        model.itemId = itemTemp.id;
                        model.name = rdr.IsDBNull(modelNameIndex) ? string.Empty : rdr.GetString(modelNameIndex);

                        model.price = rdr.IsDBNull(modelPriceIndex) ? 0 : rdr.GetInt32(modelPriceIndex);
                        model.bookCoverPrice = rdr.IsDBNull(modelBookCoverPriceIndex) ? 0 : rdr.GetInt32(modelBookCoverPriceIndex);
                        model.soldQuantity = rdr.IsDBNull(modelSoldQuantityIndex) ? 0 : rdr.GetInt32(modelSoldQuantityIndex);
                        model.discount = rdr.IsDBNull(modelDiscountIndex) ? 0 : rdr.GetFloat(modelDiscountIndex);
                        itemTemp.models.Add(model);
                    }
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

        public int SearchItemCount(ItemModelSearchParameter searchParameter)
        {
            int count = 0;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Search_Count_Record", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisherIdPara", searchParameter.publisherId);
                cmd.Parameters.AddWithValue("@inNamePara", searchParameter.name);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    count = MyMySql.GetInt32(rdr, "CountRecord");
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return count;
        }

        public int SearchItemCountConnectOut(ItemModelSearchParameter searchParameter, MySqlConnection conn)
        {
            int count = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbItem_Search_Count_Record", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisherIdPara", searchParameter.publisherId);
                cmd.Parameters.AddWithValue("@inNamePara", searchParameter.name);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    count = MyMySql.GetInt32(rdr, "CountRecord");
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return count;
        }

        /// <summary>
        /// Giống hàm SearchItemPage nhưng thêm thông tin mapping phục vụ lọc kết quả ở trang quản trị /ItemModel/Search
        /// </summary>
        /// <param name="namePara"></param>
        /// <param name="start">Tính từ 0</param>
        /// <param name="offset">Số item max trên 1 page</param>
        /// <returns></returns>
        public List<Item> SearchItemPageIncludeMapping(ItemModelSearchParameter searchParameter)
        {
            List<Item> ls = new List<Item>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItemModel_Search_Include_Mapping", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inPublisherIdPara", searchParameter.publisherId);
                cmd.Parameters.AddWithValue("@inNamePara", searchParameter.name);
                cmd.Parameters.AddWithValue("@inStart", searchParameter.start);
                cmd.Parameters.AddWithValue("@inOffset", searchParameter.offset);
                int itemId = 0;
                int modelId = 0;
                Item itemTemp = null;
                Model modelTemp = null;
                MySqlDataReader rdr = cmd.ExecuteReader();
                int itemIdIndex = rdr.GetOrdinal("ItemId");
                int itemNameIndex = rdr.GetOrdinal("ItemName");
                int modelIdIndex = rdr.GetOrdinal("ModelId");
                int modelNameIndex = rdr.GetOrdinal("ModelName");
                int modelPriceIndex = rdr.GetOrdinal("ModelPrice");
                int modelBookCoverPriceIndex = rdr.GetOrdinal("ModelBookCoverPrice");
                int modelSoldQuantityIndex = rdr.GetOrdinal("ModelSoldQuantity");
                int modelDiscountIndex = rdr.GetOrdinal("ModelDiscount");
                int mappingIdIndex = rdr.GetOrdinal("MappingId");
                while (rdr.Read())
                {
                    itemId = rdr.GetInt32(itemIdIndex);
                    if (ls.Count == 0 || ls[ls.Count - 1].id != itemId) // Thêm mới item
                    {
                        itemTemp = new Item();
                        itemTemp.id = itemId;
                        itemTemp.name = rdr.IsDBNull(itemNameIndex) ? string.Empty : rdr.GetString(itemNameIndex);
                        itemTemp.SetFirstSrcImage();
                        ls.Add(itemTemp);
                    }
                    itemTemp = ls[ls.Count - 1];
                    // Thêm model
                    {
                        modelId = rdr.IsDBNull(modelIdIndex) ? -1 : rdr.GetInt32(modelIdIndex);
                        if (itemTemp.models.Count == 0 || itemTemp.models[itemTemp.models.Count - 1].id != modelId) // Thêm mới item
                        {
                            modelTemp = new Model();
                            modelTemp.id = modelId;
                            modelTemp.itemId = itemTemp.id;
                            modelTemp.name = rdr.IsDBNull(modelNameIndex) ? string.Empty : rdr.GetString(modelNameIndex);

                            modelTemp.price = rdr.IsDBNull(modelPriceIndex) ? 0 : rdr.GetInt32(modelPriceIndex);
                            modelTemp.bookCoverPrice = rdr.IsDBNull(modelBookCoverPriceIndex) ?0 : rdr.GetInt32(modelBookCoverPriceIndex);
                            modelTemp.soldQuantity = rdr.IsDBNull(modelSoldQuantityIndex) ? 0 : rdr.GetInt32(modelSoldQuantityIndex);
                            modelTemp.discount = rdr.IsDBNull(modelDiscountIndex) ? 0 : rdr.GetFloat(modelDiscountIndex); ;
                            itemTemp.models.Add(modelTemp);
                        }
                        modelTemp = itemTemp.models[itemTemp.models.Count - 1];
                    }
                    // Thêm mapping
                    {
                        Mapping mapping = new Mapping();
                        mapping.id = rdr.IsDBNull(mappingIdIndex) ? -1 : rdr.GetInt32(mappingIdIndex);
                        modelTemp.mapping.Add(mapping);
                    }
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

        // Lấy được thông tin chi tiết
        public Item GetItemFromId(int id)
        {
            Item item = null;
            List<Model> models = new List<Model>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Get_From_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (item == null)
                    {
                        item = ConvertOneRowFromDataMySqlToItem(rdr);
                    }
                    ConvertOneRowFromDataMySqlToModel(rdr, models);
                }
                rdr.Close();

                foreach (var model in models)
                {
                    model.SetQuantityFromMapping();
                }
                item.models = models;
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
                item = null;
            }

            conn.Close();
            return item;
        }

        // Tương tự hàm GetItemFromId nhưng với conn mở đóng bên ngoài
        public Item GetItemFromIdConnectOut(int id, MySqlConnection conn)
        {
            Item item = null;
            List<Model> models = new List<Model>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbItem_Get_From_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (item == null)
                    {
                        item = ConvertOneRowFromDataMySqlToItem(rdr);
                    }
                    ConvertOneRowFromDataMySqlToModel(rdr, models);
                }
                rdr.Close();

                foreach (var model in models)
                {
                    model.SetQuantityFromMapping();
                }
                item.models = models;
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return item;
        }

        // Lấy được thông tin chi tiết theo model id,
        // những model thuộc item có id khác tham số sẽ không được lấy
        public Item GetItemFromModelId(int id)
        {
            Item item = null;
            List<Model> models = new List<Model>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Get_From_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (item == null)
                    {
                        item = ConvertOneRowFromDataMySqlToItem(rdr);
                    }
                    ConvertOneRowFromDataMySqlToModel(rdr, models);
                }
                rdr.Close();

                foreach (var model in models)
                {
                    model.SetQuantityFromMapping();
                }
                item.models = models;
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return item;
        }

        // Kết nối mở, đóng bên ngoài
        // Lấy được thông tin chi tiết theo model id,
        // những model thuộc item có id khác tham số sẽ không được lấy
        public Item GetItemFromModelIdConnectOut(int id, MySqlConnection conn)
        {
            Item item = null;
            List<Model> models = new List<Model>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbModel_Get_From_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", id);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (item == null)
                    {
                        item = ConvertOneRowFromDataMySqlToItem(rdr);
                    }
                    ConvertOneRowFromDataMySqlToModel(rdr, models);
                }
                rdr.Close();

                if (item != null)
                {
                    foreach (var model in models)
                    {
                        model.SetQuantityFromMapping();
                    }
                    item.models = models;
                }
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return item;
        }

        public void ConvertItemToCartCookie(Item item, Cart cart)
        {
            if (item == null || cart == null)
                return;

            cart.itemId = item.id;
            cart.itemName = item.name;
            cart.modelName = item.models[0].name;
            // Nếu item chỉ có 1 model và model không có ảnh đại diện.
            // Ta lấy ảnh đại diện, tên của item thay
            if (item.models[0].imageSrc != Common.srcNoImageThumbnail)
            {
                cart.imageSrc = item.models[0].imageSrc;
            }
            else
            {
                cart.imageSrc = item.imageSrc[0];
            }
            cart.Copy((PriceQuantity)item.models[0]);
            cart.UpdateQ();
        }

        public MySqlResultState UpdateDiscount(int modelId, float discount)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Update_Discount", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", modelId);
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

        public MySqlResultState UpdateMapping(int modelId,
            List<int> mappingOnlyProductId,
            List<int> mappingOnlyQuantity)
        {
            MySqlResultState result = new MySqlResultState();

            result = DeleteMapping(modelId);
            if(result.State != EMySqlResultState.OK)
            {
                return result;
            }

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbMapping_Insert_V2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inModelId", modelId);
                cmd.Parameters.AddWithValue("@inProductId", 0);
                cmd.Parameters.AddWithValue("@inQuantity", 0);

                int length = mappingOnlyProductId.Count;
                for (int i = 0; i < length; i++)
                {
                    cmd.Parameters[1].Value = mappingOnlyProductId[i];
                    cmd.Parameters[2].Value = mappingOnlyQuantity[i];
                    cmd.ExecuteNonQuery();
                }

                // Cập nhật giá bìa, giá theo chiết khấu (thực tế bán) sau khi thay đổi mapping, chiết khấu,...
                MySqlCommand cmdTemp = new MySqlCommand("st_tbModel_Update_Price", conn);
                cmdTemp.CommandType = CommandType.StoredProcedure;
                cmdTemp.Parameters.AddWithValue("@inModelId", modelId);
                cmdTemp.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState UpdateModelName(int modelId, string name)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Update_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", modelId);
                cmd.Parameters.AddWithValue("@inName", name);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState UpdateItemName(int itemId, string name)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Update_Name", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", itemId);
                cmd.Parameters.AddWithValue("@inName", name);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        // listItemId có dạng: 1,2,3
        public MySqlResultState UpdateDiscountForListItem(float discount, string listItemId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Update_Discount_ListItemId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inDiscount", discount);
                cmd.Parameters.AddWithValue("@inListItemId", listItemId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        // listModelId có dạng: 1,2,3
        public MySqlResultState UpdateDiscountForListModleId(float discount, string listModelId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Update_Discount_ListModelId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inDiscount", discount);
                cmd.Parameters.AddWithValue("@inListModelId", listModelId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState UpdateItemCategory(int itemId, int categoryId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Update_Category", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inId", itemId);
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

        /// <summary>
        /// Xóa nếu tồn tại và insert vào bảng tbModel. Lấy được id mới
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="modelId">Nếu chưa tồn tại mặc định là 0</param>
        /// <param name="modelName"></param>
        /// <param name="quota"></param>
        /// <param name="discount"></param>
        /// <param name="shopeeItemId"></param>
        /// <param name="shopeeModelId"></param>
        /// <returns></returns>
        public MySqlResultState BornModelFromShopeeModel( int itemId, int modelId,
            string modelName,
            int quota, float discount, int price, int bookCoverPrice,
            long shopeeItemId,
            long shopeeModelId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Insert_From_Shopee_Model", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inItemId", itemId);
                cmd.Parameters.AddWithValue("@inModelId", modelId);
                cmd.Parameters.AddWithValue("@inName", modelName);
                cmd.Parameters.AddWithValue("@inQuota", quota);
                cmd.Parameters.AddWithValue("@inDiscount", discount);
                cmd.Parameters.AddWithValue("@inPrice", price);
                cmd.Parameters.AddWithValue("@inBookCoverPrice", bookCoverPrice);
                cmd.Parameters.AddWithValue("@inShopeeItemId", shopeeItemId);
                cmd.Parameters.AddWithValue("@inShopeeModelId", shopeeModelId);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.myAnything = MyMySql.GetInt32(rdr, "LastId");
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

        // Từ model Id sản phẩm trên voibenho lấy được item id
        public MySqlResultState GetVBNItemIdFromModelId(int modelId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT ItemId FROM tbModel WHERE Id=@inModelId;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inModelId", modelId);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.myAnything = MyMySql.GetInt32(rdr, "ItemId");
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

        // Từ model id voibenho lấy được item id shopee
        public MySqlResultState GetTMDTShopeeItemIdFromModelId(int modelId)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbPWMMappingOTher_Get_Shopee_Item_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inModelId", modelId);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.myAnythingLong = MyMySql.GetInt64(rdr, "TMDTShopeeItemId");
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
    }
}