using MVCPlayWithMe.General;
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
        /// status: Lấy theo shopee status:0: UNPAID, 1:  READY_TO_SHIP,
        /// 2: PROCESSED, // Đây là trạng thái sau khi in đơn 3:  SHIPPED, 4:  COMPLETED,
        /// 5: IN_CANCEL, 6:  CANCELLED, 7:  INVOICE_PENDING, 8: ALL
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="shipFee"></param>
        /// <param name="status"></param>
        /// <param name="ls"></param>
        /// <param name="cusInfor"></param>
        public int AddOrder(int customerId, int shipFee, int status, string note,
            CustomerInforCookie cusInfor)
        {
            int id = -1;
            if (cusInfor == null)
                return id;

            MySqlParameter[] paras = new MySqlParameter[10];

            paras[0] = new MySqlParameter("@inCustomerId", customerId);
            paras[1] = new MySqlParameter("@inShipFee", shipFee);
            paras[2] = new MySqlParameter("@inStatus", status);
            paras[3] = new MySqlParameter("@inName", cusInfor.name);
            paras[4] = new MySqlParameter("@inPhone", cusInfor.phone);
            paras[5] = new MySqlParameter("@inProvince", cusInfor.province);
            paras[6] = new MySqlParameter("@inDistrict", cusInfor.district);
            paras[7] = new MySqlParameter("@inSubDistrict", cusInfor.subdistrict);
            paras[8] = new MySqlParameter("@inDetail", cusInfor.detail);
            paras[9] = new MySqlParameter("@inNote", note);

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
    }
}