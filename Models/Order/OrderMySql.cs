using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.ItemModel;
using MVCPlayWithMe.Models.Order;
using MVCPlayWithMe.OpenPlatform.Model;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static MVCPlayWithMe.General.Common;

namespace MVCPlayWithMe.Models.Order
{
    public class OrderMySql : BasicMySql
    {
        // Từ model id, lấy được thông tin của model
        public void GetCart(List<Cart> ls)
        {
            if (ls == null || ls.Count() == 0)
                return;
            ItemModelMySql itemModelsqler = new ItemModelMySql();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();
                foreach (var cart in ls)
                {
                    Item item = itemModelsqler.GetItemFromModelIdConnectOut(cart.id, conn);
                    if (item != null)
                    {
                        itemModelsqler.ConvertItemToCartCookie(item, cart);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
        }

        /// <summary>
        /// Lấy danh sách cart
        /// </summary>
        /// <param name="customerId"></param>
        public List<Cart> GetListCart(int customerId)
        {
            List<Cart> ls = new List<Cart>();
            MySqlParameter[] paras = new MySqlParameter[1];

            paras[0] = new MySqlParameter("@inCustomerId", customerId);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCart_Get_From_CustormerId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                MySqlDataReader rdr = cmd.ExecuteReader();

                int modelIdIndex = rdr.GetOrdinal("ModelId");
                int quantityIndex = rdr.GetOrdinal("Quantity");
                int realIndex = rdr.GetOrdinal("Real");

                while (rdr.Read())
                {
                    Cart cart = new Cart();
                    cart.id = rdr.GetInt32(modelIdIndex);
                    cart.q = rdr.GetInt32(quantityIndex);
                    cart.real = rdr.GetInt32(realIndex);
                    ls.Add(cart);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            return ls;
        }

        /// <summary>
        /// status: Lấy theo shopee status:0: UNPAID, 1:  READY_TO_SHIP,
        /// 2: PROCESSED, // Đây là trạng thái sau khi in đơn 3:  SHIPPED, 4:  COMPLETED,
        /// 5: IN_CANCEL, 6:  CANCELLED, 7:  INVOICE_PENDING, 8: ALL
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="ls"></param>
        /// <param name="cusInfor"></param>
        public int AddOrder(int customerId, string note, int isNotWeb,
            Address cusInfor)
        {
            int id = -1;
            //if (cusInfor == null)
            //    return id;

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbOrder_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                if (cusInfor != null)
                {
                    cmd.Parameters.AddWithValue("@inName", cusInfor.name);
                    cmd.Parameters.AddWithValue("@inPhone", cusInfor.phone);
                    cmd.Parameters.AddWithValue("@inProvince", cusInfor.province);
                    cmd.Parameters.AddWithValue("@inDistrict", cusInfor.district);
                    cmd.Parameters.AddWithValue("@inSubDistrict", cusInfor.subdistrict);
                    cmd.Parameters.AddWithValue("@inDetail", cusInfor.detail);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@inName", null);
                    cmd.Parameters.AddWithValue("@inPhone", null);
                    cmd.Parameters.AddWithValue("@inProvince", null);
                    cmd.Parameters.AddWithValue("@inDistrict", null);
                    cmd.Parameters.AddWithValue("@inSubDistrict", null);
                    cmd.Parameters.AddWithValue("@inDetail", null);
                }
                cmd.Parameters.AddWithValue("@inNote", note);
                cmd.Parameters.AddWithValue("@inIsNotWeb", isNotWeb);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    id = MyMySql.GetInt32(rdr, "LastId");
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();

            return id;
        }

        public void AddTrackOrder(int orderId, int status)
        {
            MySqlParameter[] paras = new MySqlParameter[4];

            paras[0] = new MySqlParameter("@inOrderId", orderId);
            paras[1] = new MySqlParameter("@inStatus", status);

            MyMySql.AddOutParameters(paras);

            MyMySql.ExcuteNonQueryStoreProceduce("st_tbTrackOrder_Insert", paras);
        }

        public void AddDetailOrder(int orderId, List<Cart> lsCartCookie)
        {
            MySqlParameter[] paras = new MySqlParameter[7];

            paras[0] = new MySqlParameter("@inOrderId", (object)0);
            paras[1] = new MySqlParameter("@inModelId", (object)0);
            paras[2] = new MySqlParameter("@inQuantity", (object)0);
            paras[3] = new MySqlParameter("@inBookCoverPrice", (object)0);
            paras[4] = new MySqlParameter("@inPrice", (object)0);
            MyMySql.AddOutParameters(paras);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

               foreach(var cart in lsCartCookie)
               {
                    paras[0].Value = orderId;
                    paras[1].Value = cart.id;
                    paras[2].Value = cart.q;
                    paras[3].Value = cart.bookCoverPrice;
                    paras[4].Value = cart.price;
                   cmd.ExecuteNonQuery();
               }

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
        }

        public MySqlResultState AddPayOrder(int orderId, List<OrderPay> ls)
        {
            MySqlResultState result = new MySqlResultState();
            if (ls == null || ls.Count() == 0)
                return result;

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inOrderId", orderId);
                cmd.Parameters.AddWithValue("@inType", 0);
                cmd.Parameters.AddWithValue("@inValue", 0);
                foreach (var orderPay in ls)
                {
                    cmd.Parameters[1].Value = orderPay.type;
                    cmd.Parameters[2].Value = orderPay.value;

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();

            return result;
        }

        /// <summary>
        /// Set Real = 0
        /// </summary>
        /// <param name="customerId"></param>
        public MySqlResultState RefreshRealOfCart(int customerId)
        {
            MySqlParameter[] paras = new MySqlParameter[1];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            return MyMySql.ExcuteNonQuery("st_tbCart_Refresh_Real_From_CustormerId", paras);
        }

        /// <summary>
        /// Xóa sản phẩm ra khỏi giỏ hàng
        /// </summary>
        /// <param name="modelId"></param>
        public MySqlResultState DeleteOneCart(int customerId, int modelId)
        {
            MySqlParameter[] paras = new MySqlParameter[2];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inModelId", modelId);
            return MyMySql.ExcuteNonQuery("st_tbCart_Delete_From_Customer_ModelId", paras);
        }

        public MySqlResultState DeleteListCart(int customerId, List<Cart> ls)
        {
            MySqlParameter[] paras = new MySqlParameter[2];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inModelId", (object)0);
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);

            MySqlCommand cmd = new MySqlCommand("st_tbCart_Delete_From_Customer_ModelId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(paras);
            try
            {
                conn.Open();
                foreach (var cart in ls)
                {
                    cmd.Parameters[1].Value = cart.id;
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();
            return result;
        }

        /// <summary>
        /// Cập nhật số lượng cart
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="modelId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public MySqlResultState UpdateCartQuantity(int customerId, int modelId, int quantity)
        {
            MySqlParameter[] paras = new MySqlParameter[3];
            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inModelId", modelId);
            paras[2] = new MySqlParameter("@inQuantity", quantity);
            return MyMySql.ExcuteNonQuery("st_tbCart_Update_Quantity", paras);
        }

        /// <summary>
        /// Lấy số lượng sản phẩm trong giỏ hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public MySqlResultState GetCartCount(int customerId)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbCart_Count_From_CustormerId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.myAnything = MyMySql.GetInt32(rdr, "Count");
                }

                rdr.Close();

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }
            conn.Close();
            return result;
        }

        /// <summary>
        /// Đếm số lượng đơn hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="statusOrder"></param>
        /// <returns></returns>
        public MySqlResultState SearchOrderCount(int customerId, int statusOrder)
        {
            MySqlResultState result = new MySqlResultState();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbOrder_Search_Count", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                cmd.Parameters.AddWithValue("@inStatus", statusOrder);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.myAnything = MyMySql.GetInt32(rdr, "CountRecord");
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            conn.Close();
            return result;
        }

        /// <summary>
        /// Lấy index trong list có orderId bằng tham số truyền vào
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="fromIndex">index bắt đầu tìm kiếm</param>
        /// <param name="count">số phần tử của danh sách</param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private int GetIndex(List<Order> ls, int fromIndex,int count, int orderId)
        {
            int index = -1;// nếu không tìm thấy
            for(int i = fromIndex; i < count; i++)
            {
                if(ls[i].id == orderId)
                {
                    index = i;
                }
            }
            return index;
        }

        //public MySqlResultState SearchOrderChangePage(int customerId, int statusOrder, int start, int offset)
        //{
        //    MySqlResultState result = new MySqlResultState();
        //    List<Order> ls = new List<Order>();

        //    MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
        //    try
        //    {
        //        conn.Open();

        //        // Lấy thông tin từ bảng tbOrder
        //        {
        //            MySqlCommand cmd = new MySqlCommand("st_tbOrder_Search", conn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@inCustomerId", customerId);
        //            cmd.Parameters.AddWithValue("@inStatus", statusOrder);
        //            cmd.Parameters.AddWithValue("@inStart", start);
        //            cmd.Parameters.AddWithValue("@inOffset", offset);

        //            MySqlDataReader rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                Order order = new Order();
        //                ReadOrder(order, rdr);

        //                ls.Add(order);
        //            }

        //            if (rdr != null)
        //                rdr.Close();
        //        }

        //        int index = 0;
        //        int indexTemp = 0;
        //        int orderIdTemp = 0;
        //        int count = ls.Count();
        //        if (count > 0)
        //        {
        //            // Lấy thông tin từ bảng tbTrackOrder
        //            {
        //                MySqlCommand cmd = new MySqlCommand("st_tbTrackOrder_Search", conn);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@inCustomerId", customerId);

        //                index = 0;
        //                MySqlDataReader rdr = cmd.ExecuteReader();
        //                while (rdr.Read())
        //                {
        //                    orderIdTemp = MyMySql.GetInt32(rdr, "OrderId");
        //                    if (orderIdTemp < ls[index].id)
        //                        continue;
        //                    indexTemp = GetIndex(ls, index, count, orderIdTemp);
        //                    if (indexTemp == -1)
        //                        continue;

        //                    index = indexTemp;

        //                    OrderTrack track = new OrderTrack();
        //                    ReadOrderTrack(track, rdr);
        //                    ls[index].lsOrderTrack.Add(track);
        //                }

        //                if (rdr != null)
        //                    rdr.Close();
        //            }

        //            // Lấy thông tin từ bảng tbPayOrder
        //            {
        //                MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Search", conn);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@inCustomerId", customerId);

        //                index = 0;
        //                MySqlDataReader rdr = cmd.ExecuteReader();
        //                while (rdr.Read())
        //                {
        //                    orderIdTemp = MyMySql.GetInt32(rdr, "OrderId");
        //                    if (orderIdTemp < ls[index].id)
        //                        continue;
        //                    indexTemp = GetIndex(ls, index, count, orderIdTemp);
        //                    if (indexTemp == -1)
        //                        continue;

        //                    index = indexTemp;

        //                    OrderPay pay = new OrderPay();
        //                    ReadOrderPay(pay, rdr);
        //                    ls[index].lsOrderPay.Add(pay);
        //                }

        //                if (rdr != null)
        //                    rdr.Close();
        //            }

        //            // Lấy thông tin từ bảng tbDetailOrde
        //            {
        //                MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Search", conn);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@inCustomerId", customerId);

        //                index = 0;
        //                MySqlDataReader rdr = cmd.ExecuteReader();
        //                while (rdr.Read())
        //                {
        //                    orderIdTemp = MyMySql.GetInt32(rdr, "OrderId");
        //                    if (orderIdTemp < ls[index].id)
        //                        continue;
        //                    indexTemp = GetIndex(ls, index, count, orderIdTemp);
        //                    if (indexTemp == -1)
        //                        continue;

        //                    index = indexTemp;

        //                    OrderDetail detail = new OrderDetail();
        //                    ReadOrderDetail(detail, rdr);

        //                    ls[index].lsOrderDetail.Add(detail);
        //                }

        //                if (rdr != null)
        //                    rdr.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.SetResultException(ex, result);
        //        ls.Clear();
        //    }

        //    conn.Close();
        //    result.myJson = ls;//JsonConvert.SerializeObject(ls);
        //    return result;
        //}

        // Lấy tất cả đơn
        public MySqlResultState GetAllOrder (int customerId)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();
                // Truy vấn đều order by tăng dần theo OrderId, TrackOrderId/PayOrderId/DetailOrderId
                // Lấy thông tin từ bảng tbOrder
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_All_Order", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    int idIndex = rdr.GetOrdinal("Id");
                    int customerIdIndex = rdr.GetOrdinal("CustomerId");
                    int nameIndex = rdr.GetOrdinal("Name");
                    int phoneIndex = rdr.GetOrdinal("Phone");
                    int provinceIndex = rdr.GetOrdinal("Province");
                    int districtIndex = rdr.GetOrdinal("District");
                    int subdistrictIndex = rdr.GetOrdinal("SubDistrict");
                    int detailIndex = rdr.GetOrdinal("Detail");
                    int noteIndex = rdr.GetOrdinal("Note");
                    int timeIndex = rdr.GetOrdinal("Time");

                    while (rdr.Read())
                    {
                        Order order = new Order();
                        order.id = rdr.GetInt32(idIndex);
                        order.customerId = rdr.GetInt32(customerIdIndex);
                        order.address.name = rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex);
                        order.address.phone = rdr.IsDBNull(phoneIndex) ? string.Empty : rdr.GetString(phoneIndex);
                        order.address.province = rdr.IsDBNull(provinceIndex) ? string.Empty : rdr.GetString(provinceIndex);
                        order.address.district = rdr.IsDBNull(districtIndex) ? string.Empty : rdr.GetString(districtIndex);
                        order.address.subdistrict = rdr.IsDBNull(subdistrictIndex) ? string.Empty : rdr.GetString(subdistrictIndex);
                        order.address.detail = rdr.IsDBNull(detailIndex) ? string.Empty : rdr.GetString(detailIndex);
                        order.note = rdr.IsDBNull(noteIndex) ? string.Empty : rdr.GetString(noteIndex);
                        order.time = rdr.IsDBNull(timeIndex) ? DateTime.MinValue : rdr.GetDateTime(timeIndex);

                        ls.Add(order);
                    }

                    rdr.Close();
                }

                int index = 0;
                int indexTemp = 0;
                int orderIdTemp = 0;
                int count = ls.Count();
                if (count > 0)
                {
                    // Lấy thông tin từ bảng tbTrackOrder
                    {
                        MySqlCommand cmd = new MySqlCommand("st_tbTrackOrder_Search", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                        index = 0;
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        int orderIdIndex = rdr.GetOrdinal("OrderId");
                        while (rdr.Read())
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

                        rdr.Close();
                    }

                    // Lấy thông tin từ bảng tbPayOrder
                    {
                        MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Search", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                        index = 0;
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        int orderIdIndex = rdr.GetOrdinal("OrderId");
                        while (rdr.Read())
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

                        rdr.Close();
                    }

                    // Lấy thông tin từ bảng tbDetailOrder
                    {
                        MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Search", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                        index = 0;
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        int orderIdIndex = rdr.GetOrdinal("OrderId");
                        while (rdr.Read())
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

                        rdr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                ls.Clear();
            }

            conn.Close();
            result.myJson = ls;
            return result;
        }

        private void ReadOrder(Order order, MySqlDataReader rdr)
        {
            order.id = MyMySql.GetInt32(rdr, "Id");
            order.customerId = MyMySql.GetInt32(rdr, "CustomerId");
            order.address.name = MyMySql.GetString(rdr, "Name");
            order.address.phone = MyMySql.GetString(rdr, "Phone");
            order.address.province = MyMySql.GetString(rdr, "Province");
            order.address.district = MyMySql.GetString(rdr, "District");
            order.address.subdistrict = MyMySql.GetString(rdr, "SubDistrict");
            order.address.detail = MyMySql.GetString(rdr, "Detail");
            order.note = MyMySql.GetString(rdr, "Note");
            order.time = MyMySql.GetDateTime(rdr, "Time");
        }

        private Order GetOrderConnectOut(int id, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_Order", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", id);

            Order order = null;
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                order = new Order();
                ReadOrder(order, rdr);
            }

            rdr.Close();
            return order;
        }

        private void ReadOrderTrack(OrderTrack track, MySqlDataReader rdr)
        {
            track.id = MyMySql.GetInt32(rdr, "Id");
            track.orderId = MyMySql.GetInt32(rdr, "OrderId");
            track.status = (EOrderStatus)MyMySql.GetInt32(rdr, "Status");
            track.time = MyMySql.GetDateTime(rdr, "Time");
            track.SetStrStatus();
        }

        private OrderTrack GetOrderTrackConnectOut(Order order, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbTrackOrder_Get_From_OrderId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", order.id);
            OrderTrack track = null;
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                track = new OrderTrack();
                ReadOrderTrack(track, rdr);
                order.lsOrderTrack.Add(track);
            }

            rdr.Close();

            return track;
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

        private OrderPay GetOrderPayConnectOut(Order order, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Get_From_OrderId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", order.id);

            OrderPay pay = null;
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                pay = new OrderPay();
                ReadOrderPay(pay, rdr);
                order.lsOrderPay.Add(pay);
            }

            rdr.Close();

            return pay;
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

        private OrderDetail GetOrderDetailConnectOut(Order order, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Get_From_OrderId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inOrderId", order.id);

            OrderDetail detail = null;
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                detail = new OrderDetail();
                ReadOrderDetail(detail, rdr);
                order.lsOrderDetail.Add(detail);
            }

            rdr.Close();

            return detail;
        }

        // Lay thong tin 1 don hang tu id
        public MySqlResultState GetOrderFromId(int orderId)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();
            Order order = null;

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Lấy thông tin từ bảng tbOrder
                order = GetOrderConnectOut(orderId, conn);

                if (order == null)
                {
                    return result;
                }

                // Lấy thông tin từ bảng tbTrackOrder
                GetOrderTrackConnectOut(order, conn);

                // Lấy thông tin từ bảng tbPayOrder
                GetOrderPayConnectOut(order, conn);

                // Lấy thông tin từ bảng tbDetailOrder
                GetOrderDetailConnectOut(order, conn);

                ls.Add(order);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                order = null;
            }

            conn.Close();
            result.myJson = ls;
            return result;
        }

        // Lay thong tin 1 don hang tu code
        public Order GetOrderFromIdConnectOut(int orderId, MySqlConnection conn)
        {
            Order order = null;

            try
            {
                // Lấy thông tin từ bảng tbOrder
                order = GetOrderConnectOut(orderId, conn);

                if (order == null)
                    return order;

                // Lấy thông tin từ bảng tbTrackOrder
                GetOrderTrackConnectOut(order, conn);

                // Lấy thông tin từ bảng tbPayOrder
                GetOrderPayConnectOut(order, conn);

                // Lấy thông tin từ bảng tbDetailOrder
                GetOrderDetailConnectOut(order, conn);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                order = null;
            }

            return order;
        }

        // Lay tat ca don hang tu danh sach mã đơn hàng
        public MySqlResultState GetAllOrderFromListId(List<int> ids)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();
                foreach(var id in ids)
                {
                    Order order = GetOrderFromIdConnectOut(id, conn);
                    if(order != null)
                    {
                        ls.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                ls.Clear();
            }

            conn.Close();
            result.myJson = ls;
            return result;
        }

        // Lấy tất cả đơn
        public MySqlResultState SearchOrderForAnonymous(string sdtNameForSearch)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();
                // Truy vấn đều order by tăng dần theo OrderId, TrackOrderId/PayOrderId/DetailOrderId
                // Lấy thông tin từ bảng tbOrder
                MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_Order_From_Name_SDT_For_Anonymous", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inNameOrLastSDT", sdtNameForSearch);

                MySqlDataReader rdr = cmd.ExecuteReader();

                int idIndex = rdr.GetOrdinal("Id");
                int customerIdIndex = rdr.GetOrdinal("CustomerId");
                int nameIndex = rdr.GetOrdinal("Name");
                int phoneIndex = rdr.GetOrdinal("Phone");
                int provinceIndex = rdr.GetOrdinal("Province");
                int districtIndex = rdr.GetOrdinal("District");
                int subdistrictIndex = rdr.GetOrdinal("SubDistrict");
                int detailIndex = rdr.GetOrdinal("Detail");
                int noteIndex = rdr.GetOrdinal("Note");
                int timeIndex = rdr.GetOrdinal("Time");

                while (rdr.Read())
                {
                    Order order = new Order();
                    order.id = rdr.GetInt32(idIndex);
                    order.customerId = rdr.GetInt32(customerIdIndex);
                    order.address.name = rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex);
                    order.address.phone = rdr.IsDBNull(phoneIndex) ? string.Empty : rdr.GetString(phoneIndex);
                    order.address.province = rdr.IsDBNull(provinceIndex) ? string.Empty : rdr.GetString(provinceIndex);
                    order.address.district = rdr.IsDBNull(districtIndex) ? string.Empty : rdr.GetString(districtIndex);
                    order.address.subdistrict = rdr.IsDBNull(subdistrictIndex) ? string.Empty : rdr.GetString(subdistrictIndex);
                    order.address.detail = rdr.IsDBNull(detailIndex) ? string.Empty : rdr.GetString(detailIndex);
                    order.note = rdr.IsDBNull(noteIndex) ? string.Empty : rdr.GetString(noteIndex);
                    order.time = rdr.IsDBNull(timeIndex) ? DateTime.MinValue : rdr.GetDateTime(timeIndex);

                    ls.Add(order);
                }
                rdr.Close();

                foreach( Order order in ls)
                {
                    // Lấy thông tin từ bảng tbTrackOrder
                    GetOrderTrackConnectOut(order, conn);

                    // Lấy thông tin từ bảng tbPayOrder
                    GetOrderPayConnectOut(order, conn);

                    // Lấy thông tin từ bảng tbDetailOrder
                    GetOrderDetailConnectOut(order, conn);

                    // Ẩn số điện thoại, chỉ hiện 4 số cuối
                    order.address.phone = "******" + order.address.phone.Substring(6);
                    // Ẩn địa chị chi tiết và phường
                    order.address.detail = "";
                    order.address.subdistrict = "";
                }

            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
                ls.Clear();
            }

            conn.Close();
            result.myJson = ls;//JsonConvert.SerializeObject(ls);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromTo"> 0: 1 ngày, 1: 7 ngày, 2: 30 ngày</param>
        /// <returns></returns>
        public List<CommonOrder> GetListCommonOrder(int fromTo)
        {
            List<CommonOrder> ls = new List<CommonOrder>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbOrder_Get_To_Pack_Order", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inFromTo", fromTo);

                MySqlDataReader rdr = cmd.ExecuteReader();
                CommonOrder commonOrder = null;
                long id = 0;
                long itemId = 0;
                long modelId = 0;
                int modelQuantity = 0; // số lượng model trog đơn
                string itemName = "";
                string modelName = "";
                string imgSrc = "";

                int orderIdIndex = rdr.GetOrdinal("OrderId");
                int orderCodeIndex = rdr.GetOrdinal("OrderCode");
                int orderTimeIndex = rdr.GetOrdinal("OrderTime");
                int statusIndex = rdr.GetOrdinal("StatusInTrackOrder");
                int modelIdIndex = rdr.GetOrdinal("ModelId");
                int itemIdIndex = rdr.GetOrdinal("ItemId");
                int modelQuantityIndex = rdr.GetOrdinal("ModelQuantity");
                int itemNameIndex = rdr.GetOrdinal("ItemName");
                int modelNameIndex = rdr.GetOrdinal("ModelName");

                while (rdr.Read())
                {
                    id = (long)rdr.GetInt32(orderIdIndex);
                    if (ls.Count == 0 || ls[ls.Count - 1].id != id)
                    {
                        ls.Add(new CommonOrder());
                    }
                    commonOrder = ls[ls.Count - 1];
                    commonOrder.ecommerceName = Common.ePlayWithMe;
                    if (commonOrder.id != id )
                    {
                        commonOrder.id = id;
                        commonOrder.code = rdr.IsDBNull(orderCodeIndex) ? string.Empty : rdr.GetString(orderCodeIndex);
                        commonOrder.created_at = rdr.IsDBNull(orderTimeIndex) ? DateTime.MinValue : rdr.GetDateTime(orderTimeIndex);
                        commonOrder.status = OrderTrack.GetString(rdr.IsDBNull(statusIndex) ? -1 : rdr.GetInt32(statusIndex));
                    }

                    modelId = rdr.IsDBNull(modelIdIndex) ? -1L : (long)rdr.GetInt32(modelIdIndex);

                    if (commonOrder.listModelId.Count == 0 ||
                        commonOrder.listModelId[commonOrder.listModelId.Count - 1] != modelId)
                    {
                        itemId = (long)rdr.GetInt32(itemIdIndex);
                        modelQuantity = rdr.IsDBNull(modelQuantityIndex) ? -1 : rdr.GetInt32(modelQuantityIndex);
                        itemName = rdr.IsDBNull(itemNameIndex) ? string.Empty : rdr.GetString(itemNameIndex);
                        modelName = rdr.IsDBNull(modelNameIndex) ? string.Empty : rdr.GetString(modelNameIndex);
                        imgSrc = Common.GetModelImageSrc(Common.ConvertLongToInt(itemId), Common.ConvertLongToInt(modelId));

                        commonOrder.listItemId.Add(itemId);
                        commonOrder.listModelId.Add(modelId);
                        commonOrder.listItemName.Add(itemName);
                        commonOrder.listModelName.Add(modelName);
                        commonOrder.listQuantity.Add(modelQuantity);
                        commonOrder.listThumbnail.Add(imgSrc);
                    }
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
                ls.Clear();
            }

            conn.Close();
            return ls;
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public void PlayWithMeGetMappingOfCommonOrderConnectOut(CommonOrder commonOrder, MySqlConnection conn)
        {
            string status = string.Empty;
            try
            {
                MySqlCommand cmd = new MySqlCommand("st_tbMapping_Get_From_ModelId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inModelId", int.MinValue);

                int quantity = 0;
                Product pro = null;
                MySqlDataReader rdr;
                {
                    for (int i = 0; i < commonOrder.listModelId.Count; i++)
                    {
                        cmd.Parameters[0].Value = Common.ConvertLongToInt(commonOrder.listModelId[i]);

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

        // Từ đơn hàng, cập nhật trạng thái sản phẩm trên sàn vì có sản phẩm trên sàn được bật bán
        // trở lại
        public void UpdateStatusNormalOfTMDTItemConnectOut(CommonOrder order, MySqlConnection conn)
        {
            string status = string.Empty;
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
                        cmd.ExecuteNonQuery();
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
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
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
                    if (order.ecommerceName == Common.eTiki)
                        cmd.Parameters[1].Value = (int)EECommerceType.TIKI;
                    else if (order.ecommerceName == Common.eShopee)
                        cmd.Parameters[1].Value = (int)EECommerceType.SHOPEE;
                    else if (order.ecommerceName == Common.ePlayWithMe)
                        cmd.Parameters[1].Value = (int)EECommerceType.PLAY_WITH_ME;

                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        status = Common.OrderStatusArray[MyMySql.GetInt32(rdr, "Status")];
                    }
                    order.orderStatusInWarehoue = status;
                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
        }
    }
}