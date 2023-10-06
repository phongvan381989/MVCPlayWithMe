using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class ItemModelMySql : BasicMySql
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

        private void ItemParameters(Item item, MySqlParameter[] paras)
        {
            paras[0] = new MySqlParameter("@inId", item.id);
            paras[1] = new MySqlParameter("@inName", item.name);
            paras[2] = new MySqlParameter("@inStatus", item.status);
            paras[3] = new MySqlParameter("@inDetail", item.detail);
            paras[4] = new MySqlParameter("@inQuota", item.quota);

            MyMySql.AddOutParameters(paras);
        }

        public int AddItem(Item item)
        {
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inName", item.name);
            paras[1] = new MySqlParameter("@inStatus", item.status);
            paras[2] = new MySqlParameter("@inDetail", item.detail);
            paras[3] = new MySqlParameter("@inQuota", item.quota);

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

                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }
            conn.Close();

            return id;
        }

        public MySqlResultState UpdateItem(Item it)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[7];
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

        /// <summary>
        /// Từ list model hiện tại, xóa hết model cũ và trả về list model đã bị xóa
        /// </summary>
        /// <param name="lsNewModel"></param>
        public List<Model> DeleteOldModel(List<int> lsNewModel, int itemId)
        {
            List<Model> lsDeletedModel = new List<Model>();
            List<Model> lsOldModel = new List<Model>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            MySqlResultState result = new MySqlResultState();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbModel_Select_From_ItemId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inItemId", itemId);

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null && rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        lsOldModel.Add(ConvertFromDataMySql(rdr));
                    }
                }
                if (rdr != null)
                    rdr.Close();

                // Xóa model cũ
                MySqlParameter[] paras = null;
                int lengthPara = 3;
                paras = new MySqlParameter[lengthPara];
                paras[0] = new MySqlParameter("@inId", 0);
                MyMySql.AddOutParameters(paras);

                MySqlCommand cmdDel = new MySqlCommand("st_tbModel_Delete_From_Id", conn);
                cmdDel.CommandType = CommandType.StoredProcedure;
                cmdDel.Parameters.AddRange(paras);

                foreach (var old in lsOldModel)
                {
                    if (!lsNewModel.Contains(old.id))
                    {
                        cmdDel.Parameters[0].Value = old.id;
                        lsDeletedModel.Add(old);
                    }
                    cmdDel.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return lsDeletedModel;
        }

        private void ModelParameters(Model model, MySqlParameter[] paras)
        {
            paras[0] = new MySqlParameter("@inId", model.id);
            paras[1] = new MySqlParameter("@inItemId", model.itemId);
            paras[2] = new MySqlParameter("@inName", model.name);
            paras[3] = new MySqlParameter("@inBookCoverPrice", model.bookCoverPrice);
            paras[4] = new MySqlParameter("@inPrice", model.price);
            paras[5] = new MySqlParameter("@inStatus", model.status);
            paras[6] = new MySqlParameter("@inQuota", model.quota);
            paras[7] = new MySqlParameter("@inQuantity", model.quantity);

            MyMySql.AddOutParameters(paras);
        }

        public int AddModel(Model model)
        {
            int id = -1;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[7];
            paras[0] = new MySqlParameter("@inItemId", model.itemId);
            paras[1] = new MySqlParameter("@inName", model.name);
            paras[2] = new MySqlParameter("@inBookCoverPrice", model.bookCoverPrice);
            paras[3] = new MySqlParameter("@inPrice", model.price);
            paras[4] = new MySqlParameter("@inStatus", model.status);
            paras[5] = new MySqlParameter("@inQuota", model.quota);
            paras[6] = new MySqlParameter("@inQuantity", model.quantity);
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

                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }
            conn.Close();

            return id;
        }

        public MySqlResultState UpdateModel(Model model)
        {
            MySqlResultState result = null;
            MySqlParameter[] paras = null;

            paras = new MySqlParameter[10];
            ModelParameters(model, paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbModel_Update", paras);

            return result;
        }

        public MySqlResultState AddMapping(Model model)
        {
            MySqlResultState result = new MySqlResultState();
            if(model.mappingOnlyProductId.Count()  == 0)
            {
                return result;
            }

            MySqlParameter[] paras = null;

            paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inModelId", model.id);
            paras[1] = new MySqlParameter("@inProductId", 0);
            MyMySql.AddOutParameters(paras);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbMapping_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                foreach (var id in model.mappingOnlyProductId)
                {
                    paras[1].Value = id;
                    cmd.ExecuteNonQuery();
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

        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product
        /// </summary>
        /// <param name="rdr">Trả về ngay từ câu select</param>
        /// <returns></returns>
        private Product ConvertOneRowFromDataMySqlToProduct(MySqlDataReader rdr)
        {
            Product product = new Product();
            product.id = MyMySql.GetInt32(rdr, "ProductId");
            product.code = MyMySql.GetString(rdr, "ProductCode");
            product.barcode = MyMySql.GetString(rdr, "ProductBarcode");
            product.name = MyMySql.GetString(rdr, "ProductName");
            product.comboId = MyMySql.GetInt32(rdr, "ComboId");
            product.comboName = MyMySql.GetString(rdr, "ComboName");
            product.categoryId = MyMySql.GetInt32(rdr, "CategoryId");
            product.categoryName = MyMySql.GetString(rdr, "CategoryName");
            product.bookCoverPrice = MyMySql.GetInt32(rdr, "ProductBookCoverPrice");
            product.author = MyMySql.GetString(rdr, "ProductAuthor");
            product.translator = MyMySql.GetString(rdr, "ProductTranslator");
            product.publisherId = MyMySql.GetInt32(rdr, "PublisherId");
            product.publisherName = MyMySql.GetString(rdr, "PublisherName");
            product.publishingCompany = MyMySql.GetString(rdr, "ProductPublishingCompany");
            product.publishingTime = MyMySql.GetInt32(rdr, "ProductPublishingTime");
            product.productLong = MyMySql.GetInt32(rdr, "ProductLong");
            product.productWide = MyMySql.GetInt32(rdr, "ProductWide");
            product.productHigh = MyMySql.GetInt32(rdr, "ProductHigh");
            product.productWeight = MyMySql.GetInt32(rdr, "ProductWeight");
            product.positionInWarehouse = MyMySql.GetString(rdr, "ProductPositionInWarehouse");
            product.hardCover = MyMySql.GetInt32(rdr, "ProductHardCover");
            product.minAge = MyMySql.GetInt32(rdr, "ProductMinAge");
            product.maxAge = MyMySql.GetInt32(rdr, "ProductMaxAge");
            product.parentId = MyMySql.GetInt32(rdr, "ParentId");
            product.republish = MyMySql.GetInt32(rdr, "ProductRepublish");
            product.detail = MyMySql.GetString(rdr, "ProductDetail");
            product.status = MyMySql.GetInt32(rdr, "ProductStatus");

            product.SetSrcImageVideo();

            return product;
        }

        private void ConvertOneRowFromDataMySqlToModel(MySqlDataReader rdr, List<Model> lsModel)
        {
            int modelId = MyMySql.GetInt32(rdr, "ModelId");
            if (modelId != -1)// item đã có model
            {
                // check lsModel đã có model này chưa? Chỉ cần check phần tử cuối cùng của lsModel
                if (lsModel.Count() == 0 || lsModel[lsModel.Count() - 1].id != modelId)
                {
                    Model model = new Model();
                    model.id = modelId;
                    model.itemId = MyMySql.GetInt32(rdr, "ItemId");
                    model.name = MyMySql.GetString(rdr, "ModelName");
                    model.bookCoverPrice = MyMySql.GetInt32(rdr, "ModelBookCoverPrice");
                    model.price = MyMySql.GetInt32(rdr, "ModelPrice");
                    model.quota = MyMySql.GetInt32(rdr, "ModelQuota");
                    model.status = MyMySql.GetInt32(rdr, "ModelStatus");
                    model.quantity = MyMySql.GetInt32(rdr, "ModelQuantity");
                    model.SetSrcImage();

                    lsModel.Add(model);
                }

                Product pro = ConvertOneRowFromDataMySqlToProduct(rdr);
                    lsModel[lsModel.Count() - 1].mapping.Add(pro);
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
            item.SetSrcImageVideo();

            return item;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namePara"></param>
        /// <param name="start">Tính từ 0</param>
        /// <param name="offset">Số item max trên 1 page</param>
        /// <returns></returns>
        public List<Item> SearchItemChangePage(string namePara, int start, int offset)
        {
            List<Item> ls = new List<Item>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Search", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inNamePara", namePara);
                cmd.Parameters.AddWithValue("@inStart", start);
                cmd.Parameters.AddWithValue("@inOffset", offset);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ls.Add(ConvertOneRowFromDataMySqlToItem(rdr));
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

        public int SearchItemCount(string namePara)
        {
            int count = 0;
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Search_Count_Record", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inNamePara", namePara);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    count = MyMySql.GetInt32(rdr, "CountRecord");
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
            return count;
        }

        public Item GetItemFromId(int id)
        {
            Item item = null;
            List<Model> lsModel = new List<Model>();
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
                    ConvertOneRowFromDataMySqlToModel(rdr, lsModel);
                }
                if (rdr != null)
                    rdr.Close();
                item.models = lsModel;
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return item;
        }
    }
}