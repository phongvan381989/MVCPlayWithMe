using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeConfig;
using MVCPlayWithMe.OpenPlatform.Model.ShopeeApp.ShopeeProduct;
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
        private int InserttbShopeeModel(int itemId, long tMDTShopeeModelId, string name,
            int status, MySqlConnection conn)
        {
            MyLogger.GetInstance().Warn("Start InserttbShopeeModel");
            MyLogger.GetInstance().Warn("itemId: " + itemId.ToString());
            MyLogger.GetInstance().Warn("tMDTShopeeModelId: " + tMDTShopeeModelId.ToString());
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
        public MySqlResultState ShopeeInsertIfDontExist(CommonItem item)
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
                        InserttbShopeeModel(itemIdInserted, item.models[i].modelId,
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
                            MySqlCommand cmdTem = new MySqlCommand("DELETE FROM webplaywithme.tbshopeemapping WHERE ShopeeModelId=@inShopeeModelId;", conn);
                            cmdTem.CommandType = CommandType.Text;
                            cmdTem.Parameters.AddWithValue("@inShopeeModelId", 0);
                            foreach (var id in lsTMDTShopeeModelNeedDeleteOnDb)
                            {
                                cmdTem.Parameters[0].Value = id;
                                cmdTem.ExecuteNonQuery();
                            }
                        }
                        // Xóa trên tbpwmmappingother
                        {
                            MySqlCommand cmdTem = new MySqlCommand("DELETE FROM webplaywithme.tbpwmmappingother WHERE ShopeeModelId=@inShopeeModelId;", conn);
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
                            MySqlCommand cmdTem = new MySqlCommand("DELETE FROM webplaywithme.tbshopeemodel WHERE Id=@inShopeeModelId;", conn);
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
                            InserttbShopeeModel(itemIdInserted, m.modelId,
                                m.name, status, conn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();
            return result;
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

        public void ShopeeGetListCommonItemFromListShopeeItem(
            List<ShopeeGetItemBaseInfoItem> lsShopeeItem,
            List<CommonItem> lsCommonItem)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbShopeeItem_Get_All_From_TMDTShopeeItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", long.MaxValue);
                long modelIdTemp;
                foreach (var pro in lsShopeeItem)
                {
                    cmd.Parameters[0].Value = pro.item_id;
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    CommonItem item = new CommonItem(pro);

                    while (rdr.Read())
                    {
                        if (MyMySql.GetInt32(rdr, "ShopeeMappingId") != -1)
                        {
                            Mapping map = new Mapping();
                            map.quantity = MyMySql.GetInt32(rdr, "ShopeeMappingQuantity");
                            //map.product = ItemModelMySql.ConvertOneRowFromDataMySqlToProduct(rdr);

                            Product product = new Product();
                            product.id = MyMySql.GetInt32(rdr, "ProductId");
                            product.name = MyMySql.GetString(rdr, "ProductName");
                            product.SetSrcImageVideo();
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

                    if (rdr != null)
                        rdr.Close();

                    lsCommonItem.Add(item);
                }
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
        public void ShopeeUpdateMappingToCommonOrder(CommonOrder commonOrder)
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
                                pro.SetSrcImageVideo();
                                commonOrder.listMapping[i].Add(new Mapping(pro, quantity));
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="commonOrder"></param>
        ///// <param name="status">Trạng thái thực tế đã thực hiện: 0: đã đóng hàng, 1: đã hoàn hàng nhập kho</param>
        //public void ShopeeEnoughProductInOrder(CommonOrder commonOrder, int status)
        //{
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Insert", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
        //        cmd.Parameters.AddWithValue("@inShipCode", commonOrder.shipCode);
        //        cmd.Parameters.AddWithValue("@inStatus", status);
        //        cmd.Parameters.AddWithValue("@inECommmerce", 2);

        //        cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        errMessage = ex.ToString();
        //        MyLogger.GetInstance().Warn(errMessage);
        //    }

        //    conn.Close();
        //}

        public ShopeeAuthen ShopeeGetAuthen()
        {
            ShopeeAuthen shopeeAuthen = new ShopeeAuthen();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Lưu vào bảng tbECommerceOrder
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tbShopeeAuthen", conn);
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
                if (rdr != null)
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
                if (rdr != null)
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
            if (rdr != null)
                rdr.Close();
            conn.Close();
        }
    }
}