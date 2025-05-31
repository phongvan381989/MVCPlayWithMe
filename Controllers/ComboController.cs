using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.OpenPlatform.Model;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class ComboController : BasicController
    {
        public ComboMySql sqler;
        public ComboController() : base()
        {
            sqler = new ComboMySql();
        }

        // GET: Combo
        public ActionResult Index()
        {
            return View();
        }

        // GET: Combo
        public ActionResult Create()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListCombo();
            return View();
        }

        public string CreateCombo(string name, string code)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                result = new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ.");
                return JsonConvert.SerializeObject(result);
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                result = new MySqlResultState(EMySqlResultState.INVALID, "Mã không hợp lệ.");
                return JsonConvert.SerializeObject(result);
            }

            result = sqler.CreateNewCombo(name, code);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string DeleteCombo(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;

            result = sqler.DeleteCombo(id);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string UpdateCombo(int id, string name, string code)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if(code == null)
            {
                code = string.Empty;
            }

            MySqlResultState result = sqler.UpdateCombo(id, name, code);
            return JsonConvert.SerializeObject(result);
        }

        public string LoadCombo()
        {
            StringBuilder sb = new StringBuilder();
            List<Combo> ls = sqler.GetListCombo();
            if (ls != null && ls.Count() > 0)
            {
                sb.Append(@"<tr>
                            <th> Tên Nhà Phát Hành </th>
                          </tr>");
                foreach (var pub in ls)
                {
                    sb.Append(@"<tr>");
                    sb.Append("<td>" + pub.name + @"</td >");

                    sb.Append(@"</ tr>");
                }
            }

            return sb.ToString();
        }

        [HttpPost]
        public string GetListCombo()
        {
            List<Combo> ls = new List<Combo>();
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(ls);
            }

            ls = sqler.GetListCombo();
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public string GetCombo(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            Combo combo = sqler.GetCombo(id);
            return JsonConvert.SerializeObject(combo);
        }

        public ActionResult UpdateDelete(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        [HttpGet]
        public ActionResult MappingOfCombo(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        private List<CommonItem> ShopeeGetListMappingOfCombo(int id, MySqlConnection conn, ProductController productController)
        {
            // Danh sách sản phẩm Shopee
            List<CommonItem> shopeeList = sqler.ShopeeGetListMappingOfCombo(id, conn);
            //productController.ShopeeGetStatusImageSrcQuantitySellable(shopeeList);
            return shopeeList;
        }

        private List<CommonItem> TikiGetListMappingOfCombo(int id, MySqlConnection conn, ProductController productController)
        {
            // Danh sách sản phẩm Tiki
            List<CommonItem> tikiList = sqler.TikiGetListMappingOfCombo(id, conn);
            //productController.TikiGetStatusImageSrcQuantitySellable(tikiList);
            return tikiList;
        }

        [HttpPost]
        public string GetListMappingOfCombo(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            List<CommonItem> ls = new List<CommonItem>();
            ProductController productController = new ProductController();
            try
            {
                conn.Open();
                List<CommonItem> shopeeList = ShopeeGetListMappingOfCombo(id, conn, productController);
                List<CommonItem> tikiList = TikiGetListMappingOfCombo(id, conn, productController);

                ls.AddRange(tikiList);
                ls.AddRange(shopeeList);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            conn.Close();
            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(ls);
        }
    }
}