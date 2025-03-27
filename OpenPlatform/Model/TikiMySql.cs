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
                
                MyLogger.GetInstance().Warn(ex.ToString());
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
                
                MyLogger.GetInstance().Warn(ex.ToString());
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

                        Product product = new Product();
                        product.id = MyMySql.GetInt32(rdr, "ProductId");
                        product.name = MyMySql.GetString(rdr, "ProductName");
                        product.SetFirstSrcImage();
                        map.product = product;

                        item.models[0].mapping.Add(map);
                    }
                }

                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
        }

        public void TikiGetListCommonItemFromListTikiProductConnectOut(
            List<TikiProduct> lsTikiItem,
            List<CommonItem> lsCommonItem,
            MySqlConnection conn)
        {
            try
            {
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

                            Product product = new Product();
                            product.id = MyMySql.GetInt32(rdr, "ProductId");
                            product.name = MyMySql.GetString(rdr, "ProductName");
                            product.SetFirstSrcImage();
                            map.product = product;
                            item.models[0].mapping.Add(map);
                        }
                    }

                    rdr.Close();
                    lsCommonItem.Add(item);
                }
            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        public void TikiUpdateStatusOfItemToDbConnectOut(
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
                MySqlCommand cmd = new MySqlCommand("UPDATE webplaywithme.tbtikiitem SET `Status` = @inStatus WHERE `TikiId` = @inTikiId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inStatus", 0);
                cmd.Parameters.AddWithValue("@inTikiId", 0);
                foreach (var commonItem in lsCommonItem)
                {
                    cmd.Parameters[0].Value = commonItem.bActive?0:1;
                    cmd.Parameters[1].Value = (int)commonItem.itemId;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
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
                
                MyLogger.GetInstance().Warn(ex.ToString());
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
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            return result;
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public void TikiGetMappingOfCommonOrderConnectOut(CommonOrder commonOrder, MySqlConnection conn)
        {
            string status = string.Empty;
            try
            {
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
                                pro.SetFirstSrcImage();
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
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Cần cập nhật số bảng output, products, tbNeedUpdateQuantity khi giữ chỗ / hủy giữ chỗ / đóng đơn / hoàn đơn
        // Kết nối được mở đóng bên ngoài hàm
        // storeName: st_tbOutput_Insert hoặc st_tbOutput_Insert_If_DontExist_When_Packing
        public MySqlResultState UpdateOutputAndProductTableFromCommonOrderConnectOut(MySqlConnection conn,
            string storeName,
            CommonOrder commonOrder, ECommerceOrderStatus status,
            EECommerceType eCommerceType)
        {
            Boolean isNeedExecuteReader = true;
            MySqlDataReader rdr = null;
            if (storeName == "st_tbOutput_Insert")
            {
                isNeedExecuteReader = false;
            }

            MySqlResultState resultState = new MySqlResultState();
            try
            {
                // Lưu vào bảng tbOutput, tbProducts, tbNeedUpdateQuantity
                {
                    MySqlCommand cmd = new MySqlCommand(storeName, conn);
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
                            quantity = commonOrder.listQuantity[i] * commonOrder.listMapping[i][j].quantity;
                            if(quantity == 0)
                            {
                                continue;
                            }

                            productId = commonOrder.listMapping[i][j].product.id;
                            cmd.Parameters[2].Value = productId;
                            if (status == ECommerceOrderStatus.RETURNED || status == ECommerceOrderStatus.UNBOOKED)
                            {
                                cmd.Parameters[3].Value = quantity * -1;
                            }
                            else
                            {
                                cmd.Parameters[3].Value = quantity;
                            }
                            if (!isNeedExecuteReader)
                            {
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                rdr = cmd.ExecuteReader();
                                while (rdr.Read())
                                {
                                        quantity = MyMySql.GetInt32(rdr, "Result");
                                }
                                if (rdr != null)
                                    rdr.Close();
                            }
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

        public TbEcommerceOrder GetLastestStatusOfECommerceOrder(string code,
            EECommerceType eCommerceType,
            MySqlConnection conn)
        {
            TbEcommerceOrder lastest = new TbEcommerceOrder();
            lastest.status = (int)ECommerceOrderStatus.DONT_EXIST; // Giá trị mặc định
            try
            {
                // Lấy trạng thái cuối cùng của đơn trong bảng tbECommerceOrder
                MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Get_Lastest_Status_From_Code", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCode", code);
                cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                MySqlDataReader rdr;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lastest.status = MyMySql.GetInt32(rdr, "Status");
                    lastest.updateTime= MyMySql.GetInt64(rdr, "UpdateTime");
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return lastest;
        }

        // Từ trạng thái mới, cũ của đơn hàng kiểm tra xem cần tiếp tục cập nhật vào db
        // true: là tiếp tục ngược lại là false
        public Boolean IsNeedUpdateQuantityOfProductInWarehouseFromOrderStatus(
            ECommerceOrderStatus status, ECommerceOrderStatus? oldStatus)
        {
            if (status == oldStatus)
            {
                return false;
            }

            if ((status == ECommerceOrderStatus.BOOKED && oldStatus == ECommerceOrderStatus.PACKED) ||
                (status == ECommerceOrderStatus.UNBOOKED && oldStatus == ECommerceOrderStatus.RETURNED)||
                // Chuẩn bị đóng thì hủy đơn
                (status == ECommerceOrderStatus.PACKED && oldStatus == ECommerceOrderStatus.UNBOOKED))
            {
                return false;
            }

            return true;
        }

        

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong kho khi giữ chỗ / hủy giữ chỗ / đóng đơn / hoàn đơn
        /// Insert thông tin trạng thái đơn hàng vào bảng tbECommerceOrder
        /// Cập nhật số lượng ở tbProducts
        /// </summary>
        /// <param name="commonOrder"></param>
        /// <param name="status">Trạng thái thực tế đã thực hiện: 0: đã đóng hàng, 1: đã hoàn hàng nhập kho, 2: giữ chỗ, 3: hủy giữ chỗ</param>
        public MySqlResultState UpdateQuantityOfProductInWarehouseFromOrderConnectOut(
            CommonOrder commonOrder,
            ECommerceOrderStatus status,
            long update_time, // Thời gian event được sàn ghi nhận
            ECommerceOrderStatus oldStatus, // Trạng thái mới nhất của đơn trong DB.
            EECommerceType eCommerceType,
            MySqlConnection conn)
        {
            MySqlResultState resultState = new MySqlResultState();
            try
            {
                do
                {
                    // Lưu vào bảng tbECommerceOrder
                    {
                        MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Insert", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
                        cmd.Parameters.AddWithValue("@inShipCode", commonOrder.shipCode);
                        cmd.Parameters.AddWithValue("@inStatus", (int)status);
                        cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                        cmd.Parameters.AddWithValue("@inUpdateTime", update_time);

                        cmd.ExecuteNonQuery();
                    }
                    // Rủi ro: Nếu đơn có sản phẩm chưa được mapping thì sẽ không được cập nhật số lượng chính xác khi giữ chỗ
                    // Xử lý: Khi đóng đơn, (lúc này bắt buộc đã mapping) ta cần kiểm tra lại 1 lượt xem có sản phẩm, 
                    // số lượng chưa tồn tại trong tbOutput
                    // Trường hợp: thay đổi mapping khi đóng đơn so với khi giữ chỗ thì quá khù khoằm, tự sửa thủ công

                    // Trường hợp: Bước 1: giữ chỗ, Bước 2: thay đổi mapping (sửa mapping đã có / tạo mapping chưa có), Bước 3 hủy giữ chỗ (chưa đóng đơn)
                    // Sẽ làm sai khác xuất / nhập kho. Khù khoằm không giải quyết, tự sửa thủ công..

                    // => TẤT CẢ RỦI RO TRÊN ĐƯỢC GIẢI QUYẾT khi mapping ngay khi đăng sản phẩm (giải pháp hiện tại check thủ công),
                    ///và khi thay đổi mapping phải kiểm tra xem có đơn nào đang trong tiến trình xử lý không
                    if (status == ECommerceOrderStatus.BOOKED)
                    {
                        Boolean isNeedInsert = false;
                        for (int i = 0; i < commonOrder.listMapping.Count; i++)
                        {
                            if (commonOrder.listMapping[i].Count == 0)
                            {
                                isNeedInsert = true;
                                break;
                            }
                        }
                        if (isNeedInsert)
                        {
                            // Có item chưa được mapping,
                            MySqlCommand cmd = new MySqlCommand("INSERT INTO tbEcommerceOrder_DontMapping(`Code`, `ECommmerce`) VALUES(@inCode, @inECommmerce);", conn);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
                            cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);

                            cmd.ExecuteNonQuery();
                        }

                    }

                    if (status == ECommerceOrderStatus.PACKED && oldStatus == ECommerceOrderStatus.BOOKED)
                    {
                        // Kiểm tra có item chưa được mapping khi giữ chỗ
                        MySqlCommand cmd = new MySqlCommand("SELECT `Id` FROM tbEcommerceOrder_DontMapping WHERE `Code` = @inCode AND `ECommmerce` = @inECommmerce;", conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
                        cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                        Boolean isExist = false;
                        MySqlDataReader rdr = null;
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            isExist = true;
                            break;
                        }
                        rdr.Close();

                        if (isExist)
                        {
                            resultState = UpdateOutputAndProductTableFromCommonOrderConnectOut(conn, "st_tbOutput_Insert_If_DontExist_When_Packing", commonOrder, status, eCommerceType);
                            resultState.myAnything = 1; // Có thay đổi tồn kho.

                            // Xóa dữ liệu vì item trong đơn đã mapping
                            MySqlCommand cmdDelete = new MySqlCommand("DELETE FROM tbEcommerceOrder_DontMapping WHERE `Code` = @inCode AND `ECommmerce` = @inECommmerce;", conn);
                            cmdDelete.CommandType = CommandType.Text;
                            cmdDelete.Parameters.AddWithValue("@inCode", commonOrder.code);
                            cmdDelete.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                            cmdDelete.ExecuteNonQuery();
                        }
                    }
                    // Kiểm tra xem cần thay đổi tồn kho không?
                    else if ( // Cần xuất kho và chưa chưa xuất kho
                        (status == ECommerceOrderStatus.BOOKED && oldStatus != ECommerceOrderStatus.PACKED) ||
                        
                        (status == ECommerceOrderStatus.PACKED && oldStatus != ECommerceOrderStatus.BOOKED &&
                        oldStatus != ECommerceOrderStatus.UNBOOKED) ||

                        // Cần nhập kho và chưa nhập kho
                        //// Đơn hủy nhưng Đã Đóng (có thể đang trên đường vận chuyển) nên cần Hoàn Hàng để sản phẩm thực tế về kho
                        (status == ECommerceOrderStatus.UNBOOKED && oldStatus != ECommerceOrderStatus.RETURNED 
                        && oldStatus != ECommerceOrderStatus.PACKED) ||

                        // Đơn hủy, nhưng đợi nhận hàng hoàn mới thay đổi tồn kho.
                        (status == ECommerceOrderStatus.RETURNED))
                    {
                        resultState = UpdateOutputAndProductTableFromCommonOrderConnectOut(conn, "st_tbOutput_Insert", commonOrder, status, eCommerceType);
                        resultState.myAnything = 1; // Có thay đổi tồn kho.
                    }
                    else
                    {
                        resultState.myAnything = 0; // Không có thay đổi tồn kho.
                    }
                }
                while (false);
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, resultState);
            }

            return resultState;
        }


        ///// <summary>
        ///// Trừ số lượng sản phẩm trong kho khi đóng đơn
        ///// </summary>
        ///// <param name="commonOrder"></param>
        ///// <param name="status">Trạng thái thực tế đã thực hiện: 0: đã đóng hàng, 1: đã hoàn hàng nhập kho</param>
        //public MySqlResultState EnoughProductInOrder(CommonOrder commonOrder,
        //    ECommerceOrderStatus status,
        //    EECommerceType eCommerceType)
        //{
        //    return UpdateQuantityOfProductInOrder(commonOrder, status, eCommerceType);
        //}

        ///// Cộng số lượng sản phẩm trong kho khi hoàn đơn
        //public MySqlResultState ReturnedOrder (CommonOrder commonOrder,
        //    ECommerceOrderStatus status,
        //    EECommerceType eCommerceType)
        //{
        //    return UpdateQuantityOfProductInOrder(commonOrder, status, eCommerceType);
        //}

        //// Trừ số lượng sản phẩm trong kho khi giữ chỗ
        //public MySqlResultState BookedOrder(CommonOrder commonOrder,
        //    ECommerceOrderStatus status,
        //    EECommerceType eCommerceType)
        //{
        //    return UpdateQuantityOfProductInOrder(commonOrder, status, eCommerceType);
        //}

        //// Cộng số lượng sản phẩm trong kho khi hủy giữ chỗ
        //public MySqlResultState UnBookedOrder(CommonOrder commonOrder,
        //    ECommerceOrderStatus status,
        //    EECommerceType eCommerceType)
        //{
        //    return UpdateQuantityOfProductInOrder(commonOrder, status, eCommerceType);
        //}

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

        public List<CommonItem> GetForSaveImageSource()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_For_Save_Image_Source", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();

                CommonItem commonItem = null;
                while (rdr.Read())
                {
                    commonItem = new CommonItem(Common.eTiki);
                    lsCommonItem.Add(commonItem);

                    commonItem.itemId = MyMySql.GetInt32(rdr, "TikiId");
                    commonItem.dbItemId = MyMySql.GetInt32(rdr, "Id");
                }

                rdr.Close();
            }
            catch (Exception ex)
            {

                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return lsCommonItem;
        }

        public void UpdateSourceTotbTikiItem(List<CommonItem> lsCommonItem)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Cập nhật source của item
                MySqlCommand cmdItem = new MySqlCommand("UPDATE tbTikiItem SET Image = @inUrl WHERE Id = @inId", conn);
                cmdItem.CommandType = CommandType.Text;
                cmdItem.Parameters.AddWithValue("@inUrl", "");
                cmdItem.Parameters.AddWithValue("@inId", 1);

                foreach (var item in lsCommonItem)
                {
                    cmdItem.Parameters[0].Value = item.imageSrc;
                    cmdItem.Parameters[1].Value = item.dbItemId;
                    cmdItem.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
        }

        // Lấy ack_id của pull event cuối cùng
        public string GetAckIdOfLastestPullConnectOut(MySqlConnection conn)
        {
            string ack_id = string.Empty;
            MySqlCommand cmd = new MySqlCommand("SELECT `Ack_Id` FROM webplaywithme.tbtikiqueue WHERE `Id` = 1; ", conn);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader rdr;
            try
            {
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ack_id = MyMySql.GetString(rdr, "Ack_Id");
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return ack_id;
        }

        // Cập nhật ack_id mới nhất của pull event
        public void UpdateAckIdOfLastestPullConnectOut(string ack_id, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("UPDATE webplaywithme.tbtikiqueue SET `Ack_Id` = @inAck_Id  WHERE `Id` = 1 ", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@inAck_Id", ack_id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }
    }
}