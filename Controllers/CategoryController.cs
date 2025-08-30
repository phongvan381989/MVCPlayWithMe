using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
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
    public class CategoryController : BasicController
    {
        public CategoryMySql sqler;
        public CategoryController() : base()
        {
            sqler = new CategoryMySql();
        }

        // GET:  Category
        public ActionResult Index()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }

            return View();
        }

        // GET:  Category
        public ActionResult Create()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListCategory();
            return View();
        }

        [HttpPost]
        public string CreateCategory(string name)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;


            result = sqler.CreateNewCategory(name);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string DeleteCategory(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;

            result = sqler.DeleteCategory(id);
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

        public ActionResult UpdateDelete(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                conn.Open();
                Category category = sqler.GetCategory(id, conn);
                if (category != null)
                {
                    ViewData["categoryName"] = category.name;
                }
            }
            return View();
        }

        [HttpPost]
        public string UpdateCategory(int id, string name)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;

            result = sqler.UpdateCategory(id, name);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string GetListCategory()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<Category>());
            }

            List<Category> ls = sqler.GetListCategory();
            return JsonConvert.SerializeObject(ls);
        }
    }
}