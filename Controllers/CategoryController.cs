using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MySql.Data.MySqlClient;
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
    public class CategoryController : BasicController
    {
        public CategoryMySql sqler;
        public CategoryController() : base()
        {
            sqler = new CategoryMySql();
        }

        // GET:  Category
        public async Task<ActionResult> Index()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        // GET:  Category
        public async Task<ActionResult> Create()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListCategory();
            return View();
        }

        [HttpPost]
        public async Task<string> CreateCategory(string name)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await sqler.CreateNewCategoryAsync(name);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> DeleteCategory(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await sqler.DeleteCategoryAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        //public string LoadCategory()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    List<Category> ls = sqler.GetListCategory();
        //    if (ls != null && ls.Count() > 0)
        //    {
        //        sb.Append(@"<tr>
        //                    <th> Tên Nhà Phát Hành </th>
        //                  </tr>");
        //        foreach (var pub in ls)
        //        {
        //            sb.Append(@"<tr>");
        //            sb.Append("<td>" + pub.name + @"</td >");

        //            sb.Append(@"</ tr>");
        //        }
        //    }

        //    return sb.ToString();
        //}

        public async Task<ActionResult> UpdateDelete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                Category category = await sqler.GetCategoryAsync(id, conn);
                if (category != null)
                {
                    ViewData["categoryName"] = category.name;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<string> UpdateCategory(int id, string name)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await sqler.UpdateCategoryAsync(id, name);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> GetListCategory()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Category>());
            }

            List<Category> ls = await sqler.GetListCategoryAsync();
            return JsonConvert.SerializeObject(ls);
        }
    }
}