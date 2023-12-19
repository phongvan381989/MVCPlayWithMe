using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.Customer;
using MVCPlayWithMe.Models.Order;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
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
    }
}