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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Models.Order
{
    public class OrderMySql
    {
        /// <summary>
        /// Lấy index trong list có orderId bằng tham số truyền vào
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="fromIndex">index bắt đầu tìm kiếm</param>
        /// <param name="count">số phần tử của danh sách</param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private static int GetIndex(List<Order> ls, int fromIndex, int count, int orderId)
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

        /// <summary>
        /// Đọc Order từ MySqlDataReader (dùng column index để tối ưu performance)
        /// </summary>
        private static async Task<List<Order>> ReadOrder(MySqlCommand cmd)
        {
            List<Order> ls = new List<Order>();

            using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
            {
                // Lấy column ordinal một lần duy nhất (tối ưu performance)
                int idIndex = rdr.GetOrdinal("Id");
                int customerIdIndex = rdr.GetOrdinal("CustomerId");
                int nameIndex = rdr.GetOrdinal("Name");
                int phoneIndex = rdr.GetOrdinal("Phone");
                int provinceIndex = rdr.GetOrdinal("Province");
                int subDistrictIndex = rdr.GetOrdinal("SubDistrict");
                int detailIndex = rdr.GetOrdinal("Detail");
                int noteIndex = rdr.GetOrdinal("Note");
                int timeIndex = rdr.GetOrdinal("Time");
                int codeIndex = rdr.GetOrdinal("Code");
                int fromIndex = rdr.GetOrdinal("From");
                int payStatusIndex = rdr.GetOrdinal("PayStatus");
                int paymentMethodIndex = rdr.GetOrdinal("PaymentMethod");
                int statusIndex = rdr.GetOrdinal("Status");
                int paymentDeadlineIndex = rdr.GetOrdinal("PaymentDeadline");

                while (await rdr.ReadAsync())
                {
                    Order order = new Order();

                    order.id = MyMySql.GetInt32(rdr, idIndex);
                    order.customerId = MyMySql.GetInt32(rdr, customerIdIndex);
                    order.name = MyMySql.GetString(rdr, nameIndex);
                    order.phone = MyMySql.GetString(rdr, phoneIndex);
                    order.province = MyMySql.GetString(rdr, provinceIndex);
                    order.subdistrict = MyMySql.GetString(rdr, subDistrictIndex);
                    order.detail = MyMySql.GetString(rdr, detailIndex);
                    order.note = MyMySql.GetString(rdr, noteIndex);
                    order.time = MyMySql.GetDateTime(rdr, timeIndex);
                    order.code = MyMySql.GetString(rdr, codeIndex);
                    order.from = MyMySql.GetInt32(rdr, fromIndex);

                    // Thông tin thanh toán & trạng thái
                    order.OrderCode = MyMySql.GetString(rdr, codeIndex);
                    order.OrderPayStatus = (EOrderPayStatus)MyMySql.GetSByte(rdr, payStatusIndex);
                    order.PaymentMethod = (EPaymentMethod)MyMySql.GetSByte(rdr, paymentMethodIndex);
                    order.OrderStatus = (EOrderStatus)MyMySql.GetSByte(rdr, statusIndex);
                    order.PaymentDeadline = MyMySql.GetDateTime(rdr, paymentDeadlineIndex);
                    ls.Add(order);
                }
            }
            return ls;
        }

        /// <summary>
        /// Đọc OrderTrack từ MySqlDataReader (dùng column index để tối ưu performance)
        /// </summary>
        private static async Task<List<OrderTrack>> ReadOrderTrack(MySqlCommand cmd)
        {
            List<OrderTrack> ls = new List<OrderTrack>();

            using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
            {
                // Lấy column ordinal một lần duy nhất
                int idIndex = rdr.GetOrdinal("Id");
                int orderIdIndex = rdr.GetOrdinal("OrderId");
                int statusIndex = rdr.GetOrdinal("Status");
                int timeIndex = rdr.GetOrdinal("Time");

                while (await rdr.ReadAsync())
                {
                    OrderTrack track = new OrderTrack();
                    track.id = MyMySql.GetInt32(rdr, idIndex);
                    track.orderId = MyMySql.GetInt32(rdr, orderIdIndex);
                    track.status = (EOrderStatus)MyMySql.GetSByte(rdr, statusIndex);
                    track.time = MyMySql.GetDateTime(rdr, timeIndex);
                    track.SetStrStatus();
                    ls.Add(track);
                }
            }

            return ls;
        }

        /// <summary>
        /// Đọc OrderPay từ MySqlDataReader (dùng column index để tối ưu performance)
        /// </summary>
        private static async Task<List<OrderPay>> ReadOrderPay(MySqlCommand cmd)
        {
            List<OrderPay> ls = new List<OrderPay>();

            using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
            {
                // Lấy column ordinal một lần duy nhất
                int idIndex = rdr.GetOrdinal("Id");
                int orderIdIndex = rdr.GetOrdinal("OrderId");
                int typeIndex = rdr.GetOrdinal("Type");
                int valueIndex = rdr.GetOrdinal("Value");
                int orderSimplePromoIdIndex = rdr.GetOrdinal("OrderSimplePromoId");

                while (await rdr.ReadAsync())
                {
                    OrderPay pay = new OrderPay();
                    pay.id = MyMySql.GetInt32(rdr, idIndex);
                    pay.orderId = MyMySql.GetInt32(rdr, orderIdIndex);
                    pay.type = (EOrderPayType)MyMySql.GetSByte(rdr, typeIndex);
                    pay.value = MyMySql.GetInt32(rdr, valueIndex);
                    pay.orderSimplePromoId = MyMySql.GetInt32(rdr, orderSimplePromoIdIndex);
                    ls.Add(pay);
                }
            }

            return ls;
        }

        /// <summary>
        /// Đọc OrderDetail từ MySqlDataReader (dùng column index để tối ưu performance)
        /// </summary>
        private static async Task<List<OrderDetail>> ReadOrderDetail(MySqlCommand cmd)
        {
            List<OrderDetail> ls = new List<OrderDetail>();

            using (MySqlDataReader rdr = await cmd.ExecuteReaderAsync())
            {
                // Lấy column ordinal một lần duy nhất
                int idIndex = rdr.GetOrdinal("Id");
                int orderIdIndex = rdr.GetOrdinal("OrderId");
                int productIdIndex = rdr.GetOrdinal("ProductId");
                int quantityIndex = rdr.GetOrdinal("Quantity");
                int bookCoverPriceIndex = rdr.GetOrdinal("BookCoverPrice");
                int priceIndex = rdr.GetOrdinal("Price");
                int productNameIndex = rdr.GetOrdinal("ProductName");
                int coverImageFileNameIndex = rdr.GetOrdinal("CoverImageFileName");

                while (await rdr.ReadAsync())
                {
                    OrderDetail detail = new OrderDetail();
                    detail.id = MyMySql.GetInt32(rdr, idIndex);
                    detail.orderId = MyMySql.GetInt32(rdr, orderIdIndex);
                    detail.sanPhamId = MyMySql.GetInt32(rdr, productIdIndex);
                    detail.quantity = MyMySql.GetInt32(rdr, quantityIndex);
                    detail.bookCoverPrice = MyMySql.GetInt32(rdr, bookCoverPriceIndex);
                    detail.price = MyMySql.GetInt32(rdr, priceIndex);
                    detail.name = MyMySql.GetString(rdr, productNameIndex);
                    detail.CoverImageFileName = MyMySql.GetString(rdr, coverImageFileNameIndex);
                    ls.Add(detail);
                }
            }

            return ls;
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public static async Task PlayWithMeGetMappingOfCommonOrderConnectOut(CommonOrder commonOrder, MySqlConnection conn)
        {
            string status = string.Empty;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbMapping_Get_From_ModelId", conn))
                {
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
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // Từ đơn hàng, cập nhật trạng thái sản phẩm trên sàn vì có sản phẩm trên sàn được bật bán trở lại
        public static async Task UpdateStatusNormalOfTMDTItemConnectOut(CommonOrder order, MySqlConnection conn)
        {
            try
            {
                if (order.ecommerceName == Common.eShopee)
                {
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE tbShopeeItem SET Status=0 WHERE Status<>0 AND TMDTShopeeItemId=@inTMDTShopeeItemId", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inTMDTShopeeItemId", 0L);
                        foreach (var id in order.listItemId)
                        {
                            cmd.Parameters[0].Value = id;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                else if (order.ecommerceName == Common.eTiki)
                {
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE tbTikiItem SET Status=0 WHERE Status<>0 AND TikiId=@inTikiId", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@inTikiId", 0);
                        foreach (var id in order.listItemId)
                        {
                            cmd.Parameters[0].Value = (int)id;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                else if (order.ecommerceName == Common.eLazada)
                {
                    {
                        using (MySqlCommand cmd = new MySqlCommand(
                            "UPDATE tb_lazada_item SET Status=0 WHERE Status<>0 AND TMDTLazadaItemId=@inTMDTItemId", conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@inTMDTItemId", 0L);
                            foreach (var id in order.listItemId)
                            {
                                cmd.Parameters[0].Value = id;
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    {
                        using (MySqlCommand cmd = new MySqlCommand(
                            "UPDATE tb_lazada_model SET Status=0 WHERE Status<>0 AND TMDTLazadaModelId=@inTMDTModelId", conn))
                        {
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
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        // ── Async versions ────────────────────────────────────────────────────

        private static async Task<Order> GetCommonOrderFromCodeIdConnectOutAsync(int orderId, MySqlConnection conn)
        {
            Order order = null;
            using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_Order_From_Id", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderId", orderId);

                List<Order> ls = await ReadOrder(cmd);
                if(ls != null && ls.Count == 1)
                {
                    order = ls[0];
                }
            }
            return order;
        }

        private static async Task GetOrderTrackConnectOutAsync(Order order, MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand("st_tbOrderTrack_Get_From_OrderId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderId", order.id);
                List<OrderTrack> ls = await ReadOrderTrack(cmd);

                if(ls != null)
                {
                    order.lsOrderTrack = ls;
                }

            }
        }

        private static async Task GetOrderPayConnectOutAsync(Order order,
            List<OrderSimplePromotion> promotions,
            MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand("st_tbOrderPay_Get_From_OrderId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderId", order.id);
                List<OrderPay> ls = await ReadOrderPay(cmd);
                if(ls != null)
                {
                    order.lsOrderPay = ls;
                }

                UpdateOrderSimplePromotion(promotions, order);
            }
        }

        private static async Task GetOrderDetailConnectOutAsync(Order order, MySqlConnection conn)
        {
            using (MySqlCommand cmd = new MySqlCommand("st_tbOrderDetail_Get_From_OrderId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderId", order.id);
                List<OrderDetail> ls = await ReadOrderDetail(cmd);
                if(ls != null)
                {
                    order.lsOrderDetail = ls;
                }
            }
        }

        private static async Task<Order> GetOrderByOrderIdConnectOutAsync(int orderId, MySqlConnection conn)
        {
            Order order = await GetCommonOrderFromCodeIdConnectOutAsync(orderId, conn);

            if (order == null)
                return order;

            // Lấy promotions (để load vào OrderPay)
            List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsConnectOutAsync(conn);
            await GetOrderTrackConnectOutAsync(order, conn);
            await GetOrderPayConnectOutAsync(order, promotions, conn);
            await GetOrderDetailConnectOutAsync(order, conn);

            return order;
        }

        public static async Task<List<Cart>> GetListCartAsync(int customerId)
        {
            List<Cart> ls = new List<Cart>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbCart_Get_From_CustormerId", conn))
                    {
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
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                    ls.Clear();
                }
            }
            return ls;
        }

        public static async Task<MySqlResultState> AddOrderAsync(int customerId,
            string note,
            int from,
            Address cusInfor)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Sinh mã đơn hàng unique
                    string orderCode = await OrderCodeSequenceMySql.GenerateUniqueOrderCodeAsync();

                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@inCustomerId", MySqlDbType.Int32).Value = customerId;

                        cmd.Parameters.Add("@inName", MySqlDbType.VarChar).Value = cusInfor.name;
                        cmd.Parameters.Add("@inCode", MySqlDbType.VarChar).Value = orderCode;
                        cmd.Parameters.Add("@inPhone", MySqlDbType.VarChar).Value = cusInfor.phone;
                        cmd.Parameters.Add("@inProvince", MySqlDbType.VarChar).Value = cusInfor.province;
                        cmd.Parameters.Add("@inSubDistrict",MySqlDbType.VarChar).Value = cusInfor.subdistrict;
                        cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = cusInfor.detail;

                        cmd.Parameters.Add("@inNote", MySqlDbType.VarChar).Value = note;
                        cmd.Parameters.Add("@inFrom", MySqlDbType.Byte).Value = from;

                        object scalarResult = await cmd.ExecuteScalarAsync();
                        result.myAnythingLong = Convert.ToInt64(scalarResult);
                        result.myAnything = (int)result.myAnythingLong;
                        result.Message = orderCode;
                    }
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public static async Task<MySqlResultState> AddOrderTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            int customerId,
            string note,
            SByte from,
            string orderCode, // mã đơn hàng unique
            SByte status,
            SByte payStatus, // 0: ĐANG CHỜ XỬ LÝ, 1: ĐÃ THANH TOÁN, 2: ĐÃ HOÀN TIỀN
            SByte paymentMethod, // 0: tiền mặt, 1: chuyển khoản
            DateTime paymentDeadline, // thời hạn thanh toán, quá sẽ bị admin hủy đơn
            Address cusInfor)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Insert", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@inCustomerId", MySqlDbType.Int32).Value = customerId;

                    cmd.Parameters.Add("@inName", MySqlDbType.VarChar).Value = cusInfor.name;
                    cmd.Parameters.Add("@inCode", MySqlDbType.VarChar).Value = orderCode;
                    cmd.Parameters.Add("@inPhone", MySqlDbType.VarChar).Value = cusInfor.phone;
                    cmd.Parameters.Add("@inProvince", MySqlDbType.VarChar).Value = cusInfor.province;
                    cmd.Parameters.Add("@inSubDistrict", MySqlDbType.VarChar).Value = cusInfor.subdistrict;
                    cmd.Parameters.Add("@inDetail", MySqlDbType.VarChar).Value = cusInfor.detail;

                    cmd.Parameters.Add("@inNote", MySqlDbType.VarChar).Value = note;
                    cmd.Parameters.Add("@inFrom", MySqlDbType.Byte).Value = from;
                    cmd.Parameters.Add("@inPayStatus", MySqlDbType.Byte).Value = payStatus;
                    cmd.Parameters.Add("@inPaymentMethod", MySqlDbType.Byte).Value = paymentMethod;
                    cmd.Parameters.Add("@inStatus", MySqlDbType.Byte).Value = status;
                    cmd.Parameters.Add("@inPaymentDeadline", MySqlDbType.DateTime).Value = paymentDeadline;

                    object scalarResult = await cmd.ExecuteScalarAsync();
                    result.myAnythingLong = Convert.ToInt64(scalarResult);
                    result.myAnything = (int)result.myAnythingLong;
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public static async Task<MySqlResultState> AddTrackOrderAsync(int orderId, SByte status)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrderTrack_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@inOrderId", MySqlDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("@inStatus", MySqlDbType.Byte).Value = status;
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

        public static async Task<MySqlResultState> AddTrackOrderTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            int orderId, SByte status)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbOrderTrack_Insert", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@inOrderId", MySqlDbType.Int32).Value = orderId;
                    cmd.Parameters.Add("@inStatus", MySqlDbType.Byte).Value = status;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
        }

        public static async Task<MySqlResultState> AddDetailOrderAsync(int orderId, List<Cart> lsCartCookie)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrderDetail_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@inOrderId", MySqlDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("@inProductId", MySqlDbType.Int32).Value = 0;
                        cmd.Parameters.Add("@inQuantity", MySqlDbType.Int32).Value = 0;
                        cmd.Parameters.Add("@inBookCoverPrice", MySqlDbType.Int32).Value = 0;
                        cmd.Parameters.Add("@inPrice", MySqlDbType.Int32).Value = 0;

                        foreach (var cart in lsCartCookie)
                        {
                            cmd.Parameters[1].Value = cart.sanPhamId;
                            cmd.Parameters[2].Value = cart.quantity;
                            cmd.Parameters[3].Value = cart.sanPhamBasicInfo.BookCoverPrice;
                            cmd.Parameters[4].Value = cart.sanPhamBasicInfo.SalePrice;
                            await cmd.ExecuteNonQueryAsync();
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

        public static async Task<MySqlResultState> AddDetailOrderTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            int orderId, List<Cart> lsCartCookie)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbOrderDetail_Insert", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@inOrderId", MySqlDbType.Int32).Value = orderId;
                    cmd.Parameters.Add("@inProductId", MySqlDbType.Int32).Value = 0;
                    cmd.Parameters.Add("@inQuantity", MySqlDbType.Int32).Value = 0;
                    cmd.Parameters.Add("@inBookCoverPrice", MySqlDbType.Int32).Value = 0;
                    cmd.Parameters.Add("@inPrice", MySqlDbType.Int32).Value = 0;

                    foreach (var cart in lsCartCookie)
                    {
                        cmd.Parameters[1].Value = cart.sanPhamId;
                        cmd.Parameters[2].Value = cart.quantity;
                        cmd.Parameters[3].Value = cart.sanPhamBasicInfo.BookCoverPrice;
                        cmd.Parameters[4].Value = cart.sanPhamBasicInfo.SalePrice;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
        }

        public static async Task<MySqlResultState> AddPayOrderAsync(int orderId, List<OrderPay> ls)
        {
            MySqlResultState result = new MySqlResultState();
            if (ls == null || ls.Count() == 0) return result;

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrderPay_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@inOrderId", MySqlDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("@inType", MySqlDbType.Byte).Value = 0;
                        cmd.Parameters.Add("@inValue", MySqlDbType.Int32).Value = 0;
                        cmd.Parameters.Add("@inOrderSimplePromoId", MySqlDbType.Int32).Value = 0;
                        foreach (var orderPay in ls)
                        {
                            cmd.Parameters[1].Value = orderPay.type;
                            cmd.Parameters[2].Value = orderPay.value;
                            if(orderPay.type == EOrderPayType.PROMOTION)
                            {
                                cmd.Parameters[2].Value = orderPay.orderSimplePromotion.Id;
                            }    
                            await cmd.ExecuteNonQueryAsync();
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

        public static async Task<MySqlResultState> AddPayOrderTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            int orderId, List<OrderPay> ls)
        {
            MySqlResultState result = new MySqlResultState();
            if (ls == null || ls.Count() == 0) return result;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbOrderPay_Insert", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@inOrderId", MySqlDbType.Int32).Value = orderId;
                    cmd.Parameters.Add("@inType", MySqlDbType.Byte).Value = 0;
                    cmd.Parameters.Add("@inValue", MySqlDbType.Int32).Value = 0;
                    cmd.Parameters.Add("@inOrderSimplePromoId", MySqlDbType.Int32).Value = 0;
                    foreach (var orderPay in ls)
                    {
                        cmd.Parameters[1].Value = orderPay.type;
                        cmd.Parameters[2].Value = orderPay.value;
                        if (orderPay.type == EOrderPayType.PROMOTION)
                        {
                            cmd.Parameters[3].Value = orderPay.orderSimplePromotion.Id;
                        }
                        else
                        {
                            cmd.Parameters[3].Value = 0;
                        }    
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public static async Task<MySqlResultState> RefreshRealOfCartAsync(int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[1];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            return await MyMySql.ExcuteNonQueryAsync("st_tbCart_Refresh_Real_From_CustormerId", paras);
        }

        /// <summary>
        /// Lấy cart items với real=1 (sản phẩm được chọn mua)
        /// </summary>
        public static async Task<List<Cart>> GetRealCartAsync(int customerId)
        {
            List<Cart> lsCart = new List<Cart>();

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT SanPhamId, Quantity, Real FROM tbCart WHERE CustomerId = @customerId AND Real = 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
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
        public static async Task<MySqlResultState> UpdateRealCartAsync(int customerId, List<int> sanPhamIds)
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
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@customerId", MySqlDbType.Int32).Value = customerId;
                        cmd.Parameters.Add("@sanPhamId", MySqlDbType.Int32).Value = 0;

                        foreach (int sanPhamId in sanPhamIds)
                        {
                            cmd.Parameters["@sanPhamId"].Value = sanPhamId;
                            await cmd.ExecuteNonQueryAsync();
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

        public static async Task<MySqlResultState> DeleteSanPhamOnCartAsync(int customerId, int sanPhamId)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbCart_Delete_From_Customer_SanPhamId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@inCustomerId", MySqlDbType.Int32).Value = customerId;
                        cmd.Parameters.Add("@inSanPhamId", MySqlDbType.Int32).Value = sanPhamId;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        public static async Task<MySqlResultState> DeleteListCartAsync(int customerId, List<Cart> ls)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand("st_tbCart_Delete_From_Customer_SanPhamId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@inCustomerId", MySqlDbType.Int32).Value = customerId;
                    cmd.Parameters.Add("@inCustomerId", MySqlDbType.Int32).Value = 0;
                    try
                    {
                        foreach (var cart in ls)
                        {
                            cmd.Parameters[1].Value = cart.sanPhamId;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.SetResultException(ex, result);
                    }
                }
            }
            return result;
        }

        public static async Task<MySqlResultState> DeleteListCartTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            int customerId,
            List<Cart> ls)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlCommand cmd = new MySqlCommand("st_tbCart_Delete_From_Customer_SanPhamId", conn, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@inCustomerId", MySqlDbType.Int32).Value = customerId;
                cmd.Parameters.Add("@inSanPhamId", MySqlDbType.Int32).Value = 0;
                try
                {
                    foreach (var cart in ls)
                    {
                        cmd.Parameters[1].Value = cart.sanPhamId;
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

        public static async Task<MySqlResultState> UpdateSanPhamQuantityOnCartAsync(int customerId, int sanPhamId, int quantity)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inSanPhamId", sanPhamId);
            paras[2] = new MySqlParameter("@inQuantity", quantity);
            return await MyMySql.ExcuteNonQueryAsync("st_tbCart_Update_Quantity", paras);
        }

        public static async Task<MySqlResultState> UpdateSanPhamQuantityListOnCartAsync(
            int customerId,
            Dictionary<int, int> updates)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand("st_tbCart_Update_Quantity", conn))
                {
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
            }
            return result;
        }

        public static async Task<MySqlResultState> GetCartCountAsync(int customerId)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbCart_Count_From_CustormerId", conn))
                    {
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
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                }
            }
            return result;
        }

        public static async Task<MySqlResultState> GetOrderByOrderIdAsync(int orderId)
        {
            MySqlResultState result = new MySqlResultState();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    Order order = await GetOrderByOrderIdConnectOutAsync(orderId, conn);
                    result.myJson = order;
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }

            return result;
        }

        public static async Task GetOrderOtherListByOrderIdsAsync(
            List<Order> orderList,
            List<int> ids,
            MySqlConnection conn
            )
        {
            // Extract OrderIds từ orderList → comma-separated string
            string orderIdsStr = string.Empty;
            if (ids != null)
            {
                orderIdsStr = string.Join(",", ids);
            }
            else
            {
                orderIdsStr = string.Join(",", orderList.Select(o => o.id));
            }

            // Step 2: Lấy OrderDetail theo OrderIds
            List<OrderDetail> allDetails;
            using (MySqlCommand cmd = new MySqlCommand("st_tbOrderDetail_Get_By_OrderIds", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderIds", orderIdsStr);
                allDetails = await ReadOrderDetail(cmd);
            }

            foreach (var detail in allDetails)
            {
                Order order = orderList.Find(o => o.id == detail.orderId);
                if (order != null)
                {
                    order.lsOrderDetail.Add(detail);
                }
            }

            // Lấy promotions (để load vào OrderPay)
            List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsConnectOutAsync(conn);

            // Step 3: Lấy OrderPay theo OrderIds
            List<OrderPay> allPays;
            using (MySqlCommand cmd = new MySqlCommand("st_tbOrderPay_Get_By_OrderIds", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderIds", orderIdsStr);
                allPays = await ReadOrderPay(cmd);
            }

            foreach (var pay in allPays)
            {
                Order order = orderList.Find(o => o.id == pay.orderId);
                if (order != null)
                {
                    order.lsOrderPay.Add(pay);
                }
            }

            foreach (var order in orderList)
            {
                UpdateOrderSimplePromotion(promotions, order);
            }

            // Step 4: Lấy OrderTrack theo OrderIds
            List<OrderTrack> allTracks;
            using (MySqlCommand cmd = new MySqlCommand("st_tbOrderTrack_Get_By_OrderIds", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderIds", orderIdsStr);
                allTracks = await ReadOrderTrack(cmd);
            }

            foreach (var track in allTracks)
            {
                Order order = orderList.Find(o => o.id == track.orderId);
                if (order != null)
                {
                    order.lsOrderTrack.Add(track);
                }
            }
        }

        /// <summary>
        /// Lấy danh sách Order từ list OrderIds (bao gồm OrderDetail, OrderPay, OrderTrack)
        /// Approach: OrderIds → Query bulk (1 connection, 5 queries thay vì N connections, N*4 queries)
        /// </summary>
        /// <param name="ids">Danh sách OrderId</param>
        /// <returns>MySqlResultState với myJson = List&lt;Order&gt;</returns>
        public static async Task<MySqlResultState> GetOrderListByOrderIdsAsync(List<int> ids)
        {
            MySqlResultState result = new MySqlResultState();

            if (ids == null || ids.Count == 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Danh sách OrderId trống";
                result.myJson = new List<Order>();
                return result;
            }

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Convert List<int> sang comma-separated string
                    string orderIdsStr = string.Join(",", ids);

                    // Step 1: Lấy danh sách Orders theo OrderIds
                    List<Order> orderList;
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_By_OrderIds", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inOrderIds", orderIdsStr);
                        orderList = await ReadOrder(cmd);
                    }

                    if (orderList.Count == 0)
                    {
                        result.Message = "Không tìm thấy đơn hàng nào";
                        result.myJson = new List<Order>();
                        return result;
                    }

                    await GetOrderOtherListByOrderIdsAsync(orderList, ids, conn);

                    result.Message = $"Lấy được {orderList.Count} đơn hàng";
                    result.myJson = orderList;
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    MyLogger.GetInstance().Error($"GetAllOrderFromListIdAsync error: {ex}");
                    result.myJson = new List<Order>();
                }
            }

            return result;
        }

        public static async Task<MySqlResultState> SearchOrderForAnonymousAsync(string sdtNameForSearch)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> orderList = new List<Order>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_Order_From_Name_SDT_For_Anonymous", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inNameOrLastSDT", sdtNameForSearch);
                        orderList = await ReadOrder(cmd);
                    }

                    if (orderList == null || orderList.Count == 0)
                    {
                        result.Message = "Không tìm thấy đơn hàng nào";
                        result.myJson = new List<Order>();
                        return result;
                    }

                    await GetOrderOtherListByOrderIdsAsync(orderList, null, conn);
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    orderList.Clear();
                }
            }
            result.myJson = orderList;
            return result;
        }

        public static async Task<List<CommonOrder>> GetListCommonOrderAsync(int fromTo)
        {
            List<CommonOrder> ls = new List<CommonOrder>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_To_Pack_Order", conn))
                    {
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
                                commonOrder.status = OrderStatus.GetOrderStatus((EOrderStatus)(rdr.IsDBNull(statusIndex) ? -1 : rdr.GetInt32(statusIndex)));
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
        public static async Task GetCartsSanPhamBasicInfoAsync(List<Cart> ls)
        {
            if (ls == null || ls.Count() == 0)
                return;

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

        public static async Task GetOrderStatusInWarehouseToCommonOrderAsync(List<CommonOrder> ls)
        {
            if (ls == null || ls.Count == 0) return;

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand("st_tbECommerceOrder_Get_Lastest_Status_From_Code", conn))
                    {     
                        using (MySqlCommand cmdBooking = new MySqlCommand("st_tbECommerceBooking_Get_Lastest_Status_From_Code", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@inCode", "");
                            cmd.Parameters.AddWithValue("@inECommmerce", 0);

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
                                order.orderStatusInWarehouse = status;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
        }

        #region Bank Transfer Payment Support

        /// <summary>
        /// Lấy danh sách đơn hàng theo OrderStatus
        /// </summary>
        public static async Task<List<Order>> GetOrdersByStatusAsync(string orderStatus)
        {
            List<Order> ls = new List<Order>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    string query = @"
                        SELECT * FROM tbOrder
                        WHERE Status = @orderStatus
                        ORDER BY time DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderStatus", MySqlDbType.VarChar).Value = orderStatus;

                        ls = await ReadOrder(cmd);
                        // Lấy promotions (để load vào OrderPay)
                        List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsConnectOutAsync(conn);
                        // Load OrderDetail và OrderPay cho mỗi order
                        foreach (var order in ls)
                        {
                            await GetOrderDetailConnectOutAsync(order, conn);
                            await GetOrderPayConnectOutAsync(order, promotions, conn);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetOrdersByStatusAsync error: {ex.Message}");
            }
            return ls;
        }

        /// <summary>
        /// Lấy order theo ID với đầy đủ thông tin
        /// </summary>
        public static async Task<Order> GetCommonOrderFromCodeIdAsync(int orderId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    return await GetCommonOrderFromCodeIdConnectOutAsync(orderId, conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"GetCommonOrderFromCodeIdAsync error: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Update order status (standalone version)
        /// </summary>
        public static async Task<MySqlResultState> UpdateOrderStatusAsync(int orderId, string orderStatus)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tbOrder SET OrderStatus = @orderStatus WHERE Id = @orderId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("@orderStatus", MySqlDbType.VarChar).Value = orderStatus;

                        await cmd.ExecuteNonQueryAsync();
                        result.Message = "Cập nhật trạng thái đơn hàng thành công";
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        /// <summary>
        /// Update order status (transaction version)
        /// </summary>
        public static async Task<MySqlResultState> UpdateOrderStatusTransactionAsync(
            MySqlConnection conn,
            MySqlTransaction transaction,
            int orderId,
            string orderStatus)
        {
            MySqlResultState result = new MySqlResultState();
            try
            {
                string query = "UPDATE tbOrder SET OrderStatus = @orderStatus WHERE Id = @orderId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;
                    cmd.Parameters.Add("@orderStatus", MySqlDbType.VarChar).Value = orderStatus;

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Lấy danh sách đơn hàng của 1 khách hàng (bao gồm OrderDetail, OrderPay, OrderTrack)
        /// Sử dụng stored procedures: st_tbOrder_Get_List_By_CustomerId
        /// </summary>
        /// <param name="customerId">ID khách hàng</param>
        /// <returns>MySqlResultState với myJson chứa List<Order></returns>
        public static async Task<MySqlResultState> GetOrderListByCustomerIdAsync(int customerId)
        {
            MySqlResultState result = new MySqlResultState();

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Step 1: Lấy danh sách Order
                    List<Order> orderList;
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_List_By_CustomerId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                        orderList = await ReadOrder(cmd);
                    }

                    if (orderList == null ||orderList.Count == 0)
                    {
                        result.Message = "Không tìm thấy đơn hàng nào";
                        result.myJson = orderList;
                        return result;
                    }

                    // Step 2: Lấy OrderDetail, OrderPay, OrderTrack cho tất cả orders
                    await GetOrderOtherListByOrderIdsAsync(orderList, null, conn);

                    result.Message = $"Lấy được {orderList.Count} đơn hàng";
                    result.myJson = orderList;
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    MyLogger.GetInstance().Error($"GetOrderListByCustomerIdAsync error: {ex}");
                }
            }

            return result;
        }

        /// <summary>
        /// Lấy 1 Order theo OrderCode (bao gồm OrderDetail, OrderPay, OrderTrack)
        /// Approach: OrderCode → OrderId → Details/Pays/Tracks
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng (OrderCode)</param>
        /// <returns>MySqlResultState với myJson = Order (null nếu không tìm thấy)</returns>
        public static async Task<MySqlResultState> GetOrderByOrderCodeAsync(string orderCode)
        {
            MySqlResultState result = new MySqlResultState();

            if (string.IsNullOrWhiteSpace(orderCode))
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "OrderCode không hợp lệ";
                return result;
            }

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Step 1: Lấy Order theo OrderCode → có OrderId
                    List<Order> orderList;
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_By_OrderCode", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inOrderCode", orderCode);
                        orderList = await ReadOrder(cmd);
                    }

                    if (orderList == null || orderList.Count == 0)
                    {
                        result.Message = $"Không tìm thấy đơn hàng với mã: {orderCode}";
                        result.myJson = null;
                        return result;
                    }

                    Order order = orderList[0];
                    int orderId = order.id;

                    // Lấy promotions (để load vào OrderPay)
                    List<OrderSimplePromotion> promotions = await OrderSimplePromotionMySql.GetActivePromotionsConnectOutAsync(conn);

                    await GetOrderTrackConnectOutAsync(order, conn);
                    await GetOrderPayConnectOutAsync(order, promotions, conn);
                    await GetOrderDetailConnectOutAsync(order, conn);

                    result.Message = "Lấy thông tin đơn hàng thành công";
                    result.myJson = order;
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    MyLogger.GetInstance().Error($"GetOrderByOrderCodeAsync error: {ex}");
                }
            }

            return result;
        }

        private static void UpdateOrderSimplePromotion(List<OrderSimplePromotion> promotions, Order order)
        {
            foreach (var pay in order.lsOrderPay)
            {
                if (pay.type == EOrderPayType.PROMOTION)
                {
                    OrderSimplePromotion promotion = promotions.Find(p => p.Id == pay.orderSimplePromoId);
                    if (promotion != null)
                    {
                        pay.orderSimplePromotion = promotion;
                    }
                }
            }
        }

        /// <summary>
        /// Lấy danh sách Order theo list OrderCodes (bao gồm OrderDetail, OrderPay, OrderTrack)
        /// Approach: OrderCodes → List&lt;OrderId&gt; → Details/Pays/Tracks
        /// </summary>
        /// <param name="orderCodes">Danh sách mã đơn hàng</param>
        /// <returns>MySqlResultState với myJson = List&lt;Order&gt;</returns>
        public static async Task<MySqlResultState> GetOrderListByOrderCodesAsync(List<string> orderCodes)
        {
            MySqlResultState result = new MySqlResultState();

            if (orderCodes == null || orderCodes.Count == 0)
            {
                result.State = EMySqlResultState.ERROR;
                result.Message = "Danh sách OrderCode trống";
                result.myJson = new List<Order>();
                return result;
            }

            // Convert List<string> sang comma-separated string
            string orderCodesStr = string.Join(",", orderCodes);

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Step 1: Lấy danh sách Orders theo OrderCodes → có OrderIds
                    List<Order> orderList;
                    using (MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_List_By_OrderCodes", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inOrderCodes", orderCodesStr);
                        orderList = await ReadOrder(cmd);
                    }

                    if (orderList == null || orderList.Count == 0)
                    {
                        result.Message = "Không tìm thấy đơn hàng nào";
                        result.myJson = new List<Order>();
                        return result;
                    }

                    await GetOrderOtherListByOrderIdsAsync(orderList, null, conn);

                    result.Message = $"Lấy được {orderList.Count} đơn hàng";
                    result.myJson = orderList;
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    MyLogger.GetInstance().Error($"GetOrderListByOrderCodesAsync error: {ex}");
                }
            }

            return result;
        }

        /// <summary>
        /// Lấy TẤT CẢ đơn hàng (Admin only) với filter theo date range
        /// </summary>
        public static async Task<MySqlResultState> GetAllOrdersAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            MySqlResultState result = new MySqlResultState();

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();

                    // Step 1: Build query với date filter
                    string query = @"SELECT * FROM tbOrder WHERE 1=1";

                    if (fromDate.HasValue)
                    {
                        query += " AND Time >= @fromDate";
                    }

                    if (toDate.HasValue)
                    {
                        query += " AND Time <= @toDate";
                    }

                    query += " ORDER BY Time DESC";

                    List<Order> orderList;
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (fromDate.HasValue)
                        {
                            cmd.Parameters.Add("@fromDate", MySqlDbType.DateTime).Value = fromDate.Value;
                        }

                        if (toDate.HasValue)
                        {
                            cmd.Parameters.Add("@toDate", MySqlDbType.DateTime).Value = toDate.Value;
                        }

                        orderList = await ReadOrder(cmd);
                    }

                    if (orderList == null || orderList.Count == 0)
                    {
                        result.Message = "Không có đơn hàng nào";
                        result.myJson = new List<Order>();
                        return result;
                    }

                    // Step 2: Lấy OrderDetail, OrderPay, OrderTrack cho tất cả orders
                    await GetOrderOtherListByOrderIdsAsync(orderList, null, conn);

                    result.Message = $"Lấy được {orderList.Count} đơn hàng";
                    result.myJson = orderList;
                }
                catch (Exception ex)
                {
                    Common.SetResultException(ex, result);
                    MyLogger.GetInstance().Error($"GetAllOrdersAsync error: {ex}");
                }
            }

            return result;
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng (OrderStatus) bằng enum int
        /// </summary>
        public static async Task<MySqlResultState> UpdateOrderStatusByIdAsync(int orderId, int status)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tbOrder SET Status = @status WHERE Id = @orderId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("@status", MySqlDbType.Byte).Value = status;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            result.State = EMySqlResultState.ERROR;
                            result.Message = "Không tìm thấy đơn hàng";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán (OrderPayStatus) bằng enum int
        /// </summary>
        public static async Task<MySqlResultState> UpdatePaymentStatusByIdAsync(int orderId, int paymentStatus)
        {
            MySqlResultState result = new MySqlResultState();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE tbOrder SET PayStatus = @paymentStatus WHERE Id = @orderId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@orderId", MySqlDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("@paymentStatus", MySqlDbType.Byte).Value = paymentStatus;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            result.State = EMySqlResultState.ERROR;
                            result.Message = "Không tìm thấy đơn hàng";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return result;
        }

    }
}
