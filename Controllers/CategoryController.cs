using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
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
            return View();
        }

        // GET:  Category
        public ActionResult Create()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            ViewDataGetListCategory();
            return View();
        }

        public string CreateCategory(string name)
        {
            //if (AuthentAdministrator() == null)
            //{
            //    return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.OK, MySqlResultState.authenFailMessage));
            //}

            MySqlResultState result = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                result = new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ.");
                return JsonConvert.SerializeObject(result);
            }

            result = sqler.CreateNewCategory(name);
            return JsonConvert.SerializeObject(result);
        }

        public string DeleteCategory(string name)
        {
            MySqlResultState result = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                result = new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ.");
                return JsonConvert.SerializeObject(result);
            }

            result = sqler.DeleteCategory(name);
            return JsonConvert.SerializeObject(result);
        }

        public string LoadCategory()
        {
            StringBuilder sb = new StringBuilder();
            List<Category> ls = sqler.GetListCategory();
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

        public ActionResult Delete()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            ViewDataGetListCategory();
            return View();
        }
    }
}