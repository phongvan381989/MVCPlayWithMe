using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    // Xử lý db liên quan đến Lazada
    public class LazadaMySql
    {

        // -2 nếu tMDTLazadaItemId đã tồn tại, -1 nếu có lỗi</returns>
        public int InserttbLazadaItem(CommonItem item, MySqlConnection conn)
        {
            int id = 0;
            MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", item.itemId);
            cmd.Parameters.AddWithValue("@inName", item.name);

            cmd.Parameters.AddWithValue("@inStatus", item.bActive?0:1);
            cmd.Parameters.AddWithValue("@inImage", item.imageSrc);
            cmd.Parameters.AddWithValue("@inDetail", item.detail);

            try
            {
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        id = MyMySql.GetInt32(rdr, "LastId");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                id = -1;
            }

            return id;
        }

        //-2 nếu tMDTLazadaModelId đã tồn tại, -1 nếu có lỗi</returns>
        public int InserttbLazadaModel(int itemId, CommonModel model, MySqlConnection conn)
        {
            MyLogger.GetInstance().Warn("Start InserttbLazadaModel");
            MyLogger.GetInstance().Warn("itemId: " + itemId.ToString());
            MyLogger.GetInstance().Warn("tMDTLazadaModelId: " + model.modelId.ToString());
            int id = 0;
            MySqlCommand cmd = new MySqlCommand("st_tbLazadaModel_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inItemId", itemId);
            cmd.Parameters.AddWithValue("@inTMDTLazadaModelId", model.modelId);
            cmd.Parameters.AddWithValue("@inName", model.name);

            cmd.Parameters.AddWithValue("@inStatus", model.bActive ? 0 : 1);
            cmd.Parameters.AddWithValue("@inImage", model.imageSrc);
            cmd.Parameters.AddWithValue("@inSellerSku", model.sellerSku);

            try
            {
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        id = MyMySql.GetInt32(rdr, "LastId");
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                id = -1;
            }

            return id;
        }

        public void LazadaUpdateStatusOfItemToDbConnectOut(
            CommonItem commonItem,
            MySqlConnection conn)
        {
            try
            {
                // Cập nhật source của item
                MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE tb_lazada_item SET Status = @inStatus WHERE TMDTLazadaItemId = @inItemId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inStatus", commonItem.bActive ? 0 : 1);
                cmd.Parameters.AddWithValue("@inItemId", commonItem.itemId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public void LazadaUpdateStatusOfModelToDbConnectOut(
            List<CommonModel> commonModelList,
            MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE tb_lazada_model SET Status = @inStatus WHERE TMDTLazadaModelId = @inModelId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inStatus", 0);
                cmd.Parameters.AddWithValue("@inModelId", 0L);
                foreach (var commonModel in commonModelList)
                {
                    cmd.Parameters[0].Value = commonModel.bActive ? 0 : 1;
                    cmd.Parameters[1].Value = commonModel.modelId;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Lấy được danh sách LazadaModelId, TMDTLazadaModelId 
        private List<Tuple<int, long>> ListModelOfItem(long tMDTLazadaItemId,
            MySqlConnection conn)
        {
            List<Tuple<int, long>> lsModel = new List<Tuple<int, long>>();
            MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_Model_From_TMDTItemId", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", tMDTLazadaItemId);

            try
            {
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lsModel.Add(Tuple.Create(MyMySql.GetInt32(rdr, "ModelId"),
                            MyMySql.GetInt64(rdr, "TMDTLazadaModelId")));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return lsModel;
        }

        // Check item, model đã được lưu vào bảng tương ứng item, model
        // Nếu chưa lưu ta thực hiện lưu
        // Nếu đã lưu model trong db nhưng không còn tồn tại trên sàn TMDT ta xóa trong db
        // Ta chỉ check được với model khi chọn xem item, check item đã bị xóa trên db 
        // không thực hiện ở đây
        // Trả về true nếu item đã tồn tại và không thêm mới model nào,
        // ngược lại trả về false
        public Boolean LazadaInsertIfDontExistConnectOut(CommonItem item,
            MySqlConnection conn)
        {
            Boolean exist = false;
            try
            {
                // Kiểm tra itemId đã tồn tại trong bảng tbLazadaitem
                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT * FROM webplaywithme.tb_lazada_item WHERE TMDTLazadaItemId = @inTMDTLazadaItemId",
                    conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", item.itemId);

                int itemIdInserted = 0;

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        itemIdInserted = MyMySql.GetInt32(rdr, "Id");
                        exist = true;
                    }
                }
                // Lưu item, model vào db lần đầu
                if (!exist)
                {
                    itemIdInserted = InserttbLazadaItem(item, conn);

                    int length = item.models.Count;
                    for (int i = 0; i < length; i++)
                    {
                        InserttbLazadaModel(itemIdInserted, item.models[i], conn);
                    }
                }
                else // Model trên sàn nào chưa lưu ta lưu vào db, model nào đã xóa trên sàn ta xóa trên db
                {
                    MyLogger.GetInstance().Warn("Lazada Start xử lý model trên sàn có thay đổi so với model lưu trên db");

                    // Cập nhật trạng thái item vào DB
                    LazadaUpdateStatusOfItemToDbConnectOut(item, conn);

                    // Lấy list Lazada model id đã lưu trong db
                    List<Tuple<int, long>> lsTupleTMDTModelOnDb = ListModelOfItem(item.itemId, conn);

                    // Danh sách model đã bị xóa trên sàn
                    List<int> lsTMDTLazadaModelNeedDeleteOnDb = new List<int>();
                    Boolean existTemp = false;
                    foreach (var id in lsTupleTMDTModelOnDb)
                    {
                        existTemp = false;
                        foreach (var m in item.models)
                        {
                            if (id.Item2 == m.modelId)
                            {
                                existTemp = true;
                                break;
                            }
                        }
                        if (!existTemp)
                        {
                            lsTMDTLazadaModelNeedDeleteOnDb.Add(id.Item1);
                        }
                    }
                    if (lsTMDTLazadaModelNeedDeleteOnDb.Count > 0)
                    {
                        // Xóa trên tbLazadamapping, tbpwmmappingother, tbLazadamodel
                        {
                            MySqlCommand cmdTem = new MySqlCommand("st_tbLazadaModel_Delete_From_Id", conn);
                            cmdTem.CommandType = CommandType.StoredProcedure;
                            cmdTem.Parameters.AddWithValue("@inLazadaModelId", 0);
                            foreach (var id in lsTMDTLazadaModelNeedDeleteOnDb)
                            {
                                cmdTem.Parameters[0].Value = id;
                                cmdTem.ExecuteNonQuery();
                            }
                        }
                    }

                    // Danh sách model có trên sàn nhưng không có trên db
                    List<CommonModel> lsTMDTModelNeedSaveOnDb = new List<CommonModel>();
                    List<CommonModel> lsTMDTModelExistOnDb = new List<CommonModel>();
                    foreach (var m in item.models)
                    {
                        existTemp = false;
                        foreach (var id in lsTupleTMDTModelOnDb)
                        {
                            if (id.Item2 == m.modelId)
                            {
                                existTemp = true;
                                break;
                            }
                        }
                        if (!existTemp)
                        {
                            lsTMDTModelNeedSaveOnDb.Add(m);
                        }
                        else
                        {
                            lsTMDTModelExistOnDb.Add(m);
                        }
                    }
                    // Cập nhật trạng thái của model nếu đã tồn tại
                    if (lsTMDTModelExistOnDb.Count > 0)
                    {
                        LazadaUpdateStatusOfModelToDbConnectOut(lsTMDTModelExistOnDb, conn);
                    }

                    // Thêm model mới vào tbLazadamodel
                    {
                        if (lsTMDTModelNeedSaveOnDb.Count > 0)
                        {
                            exist = false;
                            foreach (var m in lsTMDTModelNeedSaveOnDb)
                            {
                                InserttbLazadaModel(itemIdInserted, m, conn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return exist;
        }

        private void LazadaReadMapping(CommonItem item, MySqlDataReader rdr)
        {
            long modelIdTemp;
            while (rdr.Read())
            {
                modelIdTemp = MyMySql.GetInt64(rdr, "TMDTModelId");

                // Lấy dữ liệu về mapping
                if (MyMySql.GetInt32(rdr, "MappingId") != -1)
                {
                    Mapping map = new Mapping();
                    map.quantity = MyMySql.GetInt32(rdr, "MappingQuantity");

                    Product product = new Product();
                    product.id = MyMySql.GetInt32(rdr, "ProductId");
                    product.name = MyMySql.GetString(rdr, "ProductName");
                    product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
                    product.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
                    product.discount = MyMySql.GetFloat(rdr, "Discount");
                    product.publisherId = MyMySql.GetInt32(rdr, "PublisherId");
                    product.SetFirstSrcImage();
                    map.product = product;

                    // Tìm model object
                    foreach (var model in item.models)
                    {
                        if (model.modelId == modelIdTemp)
                        {
                            model.mapping.Add(map);
                            break;
                        }
                    }
                }
            }
        }

        public void LazadaGetListCommonItemFromListItemConnectOut(
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_All_From_TMDTItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", long.MaxValue);
                long modelIdTemp;
                foreach (var item in lsCommonItem)
                {
                    cmd.Parameters[0].Value = item.itemId;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        LazadaReadMapping(item, rdr);
                    }

                    lsCommonItem.Add(item);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public void LazadaReadCommonItem(List<CommonItem> list, MySqlDataReader rdr)
        {
            CommonItem commonItem = null;
            CommonModel commonModel = null;
            int dbItemId = MyMySql.GetInt32(rdr, "ItemId");
            int dbModelId = MyMySql.GetInt32(rdr, "ModelId");
            if (list.Count() == 0)
            {
                commonItem = new CommonItem(Common.eLazada);
                list.Add(commonItem);
            }
            else
            {
                // Đọc sang item mới vì kết quả sql đã được order by theo itemid, modelid
                if (list[list.Count() - 1].dbItemId != dbItemId)
                {
                    commonItem = new CommonItem(Common.eLazada);
                    list.Add(commonItem);
                }
            }
            if (commonItem != null)
            {
                commonItem.dbItemId = dbItemId;
                commonItem.itemId = MyMySql.GetInt64(rdr, "TMDTItemId");
                commonItem.name = MyMySql.GetString(rdr, "ItemName");
                commonItem.imageSrc = MyMySql.GetString(rdr, "ItemImage");

                int status = MyMySql.GetInt32(rdr, "ItemStatus");
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
                commonModel.modelId = MyMySql.GetInt64(rdr, "TMDTModelId");
                commonModel.name = MyMySql.GetString(rdr, "ModelName");
                commonModel.imageSrc = MyMySql.GetString(rdr, "ModelImage");
                int status = MyMySql.GetInt32(rdr, "ModelStatus");
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
            mapping.quantity = MyMySql.GetInt32(rdr, "MappingQuantity");

            Product product = new Product();
            product.id = MyMySql.GetInt32(rdr, "ProductId");
            product.name = MyMySql.GetString(rdr, "ProductName");
            product.status = MyMySql.GetInt32(rdr, "ProductStatus");
            product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
            product.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
            product.discount = MyMySql.GetFloat(rdr, "Discount");
            product.publisherId = MyMySql.GetInt32(rdr, "PublisherId");
            mapping.product = product;

            commonModel.mapping.Add(mapping);
        }

        // Từ bảng tbNeedUpdateQuantity lấy được danh sách sản phẩm shopee có thay đổi số lượng
        // cần cập nhật
        public List<CommonItem> LazadaGetListNeedUpdateQuantityConnectOut(MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_Need_Update_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        LazadaReadCommonItem(listCI, rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return listCI;
        }

        public List<CommonItem> LazadaGetListMappingOfProduct(int productId, MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_From_Mapping_Product_Id", conn);
                cmd.Parameters.AddWithValue("@inProductId", productId);
                cmd.CommandType = CommandType.StoredProcedure;
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        LazadaReadCommonItem(listCI, rdr);
                    }
                };
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                listCI.Clear();
            }
            return listCI;
        }

        // Lấy được thông tin chi tiết
        public void LazadaGetItemFromIdConnectOut(long id,
            CommonItem item,
            MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_All_From_TMDTItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", id);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    LazadaReadMapping(item, rdr);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public MySqlResultState LazadaUpdateMapping(List<CommonForMapping> ls)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            int length = ls.Count;
            try
            {
                conn.Open();
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbLazadaModel_Get_From_ItemId_ModelId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inItemId", Int64.MaxValue);
                    cmd.Parameters.AddWithValue("@inModelId", Int64.MaxValue);

                    for (int i = 0; i < length; i++)
                    {
                        cmd.Parameters[0].Value = ls[i].itemId;
                        cmd.Parameters[1].Value = ls[i].modelId;
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ls[i].dbModelId = MyMySql.GetInt32(rdr, "ModelId");
                            }
                        }
                    }
                }

                // Xóa mapping cũ
                {
                    MySqlCommand cmd = new MySqlCommand(
                        @"DELETE FROM `tb_lazada_mapping` WHERE `LazadaModelId` = @inModelId;",
                        conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inModelId", 0);
                    for (int i = 0; i < length; i++)
                    {
                        cmd.Parameters[0].Value = ls[i].dbModelId;
                        cmd.ExecuteNonQuery();
                    }
                }

                // Insert mapping mới
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbLazadaMapping_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inModelId", 0);
                    cmd.Parameters.AddWithValue("@inProductId", 0);
                    cmd.Parameters.AddWithValue("@inProductQuantity", 0);
                    for (int i = 0; i < length; i++)
                    {
                        if (ls[i].dbModelId > 0)
                        {
                            cmd.Parameters[0].Value = ls[i].dbModelId;
                            for (int j = 0; j < ls[i].lsProductId.Count; j++)
                            {
                                cmd.Parameters[1].Value = ls[i].lsProductId[j];
                                cmd.Parameters[2].Value = ls[i].lsProductQuantity[j];
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            return result;
        }

        public List<CommonItem> LazadaGetItemOnDB(MySqlConnection conn)
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_Item_Model_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        LazadaReadCommonItem(lsCommonItem, rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                lsCommonItem = null;
            }
            return lsCommonItem;
        }
    }
}