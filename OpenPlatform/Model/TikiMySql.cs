using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Config;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product;
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

        public void TikiGetListCommonItemFromListTikiProduct(
            List<TikiProduct> lsTikiItem,
            List<CommonItem> lsCommonItem)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_All_From_TMDTTikiItem_Id", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTTikiItemId", 0);

                foreach (var pro in lsTikiItem)
                {
                    cmd.Parameters[0].Value = pro.product_id;
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    CommonItem item = new CommonItem(pro);

                    while (rdr.Read())
                    {
                        if (MyMySql.GetInt32(rdr, "TikiMappingId") != -1)
                        {
                            Mapping map = new Mapping();
                            map.quantity = MyMySql.GetInt32(rdr, "TikiMappingQuantity");
                            //map.product = ItemModelMySql.ConvertOneRowFromDataMySqlToProduct(rdr);
                            Product product = new Product();
                            product.id = MyMySql.GetInt32(rdr, "ProductId");
                            product.name = MyMySql.GetString(rdr, "ProductName");
                            product.SetSrcImageVideo();
                            map.product = product;
                            item.models[0].mapping.Add(map);
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

        public List<CommonItem> TikiGetItemOnDB()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbtikiitem", conn);
                cmd.CommandType = CommandType.Text;

                MySqlDataReader rdr = cmd.ExecuteReader();

                CommonItem commonItem = null;
                while (rdr.Read())
                {
                    commonItem = new CommonItem(Common.eTiki);
                    lsCommonItem.Add(commonItem);

                    commonItem.itemId = MyMySql.GetInt32(rdr, "TikiId");
                    commonItem.name = MyMySql.GetString(rdr, "Name");

                    CommonModel commonModel = new CommonModel();
                    commonModel.modelId = -1;
                    commonItem.models.Add(commonModel);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                lsCommonItem = null;
            }

            conn.Close();
            return lsCommonItem;
        }

        public MySqlResultState TikiDeleteItemOnDB(int itemId)
        {
            MySqlResultState resultState = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Delete_From_TMDTItemId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTMDTId", itemId);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            conn.Close();
            return resultState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ls">luôn có 1 phần tử</param>
        /// <returns></returns>
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
                        if (commonForMapping.lsProductId.Count > 0)
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
        // Hàm này dùng cho sàn: web PWM, Tiki, Shopee,...
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
                        cmd.Parameters[1].Value = (int)EECommerceType.TIKI;
                    else if (order.ecommerceName == Common.eShopee)
                        cmd.Parameters[1].Value = (int)EECommerceType.SHOPEE;
                    else if (order.ecommerceName == Common.ePlayWithMe)
                        cmd.Parameters[1].Value = (int)EECommerceType.PLAY_WITH_ME;

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
        public void TikiUpdateMappingToCommonOrder(CommonOrder commonOrder)
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
                {

                    for (int i = 0; i < commonOrder.listItemId.Count; i++)
                    {
                        cmd.Parameters[0].Value = Common.ConvertLongToInt(commonOrder.listItemId[i]);
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

        // Cập nhật số bảng output và products khi đóng đơn / hoàn đơn
        // Kết nối được mở đóng bên ngoài hàm
        public MySqlResultState UpdateOutputAndProductTable(MySqlConnection conn,
            CommonOrder commonOrder, ECommerceOrderStatus status,
            EECommerceType eCommerceType)
        {
            MySqlResultState resultState = new MySqlResultState();
            try
            {
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
                    for (int i = 0; i < commonOrder.listMapping.Count; i++)
                    {
                        for (int j = 0; j < commonOrder.listMapping[i].Count; j++)
                        {
                            productId = commonOrder.listMapping[i][j].product.id;
                            quantity = commonOrder.listQuantity[i] * commonOrder.listMapping[i][j].quantity;
                            cmd.Parameters[2].Value = productId;
                            if(status == ECommerceOrderStatus.RETURNED)
                            cmd.Parameters[3].Value = quantity * -1;
                                else
                            cmd.Parameters[3].Value = quantity;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                //// Cập nhật số lượng sản phẩm trong kho
                //{
                //    MySqlCommand cmd = new MySqlCommand("st_tbProducts_Add_Quantity", conn);
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.Parameters.AddWithValue("@inProductId", 0);
                //    cmd.Parameters.AddWithValue("@inAdd", 0);
                //    int productId = 0;
                //    int quantity = 0;
                //    for (int i = 0; i < commonOrder.listMapping.Count; i++)
                //    {
                //        for (int j = 0; j < commonOrder.listMapping[i].Count; j++)
                //        {
                //            productId = commonOrder.listMapping[i][j].product.id;
                //            quantity = commonOrder.listQuantity[i] * commonOrder.listMapping[i][j].quantity;
                //            cmd.Parameters[0].Value = productId;
                //            if (isReturnedOrder)
                //                cmd.Parameters[1].Value = quantity;// Tăng số lượng tồn kho
                //            else
                //                cmd.Parameters[1].Value = quantity * -1;// Giảm số lượng tồn kho
                //            cmd.ExecuteNonQuery();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            return resultState;
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong kho khi đóng đơn/ hoàn đơn
        /// </summary>
        /// <param name="commonOrder"></param>
        /// <param name="status">Trạng thái thực tế đã thực hiện: 0: đã đóng hàng, 1: đã hoàn hàng nhập kho</param>
        public MySqlResultState UpdateQuantityOfProductInOrder(CommonOrder commonOrder,
            ECommerceOrderStatus status,
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

                resultState = UpdateOutputAndProductTable(conn, commonOrder, status, eCommerceType);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            conn.Close();
            return resultState;
        }


        /// <summary>
        /// Trừ số lượng sản phẩm trong kho khi đóng đơn
        /// </summary>
        /// <param name="commonOrder"></param>
        /// <param name="status">Trạng thái thực tế đã thực hiện: 0: đã đóng hàng, 1: đã hoàn hàng nhập kho</param>
        public MySqlResultState EnoughProductInOrder(CommonOrder commonOrder,
            ECommerceOrderStatus status,
            EECommerceType eCommerceType)
        {
            return UpdateQuantityOfProductInOrder(commonOrder, status, eCommerceType);
        }

        /// Cộng số lượng sản phẩm trong kho khi hoàn đơn
        public MySqlResultState ReturnedOrder (CommonOrder commonOrder,
            ECommerceOrderStatus status,
            EECommerceType eCommerceType)
        {
            return UpdateQuantityOfProductInOrder(commonOrder, status, eCommerceType);
        }

        public TikiConfigApp GetTikiConfigApp()
        {
            TikiConfigApp config = new TikiConfigApp();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Lưu vào bảng tbECommerceOrder
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tbTikiAuthen", conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    config.appID = MyMySql.GetString(rdr, "AppId");
                    config.homeAddress = MyMySql.GetString(rdr, "Home");
                    config.secretAppCode = MyMySql.GetString(rdr, "Secret");
                    config.tikiAu.access_token = MyMySql.GetString(rdr, "AccessToken");
                    config.tikiAu.expires_in = MyMySql.GetString(rdr, "ExpiresIn");
                    config.tikiAu.token_type = MyMySql.GetString(rdr, "TokenType");
                    config.tikiAu.scope = MyMySql.GetString(rdr, "Scope");
                }
                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                config = null;
            }

            conn.Close();

            return config;
        }

        public MySqlResultState TikiSaveAccessToken(TikiAuthorization accessToken)
        {
            MySqlResultState resultState = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE `tbTikiAuthen` SET `AccessToken`=@AccessToken" +
                    ",`ExpiresIn`=@ExpiresIn" +
                    ",`TokenType`=@TokenType" +
                    ",`Scope`=@Scope" +
                    " WHERE `Id` = 1;", conn);
                cmd.Parameters.AddWithValue("@AccessToken", accessToken.access_token);
                cmd.Parameters.AddWithValue("@ExpiresIn", accessToken.expires_in);
                cmd.Parameters.AddWithValue("@TokenType", accessToken.token_type);
                cmd.Parameters.AddWithValue("@Scope", accessToken.scope);
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
    }
}