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

        public string CreateCombo(string name)
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

            result = sqler.CreateNewCombo(name);
            return JsonConvert.SerializeObject(result);
        }

        public string DeleteCombo(string name)
        {
            MySqlResultState result = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                result = new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ.");
                return JsonConvert.SerializeObject(result);
            }

            result = sqler.DeleteCombo(name);
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
            List<Combo> ls = sqler.GetListCombo();
            return JsonConvert.SerializeObject(ls);
        }

        public ActionResult Delete()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListCombo();
            return View();
        }
    }
}