using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeConfig;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;
using static MVCPlayWithMe.General.Common;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Xử lý db liên quan đến shopee
    /// </summary>
    public class ShopeeMySql
    {
        /// <summary>
        /// Đóng mở kết nối bên ngoài
        /// </summary>
        /// <returns>-2 nếu tMDTShopeeItemId đã tồn tại, -1 nếu có lỗi</returns>
        public async Task<int> InserttbShopeeItemAsync(CommonItem item, MySqlConnection conn)
        {
            int id = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", item.itemId);
                    cmd.Parameters.AddWithValue("@inName", item.name);
                    cmd.Parameters.AddWithValue("@inStatus", CommonOpenPlatform.ShopeeGetEnumValueFromString(item.item_status));
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

        /// <summary>
        /// Đóng mở kết nối bên ngoài
        /// </summary>
        /// <returns>-2 nếu tMDTShopeeModelId đã tồn tại, -1 nếu có lỗi</returns>
        public async Task<int> InserttbShopeeModelAsync(int itemId, CommonModel model, MySqlConnection conn)
        {
            MyLogger.GetInstance().Warn("Start InserttbShopeeModel");
            MyLogger.GetInstance().Warn("itemId: " + itemId.ToString());
            MyLogger.GetInstance().Warn("tMDTShopeeModelId: " + model.modelId.ToString());
            int id = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@inItemId", itemId);
                    cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", model.modelId);
                    cmd.Parameters.AddWithValue("@inName", model.name);
                    cmd.Parameters.AddWithValue("@inStatus", model.bActive ? 0 : 1);
                    cmd.Parameters.AddWithValue("@inImage", model.imageSrc);

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

        // Lấy được danh sách ShopeeModelId, TMDTShopeeModelId
        private async Task<List<Tuple<int, long>>> ListModelOfItemAsync(long tMDTShopeeModelId, MySqlConnection conn)
        {
            List<Tuple<int, long>> lsModel = new List<Tuple<int, long>>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_Model_From_TMDTShopeeItemId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", tMDTShopeeModelId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            //if (MyMySql.GetInt32(rdr, "ShopeeModelId") != -1 &&
                            //    MyMySql.GetInt64(rdr, "TMDTShopeeModelId") != -1)
                            //{
                            lsModel.Add(Tuple.Create(MyMySql.GetInt32(rdr, "ShopeeModelId"),
                                MyMySql.GetInt64(rdr, "TMDTShopeeModelId")));
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return lsModel;
        }

        // Check item, model đã được lưu vào bảng tương ứng tbshopeeitem, tbshopeemodel
        // Nếu chưa lưu ta thực hiện lưu
        // Nếu đã lưu model trong db nhưng không còn tồn tại trên sàn TMDT ta xóa trong db
        // Ta chỉ check được với model khi chọn xem item, check item đã bị xóa trên db
        // không thực hiện ở đây
        // Trả về true nếu item đã tồn tại và không thêm mới model nào,
        // ngược lại trả về false
        public async Task<Boolean> ShopeeInsertIfDontExistConnectOutAsync(CommonItem item, MySqlConnection conn)
        {
            Boolean exist = false;
            try
            {
                int itemIdInserted = 0;

                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_TMDTShopeeItemId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", item.itemId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
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
                    //status = item.bActive ? (int)CommonOpenPlatform.ShopeeProductStatus.NORMAL : (int)CommonOpenPlatform.ShopeeProductStatus.BANNED;
                    //status = CommonOpenPlatform.ShopeeGetEnumValueFromString(item.item_status);
                    itemIdInserted = await InserttbShopeeItemAsync(item, conn);

                    int length = item.models.Count;
                    for (int i = 0; i < length; i++)
                    {
                        await InserttbShopeeModelAsync(itemIdInserted, item.models[i], conn);
                    }
                }
                else // Model trên sàn nào chưa lưu ta lưu vào db, model nào đã xóa trên sàn ta xóa trên db
                {
                    MyLogger.GetInstance().Warn("Start xử lý model trên sàn có thay đổi so với model lưu trên db");

                    // Cập nhật trạng thái item vào DB
                    await ShopeeUpdateStatusOfItemToDbConnectOutAsync(item, conn);

                    // Lấy list shopee model id đã lưu trong db
                    List<Tuple<int, long>> lsTupleTMDTShopeeModelOnDb = await ListModelOfItemAsync(item.itemId, conn);

                    // Danh sách model đã bị xóa trên sàn
                    List<int> lsTMDTShopeeModelNeedDeleteOnDb = new List<int>();
                    Boolean existTemp = false;
                    foreach (var id in lsTupleTMDTShopeeModelOnDb)
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
                            lsTMDTShopeeModelNeedDeleteOnDb.Add(id.Item1);
                        }
                    }

                    if (lsTMDTShopeeModelNeedDeleteOnDb.Count > 0)
                    {
                        // Xóa trên tbshopeemapping, tbpwmmappingother, tbshopeemodel
                        using (MySqlCommand cmdTem = new MySqlCommand("st_tbShopeeModel_Disable_From_Id", conn))
                        {
                            cmdTem.CommandType = CommandType.StoredProcedure;
                            cmdTem.Parameters.AddWithValue("@inShopeeModelId", 0);
                            foreach (var id in lsTMDTShopeeModelNeedDeleteOnDb)
                            {
                                cmdTem.Parameters[0].Value = id;
                                await cmdTem.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Danh sách model có trên sàn nhưng không có trên db
                    List<CommonModel> lsTMDTShopeeModelNeedSaveOnDb = new List<CommonModel>();
                    foreach (var m in item.models)
                    {
                        existTemp = false;
                        foreach (var id in lsTupleTMDTShopeeModelOnDb)
                        {
                            if (id.Item2 == m.modelId)
                            {
                                existTemp = true;
                                break;
                            }
                        }
                        if (!existTemp)
                        {
                            lsTMDTShopeeModelNeedSaveOnDb.Add(m);
                        }
                    }
                    // Thêm model mới vào tbshopeemodel
                    if (lsTMDTShopeeModelNeedSaveOnDb.Count > 0)
                    {
                        exist = false;
                        foreach (var m in lsTMDTShopeeModelNeedSaveOnDb)
                        {
                            await InserttbShopeeModelAsync(itemIdInserted, m, conn);
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

        // Lấy được thông tin chi tiết
        public async Task ShopeeGetItemFromIdConnectOutAsync(long id, CommonItem item, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_All_From_TMDTShopeeItem_Id", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", id);

                    long modelIdTemp;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            modelIdTemp = MyMySql.GetInt64(rdr, "TMDTShopeeModelId");

                            // Lấy dữ liệu về mapping
                            if (MyMySql.GetInt32(rdr, "ShopeeMappingId") != -1)
                            {
                                Mapping map = new Mapping();
                                map.quantity = MyMySql.GetInt32(rdr, "ShopeeMappingQuantity");

                                Product product = new Product();
                                product.id = MyMySql.GetInt32(rdr, "ProductId");
                                product.name = MyMySql.GetString(rdr, "ProductName");
                                product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
                                product.SetFirstSrcImage();
                                map.product = product;

                                // Tìm model object
                                foreach (var model in item.models)
                                {
                                    if (model.modelId == modelIdTemp)
                                    {
                                        // Chưa được xét
                                        if (model.pWMMappingModelId == 0)
                                        {
                                            model.pWMMappingModelId = MyMySql.GetInt32(rdr, "PWMMappingModelId");
                                        }
                                        model.mapping.Add(map);
                                        break;
                                    }
                                }
                            }

                            // Lấy dữ liệu về sản phẩm trên voibenho tương ứng
                            if (MyMySql.GetInt32(rdr, "PWMMappingModelId") != -1)
                            {
                                // Tìm model object
                                foreach (var model in item.models)
                                {
                                    if (model.modelId == modelIdTemp)
                                    {
                                        // Chưa được xét
                                        if (model.pWMMappingModelId == 0)
                                        {
                                            model.pWMMappingModelId = MyMySql.GetInt32(rdr, "PWMMappingModelId");
                                        }
                                        break;
                                    }
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
        }

        public async Task ShopeeGetListCommonItemFromListShopeeItemConnectOutAsync(
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_All_From_TMDTShopeeItem_Id", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", long.MaxValue);
                    long modelIdTemp;
                    foreach (var item in lsCommonItem)
                    {
                        cmd.Parameters[0].Value = item.itemId;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                if (MyMySql.GetInt32(rdr, "ShopeeMappingId") != -1)
                                {
                                    Mapping map = new Mapping();
                                    map.quantity = MyMySql.GetInt32(rdr, "ShopeeMappingQuantity");

                                    Product product = new Product();
                                    product.id = MyMySql.GetInt32(rdr, "ProductId");
                                    product.name = MyMySql.GetString(rdr, "ProductName");
                                    product.quantity = MyMySql.GetInt32(rdr, "ProductQuantity");
                                    product.SetFirstSrcImage();
                                    map.product = product;

                                    modelIdTemp = MyMySql.GetInt64(rdr, "TMDTShopeeModelId");
                                    // Tìm model object
                                    foreach (var model in item.models)
                                    {
                                        if (model.modelId == modelIdTemp)
                                        {
                                            // Chưa được xét
                                            if (model.pWMMappingModelId == 0)
                                            {
                                                model.pWMMappingModelId = MyMySql.GetInt32(rdr, "PWMMappingModelId");
                                            }
                                            model.mapping.Add(map);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        lsCommonItem.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task ShopeeUpdateStatusOfItemListToDbConnectOutAsync(
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            if (lsCommonItem == null || lsCommonItem.Count == 0)
            {
                return;
            }

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    "UPDATE tbshopeeitem SET Status = @inStatus WHERE TMDTShopeeItemId = @inItemId", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inStatus", 0);
                    cmd.Parameters.AddWithValue("@inItemId", 0L);
                    foreach (var commonItem in lsCommonItem)
                    {
                        cmd.Parameters[0].Value = CommonOpenPlatform.ShopeeGetEnumValueFromString(commonItem.item_status);
                        cmd.Parameters[1].Value = commonItem.itemId;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task ShopeeUpdateStatusOfItemToDbConnectOutAsync(
            CommonItem commonItem,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    "UPDATE tbshopeeitem SET Status = @inStatus WHERE TMDTShopeeItemId = @inItemId", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inStatus", CommonOpenPlatform.ShopeeGetEnumValueFromString(commonItem.item_status));
                    cmd.Parameters.AddWithValue("@inItemId", commonItem.itemId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task<List<CommonItem>> ShopeeGetItemOnDBAsync(MySqlConnection conn)
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_Item_Model_All", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    long itemId = long.MaxValue;
                    long itemIdTem = long.MaxValue;

                    long modelId = long.MaxValue;
                    long modelIdTem = long.MaxValue;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int tmdtShopeeItemIdIndex = rdr.GetOrdinal("TMDTShopeeItemId");
                        int shopeeItemNameIndex = rdr.GetOrdinal("ShopeeItemName");
                        int shopeeItemStatusIndex = rdr.GetOrdinal("ShopeeItemStatus");

                        int tmdtShopeeModelIdIndex = rdr.GetOrdinal("TMDTShopeeModelId");
                        int shopeeModelNameIndex = rdr.GetOrdinal("ShopeeModelName");

                        int productStatusIndex = rdr.GetOrdinal("ProductStatus");

                        CommonItem commonItem = null;
                        CommonModel commonModel = null;
                        int status = 0;
                        while (await rdr.ReadAsync())
                        {
                            itemIdTem = rdr.GetInt64(tmdtShopeeItemIdIndex);
                            if (itemId != itemIdTem)
                            {
                                itemId = itemIdTem;
                                commonItem = new CommonItem(Common.eShopee);
                                lsCommonItem.Add(commonItem);

                                commonItem.itemId = itemId;
                                commonItem.name = rdr.GetString(shopeeItemNameIndex);
                                status = rdr.GetInt32(shopeeItemStatusIndex);
                                commonItem.item_status = ShopeeItemStatus.ShopeeItemStatusArray[status];
                                commonItem.bActive = status == 0 ? true : false;
                            }
                            modelIdTem = rdr.GetInt64(tmdtShopeeModelIdIndex);
                            if (modelId != modelIdTem)
                            {
                                modelId = modelIdTem;
                                commonModel = new CommonModel();
                                commonModel.modelId = modelIdTem;
                                commonModel.name = rdr.IsDBNull(shopeeModelNameIndex) ? string.Empty : rdr.GetString(shopeeModelNameIndex);
                                commonItem.models.Add(commonModel);
                            }

                            if (!rdr.IsDBNull(productStatusIndex))
                            {
                                Product product = new Product();
                                product.status = rdr.GetInt32(productStatusIndex);
                                commonModel.mapping.Add(new Mapping(product, 1));
                            }
                        }
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

        public async Task<MySqlResultState> ShopeeDeleteItemOnDBAsync(long itemId)
        {
            MySqlResultState resultState = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    //using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Delete_From_TMDTShopeeItemId", conn))
                    using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Disable_From_TMDTShopeeItemId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", itemId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, resultState);
                }
            }
            return resultState;
        }

        // Trường hợp item chỉ có 1 modelId, modelId khác -1 hàm dưới chưa xóa dữ liệu tương ứng trong bảng tbShopeeItem
        // Kết quả là có 1 item không có model nào trong DB
        // Xóa trên tbshopeemapping, tbpwmmappingother, tbshopeemodel
        public static async Task<MySqlResultState> ShopeeDeleteModelOnDBAsync(long modelId)
        {
            MySqlResultState resultState = new MySqlResultState();
            if (modelId == -1)
                return resultState;

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    //using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Delete_From_TMDTShopeeModeId", conn))
                    using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Disable_From_TMDTShopeeModeId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", modelId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, resultState);
                }
            }
            return resultState;
        }

        public async Task<MySqlResultState> ShopeeUpdateMappingAsync(List<CommonForMapping> ls)
        {
            MySqlResultState result = new MySqlResultState();
            int length = ls.Count;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Get_From_ItemId_ModelId", conn))
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
                                    ls[i].dbModelId = MyMySql.GetInt32(rdr, "ShopeeModelId");
                                }
                            }
                        }
                    }

                    // Xóa mapping cũ
                    using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Delete_From_ShopeeModelId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inShopeeModelId", 0);
                        for (int i = 0; i < length; i++)
                        {
                            cmd.Parameters[0].Value = ls[i].dbModelId;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    // Insert mapping mới
                    using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inShopeeModelId", 0);
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

        // Insert mapping của model mới, và model chỉ mapping với 1 sản phẩm
        public async Task<MySqlResultState> ShopeeInsertNewMappingOneOfModelAsync(int modelId,
            int productId,
            int quantity,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inShopeeModelId", modelId);
                    cmd.Parameters.AddWithValue("@inProductId", productId);
                    cmd.Parameters.AddWithValue("@inProductQuantity", quantity);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public async Task ShopeeGetMappingOfCommonOrderConnectOutAsync(CommonOrder commonOrder, MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Get_From_Item_ModelId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", long.MinValue);
                    cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", long.MinValue);

                    int quantity = 0;
                    Product pro = null;

                    for (int i = 0; i < commonOrder.listItemId.Count; i++)
                    {
                        cmd.Parameters[0].Value = commonOrder.listItemId[i];
                        if (commonOrder.listModelId[i] == 0)// Không có model, Shopee trả modelId = 0, db lưu modelId = -1
                            cmd.Parameters[1].Value = -1;
                        else
                            cmd.Parameters[1].Value = commonOrder.listModelId[i];

                        commonOrder.listMapping.Add(new List<Mapping>());

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
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
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public async Task<MySqlResultState> ShopeeSaveTokenAsync(ShopeeToken shopeeToken,
            MySqlConnection conn)
        {
            MySqlResultState resultState = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"UPDATE `tbShopeeAuthen` SET `AccessToken`=@AccessToken,
                     `RefreshToken`=@RefreshToken,
                     `ValidAccessTokenTime` = @ValidAccessTokenTime
                     WHERE `Id` = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@AccessToken", shopeeToken.access_token);
                    cmd.Parameters.AddWithValue("@RefreshToken", shopeeToken.refresh_token);
                    cmd.Parameters.AddWithValue("@ValidAccessTokenTime",
                        DateTime.Now.AddSeconds(shopeeToken.expire_in - 300)); // 300 giây là trừ hao
                    cmd.CommandType = CommandType.Text;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }
            return resultState;
        }

        public async Task<MySqlResultState> ShopeeSaveLivePartnerKeyAsync(string key, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    "UPDATE webplaywithme.tbshopeeauthen SET `PartnerKey` = @inPartnerKey  WHERE `Id` = 1", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inPartnerKey", key);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public async Task<MySqlResultState> ShopeeSaveCodeAsync(string code, MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    "UPDATE webplaywithme.tbshopeeauthen SET `Code` = @inCode, `RefreshCodeTime` = NOW() WHERE `Id` = 1", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inCode", code);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public async Task<MySqlResultState> CopyShopeeProductImageToProductAsync()
        {
            MySqlResultState resultState = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Get_Media_For_Product", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        long longItemId = 0;
                        long longModelId = 0;
                        int productId = 0;
                        //string shopeePath = "";//((App)Application.Current).temporaryShopee;
                        //string productPath = @"C:\Users\phong\TUNM\Works\WebPlayWithMe\MVCPlayWithMe\MVCPlayWithMe\Media\Product";

                        try
                        {
                            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                while (await rdr.ReadAsync())
                                {
                                    longItemId = MyMySql.GetInt64(rdr, "TMDTShopeeItemId");
                                    longModelId = MyMySql.GetInt64(rdr, "TMDTShopeeModelId");
                                    productId = MyMySql.GetInt32(rdr, "ProductId");

                                    //CopyImageToProduct(longItemId, longModelId, productId, shopeePath, productPath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MyLogger.GetInstance().Warn(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            return resultState;
        }

        //// async không hỗ trợ ref params — giữ sync
        //public void GetIdNameStatusDetailOfItemShopee(ref int itemId,
        //    ref string itemName,
        //    ref int itemStatus,
        //    ref string itemDetail,
        //    long itemIdShopee)
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_TMDTShopeeItemId", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", itemIdShopee);

        //    MySqlDataReader rdr = null;
        //    try
        //    {
        //        conn.Open();

        //        rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            itemId = MyMySql.GetInt32(rdr, "Id");
        //            itemName = MyMySql.GetString(rdr, "Name");
        //            itemStatus = MyMySql.GetInt32(rdr, "Status");
        //            itemDetail = MyMySql.GetString(rdr, "Detail");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        itemId = -1;
        //    }
        //    rdr.Close();
        //    conn.Close();
        //}

        public async Task<List<long>> GetForSaveImageSourceConnectOutAsync(MySqlConnection conn)
        {
            List<long> lsItem = new List<long>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_For_Save_Image_Source", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int tmdtShopeeItemIdIndex = rdr.GetOrdinal("TMDTShopeeItemId");
                        while (await rdr.ReadAsync())
                        {
                            lsItem.Add(rdr.GetInt64(tmdtShopeeItemIdIndex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return lsItem;
        }

        public async Task UpdateImageSourceTotbShopeeItem_ModelConnectOutAsync(
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            try
            {
                // Cập nhật source của item
                using (MySqlCommand cmdItem = new MySqlCommand(
                    "UPDATE tbshopeeitem SET Image = @inUrl WHERE TMDTShopeeItemId = @inTMDTShopeeItemId", conn))
                {
                    cmdItem.CommandType = CommandType.Text;
                    cmdItem.Parameters.AddWithValue("@inUrl", "");
                    cmdItem.Parameters.AddWithValue("@inTMDTShopeeItemId", 0L);

                    using (MySqlCommand cmdModel = new MySqlCommand(
                        "UPDATE tbshopeemodel SET Image = @inUrl WHERE TMDTShopeeModelId = @inTMDTShopeeModelId", conn))
                    {
                        cmdModel.CommandType = CommandType.Text;
                        cmdModel.Parameters.AddWithValue("@inUrl", "");
                        cmdModel.Parameters.AddWithValue("@inTMDTShopeeModelId", 0L);

                        foreach (var item in lsCommonItem)
                        {
                            cmdItem.Parameters[0].Value = item.imageSrc;
                            cmdItem.Parameters[1].Value = item.itemId;
                            await cmdItem.ExecuteNonQueryAsync();

                            foreach (var model in item.models)
                            {
                                cmdModel.Parameters[0].Value = model.imageSrc;
                                cmdModel.Parameters[1].Value = model.modelId;
                                await cmdModel.ExecuteNonQueryAsync();
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

        public async Task UpdateStatusOfItemFromTMDTItemIdAsync(long itemId, int status)
        {
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    using (MySqlCommand cmdItem = new MySqlCommand(
                        "UPDATE tbshopeeitem SET Status = @inStatus WHERE TMDTShopeeItemId = @inItemId", conn))
                    {
                        cmdItem.CommandType = CommandType.Text;
                        cmdItem.Parameters.AddWithValue("@inStatus", status);
                        cmdItem.Parameters.AddWithValue("@inItemId", itemId);
                        await cmdItem.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
        }

        public async Task GetTrackingNumberToListConnectOutAsync(
            List<ShopeeOrderDetail> rs,
            MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT `ShipCode` FROM webplaywithme.tbecommerceorder WHERE `Code` = @inCode AND `ShipCode` IS NOT NULL AND `ShipCode` <> '' AND `ECommmerce` = 2 LIMIT 1", conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inCode", "");

                // Đơn ở trạng thái: UNPAID, READY_TO_SHIP => chưa được sàn sinh mã vận chuyển.
                // Nhà bán chưa xác nhận đơn, khách hủy (trạng thái sẽ là CANCELLED) => chưa được sinh mã vận chuyển
                // Ngược lại đã được sinh mã đơn.
                // Ở trạng thái PROCESSED: Nhà bán đã xác nhận nhưng có thể chưa được đóng nên chưa có mã vận chuyển trong db
                // Nhiều khi đưa shipper đơn nhưng vẫn chưa cập nhật đã đóng => trạng thái SHIPPED, COMPLETE mà vẫn chưa có mã vận chuyển
                foreach (var e in rs)
                {
                    if (e.order_status != "UNPAID" && e.order_status != "READY_TO_SHIP")
                    {
                        cmd.Parameters[0].Value = e.order_sn;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                e.shipCode = MyMySql.GetString(rdr, "ShipCode");
                            }
                        }
                    }
                }
            }
        }

        public async Task GetBookingTrackingNumberToListConnectOutAsync(
            List<ShopeeBookingDetail> rs,
            MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT `ShipCode` FROM webplaywithme.tb_ecommerce_booking WHERE `Code` = @inCode AND `ShipCode` IS NOT NULL AND `ShipCode` <> '' AND `ECommmerce` = 2 LIMIT 1", conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inCode", "");

                // Booking ở trạng thái: READY_TO_SHIP => chưa được sàn sinh mã vận chuyển.
                // Ở trạng thái PROCESSED: Nhà bán đã xác nhận nhưng có thể chưa được đóng nên chưa có mã vận chuyển trong db
                // Nhiều khi đưa shipper đơn nhưng vẫn chưa cập nhật đã đóng => trạng thái SHIPPED mà vẫn chưa có mã vận chuyển
                foreach (var e in rs)
                {
                    if (e.booking_status != "READY_TO_SHIP")
                    {
                        cmd.Parameters[0].Value = e.booking_sn;
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                e.shipCode = MyMySql.GetString(rdr, "ShipCode");
                            }
                        }
                    }
                }
            }
        }

        // Cập nhật mã vận chuyển theo mã đơn nếu mã đơn tồn tại trong db
        public async Task UpdateTrackingNumberToDBConnectOutAsync(
            string orderSN,
            string shipCode,
            MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand(
                "UPDATE webplaywithme.tbecommerceorder SET `ShipCode` = @inShipCode WHERE `Code` = @inCode AND (`ShipCode` IS NULL OR `ShipCode` = '') AND `ECommmerce` = 2", conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inCode", orderSN);
                cmd.Parameters.AddWithValue("@inShipCode", shipCode);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Cập nhật mã vận chuyển theo mã booking nếu mã booking tồn tại trong db
        public async Task UpdateBookingTrackingNumberToDBConnectOutAsync(
            string booking_Sn,
            string shipCode,
            MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand(
                "UPDATE webplaywithme.tb_ecommerce_booking SET `ShipCode` = @inShipCode WHERE `Code` = @inCode AND (`ShipCode` IS NULL OR `ShipCode` = '') AND `ECommmerce` = 2", conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inCode", booking_Sn);
                cmd.Parameters.AddWithValue("@inShipCode", shipCode);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // khi upload ảnh lên Shopee, lưu id ảnh
        public async Task<MySqlResultState> InserttbShopeeMediaSpaceAsync(
            int mediaType, // 0: là ảnh, 1: video
            string mediaId,
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbshopee_media_space_Insert", conn))
                {
                    cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
                    cmd.Parameters.AddWithValue("@p_MediaId", mediaId);
                    cmd.Parameters.AddWithValue("@p_ProductId", productId);
                    cmd.Parameters.AddWithValue("@p_ProductType", productType);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public async Task<List<string>> GetUploadedImageOfProductOnShopeeAsync(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {
            List<string> images = new List<string>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbshopee_media_space_Get_MediaId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
                    cmd.Parameters.AddWithValue("@p_ProductId", productId);
                    cmd.Parameters.AddWithValue("@p_ProductType", productType);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int mediaIdIndex = rdr.GetOrdinal("MediaId");
                        while (await rdr.ReadAsync())
                        {
                            images.Add(rdr.GetString(mediaIdIndex));
                        }
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

        // Lấy số lượng ảnh đã up lên shopee của sản phẩm
        public async Task<int> GetQuantityOfProductImageUploadedToShopeeAsync(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {
            int count = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT COUNT(Id) AS Count FROM tbshopee_media_space
            WHERE MediaType = @p_MediaType
            AND ProductId = @p_ProductId
            AND ProductType = @p_ProductType;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
                    cmd.Parameters.AddWithValue("@p_ProductId", productId);
                    cmd.Parameters.AddWithValue("@p_ProductType", productType);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            count = rdr.GetInt32(rdr.GetOrdinal("Count"));
                        }
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

        // Vì upload lại ảnh lên shopee, xóa id ảnh cũ đã lưu trong db
        public async Task<MySqlResultState> DeleteProductImageUploadedToShopeeAsync(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("DeleteProductImageUploadedToShopee CALL");
            MyLogger.GetInstance().Info("mediaType: " + mediaType + ", productId: " + productId + ", productType: " + productType);

            MySqlResultState resultState = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"DELETE FROM tbshopee_media_space
            WHERE MediaType = @p_MediaType
            AND ProductId = @p_ProductId
            AND ProductType = @p_ProductType;", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
                    cmd.Parameters.AddWithValue("@p_ProductId", productId);
                    cmd.Parameters.AddWithValue("@p_ProductType", productType);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }
            return resultState;
        }

        // khi upload ảnh lên Shopee, lưu id ảnh
        public async Task<MySqlResultState> InserttbShopeeBrandAsync(
            long categoryId,
            List<ShopeeBrandObject> brandList,
            MySqlConnection conn)
        {
            MySqlResultState resultState = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopee_Brand_Insert", conn))
                {
                    cmd.Parameters.AddWithValue("@p_CategoryId", categoryId);
                    cmd.Parameters.AddWithValue("@p_OriginalBrandName", "");
                    cmd.Parameters.AddWithValue("@p_BrandId", 0L);
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var brand in brandList)
                    {
                        cmd.Parameters[1].Value = brand.original_brand_name;
                        cmd.Parameters[2].Value = brand.brand_id;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            return resultState;
        }

        public static void ShopeeReadCommonItem(List<CommonItem> list, MySqlDataReader rdr)
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

        public async Task<ShopeeBrandRequestParameter> GetBrandFromNameAsync(
            string brandName,
            MySqlConnection conn)
        {
            brandName = brandName.ToLower().Trim();
            ShopeeBrandRequestParameter brand = null;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT * FROM webplaywithme.tbshopee_brand WHERE LOWER(OriginalBrandName) LIKE '%" + brandName +
                    @"%' ORDER BY CHAR_LENGTH(OriginalBrandName) ASC LIMIT 1; ", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int brandIdIndex = rdr.GetOrdinal("BrandId");
                        int originalBrandNameIndex = rdr.GetOrdinal("OriginalBrandName");
                        while (await rdr.ReadAsync())
                        {
                            brand = new ShopeeBrandRequestParameter(rdr.GetInt64(brandIdIndex),
                                rdr.GetString(originalBrandNameIndex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                brand = null;
            }
            return brand;
        }

        // Kết nối đóng mở bên ngoài
        // Lấy được Shopee Item mapping với sản phẩm trong kho thuộc 1 combo
        public static async Task<List<CommonItem>> ShopeeGetListMappingOfComboAsync(int comboId, MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_Mapping_Combo_Id", conn))
                {
                    cmd.Parameters.AddWithValue("@inComboId", comboId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            ShopeeReadCommonItem(listCI, rdr);
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

        // Kết nối đóng mở bên ngoài
        // Từ bảng tbNeedUpdateQuantity lấy được danh sách sản phẩm shopee có thay đổi số lượng
        // cần cập nhật
        public static async Task<List<CommonItem>> ShopeeGetListNeedUpdateQuantityConnectOutAsync(MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_Need_Update_Quantity", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            ShopeeReadCommonItem(listCI, rdr);
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

        // Kết nối đóng mở bên ngoài
        // Lấy được Shopee Item mapping với sản phẩm trong kho
        public static async Task<List<CommonItem>> ShopeeGetListMappingOfProductAsync(int productId, MySqlConnection conn)
        {
            List<CommonItem> listCI = new List<CommonItem>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_Mapping_Product_Id", conn))
                {
                    cmd.Parameters.AddWithValue("@inProductId", productId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            ShopeeReadCommonItem(listCI, rdr);
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
    }
}
