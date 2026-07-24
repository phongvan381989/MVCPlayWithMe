using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MVCPlayWithMe.OpenPlatform.Model;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Create()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListCombo();
            return View();
        }

        public async Task<string> CreateCombo(string name, string code)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ."));
            }

            //if (string.IsNullOrWhiteSpace(code))
            //{
            //    result = new MySqlResultState(EMySqlResultState.INVALID, "Mã không hợp lệ.");
            //    return JsonConvert.SerializeObject(result);
            //}

            MySqlResultState result = await sqler.CreateNewComboAsync(name, code);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> DeleteCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await sqler.DeleteComboAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdateCombo(int id, string name, string code)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (code == null)
            {
                code = string.Empty;
            }

            MySqlResultState result = await sqler.UpdateComboAsync(id, name, code);
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> LoadCombo()
        {
            StringBuilder sb = new StringBuilder();
            List<Combo> ls = await sqler.GetListComboAsync();
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
        public async Task<string> GetListCombo()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Combo>());
            }

            List<Combo> ls = await sqler.GetListComboAsync();
            return JsonConvert.SerializeObject(ls);
        }

        [HttpPost]
        public async Task<string> GetCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(null);
            }

            Combo combo = await sqler.GetComboAsync(id);
            return JsonConvert.SerializeObject(combo);
        }

        public async Task<ActionResult> UpdateDelete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> MappingOfCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        //private List<CommonItem> ShopeeGetListMappingOfCombo(int id, MySqlConnection conn, ProductController productController)
        //{
        //    // Danh sách sản phẩm Shopee
        //    List<CommonItem> shopeeList = sqler.ShopeeGetListMappingOfCombo(id, conn);
        //    //productController.ShopeeGetStatusImageSrcQuantitySellable(shopeeList);
        //    return shopeeList;
        //}

        //private List<CommonItem> TikiGetListMappingOfComboAsync(int id, MySqlConnection conn, ProductController productController)
        //{
        //    // Danh sách sản phẩm Tiki
        //    List<CommonItem> tikiList = TikiMySql.TikiGetListMappingOfComboAsync(id, conn);
        //    //productController.TikiGetStatusImageSrcQuantitySellable(tikiList);
        //    return tikiList;
        //}

        [HttpPost]
        public async Task <string> GetListMappingOfCombo(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<CommonItem>());
            }

            List<CommonItem> ls = new List<CommonItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
                {
                    await conn.OpenAsync();
                    List<CommonItem> tikiList = await TikiMySql.TikiGetListMappingOfComboAsync(id, conn);
                    List<CommonItem> shopeeList = await ShopeeMySql.ShopeeGetListMappingOfComboAsync(id, conn);
                    List<CommonItem> lazadaList = await LazadaMySql.LazadaGetListMappingOfComboAsync(id, conn);

                    ls.AddRange(tikiList);
                    ls.AddRange(shopeeList);
                    ls.AddRange(lazadaList);
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Warn(ex.ToString());
            }
            // Lấy danh sách sản phẩm
            return JsonConvert.SerializeObject(ls);
        }
    }
}