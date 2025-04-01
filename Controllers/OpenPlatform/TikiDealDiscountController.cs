using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.DealDiscount;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers.OpenPlatform
{
    public class TikiDealDiscountController : BasicController
    {
        public TikiDealDiscountMySql sqler;

        public TikiDealDiscountController()
        {
            sqler = new TikiDealDiscountMySql();
        }

        // GET: TikiDealDiscount
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetDealDiscountOfSku(string sku)
        {
            List<DealCreatedResponseDetail> listDeal = new List<DealCreatedResponseDetail>();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(listDeal);
            }

            DealAction.SearchDeal(sku, listDeal);

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            // Inssert nếu chưa tồn tại
            sqler.InsertTikiDealDiscountConnectOut(listDeal, conn);
            conn.Close();

            return JsonConvert.SerializeObject(listDeal);
        }

        [HttpPost]
        public string SaveDealDiscountOfAllSku()
        {
            MySqlResultState result = new MySqlResultState();
            // Lấy danh sách sku đang bật bán và lấy chương trình giảm giá lần lượt
            List<string> skuList = new List<string>();

            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                conn.Open();

                string query = "SELECT Sku FROM tbtikiitem WHERE Status = 0";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skuList.Add(reader.GetString("Sku"));
                        }
                    }
                }

                foreach(var sku in skuList)
                {

                }
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}