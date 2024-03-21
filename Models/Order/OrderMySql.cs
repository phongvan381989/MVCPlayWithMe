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
                    Item item = itemModelsqler.GetItemFromModelIdWithReadyConn(cart.id, conn);
                    if (item != null)
                    {
                        itemModelsqler.ConvertItemToCartCookie(item, cart);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
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
                while (rdr.Read())
                {
                    Cart cart = new Cart();
                    cart.id = MyMySql.GetInt32(rdr, "ModelId");
                    cart.q = MyMySql.GetInt32(rdr, "Quantity");
                    cart.real = MyMySql.GetInt32(rdr, "Real");
                    ls.Add(cart);
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
        public int AddOrder(int customerId, string note,
            Address cusInfor)
        {
            int id = -1;
            if (cusInfor == null)
                return id;

            MySqlParameter[] paras = new MySqlParameter[8];

            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inName", cusInfor.name);
            paras[2] = new MySqlParameter("@inPhone", cusInfor.phone);
            paras[3] = new MySqlParameter("@inProvince", cusInfor.province);
            paras[4] = new MySqlParameter("@inDistrict", cusInfor.district);
            paras[5] = new MySqlParameter("@inSubDistrict", cusInfor.subdistrict);
            paras[6] = new MySqlParameter("@inDetail", cusInfor.detail);
            paras[7] = new MySqlParameter("@inNote", note);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("st_tbOrder_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);

                MySqlDataReader rdr = cmd.ExecuteReader();
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

            paras[0] = new MySqlParameter("@inOrderId", 0);
            paras[1] = new MySqlParameter("@inModelId", 0);
            paras[2] = new MySqlParameter("@inQuantity", 0);
            paras[3] = new MySqlParameter("@inBookCoverPrice", 0);
            paras[4] = new MySqlParameter("@inPrice", 0);
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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
            paras[1] = new MySqlParameter("@inModelId", 0);
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
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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

                if (rdr != null)
                    rdr.Close();

            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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

                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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

        public MySqlResultState SearchOrderChangePage(int customerId, int statusOrder, int start, int offset)
        {
            MySqlResultState result = new MySqlResultState();
            List<Order> ls = new List<Order>();

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                // Lấy thông tin từ bảng tbOrder
                {
                    MySqlCommand cmd = new MySqlCommand("st_tbOrder_Search", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inCustomerId", customerId);
                    cmd.Parameters.AddWithValue("@inStatus", statusOrder);
                    cmd.Parameters.AddWithValue("@inStart", start);
                    cmd.Parameters.AddWithValue("@inOffset", offset);

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Order order = new Order();
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

                        ls.Add(order);
                    }

                    if (rdr != null)
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
                        while (rdr.Read())
                        {
                            orderIdTemp = MyMySql.GetInt32(rdr, "OrderId");
                            if (orderIdTemp < ls[index].id)
                                continue;
                            indexTemp = GetIndex(ls, index, count, orderIdTemp);
                            if (indexTemp == -1)
                                continue;

                            index = indexTemp;

                            OrderTrack track = new OrderTrack();
                            track.id = MyMySql.GetInt32(rdr, "Id");
                            track.orderId = orderIdTemp;
                            track.status = (EOrderStatus)MyMySql.GetInt32(rdr, "Status");
                            track.time = MyMySql.GetDateTime(rdr, "Time");
                            track.SetStrStatus();
                            ls[index].lsOrderTrack.Add(track);
                        }

                        if (rdr != null)
                            rdr.Close();
                    }

                    // Lấy thông tin từ bảng tbPayOrder
                    {
                        MySqlCommand cmd = new MySqlCommand("st_tbPayOrder_Search", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                        index = 0;
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            orderIdTemp = MyMySql.GetInt32(rdr, "OrderId");
                            if (orderIdTemp < ls[index].id)
                                continue;
                            indexTemp = GetIndex(ls, index, count, orderIdTemp);
                            if (indexTemp == -1)
                                continue;

                            index = indexTemp;

                            OrderPay pay = new OrderPay();
                            pay.id = MyMySql.GetInt32(rdr, "Id");
                            pay.orderId = orderIdTemp;
                            pay.promotionOrderId = MyMySql.GetInt32(rdr, "PromotionOrderId");
                            pay.type = (EPayType)MyMySql.GetInt32(rdr, "Type");
                            pay.value = MyMySql.GetInt32(rdr, "Value");
                            pay.SetStrType();
                            ls[index].lsOrderPay.Add(pay);
                        }

                        if (rdr != null)
                            rdr.Close();
                    }

                    // Lấy thông tin từ bảng tbDetailOrde
                    {
                        MySqlCommand cmd = new MySqlCommand("st_tbDetailOrder_Search", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inCustomerId", customerId);

                        index = 0;
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            orderIdTemp = MyMySql.GetInt32(rdr, "OrderId");
                            if (orderIdTemp < ls[index].id)
                                continue;
                            indexTemp = GetIndex(ls, index, count, orderIdTemp);
                            if (indexTemp == -1)
                                continue;

                            index = indexTemp;

                            OrderDetail detail = new OrderDetail();
                            detail.id = MyMySql.GetInt32(rdr, "Id");
                            detail.orderId = orderIdTemp;
                            detail.itemId = MyMySql.GetInt32(rdr, "ItemId");
                            detail.itemName = MyMySql.GetString(rdr, "ItemName");
                            detail.modelId = MyMySql.GetInt32(rdr, "ModelId");
                            detail.modelName = MyMySql.GetString(rdr, "ModelName");
                            detail.quantity = MyMySql.GetInt32(rdr, "Quantity");
                            detail.bookCoverPrice = MyMySql.GetInt32(rdr, "BookCoverPrice");
                            detail.price = MyMySql.GetInt32(rdr, "Price");
                            detail.SetImageSrc();

                            ls[index].lsOrderDetail.Add(detail);
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
                result.State = EMySqlResultState.EXCEPTION;
                result.Message = errMessage;
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

                while (rdr.Read())
                {
                    id = (long)MyMySql.GetInt32(rdr, "OrderId");
                    if (ls.Count == 0 || ls[ls.Count - 1].id != id)
                    {
                        ls.Add(new CommonOrder());
                    }
                    commonOrder = ls[ls.Count - 1];
                    commonOrder.ecommerceName = Common.ePlayWithMe;
                    if (commonOrder.id != id )
                    {
                        commonOrder.id = id;
                        commonOrder.code = MyMySql.GetString(rdr, "OrderCode");
                        commonOrder.created_at = MyMySql.GetDateTime(rdr, "OrderTime");
                        commonOrder.status = OrderTrack.GetString(MyMySql.GetInt32(rdr, "StatusInTrackOrder"));
                    }

                    modelId = (long)MyMySql.GetInt32(rdr, "ModelId");

                    if (commonOrder.listModelId.Count == 0 ||
                        commonOrder.listModelId[commonOrder.listModelId.Count - 1] != modelId)
                    {
                        itemId = (long)MyMySql.GetInt32(rdr, "ItemId");
                        modelQuantity = MyMySql.GetInt32(rdr, "ModelQuantity");
                        itemName = MyMySql.GetString(rdr, "ItemName");
                        modelName = MyMySql.GetString(rdr, "ModelName");
                        imgSrc = Common.GetModelImageSrc(Common.ConvertLongToInt(itemId), Common.ConvertLongToInt(modelId));

                        commonOrder.listItemId.Add(itemId);
                        commonOrder.listModelId.Add(modelId);
                        commonOrder.listItemName.Add(itemName);
                        commonOrder.listModelName.Add(modelName);
                        commonOrder.listQuantity.Add(modelQuantity);
                        commonOrder.listThumbnail.Add(imgSrc);
                    }
                }

                if (rdr != null)
                    rdr.Close();
            }
            catch (Exception ex)
            {
                errMessage = ex.ToString();
                MyLogger.GetInstance().Warn(errMessage);
                ls.Clear();
            }

            conn.Close();
            return ls;
        }

        // Lấy mapping của sản phẩm trong đơn hàng
        public void UpdateMappingToCommonOrder(CommonOrder commonOrder)
        {
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            string status = string.Empty;
            try
            {
                conn.Open();

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
    }
}