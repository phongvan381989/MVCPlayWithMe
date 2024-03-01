using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Order;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models.ItemModel
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

            paras = new MySqlParameter[7];
            ModelParameters(model, paras);
            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbModel_Update", paras);

            return result;
        }

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
            paras[1] = new MySqlParameter("@inProductId", 0);
            paras[2] = new MySqlParameter("@inQuantity", 0);
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
            }
            conn.Close();

            return result;
        }

        // Xóa tất cả mapping của model
        public MySqlResultState DeleteMapping(Model model)
        {
            MySqlResultState result = new MySqlResultState();

            MySqlParameter[] paras = null;

            paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inModelId", model.id);
            MyMySql.AddOutParameters(paras);

            result = MyMySql.ExcuteNonQueryStoreProceduce("st_tbMapping_Delete_From_ModelId", paras);

            return result;
        }

        /// <summary>
        /// TỪ dữ liệu select db, ta trả về đối tượng Product
        /// </summary>
        /// <param name="rdr">Trả về ngay từ câu select</param>
        /// <returns></returns>
        public static Product ConvertOneRowFromDataMySqlToProduct(MySqlDataReader rdr)
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
            product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");

            product.SetSrcImageVideo();

            return product;
        }

        private void ConvertOneRowFromDataMySqlToModel(MySqlDataReader rdr, List<Model> models)
        {
            int modelId = MyMySql.GetInt32(rdr, "ModelId");
            if (modelId != -1)// item đã có model
            {
                // check models đã có model này chưa? Chỉ cần check phần tử cuối cùng của models
                if (models.Count() == 0 || models[models.Count() - 1].id != modelId)
                {
                    Model model = new Model();
                    model.id = modelId;
                    model.itemId = MyMySql.GetInt32(rdr, "ItemId");
                    model.name = MyMySql.GetString(rdr, "ModelName");

                    model.quota = MyMySql.GetInt32(rdr, "ModelQuota");
                    model.discount = MyMySql.GetInt32(rdr, "ModelDiscount");
                    model.SetSrcImage();

                    models.Add(model);
                }

                Product pro = ConvertOneRowFromDataMySqlToProduct(rdr);
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
        public List<Item> SearchItemChangePage(ItemModelSearchParameter searchParameter)
        {
            List<Item> ls = new List<Item>();
            List<int> lsId = new List<int>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbItem_Search", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inNamePara", searchParameter.name);
                cmd.Parameters.AddWithValue("@inHasMapping", searchParameter.hasMapping);
                cmd.Parameters.AddWithValue("@inStart", searchParameter.start);
                cmd.Parameters.AddWithValue("@inOffset", searchParameter.offset);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lsId.Add(MyMySql.GetInt32(rdr, "ItemId"));
                }

                if (rdr != null)
                    rdr.Close();
                foreach (var id in lsId)
                {
                    Item item = GetItemFromIdWithReadyConn(id, conn);
                    ls.Add(item);
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
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
                cmd.Parameters.AddWithValue("@inNamePara", searchParameter.name);
                cmd.Parameters.AddWithValue("@inHasMapping", searchParameter.hasMapping);

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
                if (rdr != null)
                    rdr.Close();

                foreach (var model in models)
                {
                    model.SetPriceFromMappingPriceAndQuantity();
                }
                item.models = models;
                item.SetPriceAndQuantity();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return item;
        }

        // Tương tự hàm GetItemFromId nhưng với conn mở đóng bên ngoài
        public Item GetItemFromIdWithReadyConn(int id, MySqlConnection conn)
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
                if (rdr != null)
                    rdr.Close();

                foreach (var model in models)
                {
                    model.SetPriceFromMappingPriceAndQuantity();
                }
                item.models = models;
                item.SetPriceAndQuantity();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
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
                if (rdr != null)
                    rdr.Close();

                foreach (var model in models)
                {
                    model.SetPriceFromMappingPriceAndQuantity();
                }
                item.models = models;
                item.SetPriceAndQuantity();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
            return item;
        }

        // Kết nối mở, đóng bên ngoài
        // Lấy được thông tin chi tiết theo model id,
        // những model thuộc item có id khác tham số sẽ không được lấy
        public Item GetItemFromModelIdWithReadyConn(int id, MySqlConnection conn)
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
                if (rdr != null)
                    rdr.Close();

                if (item != null)
                {
                    foreach (var model in models)
                    {
                        model.SetPriceFromMappingPriceAndQuantity();
                    }
                    item.models = models;
                    item.SetPriceAndQuantity();
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
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
    }
}