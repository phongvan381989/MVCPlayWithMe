using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.Models.ProductModel;
using MVCPlayWithMe.Models.SanPhamModel;
using MVCPlayWithMe.OpenPlatform.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Models.Order
{
    public class OrderMySql
    {
        //public int AddOrder(int customerId, string note, int isNotWeb,
        //    Address cusInfor)
        //{
        //    int id = -1;
        //    //if (cusInfor == null)
        //    //    return id;

        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbOrder_Insert", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inCustomerId", customerId);
        //        if (cusInfor != null)
        //        {
        //            cmd.Parameters.AddWithValue("@inName", cusInfor.name);
        //            cmd.Parameters.AddWithValue("@inPhone", cusInfor.phone);
        //            cmd.Parameters.AddWithValue("@inProvince", cusInfor.province);
        //            cmd.Parameters.AddWithValue("@inDistrict", cusInfor.district);
        //            cmd.Parameters.AddWithValue("@inSubDistrict", cusInfor.subdistrict);
        //            cmd.Parameters.AddWithValue("@inDetail", cusInfor.detail);
        //        }
        //        else
        //        {
        //            cmd.Parameters.AddWithValue("@inName", null);
        //            cmd.Parameters.AddWithValue("@inPhone", null);
        //            cmd.Parameters.AddWithValue("@inProvince", null);
        //            cmd.Parameters.AddWithValue("@inDistrict", null);
        //            cmd.Parameters.AddWithValue("@inSubDistrict", null);
        //            cmd.Parameters.AddWithValue("@inDetail", null);
        //        }
        //        cmd.Parameters.AddWithValue("@inNote", note);
        //        cmd.Parameters.AddWithValue("@inIsNotWeb", isNotWeb);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            id = MyMySql.GetInt32(rdr, "LastId");
        //        }

        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //    }
        //    conn.Close();

        //    return id;
        //}

        //public MySqlResultState AddPayOrder(int orderId, List<OrderPay> ls)
        //{
        //    MySqlResultState result = new MySqlResultState();
        //    if (ls == null || ls.Count() == 0)
        //        return result;

        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Insert", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inOrderId", orderId);
        //        cmd.Parameters.AddWithValue("@inType", 0);
        //        cmd.Parameters.AddWithValue("@inValue", 0);
        //        foreach (var orderPay in ls)
        //        {
        //            cmd.Parameters[1].Value = orderPay.type;
        //            cmd.Parameters[2].Value = orderPay.value;

        //            cmd.ExecuteNonQuery();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Common.SetResultException(ex, result);
        //    }
        //    conn.Close();

        //    return result;
        //}

        /// <summary>
        /// Lấy index trong list có orderId bằng tham số truyền vào
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="fromIndex">index bắt đầu tìm kiếm</param>
        /// <param name="count">số phần tử của danh sách</param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private int GetIndex(List<Order> ls, int fromIndex, int count, int orderId)
        {
            int index = -1;
            for (int i = fromIndex; i < count; i++)
            {
                if (ls[i].id == orderId)
                {
                    index = i;
                }
            }
            return index;
        }

        private void ReadOrder(Order order, MySqlDataReader rdr)
        {
            order.id = MyMySql.GetInt32(rdr, "Id");
            order.customerId = MyMySql.GetInt32(rdr, "CustomerId");
            order.address.name = MyMySql.GetString(rdr, "Name");
            order.address.phone = MyMySql.GetString(rdr, "Phone");
            order.address.province = MyMySql.GetString(rdr, "Province");
            order.address.subdistrict = MyMySql.GetString(rdr, "SubDistrict");
            order.address.detail = MyMySql.GetString(rdr, "Detail");
            order.note = MyMySql.GetString(rdr, "Note");
            order.time = MyMySql.GetDateTime(rdr, "Time");
        }

        private void ReadOrderTrack(OrderTrack track, MySqlDataReader rdr)
        {
            track.id = MyMySql.GetInt32(rdr, "Id");
            track.orderId = MyMySql.GetInt32(rdr, "OrderId");
            track.status = (EOrderStatus)MyMySql.GetInt32(rdr, "Status");
            track.time = MyMySql.GetDateTime(rdr, "Time");
            track.SetStrStatus();
        }

        private void ReadOrderPay(OrderPay pay, MySqlDataReader rdr)
        {
            pay.id = MyMySql.GetInt32(rdr, "Id");
            pay.orderId = MyMySql.GetInt32(rdr, "OrderId");
            pay.promotionOrderId = MyMySql.GetInt32(rdr, "PromotionOrderId");
            pay.type = (EPayType)MyMySql.GetInt32(rdr, "Type");
            pay.value = MyMySql.GetInt32(rdr, "Value");
            pay.SetStrType();
        }

        private void ReadOrderDetail(OrderDetail detail, MySqlDataReader rdr)
        {
            detail.id = MyMySql.GetInt32(rdr, "Id");
            detail.orderId = MyMySql.GetInt32(rdr, "OrderId");
            detail.itemId = MyMySql.GetInt32(rdr, "ItemId");
            detail.itemName = MyMySql.GetString(rdr, "ItemName");
            detail.modelId = MyMySql.GetInt32(rdr, "ModelId");
            detail.modelName = MyMySql.GetString(rdr, "ModelName");
            detail.quantity = MyMySql.GetInt32(rdr, "Quantity");
            detail.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
            detail.price = MyMySql.GetInt32(rdr, "Price");
            detail.SetImageSrc();
        }

        /// <summary>
        /// </summary>
        /// <param name="fromTo"> 0: 1 ngày, 1: 7 ngày, 2: 30 ngày</param>
        /// <returns></returns>
        //public List<CommonOrder> GetListCommonOrder(int fromTo)
        //{
        //    List<CommonOrder> ls = new List<CommonOrder>();
        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_To_Pack_Order", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inFromTo", fromTo);

        //        MySqlDataReader rdr = cmd.ExecuteReader();
        //        CommonOrder commonOrder = null;
        //        long id = 0;
        //        long itemId = 0;
        //        long modelId = 0;
        //        int modelQuantity = 0;
        //        string itemName = "";
        //        string modelName = "";
        //        string imgSrc = "";

        //        int orderIdIndex = rdr.GetOrdinal("OrderId");
        //        int orderCodeIndex = rdr.GetOrdinal("OrderCode");
        //        int orderTimeIndex = rdr.GetOrdinal("OrderTime");
        //        int statusIndex = rdr.GetOrdinal("StatusInTrackOrder");
        //        int modelIdIndex = rdr.GetOrdinal("ModelId");
        //        int itemIdIndex = rdr.GetOrdinal("ItemId");
        //        int modelQuantityIndex = rdr.GetOrdinal("ModelQuantity");
        //        int itemNameIndex = rdr.GetOrdinal("ItemName");
        //        int modelNameIndex = rdr.GetOrdinal("ModelName");

        //        while (rdr.Read())
        //        {
        //            id = (long)rdr.GetInt32(orderIdIndex);
        //            if (ls.Count == 0 || ls[ls.Count - 1].id != id)
        //            {
        //                ls.Add(new CommonOrder());
        //            }
        //            commonOrder = ls[ls.Count - 1];
        //            commonOrder.ecommerceName = Common.ePlayWithMe;
        //            if (commonOrder.id != id)
        //            {
        //                commonOrder.id = id;
        //                commonOrder.code = rdr.IsDBNull(orderCodeIndex) ? string.Empty : rdr.GetString(orderCodeIndex);
        //                commonOrder.created_at = rdr.IsDBNull(orderTimeIndex) ? DateTime.MinValue : rdr.GetDateTime(orderTimeIndex);
        //                commonOrder.status = OrderTrack.GetString(rdr.IsDBNull(statusIndex) ? -1 : rdr.GetInt32(statusIndex));
        //            }

        //            modelId = rdr.IsDBNull(modelIdIndex) ? -1L : (long)rdr.GetInt32(modelIdIndex);

        //            if (commonOrder.listModelId.Count == 0 ||
        //                commonOrder.listModelId[commonOrder.listModelId.Count - 1] != modelId)
        //            {
        //                itemId = (long)rdr.GetInt32(itemIdIndex);
        //                modelQuantity = rdr.IsDBNull(modelQuantityIndex) ? -1 : rdr.GetInt32(modelQuantityIndex);
        //                itemName = rdr.IsDBNull(itemNameIndex) ? string.Empty : rdr.GetString(itemNameIndex);
        //                modelName = rdr.IsDBNull(modelNameIndex) ? string.Empty : rdr.GetString(modelNameIndex);
        //                imgSrc = Common.GetModelImageSrc(Common.ConvertLongToInt(itemId), Common.ConvertLongToInt(modelId));

        //                commonOrder.listItemId.Add(itemId);
        //                commonOrder.listModelId.Add(modelId);
        //                commonOrder.listItemName.Add(itemName);
        //                commonOrder.listModelName.Add(modelName);
        //                commonOrder.listQuantity.Add(modelQuantity);
        //                commonOrder.listThumbnail.Add(imgSrc);
        //            }
        //        }

        //        rdr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //        ls.Clear();
        //    }

        //    conn.Close();
        //    return ls;
        //}

        // Lấy mapping của sản phẩm trong đơn hàng
        public async Task PlayWithMeGetMappingOfCommonOrderConnectOut(CommonOrder commonOrder, MySqlConnection conn)
        {
            string status = string.Empty;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbMapping_Get_From_ModelId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inModelId", int.MinValue);

                int quantity = 0;
                Product pro = null;
                for (int i = 0; i < commonOrder.listModelId.Count; i++)
                {
                    cmd.Parameters[0].Value = Common.ConvertLongToInt(commonOrder.listModelId[i]);

                    commonOrder.listMapping.Add(new List<Mapping>());

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
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

        // Từ đơn hàng, cập nhật trạng thái sản phẩm trên sàn vì có sản phẩm trên sàn được bật bán trở lại
        public async Task UpdateStatusNormalOfTMDTItemConnectOut(CommonOrder order, MySqlConnection conn)
        {
            try
            {
                if (order.ecommerceName == Common.eShopee)
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE tbShopeeItem SET Status=0 WHERE Status<>0 AND TMDTShopeeItemId=@inTMDTShopeeItemId", conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", 0L);
                    foreach (var id in order.listItemId)
                    {
                        cmd.Parameters[0].Value = id;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else if (order.ecommerceName == Common.eTiki)
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE tbTikiItem SET Status=0 WHERE Status<>0 AND TikiId=@inTikiId", conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@inTikiId", 0);
                    foreach (var id in order.listItemId)
                    {
                        cmd.Parameters[0].Value = (int)id;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else if (order.ecommerceName == Common.eLazada)
                {
                    {
                        MySqlCommand cmd = new MySqlCommand(
                            "UPDATE tb_lazada_item SET Status=0 WHERE Status<>0 AND TMDTLazadaItemId=@inTMDTItemId", conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inTMDTItemId", 0L);
                        foreach (var id in order.listItemId)
                        {
                            cmd.Parameters[0].Value = id;
                             await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    {
                        MySqlCommand cmd = new MySqlCommand(
                            "UPDATE tb_lazada_model SET Status=0 WHERE Status<>0 AND TMDTLazadaModelId=@inTMDTModelId", conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inTMDTModelId", 0L);
                        foreach (var id in order.listModelId)
                        {
                            cmd.Parameters[0].Value = id;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // ── Async versions ────────────────────────────────────────────────────

        private async Task<Order> GetOrderConnectOutAsync(int id, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_Order", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", id);

            Order order = null;
            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
            {
                while (await rdr.ReadAsync())
                {
                    order = new Order();
                    ReadOrder(order, rdr);
                }
            }
            return order;
        }

        private async Task GetOrderTrackConnectOutAsync(Order order, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbTrackOrder_Get_From_OrderId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", order.id);
            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
            {
                while (await rdr.ReadAsync())
                {
                    OrderTrack track = new OrderTrack();
                    ReadOrderTrack(track, rdr);
                    order.lsOrderTrack.Add(track);
                }
            }
        }

        private async Task GetOrderPayConnectOutAsync(Order order, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Get_From_OrderId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", order.id);
            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
            {
                while (await rdr.ReadAsync())
                {
                    OrderPay pay = new OrderPay();
                    ReadOrderPay(pay, rdr);
                    order.lsOrderPay.Add(pay);
                }
            }
        }

        private async Task GetOrderDetailConnectOutAsync(Order order, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Get_From_OrderId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", order.id);
            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
            {
                while (await rdr.ReadAsync())
                {
                    OrderDetail detail = new OrderDetail();
                    ReadOrderDetail(detail, rdr);
                    order.lsOrderDetail.Add(detail);
                }
            }
        }

        private async Task<Order> GetOrderFromIdConnectOutAsync(int orderId, MySqlConnection conn)
        {
            Order order = null;
            try
            {
                order = await GetOrderConnectOutAsync(orderId, conn);
                if (order == null) return order;
                await GetOrderTrackConnectOutAsync(order, conn);
                await GetOrderPayConnectOutAsync(order, conn);
                await GetOrderDetailConnectOutAsync(order, conn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                order = null;
            }
            return order;
        }

        public async Task<List<Cart>> GetListCartAsync(int customerId)
        {
            List<Cart> ls = new List<Cart>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCart_Get_From_CustormerId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int sanPhamIdIndex = rdr.GetOrdinal("SanPhamId");
                        int quantityIndex = rdr.GetOrdinal("Quantity");
                        int realIndex = rdr.GetOrdinal("Real");
                        while (await rdr.ReadAsync())
                        {
                            Cart cart = new Cart();
                            cart.sanPhamId = rdr.GetInt32(sanPhamIdIndex);
                            cart.quantity = rdr.GetInt32(quantityIndex);
                            cart.real = rdr.GetInt32(realIndex);
                            ls.Add(cart);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    ls.Clear();
                }
            }
            return ls;
        }

        public async Task<int> AddOrderAsync(int customerId, string note, int isNotWeb, Address cusInfor)
        {
            int id = -1;
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbOrder_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                    if (cusInfor != null)
                    {
                        cmd.Parameters.AddWithValue("@inName", cusInfor.name);
                        cmd.Parameters.AddWithValue("@inPhone", cusInfor.phone);
                        cmd.Parameters.AddWithValue("@inProvince", cusInfor.province);
                        cmd.Parameters.AddWithValue("@inSubDistrict", cusInfor.subdistrict);
                        cmd.Parameters.AddWithValue("@inDetail", cusInfor.detail);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@inName", null);
                        cmd.Parameters.AddWithValue("@inPhone", null);
                        cmd.Parameters.AddWithValue("@inProvince", null);
                        cmd.Parameters.AddWithValue("@inSubDistrict", null);
                        cmd.Parameters.AddWithValue("@inDetail", null);
                    }
                    cmd.Parameters.AddWithValue("@inNote", note);
                    cmd.Parameters.AddWithValue("@inIsNotWeb", isNotWeb);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            id = MyMySql.GetInt32(rdr, "LastId");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return id;
        }

        public async Task AddTrackOrderAsync(int orderId, int status)
        {
            MySqlParameter[] paras = new MySqlParameter[4];
            paras[0] = new MySqlParameter("@inOrderId", orderId);
            paras[1] = new MySqlParameter("@inStatus", status);
            MyMySql.AddOutParameters(paras);
            await MyMySql.ExcuteNonQueryStoreProcedureAsync("st_tbTrackOrder_Insert", paras);
        }

        public async Task AddDetailOrderAsync(int orderId, List<Cart> lsCartCookie)
        {
            MySqlParameter[] paras = new MySqlParameter[7];
            paras[0] = new MySqlParameter("@inOrderId", (object)0);
            paras[1] = new MySqlParameter("@inModelId", (object)0);
            paras[2] = new MySqlParameter("@inQuantity", (object)0);
            paras[3] = new MySqlParameter("@inBookCoverPrice", (object)0);
            paras[4] = new MySqlParameter("@inPrice", (object)0);
            MyMySql.AddOutParameters(paras);

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                try
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(paras);
                    foreach (var cart in lsCartCookie)
                    {
                        paras[0].Value = orderId;
                        paras[1].Value = cart.id;
                        paras[2].Value = cart.quantity;
                        //paras[3].Value = cart.bookCoverPrice;
                        //paras[4].Value = cart.price;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
        }

        public async Task<MySqlResultState> AddPayOrderAsync(int orderId, List<OrderPay> ls)
        {
            MySqlResultState result = new MySqlResultState();
            if (ls == null || ls.Count() == 0) return result;

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Insert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inOrderId", orderId);
                    cmd.Parameters.AddWithValue("@inType", 0);
                    cmd.Parameters.AddWithValue("@inValue", 0);
                    foreach (var orderPay in ls)
                    {
                        cmd.Parameters[1].Value = orderPay.type;
                        cmd.Parameters[2].Value = orderPay.value;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public async Task<MySqlResultState> RefreshRealOfCartAsync(int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[1];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            return await MyMySql.ExcuteNonQueryAsync("st_tbCart_Refresh_Real_From_CustormerId", paras);
        }

        /// <summary>
        /// Lấy cart items với real=1 (sản phẩm được chọn mua)
        /// </summary>
        public async Task<List<Cart>> GetRealCartAsync(int customerId)
        {
            List<Cart> lsCart = new List<Cart>();

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT SanPhamId, Quantity, Real FROM tbCart WHERE CustomerId = @customerId AND Real = 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerId", customerId);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            Cart cart = new Cart();
                            cart.sanPhamId = MyMySql.GetInt32(rdr, "SanPhamId");
                            cart.quantity = MyMySql.GetInt32(rdr, "Quantity");
                            cart.real = MyMySql.GetInt32(rdr, "Real");
                            lsCart.Add(cart);
                        }
                    }

                    // Load sản phẩm basic info
                    await GetCartsSanPhamBasicInfoAsync(lsCart);
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }

            return lsCart;
        }

        /// <summary>
        /// Update real = 1 cho list sanPhamIds được chọn mua, các sản phẩm còn lại set real = 0
        /// </summary>
        public async Task<MySqlResultState> UpdateRealCartAsync(int customerId, List<int> sanPhamIds)
        {
            MySqlResultState result = new MySqlResultState();

            // Step 1: Set tất cả real = 0
            result = await RefreshRealOfCartAsync(customerId);
            if (result.State != EMySqlResultState.OK)
            {
                return result;
            }

            // Step 2: Set real = 1 cho list sanPhamIds được chọn
            if (sanPhamIds == null || sanPhamIds.Count == 0)
            {
                return result; // Không có sản phẩm nào được chọn
            }

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tbCart SET Real = 1 WHERE CustomerId = @customerId AND SanPhamId = @sanPhamId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add("@customerId", MySqlDbType.Int32);
                    cmd.Parameters.Add("@sanPhamId", MySqlDbType.Int32);

                    cmd.Parameters["@customerId"].Value = customerId;

                    foreach (int sanPhamId in sanPhamIds)
                    {
                        cmd.Parameters["@sanPhamId"].Value = sanPhamId;
                        await cmd.ExecuteNonQueryAsync();
                    }

                    result.State = EMySqlResultState.OK;
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }

            return result;
        }

        public async Task<MySqlResultState> DeleteSanPhamOnCartAsync(int customerId, int sanPhamId)
        {
            MySqlParameter[] paras = new MySqlParameter[2];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inSanPhamId", sanPhamId);
            return await MyMySql.ExcuteNonQueryAsync("st_tbCart_Delete_From_Customer_SanPhamId", paras);
        }

        public async Task<MySqlResultState> DeleteListCartAsync(int customerId, List<Cart> ls)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                MySqlCommand cmd = new MySqlCommand("st_tbCart_Delete_From_Customer_ModelId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                cmd.Parameters.AddWithValue("@inModelId", (object)0);
                try
                {
                    await conn.OpenAsync();
                    foreach (var cart in ls)
                    {
                        cmd.Parameters[1].Value = cart.id;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public async Task<MySqlResultState> UpdateSanPhamQuantityOnCartAsync(int customerId, int sanPhamId, int quantity)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inSanPhamId", sanPhamId);
            paras[2] = new MySqlParameter("@inQuantity", quantity);
            return await MyMySql.ExcuteNonQueryAsync("st_tbCart_Update_Quantity", paras);
        }

        public async Task<MySqlResultState> UpdateSanPhamQuantityListOnCartAsync(
            int customerId,
            Dictionary<int, int> updates)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                MySqlCommand cmd = new MySqlCommand("st_tbCart_Update_Quantity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                cmd.Parameters.AddWithValue("@inSanPhamId", 0);
                cmd.Parameters.AddWithValue("@inQuantity", 0);
                try
                {
                    await conn.OpenAsync();
                    foreach (var kvp in updates)
                    {
                        cmd.Parameters[1].Value = kvp.Key;
                        cmd.Parameters[2].Value = kvp.Value;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public async Task<MySqlResultState> GetCartCountAsync(int customerId)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbCart_Count_From_CustormerId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            result.myAnything = MyMySql.GetInt32(rdr, "Count");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public async Task<MySqlResultState> GetAllOrderAsync(int customerId)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_All_Order", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            int customerIdIndex = rdr.GetOrdinal("CustomerId");
                            int nameIndex = rdr.GetOrdinal("Name");
                            int phoneIndex = rdr.GetOrdinal("Phone");
                            int provinceIndex = rdr.GetOrdinal("Province");
                            int subdistrictIndex = rdr.GetOrdinal("SubDistrict");
                            int detailIndex = rdr.GetOrdinal("Detail");
                            int noteIndex = rdr.GetOrdinal("Note");
                            int timeIndex = rdr.GetOrdinal("Time");
                            while (await rdr.ReadAsync())
                            {
                                Order order = new Order();
                                order.id = rdr.GetInt32(idIndex);
                                order.customerId = rdr.GetInt32(customerIdIndex);
                                order.address.name = rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex);
                                order.address.phone = rdr.IsDBNull(phoneIndex) ? string.Empty : rdr.GetString(phoneIndex);
                                order.address.province = rdr.IsDBNull(provinceIndex) ? string.Empty : rdr.GetString(provinceIndex);
                                order.address.subdistrict = rdr.IsDBNull(subdistrictIndex) ? string.Empty : rdr.GetString(subdistrictIndex);
                                order.address.detail = rdr.IsDBNull(detailIndex) ? string.Empty : rdr.GetString(detailIndex);
                                order.note = rdr.IsDBNull(noteIndex) ? string.Empty : rdr.GetString(noteIndex);
                                order.time = rdr.IsDBNull(timeIndex) ? DateTime.MinValue : rdr.GetDateTime(timeIndex);
                                ls.Add(order);
                            }
                        }
                    }

                    int index = 0;
                    int indexTemp = 0;
                    int orderIdTemp = 0;
                    int count = ls.Count();
                    if (count > 0)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("st_tbTrackOrder_Search", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                            index = 0;
                            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                int orderIdIndex = rdr.GetOrdinal("OrderId");
                                while (await rdr.ReadAsync())
                                {
                                    orderIdTemp = rdr.GetInt32(orderIdIndex);
                                    if (orderIdTemp > ls[index].id)
                                    {
                                        indexTemp = GetIndex(ls, index, count, orderIdTemp);
                                        index = indexTemp;
                                    }
                                    OrderTrack track = new OrderTrack();
                                    ReadOrderTrack(track, rdr);
                                    ls[index].lsOrderTrack.Add(track);
                                }
                            }
                        }

                        using (MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Search", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                            index = 0;
                            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                int orderIdIndex = rdr.GetOrdinal("OrderId");
                                while (await rdr.ReadAsync())
                                {
                                    orderIdTemp = rdr.GetInt32(orderIdIndex);
                                    if (orderIdTemp > ls[index].id)
                                    {
                                        indexTemp = GetIndex(ls, index, count, orderIdTemp);
                                        index = indexTemp;
                                    }
                                    OrderPay pay = new OrderPay();
                                    ReadOrderPay(pay, rdr);
                                    ls[index].lsOrderPay.Add(pay);
                                }
                            }
                        }

                        using (MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Search", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                            index = 0;
                            using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                            {
                                int orderIdIndex = rdr.GetOrdinal("OrderId");
                                while (await rdr.ReadAsync())
                                {
                                    orderIdTemp = rdr.GetInt32(orderIdIndex);
                                    if (orderIdTemp > ls[index].id)
                                    {
                                        indexTemp = GetIndex(ls, index, count, orderIdTemp);
                                        index = indexTemp;
                                    }
                                    OrderDetail detail = new OrderDetail();
                                    ReadOrderDetail(detail, rdr);
                                    ls[index].lsOrderDetail.Add(detail);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    ls.Clear();
                }
            }
            result.myJson = ls;
            return result;
        }

        public async Task<MySqlResultState> GetOrderFromIdAsync(int orderId)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    Order order = await GetOrderConnectOutAsync(orderId, conn);
                    if (order != null)
                    {
                        await GetOrderTrackConnectOutAsync(order, conn);
                        await GetOrderPayConnectOutAsync(order, conn);
                        await GetOrderDetailConnectOutAsync(order, conn);
                        ls.Add(order);
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            result.myJson = ls;
            return result;
        }

        public async Task<MySqlResultState> GetAllOrderFromListIdAsync(List<int> ids)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    foreach (var id in ids)
                    {
                        Order order = await GetOrderFromIdConnectOutAsync(id, conn);
                        if (order != null) ls.Add(order);
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    ls.Clear();
                }
            }
            result.myJson = ls;
            return result;
        }

        public async Task<MySqlResultState> SearchOrderForAnonymousAsync(string sdtNameForSearch)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_Order_From_Name_SDT_For_Anonymous", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inNameOrLastSDT", sdtNameForSearch);
                        using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            int idIndex = rdr.GetOrdinal("Id");
                            int customerIdIndex = rdr.GetOrdinal("CustomerId");
                            int nameIndex = rdr.GetOrdinal("Name");
                            int phoneIndex = rdr.GetOrdinal("Phone");
                            int provinceIndex = rdr.GetOrdinal("Province");
                            int subdistrictIndex = rdr.GetOrdinal("SubDistrict");
                            int detailIndex = rdr.GetOrdinal("Detail");
                            int noteIndex = rdr.GetOrdinal("Note");
                            int timeIndex = rdr.GetOrdinal("Time");
                            while (await rdr.ReadAsync())
                            {
                                Order order = new Order();
                                order.id = rdr.GetInt32(idIndex);
                                order.customerId = rdr.GetInt32(customerIdIndex);
                                order.address.name = rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex);
                                order.address.phone = rdr.IsDBNull(phoneIndex) ? string.Empty : rdr.GetString(phoneIndex);
                                order.address.province = rdr.IsDBNull(provinceIndex) ? string.Empty : rdr.GetString(provinceIndex);
                                order.address.subdistrict = rdr.IsDBNull(subdistrictIndex) ? string.Empty : rdr.GetString(subdistrictIndex);
                                order.address.detail = rdr.IsDBNull(detailIndex) ? string.Empty : rdr.GetString(detailIndex);
                                order.note = rdr.IsDBNull(noteIndex) ? string.Empty : rdr.GetString(noteIndex);
                                order.time = rdr.IsDBNull(timeIndex) ? DateTime.MinValue : rdr.GetDateTime(timeIndex);
                                ls.Add(order);
                            }
                        }
                    }

                    foreach (Order order in ls)
                    {
                        await GetOrderTrackConnectOutAsync(order, conn);
                        await GetOrderPayConnectOutAsync(order, conn);
                        await GetOrderDetailConnectOutAsync(order, conn);
                        order.address.phone = "******" + order.address.phone.Substring(6);
                        order.address.detail = "";
                        order.address.subdistrict = "";
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    ls.Clear();
                }
            }
            result.myJson = ls;
            return result;
        }

        public async Task<List<CommonOrder>> GetListCommonOrderAsync(int fromTo)
        {
            List<CommonOrder> ls = new List<CommonOrder>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_To_Pack_Order", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inFromTo", fromTo);

                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        CommonOrder commonOrder = null;
                        long id = 0;
                        int orderIdIndex = rdr.GetOrdinal("OrderId");
                        int orderCodeIndex = rdr.GetOrdinal("OrderCode");
                        int orderTimeIndex = rdr.GetOrdinal("OrderTime");
                        int statusIndex = rdr.GetOrdinal("StatusInTrackOrder");
                        int modelIdIndex = rdr.GetOrdinal("ModelId");
                        int itemIdIndex = rdr.GetOrdinal("ItemId");
                        int modelQuantityIndex = rdr.GetOrdinal("ModelQuantity");
                        int itemNameIndex = rdr.GetOrdinal("ItemName");
                        int modelNameIndex = rdr.GetOrdinal("ModelName");

                        while (await rdr.ReadAsync())
                        {
                            id = (long)rdr.GetInt32(orderIdIndex);
                            if (ls.Count == 0 || ls[ls.Count - 1].id != id)
                                ls.Add(new CommonOrder());
                            commonOrder = ls[ls.Count - 1];
                            commonOrder.ecommerceName = Common.ePlayWithMe;
                            if (commonOrder.id != id)
                            {
                                commonOrder.id = id;
                                commonOrder.code = rdr.IsDBNull(orderCodeIndex) ? string.Empty : rdr.GetString(orderCodeIndex);
                                commonOrder.created_at = rdr.IsDBNull(orderTimeIndex) ? DateTime.MinValue : rdr.GetDateTime(orderTimeIndex);
                                commonOrder.status = OrderTrack.GetString(rdr.IsDBNull(statusIndex) ? -1 : rdr.GetInt32(statusIndex));
                            }
                            long modelId = rdr.IsDBNull(modelIdIndex) ? -1L : (long)rdr.GetInt32(modelIdIndex);
                            if (commonOrder.listModelId.Count == 0 ||
                                commonOrder.listModelId[commonOrder.listModelId.Count - 1] != modelId)
                            {
                                long itemId = (long)rdr.GetInt32(itemIdIndex);
                                int modelQuantity = rdr.IsDBNull(modelQuantityIndex) ? -1 : rdr.GetInt32(modelQuantityIndex);
                                string itemName = rdr.IsDBNull(itemNameIndex) ? string.Empty : rdr.GetString(itemNameIndex);
                                string modelName = rdr.IsDBNull(modelNameIndex) ? string.Empty : rdr.GetString(modelNameIndex);
                                string imgSrc = Common.GetModelImageSrc(Common.ConvertLongToInt(itemId), Common.ConvertLongToInt(modelId));
                                commonOrder.listItemId.Add(itemId);
                                commonOrder.listModelId.Add(modelId);
                                commonOrder.listItemName.Add(itemName);
                                commonOrder.listModelName.Add(modelName);
                                commonOrder.listQuantity.Add(modelQuantity);
                                commonOrder.listThumbnail.Add(imgSrc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    ls.Clear();
                }
            }
            return ls;
        }

        // Lấy cart với các thông tin chi tiết của sản phẩm từ DB và cập nhật vào List<Cart>
        public async Task GetCartsSanPhamBasicInfoAsync(List<Cart> ls)
        {
            if (ls == null || ls.Count() == 0)
                return;
            ItemModelMySql itemModelsqler = new ItemModelMySql();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    foreach (var cart in ls)
                    {
                        cart.sanPhamBasicInfo = await SanPhamMySql.GetSanPhamBasicInfo_ConnectOutAsync(cart.sanPhamId, conn);
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
        }

        public async Task GetOrderStatusInWarehouseToCommonOrderAsync(List<CommonOrder> ls)
        {
            if (ls == null || ls.Count == 0) return;

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Get_Lastest_Status_From_Code", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCode", "");
                    cmd.Parameters.AddWithValue("@inECommmerce", 0);

                    MySqlCommand cmdBooking = new MySqlCommand("st_tbECommerceBooking_Get_Lastest_Status_From_Code", conn);
                    cmdBooking.CommandType = CommandType.StoredProcedure;
                    cmdBooking.Parameters.AddWithValue("@inCode", "");
                    cmdBooking.Parameters.AddWithValue("@inECommmerce", 0);

                    foreach (var order in ls)
                    {
                        MySqlCommand cmdTem = order.isBooking ? cmdBooking : cmd;
                        string status = string.Empty;
                        cmdTem.Parameters[0].Value = order.isBooking ? order.bookingCode : order.code;
                        if (order.ecommerceName == Common.eTiki)        cmdTem.Parameters[1].Value = (int)EECommerceType.TIKI;
                        else if (order.ecommerceName == Common.eShopee)  cmdTem.Parameters[1].Value = (int)EECommerceType.SHOPEE;
                        else if (order.ecommerceName == Common.eLazada)  cmdTem.Parameters[1].Value = (int)EECommerceType.LAZADA;
                        else if (order.ecommerceName == Common.ePlayWithMe) cmdTem.Parameters[1].Value = (int)EECommerceType.PLAY_WITH_ME;

                        using (MySqlDataReader rdr = (MySqlDataReader)await cmdTem.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                status = Common.OrderStatusArray[MyMySql.GetInt32(rdr, "Status")];
                            }
                        }
                        order.orderStatusInWarehoue = status;
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
        }

        // Lấy trạng thái từ DB và cập nhật trạng thái đơn hàng đã đóng/ đã hoàn vào List<CommonOrder>
        // Hàm này dùng cho sàn: web PWM, Tiki, Shopee,...
        //public void GetOrderStatusInWarehouseToCommonOrder(List<CommonOrder> ls)
        //{
        //    if (ls == null || ls.Count == 0)
        //    {
        //        return;
        //    }

        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    string status = string.Empty;
        //    try
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Get_Lastest_Status_From_Code", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@inCode", "");
        //        cmd.Parameters.AddWithValue("@inECommmerce", 0);

        //        MySqlCommand cmdBooking = new MySqlCommand("st_tbECommerceBooking_Get_Lastest_Status_From_Code", conn);
        //        cmdBooking.CommandType = CommandType.StoredProcedure;
        //        cmdBooking.Parameters.AddWithValue("@inCode", "");
        //        cmdBooking.Parameters.AddWithValue("@inECommmerce", 0);
        //        MySqlCommand cmdTem = null;
        //        MySqlDataReader rdr;
        //        foreach (var order in ls)
        //        {
        //            cmdTem = cmd;
        //            if (order.isBooking)
        //            {
        //                cmdTem = cmdBooking;
        //            }
        //            status = string.Empty;
        //            cmdTem.Parameters[0].Value = order.code;
        //            if (order.isBooking)
        //            {
        //                cmdTem.Parameters[0].Value = order.bookingCode;
        //            }
        //            if (order.ecommerceName == Common.eTiki)
        //                cmdTem.Parameters[1].Value = (int)EECommerceType.TIKI;
        //            else if (order.ecommerceName == Common.eShopee)
        //                cmdTem.Parameters[1].Value = (int)EECommerceType.SHOPEE;
        //            else if (order.ecommerceName == Common.eLazada)
        //                cmdTem.Parameters[1].Value = (int)EECommerceType.LAZADA;
        //            else if (order.ecommerceName == Common.ePlayWithMe)
        //                cmdTem.Parameters[1].Value = (int)EECommerceType.PLAY_WITH_ME;

        //            rdr = cmdTem.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                status = Common.OrderStatusArray[MyMySql.GetInt32(rdr, "Status")];
        //            }
        //            order.orderStatusInWarehoue = status;
        //            rdr.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Warn(ex.ToString());
        //    }

        //    conn.Close();
        //}
    }
}
