using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.DealDiscount;
using MVCPlayWithMe.OpenPlatform.Model;
using MVCPlayWithMe.OpenPlatform.Model.TikiAPI.DealDiscount;
using MVCPlayWithMe.OpenPlatform.Model.TikiApp.Deal;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();

                    // Lấy danh sách sku đang bật bán và lấy chương trình giảm giá lần lượt
                    List<string> skuList = sqler.GetListSkuOfActiveItemConnectOut(conn);

                    List<DealCreatedResponseDetail> listDeal = new List<DealCreatedResponseDetail>();
                    foreach (var sku in skuList)
                    {
                        listDeal.Clear();
                        DealAction.SearchDeal(sku, listDeal);

                        // Inssert nếu chưa tồn tại
                        sqler.InsertTikiDealDiscountConnectOut(listDeal, conn);
                        Thread.Sleep(300);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SetResultException(ex, result);
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string GetItemsNoDealDiscountRunning()
        {
            List<SimpleTikiProduct> simpleTikiProducts = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    conn.Open();
                    simpleTikiProducts = sqler.GetItemsNoDealDiscountRunning(conn);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            return JsonConvert.SerializeObject(simpleTikiProducts);
        }
    }
}