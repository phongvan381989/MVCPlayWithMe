using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    /// <summary>
    /// Xử lý db liên quan đến shopee
    /// </summary>
    public class ShopeeMySql : BasicMySql
    {
        /// <summary>
        /// Đóng mở kết nối bên ngoài
        /// </summary>
        /// <param name="tMDTShopeeItemId"></param>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <param name="detail"></param>
        /// <returns>-2 nếu tMDTShopeeItemId đã tồn tại, -1 nếu có lỗi</returns>
        private int InserttbShopeeItem(long tMDTShopeeItemId, string name, int status,
            string detail, MySqlConnection conn)
        {
            int id = 0;
            MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", tMDTShopeeItemId);
            cmd.Parameters.AddWithValue("@inName", name);

            cmd.Parameters.AddWithValue("@inStatus", status);
            cmd.Parameters.AddWithValue("@inDetail", detail);

            try
            {
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
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
        private int InsertShopeeModel(int itemId, long tMDTShopeeModelId, string name,
            int status, MySqlConnection conn)
        {
            int id = 0;
            MySqlCommand cmd = new MySqlCommand("st_tbShopeeModel_Insert", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@inItemId", itemId);
            cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", tMDTShopeeModelId);
            cmd.Parameters.AddWithValue("@inName", name);

            cmd.Parameters.AddWithValue("@inStatus", status);

            try
            {
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
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
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            return lsModel;
        }

        // Check item, model đã được lưu vào bảng tương ứng tbshopeeitem, tbshopeemodel
        // Nếu chưa lưu ta thực hiện lưu
        // Nếu đã lưu model trong db nhưng không còn tồn tại trên sàn TMDT ta xóa trong db
        // Ta chỉ check được với model khi chọn xem item, check item đã bị xóa trên db 
        // không thực hiện ở đây
        public void ShopeeInsertIfDontExist(CommonItem item)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            try
            {
                conn.Open();
 
                // Kiểm tra itemId đã tồn tại trong bảng tbshopeeitem
                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_From_TMDTShopeeItemId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", item.itemId);

                int itemIdInserted = 0;
                MySqlDataReader rdr = null;

                rdr = cmd.ExecuteReader();
                Boolean exist = false;
                while (rdr.Read())
                {
                    itemIdInserted = MyMySql.GetInt32(rdr,"Id");
                    exist = true;
                }
                if (rdr != null)
                    rdr.Close();
                int status = 0;
                // Lưu item, model vào db lần đầu
                if (!exist)
                {
                    status = item.bActive ? 0 : 1;
                    itemIdInserted = InserttbShopeeItem(item.itemId, item.name, status, item.detail, conn);

                    int length = item.models.Count;
                    for (int i = 0; i < length; i++)
                    {
                        status = item.models[i].bActive ? 0 : 1;
                        InsertShopeeModel(itemIdInserted, item.models[i].modelId,
                            item.models[i].name, status, conn);
                    }
                }
                else // Model trên sàn nào chưa lưu ta lưu vào db, model nào đã xóa trên sàn ta xóa trên db
                {
                    MyLogger.GetInstance().Warn("Start xử lý model trên sàn có thay đổi so với model lưu trên db");
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
                        // Xóa trên tbshopeemapping
                        {
                            MySqlCommand cmdTem = new MySqlCommand("DELETE * FROM webplaywithme.tbshopeemapping WHERE ShopeeModelId=@inShopeeModelId;", conn);
                            cmdTem.CommandType = CommandType.Text;
                            cmdTem.Parameters.AddWithValue("@inShopeeModelId", 0);
                            foreach (var id in lsTMDTShopeeModelNeedDeleteOnDb)
                            {
                                cmdTem.Parameters[0].Value = id;
                                cmdTem.ExecuteNonQuery();
                            }
                        }
                        // Xóa trên tbshopeemodel
                        {
                            MySqlCommand cmdTem = new MySqlCommand("DELETE * FROM webplaywithme.tbshopeemodel WHERE Id=@inShopeeModelId;", conn);
                            cmdTem.CommandType = CommandType.Text;
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
                        foreach(var m in lsTMDTShopeeModelNeedSaveOnDb)
                        {
                            status = m.bActive ? 0 : 1;
                            InsertShopeeModel(itemIdInserted, m.modelId,
                                m.name, status, conn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }
            conn.Close();
        }

        // Lấy được thông tin chi tiết
        public void ShopeeGetItemFromId(long id, CommonItem item)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_All_From_TMDTShopeeItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", id);
                MySqlDataReader rdr = cmd.ExecuteReader();

                long modelIdTemp;
                while (rdr.Read())
                {
                    if (MyMySql.GetInt32(rdr, "ShopeeMappingId") != -1)
                    {
                        Mapping map = new Mapping();
                        map.quantity = MyMySql.GetInt32(rdr, "ShopeeMappingQuantity");
                        map.product = ItemModelMySql.ConvertOneRowFromDataMySqlToProduct(rdr);

                        modelIdTemp = MyMySql.GetInt64(rdr, "TMDTShopeeModelId");
                        // Tìm model object
                        foreach (var model in item.models)
                        {
                            if(model.modelId == modelIdTemp)
                            {
                                model.mapping.Add(map);
                                break;
                            }
                        }
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
                            // Nếu model chưa được mapping productId, productQuantity là: System.Int32.MinValue;
                            if (ls[i].lsProductId[j] > 0)
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }
            conn.Close();
            return result;
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public void UpdateMappingToCommonOrder(List<CommonOrder> ls)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            string status = string.Empty;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeMapping_Get_From_Item_ModelId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", long.MinValue);
                cmd.Parameters.AddWithValue("@inTMDTShopeeModelId", long.MinValue);

                int quantity = 0;
                Product pro = null;
                MySqlDataReader rdr;
                foreach (var order in ls)
                {
                    if (order.ecommerceName != Common.eShopee)
                        continue;

                    for (int i = 0; i < order.listItemId.Count; i++)
                    {

                        cmd.Parameters[0].Value = order.listItemId[i];
                        cmd.Parameters[1].Value = order.listModelId[i];
                        order.listMapping.Add(new List<Mapping>());

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
                                pro.SetSrcImageVideo();
                                order.listMapping[i].Add(new Mapping(pro, quantity));
                            }
                        }
                        if (rdr != null)
                            rdr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
        }
    }
}