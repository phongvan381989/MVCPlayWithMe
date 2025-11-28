using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.OpenPlatform.Model.LazadaApp.LazadaProduct;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    // Xử lý db liên quan đến Lazada
    public class LazadaMySql
    {
        // -2 nếu tMDTLazadaItemId đã tồn tại, -1 nếu có lỗi</returns>
        public async Task<int> InserttbLazadaItem(CommonItem item, MySqlConnection conn)
        {
            int id = 0;

            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", item.itemId);
                    cmd.Parameters.AddWithValue("@inName", item.name);

                    cmd.Parameters.AddWithValue("@inStatus", item.bActive ? 0 : 1);
                    cmd.Parameters.AddWithValue("@inImage", item.imageSrc);
                    cmd.Parameters.AddWithValue("@inDetail", item.detail);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            id = MyMySql.GetInt32(rdr, "LastId");
                        }
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
        public async Task<int> InserttbLazadaModel(int itemId, CommonModel model, MySqlConnection conn)
        {
            MyLogger.GetInstance().Warn("Start InserttbLazadaModel");
            MyLogger.GetInstance().Warn("itemId: " + itemId.ToString());
            MyLogger.GetInstance().Warn("tMDTLazadaModelId: " + model.modelId.ToString());
            int id = 0;

            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaModel_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@inItemId", itemId);
                    cmd.Parameters.AddWithValue("@inTMDTLazadaModelId", model.modelId);
                    cmd.Parameters.AddWithValue("@inName", model.name);

                    cmd.Parameters.AddWithValue("@inStatus", model.bActive ? 0 : 1);
                    cmd.Parameters.AddWithValue("@inImage", model.imageSrc);
                    cmd.Parameters.AddWithValue("@inSellerSku", model.sellerSku);

                    using (MySqlDataReader rdr =(MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            id = MyMySql.GetInt32(rdr, "LastId");
                        }
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

        // Insert mapping của model mới, và model chỉ mapping với 1 sản phẩm
        public MySqlResultState LazadaInsertNewMappingOneOfModel(int modelId,
            int productId,
            int quantity,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlCommand cmd = new MySqlCommand("st_tbLazadaMapping_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inModelId", modelId);
            cmd.Parameters.AddWithValue("@inProductId", productId);
            cmd.Parameters.AddWithValue("@inProductQuantity", quantity);

            result = MyMySql.MyExcuteNonQuery(cmd);
            return result;
        }

        public async Task LazadaUpdateStatusOfItemToDbConnectOut(
            CommonItem commonItem,
            MySqlConnection conn)
        {
            try
            {
                // Cập nhật source của item
                using (MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE tb_lazada_item SET Status = @inStatus WHERE TMDTLazadaItemId = @inItemId", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inStatus", commonItem.bActive ? 0 : 1);
                    cmd.Parameters.AddWithValue("@inItemId", commonItem.itemId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task LazadaUpdateStatusOfModelToDbConnectOut(
            List<CommonModel> commonModelList,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE tb_lazada_model SET Status = @inStatus WHERE TMDTLazadaModelId = @inModelId", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inStatus", 0);
                    cmd.Parameters.AddWithValue("@inModelId", 0L);
                    foreach (var commonModel in commonModelList)
                    {
                        cmd.Parameters[0].Value = commonModel.bActive ? 0 : 1;
                        cmd.Parameters[1].Value = commonModel.modelId;
                        await cmd.ExecuteNonQueryAsync();
                    }
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
        public async Task<Boolean> LazadaInsertIfDontExistConnectOut(CommonItem item,
            MySqlConnection conn)
        {
            Boolean exist = false;
            try
            {
                int itemIdInserted = 0;
                // Kiểm tra itemId đã tồn tại trong bảng tbLazadaitem
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT * FROM webplaywithme.tb_lazada_item WHERE TMDTLazadaItemId = @inTMDTLazadaItemId",
                    conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", item.itemId);

                    using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            itemIdInserted = MyMySql.GetInt32(rdr, "Id");
                            exist = true;
                        }
                    }
                }
                // Lưu item, model vào db lần đầu
                if (!exist)
                {
                    itemIdInserted = await InserttbLazadaItem(item, conn);

                    int length = item.models.Count;
                    for (int i = 0; i < length; i++)
                    {
                        await InserttbLazadaModel(itemIdInserted, item.models[i], conn);
                    }
                }
                else // Model trên sàn nào chưa lưu ta lưu vào db, model nào đã xóa trên sàn ta xóa trên db
                {
                    MyLogger.GetInstance().Warn("Lazada Start xử lý model trên sàn có thay đổi so với model lưu trên db");

                    // Cập nhật trạng thái item vào DB
                    await LazadaUpdateStatusOfItemToDbConnectOut(item, conn);

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
                        using (MySqlCommand cmdTem = new MySqlCommand("st_tbLazadaModel_Delete_From_Id", conn))
                        {
                            cmdTem.CommandType = CommandType.StoredProcedure;
                            cmdTem.Parameters.AddWithValue("@inLazadaModelId", 0);
                            foreach (var id in lsTMDTLazadaModelNeedDeleteOnDb)
                            {
                                cmdTem.Parameters[0].Value = id;
                                await cmdTem.ExecuteNonQueryAsync();
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
                        await LazadaUpdateStatusOfModelToDbConnectOut(lsTMDTModelExistOnDb, conn);
                    }

                    // Thêm model mới vào tbLazadamodel
                    {
                        if (lsTMDTModelNeedSaveOnDb.Count > 0)
                        {
                            exist = false;
                            foreach (var m in lsTMDTModelNeedSaveOnDb)
                            {
                                await InserttbLazadaModel(itemIdInserted, m, conn);
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

        private async Task LazadaReadMapping(CommonItem item, MySqlDataReader rdr)
        {
            long modelIdTemp;
            while (await rdr.ReadAsync())
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

        public static void LazadaReadCommonItem(List<CommonItem> list, MySqlDataReader rdr)
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

        // Từ bảng tbNeedUpdateQuantity lấy được danh sách sản phẩm lazada có thay đổi số lượng
        // cần cập nhật
        public static async Task<List<CommonItem>> LazadaGetListNeedUpdateQuantityConnectOut(MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_Need_Update_Quantity", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            LazadaReadCommonItem(listCI, rdr);
                        }
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

        // Lấy được Item mapping với sản phẩm trong kho thuộc 1 combo
        public static async Task<List<CommonItem>> LazadaGetListMappingOfCombo(int comboId,
            MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_From_Mapping_Combo_Id", conn))
                {
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            LazadaReadCommonItem(listCI, rdr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                listCI.Clear();
            }
            return listCI;
        }

        // Lấy được thông tin chi tiết
        public async Task LazadaGetItemFromIdConnectOut(long id,
            CommonItem item,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaItem_Get_All_From_TMDTItem_Id", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTLazadaItemId", id);
                    using (MySqlDataReader rdr = (MySqlDataReader) await cmd.ExecuteReaderAsync())
                    {
                        await LazadaReadMapping(item, rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task<MySqlResultState> LazadaUpdateMapping(List<CommonForMapping> ls)
        {
            MySqlResultState result = new MySqlResultState();

            int length = ls.Count;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaModel_Get_From_ItemId_ModelId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inItemId", Int64.MaxValue);
                        cmd.Parameters.AddWithValue("@inModelId", Int64.MaxValue);

                        for (int i = 0; i < length; i++)
                        {
                            cmd.Parameters[0].Value = ls[i].itemId;
                            cmd.Parameters[1].Value = ls[i].modelId;
                            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                while (await rdr.ReadAsync())
                                {
                                    ls[i].dbModelId = MyMySql.GetInt32(rdr, "ModelId");
                                }
                            }
                        }
                    }

                    // Xóa mapping cũ
                    using (MySqlCommand cmd = new MySqlCommand(
                        @"DELETE FROM `tb_lazada_mapping` WHERE `LazadaModelId` = @inModelId;",
                        conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inModelId", 0);
                        for (int i = 0; i < length; i++)
                        {
                            cmd.Parameters[0].Value = ls[i].dbModelId;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    // Insert mapping mới
                    using (MySqlCommand cmd = new MySqlCommand("st_tbLazadaMapping_Insert", conn))
                    {
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
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
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

        // Lấy mapping của sản phẩm trong đơn hàng
        public void LazadaGetMappingOfCommonOrderConnectOut(CommonOrder commonOrder, MySqlConnection conn)
        {
            string status = string.Empty;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbLazadaMapping_Get_From_Item_ModelId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTItemId", long.MinValue);
                cmd.Parameters.AddWithValue("@inTMDTModelId", long.MinValue);

                int quantity = 0;
                Product pro = null;
                for (int i = 0; i < commonOrder.listItemId.Count; i++)
                {
                    cmd.Parameters[0].Value = commonOrder.listItemId[i];
                    if (commonOrder.listModelId[i] == 0)
                        cmd.Parameters[1].Value = -1;
                    else
                        cmd.Parameters[1].Value = commonOrder.listModelId[i];

                    commonOrder.listMapping.Add(new List<Mapping>());

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // Đã được mapping
                            if (MyMySql.GetInt32(rdr, "ProductId") != -1)
                            {
                                quantity = MyMySql.GetInt32(rdr, "Quantity");
                                pro = new Product();
                                pro.id = MyMySql.GetInt32(rdr, "ProductId");
                                pro.code = MyMySql.GetString(rdr, "ProductCode");
                                pro.barcode = MyMySql.GetString(rdr, "ProductBarcode");
                                pro.name = MyMySql.GetString(rdr, "ProductName");
                                pro.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
                                pro.positionInWarehouse = MyMySql.GetString(rdr, "ProductPositionInWarehouse");
                                pro.SetFirstSrcImage();
                                commonOrder.listMapping[i].Add(new Mapping(pro, quantity));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public void InserttbLazadaBrand(List<LazadaBrandModule> modules, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbLazadaBrand_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@p_Name", "");
            cmd.Parameters.AddWithValue("@p_GlobalIdentifier", "");
            cmd.Parameters.AddWithValue("@p_NameEn", "");
            cmd.Parameters.AddWithValue("@p_BrandId", 0L);

            try
            {
                foreach(var module in modules)
                {
                    cmd.Parameters[0].Value = module.name;
                    cmd.Parameters[1].Value = module.global_identifier;
                    cmd.Parameters[2].Value = module.name_en;
                    cmd.Parameters[3].Value = module.brand_id;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public MySqlResultState InserttbLazadaMediaSpace(
            int productId,
            int mediaType, // 0: là ảnh, 1: video
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            List<LazadaUploadImage> images,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState(); 

            if (images == null || images.Count == 0)
            {
                return result;
            }

            MySqlCommand cmd = new MySqlCommand("st_tbLazadaMediaSpace_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
            cmd.Parameters.AddWithValue("@p_Url", "");
            cmd.Parameters.AddWithValue("@p_ProductId", productId);
            cmd.Parameters.AddWithValue("@p_ProductType", productType);
            cmd.Parameters.AddWithValue("@p_HashCode", "");

            try
            {
                foreach (var image in images)
                {
                    cmd.Parameters["@p_Url"].Value = image.url;
                    cmd.Parameters["@p_HashCode"].Value = image.hash_code;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        // Lấy số lượng ảnh đã up lên Lazada của sản phẩm
        public int GetQuantityOfProductImageUploadedToLazada(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Lazada
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {
            MySqlCommand cmd =
                new MySqlCommand(@"SELECT COUNT(Id) AS Count FROM tb_lazada_media_space 
            WHERE MediaType = @p_MediaType 
            AND ProductId = @p_ProductId 
            AND ProductType = @p_ProductType;", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
            cmd.Parameters.AddWithValue("@p_ProductId", productId);
            cmd.Parameters.AddWithValue("@p_ProductType", productType);
            int count = 0;
            try
            {
                MySqlDataReader rdr = null;
                using (rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        count = rdr.GetInt32(rdr.GetOrdinal("Count"));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                count = -1; // Có lỗi
            }

            return count;
        }

        // Vì upload lại ảnh lên Lazada, xóa id ảnh cũ đã lưu trong db
        public MySqlResultState DeleteProductImageUploadedToLazada(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Lazada
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {

            MySqlCommand cmd =
                new MySqlCommand(@"DELETE FROM tb_lazada_media_space 
            WHERE MediaType = @p_MediaType 
            AND ProductId = @p_ProductId 
            AND ProductType = @p_ProductType;", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
            cmd.Parameters.AddWithValue("@p_ProductId", productId);
            cmd.Parameters.AddWithValue("@p_ProductType", productType);
            MySqlResultState resultState = MyMySql.MyExcuteNonQuery(cmd);

            return resultState;
        }

        public List<string> GetUploadedImageOfProductOnLazada(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên sàn
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn
            )
        {
            List<string> images = new List<string>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                @"SELECT Id, Url FROM tb_lazada_media_space 
    WHERE MediaType = @p_MediaType AND ProductId = @p_ProductId AND ProductType = @p_ProductType
    ORDER BY Id ASC;", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
                cmd.Parameters.AddWithValue("@p_ProductId", productId);
                cmd.Parameters.AddWithValue("@p_ProductType", productType);
                MySqlDataReader rdr = null;

                using (rdr = cmd.ExecuteReader())
                {
                    int urlIndex = rdr.GetOrdinal("Url");
                    while (rdr.Read())
                    {
                        images.Add(rdr.GetString(urlIndex));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                images.Clear();
            }
            return images;
        }
    }
}