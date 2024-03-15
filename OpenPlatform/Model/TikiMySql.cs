using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.OpenPlatform.Model
{
    public class TikiMySql : BasicMySql
    {
        private int TikiInsert(int supperId, int tikiId, string name,
            int status, MySqlConnection conn)
        {
            int id = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inSuperId", supperId);
                cmd.Parameters.AddWithValue("@inTikiId", tikiId);
                cmd.Parameters.AddWithValue("@inName", name);
                cmd.Parameters.AddWithValue("@inStatus", status);

                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "LastId");
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch(Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }
            return id;
        }

        // Check item đã được lưu vào bảng tương ứng tbtikiitem, tbtikimapping
        // Nếu chưa lưu ta thực hiện lưu
        // Nếu đã lưu item trong db nhưng không còn tồn tại trên sàn TMDT ta xóa trong db
        // Ta chỉ check chọn xem item, check item đã bị xóa trên db 
        // không thực hiện ở đây
        public void TikiInsertIfDontExist(CommonItem item)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            try
            {
                conn.Open();

                // Kiểm tra itemId đã tồn tại trong bảng tbtikiitem
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_All_From_TMDTTikiItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTTikiItemId", Common.ConvertLongToInt(item.itemId));

                int itemIdInserted = 0;
                MySqlDataReader rdr = null;

                rdr = cmd.ExecuteReader();
                Boolean exist = false;
                while (rdr.Read())
                {
                    itemIdInserted = MyMySql.GetInt32(rdr, "TikiItemId");
                    exist = true;
                    break;
                }
                if (rdr != null)
                    rdr.Close();
                int status = 0;
                // Lưu item vào db lần đầu
                if (!exist)
                {
                    status = item.bActive ? 0 : 1;
                    itemIdInserted = TikiInsert(item.tikiSuperId, Common.ConvertLongToInt(item.itemId),
                        item.name, status, conn);
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
        public void TikiGetItemFromId(int id, CommonItem item)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_All_From_TMDTTikiItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTTikiItemId", id);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (MyMySql.GetInt32(rdr, "TikiMappingId") != -1)
                    {
                        Mapping map = new Mapping();
                        map.quantity = MyMySql.GetInt32(rdr, "TikiMappingQuantity");
                        map.product = ItemModelMySql.ConvertOneRowFromDataMySqlToProduct(rdr);
                        item.models[0].mapping.Add(map);
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

        public MySqlResultState TikiUpdateMapping(List<CommonForMapping> ls)
        {
            CommonForMapping commonForMapping = ls[0];
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();
                // item Tiki không có model, chỉ có 1 model tượng trưng nên ls có 1 phần tử
                int itemIdInserted = 0;
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_All_From_TMDTTikiItem_Id", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTMDTTikiItemId", Common.ConvertLongToInt(commonForMapping.itemId));

                    MySqlDataReader rdr = null;

                    rdr = cmd.ExecuteReader();
 
                    while (rdr.Read())
                    {
                        itemIdInserted = MyMySql.GetInt32(rdr, "TikiItemId");
                        break;
                    }
                    rdr.Close();
                }

                // Xóa mapping cũ
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbTikiMapping_Delete_From_TikiItemId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTikiItemId", itemIdInserted);
                    cmd.ExecuteNonQuery();
                }

                // Insert mapping mới
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbTikiMapping_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inTikiItemId", itemIdInserted);
                    cmd.Parameters.AddWithValue("@inProductId", 0);
                    cmd.Parameters.AddWithValue("@inQuantity", 0);

                    for (int j = 0; j < commonForMapping.lsProductId.Count; j++)
                    {
                        // Nếu model chưa được mapping productId, productQuantity là: System.Int32.MinValue;
                        if (commonForMapping.lsProductId[j] > 0)
                        {
                            cmd.Parameters[1].Value = commonForMapping.lsProductId[j];
                            cmd.Parameters[2].Value = commonForMapping.lsProductQuantity[j];
                            cmd.ExecuteNonQuery();
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

        // Cập nhật trạng thái đơn hàng đã đóng/ đã hoàn
        // Hàm này dùng cho sàn: Tiki, Shopee,...
        public void UpdateOrderStatusInWarehouseToCommonOrder(List<CommonOrder> ls)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            string status = string.Empty;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Get_Lastest_Status_From_Code", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCode", "");
                cmd.Parameters.AddWithValue("@inECommmerce", 0);
                MySqlDataReader rdr;
                foreach (var order in ls)
                {
                    status = string.Empty;
                    cmd.Parameters[0].Value = order.code;
                    if(order.ecommerceName == Common.eTiki)
                        cmd.Parameters[1].Value = 1;
                    else if (order.ecommerceName == Common.eShopee)
                        cmd.Parameters[1].Value = 2;

                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (MyMySql.GetInt32(rdr, "Status") == 0)
                            status = Common.packedOrder;
                        else
                            status = Common.returnedOrder;
                    }
                    order.orderStatusInWarehoue = status;
                    if (rdr != null)
                        rdr.Close();
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
            }

            conn.Close();
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public void TikiUpdateMappingToCommonOrder(List<CommonOrder> ls)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            string status = string.Empty;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbTikiMapping_Get_From_TikiId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTTikiItemId", 0);

                int quantity = 0;
                Product pro = null;
                MySqlDataReader rdr;
                foreach (var order in ls)
                {
                    if (order.ecommerceName != Common.eTiki)
                        continue;

                    for (int i = 0; i < order.listItemId.Count; i++)
                    {
                        cmd.Parameters[0].Value = Common.ConvertLongToInt(order.listItemId[i]);
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

        /// <summary>
        /// Dùng chung cho nhiều sàn
        /// </summary>
        /// <param name="commonOrder"></param>
        /// <param name="status">Trạng thái thực tế đã thực hiện: 0: đã đóng hàng, 1: đã hoàn hàng nhập kho</param>
        public MySqlResultState EnoughProductInOrder(CommonOrder commonOrder, ECommerceOrderStatus status,
            EECommerceType eCommerceType)
        {
            MySqlResultState resultState = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Lưu vào bảng tbECommerceOrder
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
                    cmd.Parameters.AddWithValue("@inShipCode", commonOrder.shipCode);
                    cmd.Parameters.AddWithValue("@inStatus", (int)status);
                    cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);

                    cmd.ExecuteNonQuery();
                }

                // Lưu vào bảng tbOutput
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbOutput_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
                    cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                    cmd.Parameters.AddWithValue("@inProductId", 0);
                    cmd.Parameters.AddWithValue("@inQuantity", 0);
                    int productId = 0;
                    int quantity = 0;
                    for(int i = 0; i < commonOrder.listMapping.Count; i++)
                    {
                        for(int j = 0; j <commonOrder.listMapping[i].Count; j++)
                        {
                            productId = commonOrder.listMapping[i][j].product.id;
                            quantity = commonOrder.listQuantity[i] * commonOrder.listMapping[i][j].quantity;
                            cmd.Parameters[2].Value = productId;
                            cmd.Parameters[3].Value = quantity;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Cập nhật số lượng sản phẩm trong kho
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbProducts_Add_Quantity", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inProductId", 0);
                    cmd.Parameters.AddWithValue("@inAdd", 0);
                    int productId = 0;
                    int quantity = 0;
                    for (int i = 0; i < commonOrder.listMapping.Count; i++)
                    {
                        for (int j = 0; j < commonOrder.listMapping[i].Count; j++)
                        {
                            productId = commonOrder.listMapping[i][j].product.id;
                            quantity = commonOrder.listQuantity[i] * commonOrder.listMapping[i][j].quantity;
                            cmd.Parameters[0].Value = productId;
                            cmd.Parameters[1].Value = quantity * -1;// Giảm số lượng tồn kho

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                Common.SetResultException(ex, resultState);
            }

            conn.Close();
            return resultState;
        }
    }
}