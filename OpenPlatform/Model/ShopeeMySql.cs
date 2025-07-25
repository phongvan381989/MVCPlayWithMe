﻿using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeConfig;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeOrder;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using QuanLyKho.ViewModel.Dev.ShopeeAPI.ShopeeCreateProduct;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeCreateProduct;

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
        /// <param name="tMDTShopeeItemId"></param>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <param name="detail"></param>
        /// <returns>-2 nếu tMDTShopeeItemId đã tồn tại, -1 nếu có lỗi</returns>
        public int InserttbShopeeItem(CommonItem item, MySqlConnection conn)
        {
            int id = 0;
            MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", item.itemId);
            cmd.Parameters.AddWithValue("@inName", item.name);

            cmd.Parameters.AddWithValue("@inStatus", CommonOpenPlatform.ShopeeGetEnumValueFromString(item.item_status));
            cmd.Parameters.AddWithValue("@inImage", item.imageSrc);
            cmd.Parameters.AddWithValue("@inDetail", item.detail);

            try
            {
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "LastId");
                }
                rdr.Close();
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
        /// <param name="itemId"></param>
        /// <param name="tMDTShopeeModelId"></param>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <returns>-2 nếu tMDTShopeeModelId đã tồn tại, -1 nếu có lỗi</returns>
        public int InserttbShopeeModel(int itemId, CommonModel model, MySqlConnection conn)
        {
            MyLogger.GetInstance().Warn("Start InserttbShopeeModel");
            MyLogger.GetInstance().Warn("itemId: " + itemId.ToString());
            MyLogger.GetInstance().Warn("tMDTShopeeModelId: " + model.modelId.ToString());
            int id = 0;
            MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inItemId", itemId);
            cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", model.modelId);
            cmd.Parameters.AddWithValue("@inName", model.name);

            cmd.Parameters.AddWithValue("@inStatus", model.bActive ? 0 : 1);
            cmd.Parameters.AddWithValue("@inImage", model.imageSrc);

            try
            {
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "LastId");
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                id = -1;
            }

            return id;
        }

        // Lấy được danh sách ShopeeModelId, TMDTShopeeModelId 
        private List<Tuple<int,long>> ListModelOfItem(long tMDTShopeeModelId, MySqlConnection conn)
        {
            List<Tuple<int, long>> lsModel = new List<Tuple<int, long>>();
            MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_Model_From_TMDTShopeeItemId", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", tMDTShopeeModelId);

            try
            {
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lsModel.Add(Tuple.Create(MyMySql.GetInt32(rdr, "ShopeeModelId"), 
                        MyMySql.GetInt64(rdr, "TMDTShopeeModelId")));
                }
                rdr.Close();
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
        public Boolean ShopeeInsertIfDontExistConnectOut(CommonItem item, MySqlConnection conn)
        {
            Boolean exist = false;
            try
            {
                // Kiểm tra itemId đã tồn tại trong bảng tbshopeeitem
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_TMDTShopeeItemId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", item.itemId);

                int itemIdInserted = 0;

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        itemIdInserted = MyMySql.GetInt32(rdr, "Id");
                        exist = true;
                    }
                }
                int status = 0;
                // Lưu item, model vào db lần đầu
                if (!exist)
                {
                    //status = item.bActive ? (int)CommonOpenPlatform.ShopeeProductStatus.NORMAL : (int)CommonOpenPlatform.ShopeeProductStatus.BANNED;
                    //status = CommonOpenPlatform.ShopeeGetEnumValueFromString(item.item_status);
                    itemIdInserted = InserttbShopeeItem(item, conn);

                    int length = item.models.Count;
                    for (int i = 0; i < length; i++)
                    {
                        InserttbShopeeModel(itemIdInserted, item.models[i], conn);
                    }
                }
                else // Model trên sàn nào chưa lưu ta lưu vào db, model nào đã xóa trên sàn ta xóa trên db
                {
                    MyLogger.GetInstance().Warn("Start xử lý model trên sàn có thay đổi so với model lưu trên db");

                    // Cập nhật trạng thái item vào DB
                    ShopeeUpdateStatusOfItemToDbConnectOut(item, conn);

                    // Cập nhật trạng thái Item
                    List<CommonItem> listCommonItem = new List<CommonItem>();
                    listCommonItem.Add(item);
                    ShopeeUpdateStatusOfItemListToDbConnectOut(listCommonItem, conn);

                    // Lấy list shopee model id đã lưu trong db
                    List<Tuple<int, long>> lsTupleTMDTShopeeModelOnDb = ListModelOfItem(item.itemId, conn);

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
                        {
                            MySqlCommand cmdTem = new MySqlCommand("st_tbShopeeModel_Delete_From_Id", conn);
                            cmdTem.CommandType = CommandType.StoredProcedure;
                            cmdTem.Parameters.AddWithValue("@inShopeeModelId", 0);
                            foreach (var id in lsTMDTShopeeModelNeedDeleteOnDb)
                            {
                                cmdTem.Parameters[0].Value = id;
                                cmdTem.ExecuteNonQuery();
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
                    {
                        if (lsTMDTShopeeModelNeedSaveOnDb.Count > 0)
                        {
                            exist = false;
                            foreach (var m in lsTMDTShopeeModelNeedSaveOnDb)
                            {
                                InserttbShopeeModel(itemIdInserted, m, conn);
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

        // Lấy được thông tin chi tiết
        public void ShopeeGetItemFromIdConnectOut(long id, CommonItem item, MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_All_From_TMDTShopeeItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", id);
                MySqlDataReader rdr = cmd.ExecuteReader();

                long modelIdTemp;
                while (rdr.Read())
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
                            if(model.modelId == modelIdTemp)
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

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
        }

        public void ShopeeGetListCommonItemFromListShopeeItemConnectOut(
            List<ShopeeGetItemBaseInfoItem> lsShopeeItem,
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_All_From_TMDTShopeeItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", long.MaxValue);
                long modelIdTemp;
                foreach (var pro in lsShopeeItem)
                {
                    cmd.Parameters[0].Value = pro.item_id;
                    CommonItem item = new CommonItem(pro);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
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
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public void ShopeeUpdateStatusOfItemListToDbConnectOut(
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            if (lsCommonItem == null || lsCommonItem.Count == 0)
            {
                return;
            }

            try
            {
                // Cập nhật source của item
                MySqlCommand cmd = new MySqlCommand("UPDATE tbshopeeitem SET Status = @inStatus WHERE TMDTShopeeItemId = @inItemId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inStatus", 0);
                cmd.Parameters.AddWithValue("@inItemId", 0L);
                foreach (var commonItem in lsCommonItem)
                {
                    cmd.Parameters[0].Value = CommonOpenPlatform.ShopeeGetEnumValueFromString(commonItem.item_status);
                    cmd.Parameters[1].Value = commonItem.itemId;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public void ShopeeUpdateStatusOfItemToDbConnectOut(
            CommonItem commonItem,
            MySqlConnection conn)
        {
            try
            {
                // Cập nhật source của item
                MySqlCommand cmd = new MySqlCommand("UPDATE tbshopeeitem SET Status = @inStatus WHERE TMDTShopeeItemId = @inItemId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inStatus", CommonOpenPlatform.ShopeeGetEnumValueFromString(commonItem.item_status));
                cmd.Parameters.AddWithValue("@inItemId", commonItem.itemId);
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public List<CommonItem> ShopeeGetItemOnDB()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_Item_Model_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                long itemId = long.MaxValue;
                long itemIdTem = long.MaxValue;

                long modelId = long.MaxValue;
                long modelIdTem = long.MaxValue;

                MySqlDataReader rdr = cmd.ExecuteReader();

                int tmdtShopeeItemIdIndex = rdr.GetOrdinal("TMDTShopeeItemId");
                int shopeeItemNameIndex = rdr.GetOrdinal("ShopeeItemName");
                int shopeeItemStatusIndex = rdr.GetOrdinal("ShopeeItemStatus");

                int tmdtShopeeModelIdIndex = rdr.GetOrdinal("TMDTShopeeModelId");
                int shopeeModelNameIndex = rdr.GetOrdinal("ShopeeModelName");

                int productStatusIndex = rdr.GetOrdinal("ProductStatus");

                CommonItem commonItem = null;
                CommonModel commonModel = null;
                int status = 0;
                while (rdr.Read())
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
                        commonItem.bActive = status == 0 ? true: false;
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

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                lsCommonItem = null;
            }

            conn.Close();
            return lsCommonItem;
        }

        public MySqlResultState ShopeeDeleteItemOnDB(long itemId)
        {
            MySqlResultState resultState = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Delete_From_TMDTShopeeItemId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", itemId);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            conn.Close();
            return resultState;
        }

        // Trường hợp item chỉ có 1 modelId, modelId khác -1 hàm dưới chưa xóa dữ liệu tương ứng trong bảng tbShopeeItem
        // Kết quả là có 1 item không có model nào trong DB
        // Xóa trên tbshopeemapping, tbpwmmappingother, tbshopeemodel
        public MySqlResultState ShopeeDeleteModelOnDB(long modelId)
        {
            MySqlResultState resultState = new MySqlResultState();
            if (modelId == -1)
                return resultState;

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Delete_From_TMDTShopeeModeId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", modelId);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            conn.Close();
            return resultState;
        }

        public MySqlResultState ShopeeUpdateMapping(List<CommonForMapping> ls)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            int length = ls.Count;
            try
            {
                conn.Open();
                // Lấy danh sách shopeeModelId
                List<long> lsShopeeModelId = new List<long>();
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Get_From_ItemId_ModelId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inItemId", Int64.MaxValue);
                    cmd.Parameters.AddWithValue("@inModelId", Int64.MaxValue);

                    for (int i = 0; i < length; i++)
                    {
                        cmd.Parameters[0].Value = ls[i].itemId;
                        cmd.Parameters[1].Value = ls[i].modelId;
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            lsShopeeModelId.Add(MyMySql.GetInt32(rdr, "ShopeeModelId"));
                        }
                        rdr.Close();
                    }
                }

                // Xóa mapping cũ
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Delete_From_ShopeeModelId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inShopeeModelId", 0);
                    for (int i = 0; i < length; i++)
                    {
                        cmd.Parameters[0].Value = lsShopeeModelId[i];
                        cmd.ExecuteNonQuery();
                    }
                }

                // Insert mapping mới
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inShopeeModelId", 0);
                    cmd.Parameters.AddWithValue("@inProductId", 0);
                    cmd.Parameters.AddWithValue("@inProductQuantity", 0);
                    for (int i = 0; i < length; i++)
                    {
                        cmd.Parameters[0].Value = lsShopeeModelId[i];
                        for (int j = 0; j < ls[i].lsProductId.Count; j++)
                        {
                            // Nếu model chưa được mapping
                            if (ls[i].lsProductId.Count > 0)
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

        // Insert mapping của model mới, và model chỉ mapping với 1 sản phẩm
        public MySqlResultState ShopeeInsertNewMappingOneOfModel(int modelId, 
            int productId, 
            int quantity,
            MySqlConnection conn)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inShopeeModelId", modelId);
            cmd.Parameters.AddWithValue("@inProductId", productId);
            cmd.Parameters.AddWithValue("@inProductQuantity", quantity);
            result = MyMySql.MyExcuteNonQuery(cmd);
            return result;
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public void ShopeeGetMappingOfCommonOrderConnectOut(CommonOrder commonOrder, MySqlConnection conn)
        {
            string status = string.Empty;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Get_From_Item_ModelId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", long.MinValue);
                cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", long.MinValue);

                int quantity = 0;
                Product pro = null;
                MySqlDataReader rdr;
                {
                    for (int i = 0; i < commonOrder.listItemId.Count; i++)
                    {

                        cmd.Parameters[0].Value = commonOrder.listItemId[i];
                        if (commonOrder.listModelId[i] == 0)// Không có model, Shopee trả modelId = 0, db lưu modelId = -1
                            cmd.Parameters[1].Value = -1;
                        else
                            cmd.Parameters[1].Value = commonOrder.listModelId[i];

                        commonOrder.listMapping.Add(new List<Mapping>());

                        rdr = cmd.ExecuteReader();
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
                        rdr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public ShopeeAuthen ShopeeGetAuthen()
        {
            ShopeeAuthen shopeeAuthen = new ShopeeAuthen();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Lưu vào bảng tbECommerceOrder
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tbShopeeAuthen WHERE Id = 1", conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    shopeeAuthen.shopId = MyMySql.GetString(rdr, "ShopId");
                    shopeeAuthen.partnerId = MyMySql.GetString(rdr, "PartnerId");
                    shopeeAuthen.partnerKey = MyMySql.GetString(rdr, "PartnerKey");
                    shopeeAuthen.code = MyMySql.GetString(rdr, "Code");
                    shopeeAuthen.shopeeToken.access_token = MyMySql.GetString(rdr, "AccessToken");
                    shopeeAuthen.shopeeToken.refresh_token = MyMySql.GetString(rdr, "RefreshToken");
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                shopeeAuthen = null;
            }

            conn.Close();

            return shopeeAuthen;
        }

        public MySqlResultState ShopeeSaveToken(ShopeeToken shopeeToken)
        {
            MySqlResultState resultState = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd =
                    new MySqlCommand("UPDATE `tbShopeeAuthen` SET `AccessToken`=@AccessToken," +
                    " `RefreshToken`=@RefreshToken WHERE `Id` = 1", conn);
                cmd.Parameters.AddWithValue("@AccessToken", shopeeToken.access_token);
                cmd.Parameters.AddWithValue("@RefreshToken", shopeeToken.refresh_token);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            conn.Close();

            return resultState;
        }

        public MySqlResultState ShopeeSaveCode(string code)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE webplaywithme.tbshopeeauthen SET `Code` = @inCode, `RefreshCodeTime` = NOW() WHERE `Id` = 1", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inCode", code);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        public MySqlResultState CopyShopeeProductImageToProduct()
        {
            MySqlResultState resultState = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Get_Media_For_Product", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                long longItemId = 0;
                long longModelId = 0;
                int productId = 0;
                //string shopeePath = "";//((App)Application.Current).temporaryShopee;
                //string productPath = @"C:\Users\phong\TUNM\Works\WebPlayWithMe\MVCPlayWithMe\MVCPlayWithMe\Media\Product";

                MySqlDataReader rdr = null;
                try
                {
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        longItemId = MyMySql.GetInt64(rdr, "TMDTShopeeItemId");
                        longModelId = MyMySql.GetInt64(rdr, "TMDTShopeeModelId");
                        productId = MyMySql.GetInt32(rdr, "ProductId");

                        //CopyImageToProduct(longItemId, longModelId, productId, shopeePath, productPath);
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
                rdr.Close();
                conn.Close();

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            conn.Close();

            return resultState;
        }

        public void GetIdNameStatusDetailOfItemShopee(ref int itemId,
            ref string itemName,
            ref int itemStatus,
            ref string itemDetail,
            long itemIdShopee)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_TMDTShopeeItemId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", itemIdShopee);

            MySqlDataReader rdr = null;
            try
            {
                conn.Open();

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    itemId = MyMySql.GetInt32(rdr, "Id");
                    itemName = MyMySql.GetString(rdr, "Name");
                    itemStatus = MyMySql.GetInt32(rdr, "Status");
                    itemDetail = MyMySql.GetString(rdr, "Detail");
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                itemId = -1;
            }
            rdr.Close();
            conn.Close();
        }

        public List<long> GetForSaveImageSourceConnectOut(MySqlConnection conn)
        {
            List<long> lsItem = new List<long>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_For_Save_Image_Source", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    int tmdtShopeeItemIdIndex = rdr.GetOrdinal("TMDTShopeeItemId");
                    while (rdr.Read())
                    {
                        lsItem.Add(rdr.GetInt64(tmdtShopeeItemIdIndex));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return lsItem;
        }

        public void UpdateImageSourceTotbShopeeItem_ModelConnectOut(
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            try
            {
                // Cập nhật source của item
                MySqlCommand cmdItem = new MySqlCommand("UPDATE tbshopeeitem SET Image = @inUrl WHERE TMDTShopeeItemId = @inTMDTShopeeItemId", conn);
                cmdItem.CommandType = CommandType.Text;
                cmdItem.Parameters.AddWithValue("@inUrl", "");
                cmdItem.Parameters.AddWithValue("@inTMDTShopeeItemId", 0L);

                // Cập nhật source của model
                MySqlCommand cmdModel = new MySqlCommand("UPDATE tbshopeemodel SET Image = @inUrl WHERE TMDTShopeeModelId = @inTMDTShopeeModelId", conn);
                cmdModel.CommandType = CommandType.Text;
                cmdModel.Parameters.AddWithValue("@inUrl", "");
                cmdModel.Parameters.AddWithValue("@inTMDTShopeeModelId", 0L);

                foreach (var item in lsCommonItem)
                {
                    cmdItem.Parameters[0].Value = item.imageSrc;
                    cmdItem.Parameters[1].Value = item.itemId;
                    cmdItem.ExecuteNonQuery();

                    foreach(var model in item.models)
                    {
                        cmdModel.Parameters[0].Value = model.imageSrc;
                        cmdModel.Parameters[1].Value = model.modelId;
                        cmdModel.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        //
        public void UpdateStatusOfItemFromTMDTItemId(long itemId, int status)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Cập nhật source của item
                MySqlCommand cmdItem = new MySqlCommand("UPDATE tbshopeeitem SET Status = @inStatus WHERE TMDTShopeeItemId = @inItemId", conn);
                cmdItem.CommandType = CommandType.Text;
                cmdItem.Parameters.AddWithValue("@inStatus", status);
                cmdItem.Parameters.AddWithValue("@inItemId", itemId);
            }
            catch (Exception ex)
            {

                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
        }

        public void UpdateTrackingNumberToListConnectOut(
            List<ShopeeOrderDetail> rs,
            MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT `ShipCode` FROM webplaywithme.tbecommerceorder WHERE `Code` = @inCode AND `ShipCode` IS NOT NULL AND `ShipCode` <> '' AND `ECommmerce` = 2 LIMIT 1", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@inCode", "");
            MySqlDataReader rdr = null;

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
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        e.shipCode = MyMySql.GetString(rdr, "ShipCode");
                    }

                    rdr.Close();
                }
            }
        }

        // Cập nhật mã vận chuyển theo mã đơn nếu mã đơn tồn tại trong db
        public void UpdateTrackingNumberToDBConnectOut(
            string orderSN,
            string shipCode,
            MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(
                "UPDATE webplaywithme.tbecommerceorder SET `ShipCode` = @inShipCode WHERE `Code` = @inCode AND (`ShipCode` IS NULL OR `ShipCode` = '') AND `ECommmerce` = 2", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@inCode", orderSN);
            cmd.Parameters.AddWithValue("@inShipCode", shipCode);
            cmd.ExecuteNonQuery();
        }

        // Lấy mã đơn, mã đơn hàng từ mã đơn hoặc mã đơn hàng
        public void GetSN_TrackingNumberFromSN_TrackingNumberConnectOut(
            string sn_trackingNumber,
            ref string sn,
            ref string trackingNumber,
            MySqlConnection conn)
        {
            sn = string.Empty;
            trackingNumber = string.Empty;
            MySqlCommand cmd = new MySqlCommand(
                "SELECT `Code`, `ShipCode` FROM webplaywithme.tbecommerceorder WHERE (`Code` = @inCode OR `ShipCode` = @inShipCode) AND `ECommmerce` = 2 ORDER BY `ShipCode` DESC LIMIT 1", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@inCode", sn_trackingNumber);
            cmd.Parameters.AddWithValue("@inShipCode", sn_trackingNumber);
            MySqlDataReader rdr = null;

            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                sn = MyMySql.GetString(rdr, "Code");
                trackingNumber = MyMySql.GetString(rdr, "ShipCode");
            }
            rdr.Close();
        }

        public string GetTrackingNumberFromSNConnectOut(
            string sn,
            MySqlConnection conn)
        {
            string trackingNumber = string.Empty;
            MySqlCommand cmd = new MySqlCommand(
                "SELECT `ShipCode` FROM webplaywithme.tbecommerceorder WHERE `Code` = @inCode AND `ShipCode` IS NOT NULL AND `ShipCode` <> '' AND `ECommmerce` = 2 LIMIT 1", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@inCode", sn);
            MySqlDataReader rdr = null;

            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                trackingNumber = MyMySql.GetString(rdr, "ShipCode");
            }
            rdr.Close();

            return trackingNumber;
        }

        // khi upload ảnh lên Shopee, lưu id ảnh
        public MySqlResultState InserttbShopeeMediaSpace(
            int mediaType, // 0: là ảnh, 1: video
            string mediaId,
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn) 
        {
            MySqlCommand cmd =
                new MySqlCommand("st_tbshopee_media_space_Insert", conn);
            cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
            cmd.Parameters.AddWithValue("@p_MediaId", mediaId);
            cmd.Parameters.AddWithValue("@p_ProductId", productId);
            cmd.Parameters.AddWithValue("@p_ProductType", productType);
            cmd.CommandType = CommandType.StoredProcedure;

            return MyMySql.MyExcuteNonQuery(cmd);
        }

        public List<string> GetUploadedImageOfProductOnShopee(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn
            )
        {
            List<string> images = new List<string>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                "st_tbshopee_media_space_Get_MediaId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_MediaType", mediaType);
                cmd.Parameters.AddWithValue("@p_ProductId", productId);
                cmd.Parameters.AddWithValue("@p_ProductType", productType);
                MySqlDataReader rdr = null;

                using (rdr = cmd.ExecuteReader())
                {
                    int mediaIdIndex = rdr.GetOrdinal("MediaId");
                    while (rdr.Read())
                    {
                        images.Add( rdr.GetString(mediaIdIndex));
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                images.Clear();
            }
            return images;
        }

        // Lấy số lượng ảnh đã up lên shopee của sản phẩm
        public int GetQuantityOfProductImageUploadedToShopee(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {
            MySqlCommand cmd =
                new MySqlCommand(@"SELECT COUNT(Id) AS Count FROM tbshopee_media_space 
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

        // Vì upload lại ảnh lên shopee, xóa id ảnh cũ đã lưu trong db
        public MySqlResultState DeleteProductImageUploadedToShopee(
            int mediaType, // 0: là ảnh, 1: video
            int productId, // Id của sản phẩm trong kho upload ảnh lên Shopee
            int productType, // 0: là sản phẩm riêng lẻ trong kho, 1: là sản phẩm combo
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("DeleteProductImageUploadedToShopee CALL");
            MyLogger.GetInstance().Info("mediaType: " + mediaType + ", productId: " + productId + ", productType: " + productType);

            MySqlCommand cmd =
                new MySqlCommand(@"DELETE FROM tbshopee_media_space 
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


        // khi upload ảnh lên Shopee, lưu id ảnh
        public MySqlResultState InserttbShopeeBrand(
            long categoryId,
            List<ShopeeBrandObject> brandList,
            MySqlConnection conn)
        {
            MySqlResultState resultState = new MySqlResultState();
            MySqlCommand cmd =
                new MySqlCommand("st_tbShopee_Brand_Insert", conn);
            cmd.Parameters.AddWithValue("@p_CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@p_OriginalBrandName", "");
            cmd.Parameters.AddWithValue("@p_BrandId", 0L);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                foreach (var brand in brandList)
                {
                    cmd.Parameters[1].Value = brand.original_brand_name;
                    cmd.Parameters[2].Value = brand.brand_id;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            return resultState;
        }

        public ShopeeBrandRequestParameter GetBrandFromName(string brandName, MySqlConnection conn)
        {
            ShopeeBrandRequestParameter brand = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                @"SELECT * FROM webplaywithme.tbshopee_brand WHERE OriginalBrandName LIKE '%" + brandName +
                @"%' ORDER BY CHAR_LENGTH(OriginalBrandName) ASC LIMIT 1; ", conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = null;

                using (rdr = cmd.ExecuteReader())
                {
                    int brandIdIndex = rdr.GetOrdinal("BrandId");
                    int originalBrandNameIndex = rdr.GetOrdinal("OriginalBrandName");
                    while (rdr.Read())
                    {
                        brand = new ShopeeBrandRequestParameter(rdr.GetInt64(brandIdIndex),
                            rdr.GetString(originalBrandNameIndex));
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
    }
}