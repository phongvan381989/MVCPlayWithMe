using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.ProductModel;
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
    public class TikiMySql
    {
        //private int TikiInsert(int supperId, int tikiId, string name,
        //    int status, string sku, string superSku, MySqlConnection conn)
        private int TikiInsert(CommonItem item, MySqlConnection conn)
        {
            int id = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inSuperId", item.tikiSuperId);
                cmd.Parameters.AddWithValue("@inTikiId", Common.ConvertLongToInt(item.itemId));
                cmd.Parameters.AddWithValue("@inName", item.name);
                cmd.Parameters.AddWithValue("@inStatus", item.bActive ? 0 : 1);
                cmd.Parameters.AddWithValue("@inSku", item.sku);
                cmd.Parameters.AddWithValue("@inSuperSku", item.superSku);
                cmd.Parameters.AddWithValue("@inImage", item.imageSrc);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        id = MyMySql.GetInt32(rdr, "LastId");
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return id;
        }

        // Kiểm tra item đã tồn tại trong bảng tbtikiitem
        public Boolean CheckItemExistIntbTikiItem(int itemId,
            MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("SELECT `Id` FROM webplaywithme.tbtikiitem WHERE `TikiId` = @inTikiId LIMIT 1;", conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@inTikiId", itemId);

            MySqlDataReader rdr = null;
            Boolean isExist = false;

            using (rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        // Check item đã được lưu vào bảng tương ứng tbtikiitem, tbtikimapping
        // Nếu chưa lưu ta thực hiện lưu
        // Nếu đã lưu item trong db nhưng không còn tồn tại trên sàn TMDT ta xóa trong db
        // Ta chỉ check chọn xem item, check item đã bị xóa trên db 
        // không thực hiện ở đây
        // return true: nếu tồn tại ngược lại false
        public Boolean TikiInsertIfDontExistConnectOut(CommonItem item, MySqlConnection conn)
        {
            try
            {
                // Nếu Id và Supper Id bằng nhau, đây là sản phẩm cha chung ảo
                if (item.TikiCheckVirtalParent())
                {
                    return true;
                }

                // Kiểm tra itemId đã tồn tại trong bảng tbtikiitem
                Boolean exist = CheckItemExistIntbTikiItem((int)item.itemId, conn);

                int status = 0;
                // Lưu item vào db lần đầu
                if (!exist)
                {
                    status = item.bActive ? 0 : 1;
                    //itemIdInserted = TikiInsert(item, conn);
                    TikiInsert(item, conn);
                    return false;
                }
                else
                {
                    // Cập nhật trạng thái item vào DB
                    TikiUpdateStatusOfItemToDbConnectOut((int)item.itemId, item.bActive ? 0 : 1, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return true;
        }

        // Lấy được thông tin mapping chi tiết
        public void TikiGetItemFromIdConnectOut(int id, CommonItem item, MySqlConnection conn)
        {
            try
            {
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
                        product.bookCoverPrice = MyMySql.GetInt32(rdr, "ProductBookCoverPrice");
                        product.publisherId = MyMySql.GetInt32(rdr, "PublisherId");
                        product.discount = rdr.IsDBNull(rdr.GetOrdinal("ProductDiscount")) ? 0 : rdr.GetFloat("ProductDiscount");
                        product.quantity = rdr.GetInt32("ProductQuantity");
                        product.SetFirstSrcImage();
                        map.product = product;
                        if(rdr.IsDBNull(rdr.GetOrdinal("ProductDiscount")))
                        {
                            product.SetFirstSrcImage();
                        }

                        item.models[0].mapping.Add(map);
                    }
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
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

                int TikiMappingIdIndex = 0, 
                    tikiMappingQuantityIndex = 0,
                    productIdIndex = 0,
                    productNameIndex = 0,
                    productBookCoverPriceIndex = 0;
                Boolean isSetIndex = false;
                foreach (var pro in lsTikiItem)
                {
                    cmd.Parameters[0].Value = pro.product_id;
                    CommonItem item = new CommonItem(pro);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            if (!isSetIndex)
                            {
                                TikiMappingIdIndex = rdr.GetOrdinal("TikiMappingId");

                                tikiMappingQuantityIndex = rdr.GetOrdinal("TikiMappingQuantity");
                                productIdIndex = rdr.GetOrdinal("ProductId");
                                productNameIndex = rdr.GetOrdinal("ProductName");
                                productBookCoverPriceIndex = rdr.GetOrdinal("ProductBookCoverPrice");
                                isSetIndex = true;
                            }
                            if (rdr.IsDBNull(TikiMappingIdIndex))
                            {
                                continue;
                            }

                            Mapping map = new Mapping();

                            map.quantity = rdr.GetInt32(tikiMappingQuantityIndex);

                            Product product = new Product();
                            product.id = rdr.GetInt32(productIdIndex);
                            product.name = rdr.GetString(productNameIndex);
                            product.bookCoverPrice = rdr.GetInt32(productBookCoverPriceIndex);
                            product.SetFirstSrcImage();
                            map.product = product;
                            item.models[0].mapping.Add(map);
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

        public void TikiUpdateStatusOfItemListToDbConnectOut(
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

        public void TikiUpdateStatusOfItemToDbConnectOut(
            int itemId,
            int status,
            MySqlConnection conn)
        {
            try
            {
                // Cập nhật source của item
                MySqlCommand cmd = new MySqlCommand("UPDATE webplaywithme.tbtikiitem SET `Status` = @inStatus WHERE `TikiId` = @inTikiId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@inStatus", status);
                cmd.Parameters.AddWithValue("@inTikiId", itemId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Hàm này được gọi 1 lần duy nhất để lấy sku, super sku vì mới thêm 2 trường này trong bảng tbTikiItem
        // Sau khi gọi ta comment lại.
        public void TikiUpdateSku_SuperSkuOfItemToDbConnectOut(
        List<CommonItem> lsCommonItem,
        MySqlConnection conn)
        {
            //if (lsCommonItem == null || lsCommonItem.Count == 0)
            //{
            //    return;
            //}

            //try
            //{
            //    // Cập nhật source của item
            //    MySqlCommand cmd = new MySqlCommand("UPDATE webplaywithme.tbtikiitem SET `Sku` = @inSku, `SuperSku` = @inSuperSku WHERE `TikiId` = @inTikiId", conn);
            //    cmd.CommandType = CommandType.Text;
            //    cmd.Parameters.AddWithValue("@inSku", "");
            //    cmd.Parameters.AddWithValue("@inSuperSku", "");
            //    cmd.Parameters.AddWithValue("@inTikiId", 0);
            //    foreach (var commonItem in lsCommonItem)
            //    {
            //        cmd.Parameters[0].Value = commonItem.sku;
            //        cmd.Parameters[1].Value = commonItem.superSku;
            //        cmd.Parameters[2].Value = (int)commonItem.itemId;
            //        cmd.ExecuteNonQuery();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MyLogger.GetInstance().Warn(ex.ToString());
            //}
        }

        public List<CommonItem> TikiGetItemOnDB()
        {
            List<CommonItem> lsCommonItem = new List<CommonItem>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_All", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                MySqlDataReader rdr = cmd.ExecuteReader();

                CommonItem commonItem = null;
                int tikiIdIndex = rdr.GetOrdinal("TikiId");
                int nameIndex = rdr.GetOrdinal("Name");
                int statusIndex = rdr.GetOrdinal("Status");
                int skuIndex = rdr.GetOrdinal("Sku");
                int productStatusIndex = rdr.GetOrdinal("ProductStatus");

                int itemId = Int32.MaxValue;
                int itemIdTemp = Int32.MaxValue;
                while (rdr.Read())
                {
                    itemIdTemp = rdr.GetInt32(tikiIdIndex);
                    if (itemId != itemIdTemp)
                    {
                        itemId = itemIdTemp;
                        commonItem = new CommonItem(Common.eTiki);
                        lsCommonItem.Add(commonItem);

                        commonItem.itemId = itemId;
                        commonItem.name = rdr.GetString(nameIndex);
                        commonItem.sku = rdr.GetString(skuIndex);
                        commonItem.bActive = rdr.GetInt32(statusIndex) == 0 ? true : false;
                        if (commonItem.bActive)
                        {
                            commonItem.item_status = Common.tikiActive;
                        }
                        else
                        {
                            commonItem.item_status = Common.tikiUnactive;
                        }

                        CommonModel commonModel = new CommonModel();
                        commonModel.modelId = -1;
                        commonItem.models.Add(commonModel);
                    }

                    if (!rdr.IsDBNull(productStatusIndex))
                    {
                        Product product = new Product();
                        product.status = rdr.GetInt32(productStatusIndex);
                        commonItem.models[0].mapping.Add(new Mapping(product, 1));
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

        public Dictionary<int, string> TikiGetListItemDontMapping(MySqlConnection conn)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_Dont_Mapping", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    int idIndex = rdr.GetOrdinal("Id");
                    int nameIndex = rdr.GetOrdinal("Name");
                    while (rdr.Read())
                    {
                        dic.Add(rdr.GetInt32(idIndex), rdr.GetString(nameIndex));
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return dic;
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

        public MySqlResultState TikiUpdateMappingSignle(int itemId,
            int productId,
            int quantity,
            MySqlConnection conn
            )
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiMapping_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTikiItemId", itemId);
                cmd.Parameters.AddWithValue("@inProductId", productId);
                cmd.Parameters.AddWithValue("@inQuantity", quantity);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
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


                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            itemIdInserted = MyMySql.GetInt32(rdr, "TikiItemId");
                            break;
                        }
                    }
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
                Common.SetResultException(ex, result);
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
            lastest.updateTime = 0;
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

            //if ((status == ECommerceOrderStatus.BOOKED && oldStatus == ECommerceOrderStatus.PACKED) ||
            //    (status == ECommerceOrderStatus.UNBOOKED && oldStatus == ECommerceOrderStatus.RETURNED)||
                // Chuẩn bị đóng thì hủy đơn
            if(status == ECommerceOrderStatus.PACKED && oldStatus == ECommerceOrderStatus.UNBOOKED)
            {
                return false;
            }

            // Đơn đã đóng và bị hủy
            if (status == ECommerceOrderStatus.UNBOOKED && oldStatus == ECommerceOrderStatus.PACKED)
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
                    // =>    Phức tập nên bỏ check đã mapping item chưa????

                    //// Rủi ro: Nếu đơn có sản phẩm chưa được mapping thì sẽ không được cập nhật số lượng chính xác khi giữ chỗ
                    //// Xử lý: Khi đóng đơn, (lúc này bắt buộc đã mapping) ta cần kiểm tra lại 1 lượt xem có sản phẩm, 
                    //// số lượng chưa tồn tại trong tbOutput
                    //// Trường hợp: thay đổi mapping khi đóng đơn so với khi giữ chỗ thì quá khù khoằm, tự sửa thủ công

                    //// Trường hợp: Bước 1: giữ chỗ, Bước 2: thay đổi mapping (sửa mapping đã có / tạo mapping chưa có), Bước 3 hủy giữ chỗ (chưa đóng đơn)
                    //// Sẽ làm sai khác xuất / nhập kho. Khù khoằm không giải quyết, tự sửa thủ công.

                    //// => TẤT CẢ RỦI RO TRÊN ĐƯỢC GIẢI QUYẾT khi mapping ngay khi đăng sản phẩm (giải pháp hiện tại check thủ công),
                    //// và khi thay đổi mapping phải kiểm tra xem có đơn nào đang trong tiến trình xử lý không
                    //if (status == ECommerceOrderStatus.BOOKED)
                    //{
                    //    Boolean isNeedInsert = false;
                    //    for (int i = 0; i < commonOrder.listMapping.Count; i++)
                    //    {
                    //        if (commonOrder.listMapping[i].Count == 0)
                    //        {
                    //            isNeedInsert = true;
                    //            break;
                    //        }
                    //    }
                    //    if (isNeedInsert)
                    //    {
                    //        // Có item chưa được mapping,
                    //        MySqlCommand cmd = new MySqlCommand("INSERT INTO tbEcommerceOrder_DontMapping(`Code`, `ECommmerce`) VALUES(@inCode, @inECommmerce);", conn);
                    //        cmd.CommandType = CommandType.Text;
                    //        cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
                    //        cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);

                    //        cmd.ExecuteNonQuery();
                    //    }

                    //}

                    //if (status == ECommerceOrderStatus.PACKED && oldStatus == ECommerceOrderStatus.BOOKED)
                    //{
                    //    // Kiểm tra có item chưa được mapping khi giữ chỗ
                    //    MySqlCommand cmd = new MySqlCommand("SELECT `Id` FROM tbEcommerceOrder_DontMapping WHERE `Code` = @inCode AND `ECommmerce` = @inECommmerce;", conn);
                    //    cmd.CommandType = CommandType.Text;
                    //    cmd.Parameters.AddWithValue("@inCode", commonOrder.code);
                    //    cmd.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                    //    Boolean isExist = false;
                    //    MySqlDataReader rdr = null;
                    //    rdr = cmd.ExecuteReader();
                    //    while (rdr.Read())
                    //    {
                    //        isExist = true;
                    //        break;
                    //    }
                    //    rdr.Close();

                    //    if (isExist)
                    //    {
                    //        resultState = UpdateOutputAndProductTableFromCommonOrderConnectOut(conn, "st_tbOutput_Insert_If_DontExist_When_Packing", commonOrder, status, eCommerceType);
                    //        resultState.myAnything = 1; // Có thay đổi tồn kho.

                    //        // Xóa dữ liệu vì item trong đơn đã mapping
                    //        MySqlCommand cmdDelete = new MySqlCommand("DELETE FROM tbEcommerceOrder_DontMapping WHERE `Code` = @inCode AND `ECommmerce` = @inECommmerce;", conn);
                    //        cmdDelete.CommandType = CommandType.Text;
                    //        cmdDelete.Parameters.AddWithValue("@inCode", commonOrder.code);
                    //        cmdDelete.Parameters.AddWithValue("@inECommmerce", (int)eCommerceType);
                    //        cmdDelete.ExecuteNonQuery();
                    //    }
                    //}

                    // Kiểm tra xem cần thay đổi tồn kho không?
                     if ( // Cần xuất kho và chưa chưa xuất kho
                        (status == ECommerceOrderStatus.BOOKED && oldStatus == ECommerceOrderStatus.DONT_EXIST) ||

                        (status == ECommerceOrderStatus.PACKED && oldStatus == ECommerceOrderStatus.DONT_EXIST) ||

                        // Cần nhập kho và chưa nhập kho
                        // Đơn hủy và chưa Đã Đóng
                        (status == ECommerceOrderStatus.UNBOOKED && oldStatus == ECommerceOrderStatus.BOOKED) ||

                        // Đơn hủy, nhưng đang trên đường vận chuyển đợi nhận hàng hoàn mới thay đổi tồn kho.
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

        public List<int> GetForSaveImageSourceConnectOut(MySqlConnection conn)
        {
            List<int> lsItemId = new List<int>();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiItem_Get_For_Save_Image_Source", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    int tikiIdIndex = rdr.GetOrdinal("TikiId");
                    while (rdr.Read())
                    {
                        lsItemId.Add(rdr.GetInt32(tikiIdIndex));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            return lsItemId;
        }

        public void UpdateImageSourceTotbTikiItemConnectOut(List<CommonItem> lsCommonItem, MySqlConnection conn)
        {
            try
            {
                // Cập nhật source của item
                MySqlCommand cmdItem = new MySqlCommand("UPDATE tbTikiItem SET Image = @inUrl WHERE TikiId = @inTikiId", conn);
                cmdItem.CommandType = CommandType.Text;
                cmdItem.Parameters.AddWithValue("@inUrl", "");
                cmdItem.Parameters.AddWithValue("@inTikiId", 0);

                foreach (var item in lsCommonItem)
                {
                    cmdItem.Parameters[0].Value = item.imageSrc;
                    cmdItem.Parameters[1].Value = (int)item.itemId;
                    cmdItem.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
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

        // Lưu thông tin vào đơn hàng, item id, model id của đơn hàng
        public void InsertTbItemOfEcommerceOder(CommonOrder commonOrder,
            EECommerceType type,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbItemOfEcommerceOder_Insert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@p_Code", commonOrder.code);
                    cmd.Parameters.AddWithValue("@p_ECommerce", (int)type);
                    cmd.Parameters.AddWithValue("@p_ItemId", 0L);
                    cmd.Parameters.AddWithValue("@p_ModelId", 0L);
                    cmd.Parameters.AddWithValue("@p_Quantity", 0);

                    for (int i = 0; i < commonOrder.listItemId.Count; i++)
                    {
                        cmd.Parameters[2].Value = commonOrder.listItemId[i];
                        if (commonOrder.listModelId[i] <= 0)// Không có model, Shopee trả modelId = 0, tiki mặc định -1 giống db lưu modelId = -1
                            cmd.Parameters[3].Value = 0;
                        else
                            cmd.Parameters[3].Value = commonOrder.listModelId[i];

                        cmd.Parameters[4].Value = commonOrder.listQuantity[i];
                        // Thực thi Stored Procedure
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
            }
        }

        // Khi nhận được thông báo có đơn hỏa tốc, thêm dữ liệu vào bảng này. Khi xác nhận đã biết có đơn hỏa tốc
        // (từ mini app khởi động cùng window, khi được đóng,...) sẽ xóa khỏi bảng này
        public void InsertTbExpressOrder(string code,
            EECommerceType type,
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("Call InsertTbExpressOrder, code: " + code);
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbExpressOrder_Insert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@p_Code", code);
                    cmd.Parameters.AddWithValue("@p_ECommerce", (int)type);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
            }
        }

        public void UpdateStatusToReadyToShipTbExpressOrder(string code,
            EECommerceType type,
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("Call UpdateStatusToReadyToShipTbExpressOrder");
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbExpressOrder_Update_Status_To_Ready_To_Ship", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@p_Code", code);
                    cmd.Parameters.AddWithValue("@p_ECommerce", (int)type);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
            }
        }

        public void UpdateStatusToKnownTbExpressOrder(string code,
            EECommerceType type,
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("Call UpdateStatusToKnownTbExpressOrder");
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbExpressOrder_Update_Status_To_Known", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@p_Code", code);
                    cmd.Parameters.AddWithValue("@p_ECommerce", (int)type);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
            }
        }

        public void UpdateCancelledStatusTbItemOfEcommerceOder(CommonOrder commonOrder,
             EECommerceType type,
            MySqlConnection conn)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbItemOfEcommerceOder_Update_Cancelled_Status", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@p_Code", commonOrder.code);
                    cmd.Parameters.AddWithValue("@p_ECommerce", (int)type);
                    cmd.Parameters.AddWithValue("@p_ItemId", 0L);
                    cmd.Parameters.AddWithValue("@p_ModelId", 0L);

                    for (int i = 0; i < commonOrder.listItemId.Count; i++)
                    {
                        cmd.Parameters[2].Value = commonOrder.listItemId[i];
                        if (commonOrder.listModelId[i] <= 0)// Không có model, Shopee trả modelId = 0, tiki mặc định -1 giống db lưu modelId = -1
                            cmd.Parameters[3].Value = 0;
                        else
                            cmd.Parameters[3].Value = commonOrder.listModelId[i];

                        // Thực thi Stored Procedure
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
            }
        }

        // Lấy danh sách xuất hàng theo đơn hàng của một sản phẩm
        // Dữ liệu được sắp xếp từ mới đến cũ theo thời gian
        public List<TbEcommerceOrder> GetOrderStatistics(
            int eCommmerce, // -1 nếu lấy tất cả các sàn, web
            int intervalDay,
            MySqlConnection conn)
        {
            var list = new List<TbEcommerceOrder>();

            using (var command = new MySqlCommand("st_tbECommerceOrder_Order_Statistics", conn))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("inECommmerce", eCommmerce);
                command.Parameters.AddWithValue("inIntervalDay", intervalDay);

                using (var rdr = command.ExecuteReader())
                {
                    // Lấy chỉ số cột một lần trước khi vào vòng lặp
                    //int idIndex = rdr.GetOrdinal("Id");
                    int codeIndex = rdr.GetOrdinal("Code");
                    int shipcodeIndex = rdr.GetOrdinal("ShipCode");
                    int eCommmerceIndex = rdr.GetOrdinal("ECommmerce");
                    //int productIdIndex = rdr.GetOrdinal("ProductId");
                    //int quantityIndex = rdr.GetOrdinal("Quantity");
                    int timeIndex = rdr.GetOrdinal("Time");
                    int eCommerceOrderIndex = rdr.GetOrdinal("ECommerceOrder");

                    while (rdr.Read())
                    {
                        var output = new TbEcommerceOrder
                        {
                            //id = rdr.GetInt32(idIndex),
                            code = rdr.GetString(codeIndex),
                            shipCode = rdr.IsDBNull(shipcodeIndex) ? string.Empty: rdr.GetString(shipcodeIndex),
                            eCommerce = rdr.GetInt32(eCommmerceIndex),
                            //productId = rdr.GetInt32(productIdIndex),
                            //quantity = rdr.GetInt32(quantityIndex),
                            time = rdr.GetDateTime(timeIndex),
                            status = rdr.IsDBNull(eCommerceOrderIndex) ? 0 : 1 // khác 0 ý nghĩa chung là đơn hủy
                        };

                        list.Add(output);
                    }
                }
            }

            return list;
        }

        // Khi nhận được thông báo có đơn hỏa tốc, thêm dữ liệu vào bảng này. Khi xác nhận đã biết có đơn hỏa tốc
        // (từ mini app khởi động cùng window, khi được đóng,...) sẽ xóa khỏi bảng này
        public void InsertTbTikiCategory(
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiCategory> ls,
            MySqlConnection conn)
        {
            MyLogger.GetInstance().Info("Call InsertTbTikiCategory");
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbTikiCategory_Insert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@p_Name", "");
                    cmd.Parameters.AddWithValue("@p_TikiCategoryId", 0);
                    foreach (var category in ls)
                    {
                        cmd.Parameters[0].Value = category.name;
                        cmd.Parameters[1].Value = category.id;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
            }
        }

        public List<int> GetCatetoryIdList(MySqlConnection conn)
        {
            List<int> CatetoryIdList = new List<int>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT TikiCategoryId FROM webplaywithme.tb_tiki_category;", conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (var reader = cmd.ExecuteReader())
                    {
                        // Sử dụng GetOrdinal để lấy index của cột trước khi đọc
                        int TikiCategoryIdOrdinal = reader.GetOrdinal("TikiCategoryId");

                        while (reader.Read())
                        {
                            CatetoryIdList.Add(reader.GetInt32(TikiCategoryIdOrdinal));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Info(ex.ToString());
                CatetoryIdList.Clear();
            }
            return CatetoryIdList;
        }

        public List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> 
            GetTikiAttributesOfCategory(int categoryId, MySqlConnection conn)
        {
            var attributes = new List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute>();
            var query = "SELECT * FROM tb_tiki_attribute_of_category WHERE CategoryId = @in_CategoryId";
            using (var command = new MySqlCommand(query, conn))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@in_CategoryId", categoryId);
                using (var reader = command.ExecuteReader())
                {
                    // Sử dụng GetOrdinal để lấy index của cột trước khi đọc
                    int codeOrdinal = reader.GetOrdinal("Code");
                    int dataTypeOrdinal = reader.GetOrdinal("DataType");
                    int defaultValueOrdinal = reader.GetOrdinal("DefaultValue");
                    int descriptionOrdinal = reader.GetOrdinal("Description");
                    int descriptionEnOrdinal = reader.GetOrdinal("DescriptionEn");
                    int displayNameOrdinal = reader.GetOrdinal("DisplayName");
                    int displayNameEnOrdinal = reader.GetOrdinal("DisplayNameEn");
                    int attributeIdOrdinal = reader.GetOrdinal("AttributeId");
                    int inputTypeOrdinal = reader.GetOrdinal("InputType");
                    int isRequiredOrdinal = reader.GetOrdinal("IsRequired");
                    int categoryIdOrdinal = reader.GetOrdinal("CategoryId");

                    while (reader.Read())
                    {
                        var attribute = new MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute
                        {
                            code = reader.GetString(codeOrdinal),
                            data_type = reader.IsDBNull(dataTypeOrdinal) ? null : reader.GetString(dataTypeOrdinal),
                            default_value = reader.IsDBNull(defaultValueOrdinal) ? null : reader.GetString(defaultValueOrdinal),
                            description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                            description_en = reader.IsDBNull(descriptionEnOrdinal) ? null : reader.GetString(descriptionEnOrdinal),
                            display_name = reader.IsDBNull(displayNameOrdinal) ? null : reader.GetString(displayNameOrdinal),
                            display_name_en = reader.IsDBNull(displayNameEnOrdinal) ? null : reader.GetString(displayNameEnOrdinal),
                            id = reader.GetInt32(attributeIdOrdinal),
                            input_type = reader.IsDBNull(inputTypeOrdinal) ? null : reader.GetString(inputTypeOrdinal),
                            is_required = reader.GetBoolean(isRequiredOrdinal),
                            category_id = reader.GetInt32(categoryIdOrdinal)
                        };
                        attributes.Add(attribute);
                    }
                }
            }

            return attributes;
        }

        public void InsertTikiAttributesOfCategory(
            List<MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category.TikiAttribute> attributes,
            MySqlConnection conn
            )
        {
            using (var command = new MySqlCommand("st_tbTikiAttributeOfCategory_Insert", conn))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Tạo các tham số một lần
                command.Parameters.Add("@p_Code", MySqlDbType.VarChar, 255);
                command.Parameters.Add("@p_DataType", MySqlDbType.VarChar, 100);
                command.Parameters.Add("@p_DefaultValue", MySqlDbType.VarChar, 255);
                command.Parameters.Add("@p_Description", MySqlDbType.Text);
                command.Parameters.Add("@p_DescriptionEn", MySqlDbType.Text);
                command.Parameters.Add("@p_DisplayName", MySqlDbType.VarChar, 255);
                command.Parameters.Add("@p_DisplayNameEn", MySqlDbType.VarChar, 255);
                command.Parameters.Add("@p_AttributeId", MySqlDbType.Int32);
                command.Parameters.Add("@p_InputType", MySqlDbType.VarChar, 100);
                command.Parameters.Add("@p_IsRequired", MySqlDbType.Bit);
                command.Parameters.Add("@p_CategoryId", MySqlDbType.Int32);

                // Lặp qua danh sách và cập nhật giá trị tham số
                foreach (var attribute in attributes)
                {
                    command.Parameters["@p_Code"].Value = (object)attribute.code ?? DBNull.Value;
                    command.Parameters["@p_DataType"].Value = (object)attribute.data_type ?? DBNull.Value;
                    command.Parameters["@p_DefaultValue"].Value = (object)attribute.default_value ?? DBNull.Value;
                    command.Parameters["@p_Description"].Value = (object)attribute.description ?? DBNull.Value;
                    command.Parameters["@p_DescriptionEn"].Value = (object)attribute.description_en ?? DBNull.Value;
                    command.Parameters["@p_DisplayName"].Value = (object)attribute.display_name ?? DBNull.Value;
                    command.Parameters["@p_DisplayNameEn"].Value = (object)attribute.display_name_en ?? DBNull.Value;
                    command.Parameters["@p_AttributeId"].Value = attribute.id;
                    command.Parameters["@p_InputType"].Value = (object)attribute.input_type ?? DBNull.Value;
                    command.Parameters["@p_IsRequired"].Value = attribute.is_required;
                    command.Parameters["@p_CategoryId"].Value = attribute.category_id;

                    // Thực thi lệnh
                    command.ExecuteNonQuery();
                }
            }
        }

        public void TikiInsert_tbTikiTrackCreateProduct(string track_id,
            string state,
            string reason,
            string request_id,
            string name,
            MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbTikiTrackCreateProduct_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@intrack_id", track_id);
                cmd.Parameters.AddWithValue("@instate", state);
                cmd.Parameters.AddWithValue("@inreason", reason);
                cmd.Parameters.AddWithValue("@inrequest_id", request_id);
                cmd.Parameters.AddWithValue("@inName", name);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }
    }
}