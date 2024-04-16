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
    public class PublisherController : BasicController
    {
        public PublisherMySql sqler;
        public PublisherController() : base()
        {
            sqler = new PublisherMySql();
        }

        // GET: Publisher
        public ActionResult Index()
        {
            return View();
        }

        public string CreatePublisher(string name, string detail)
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

            result = sqler.CreateNewPublisher(name, detail);
            return JsonConvert.SerializeObject(result);
        }

        public string DeletePublisher(string name)
        {
            MySqlResultState result = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                result = new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ.");
                return JsonConvert.SerializeObject(result);
            }

            result = sqler.DeletePublisher(name);
            return JsonConvert.SerializeObject(result);
        }

        public string UpdatePublisher(string name, string detail)
        {
            MySqlResultState result = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                result = new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ.");
                return JsonConvert.SerializeObject(result);
            }

            if (Common.ParameterOfURLQueryIsNullOrEmpty(detail))
                detail = string.Empty;

            result = sqler.UpdatePublisher(name, detail);
            return JsonConvert.SerializeObject(result);
        }

        public string LoadPublisher()
        {
            StringBuilder sb = new StringBuilder();
            List<Publisher> ls = sqler.GetListPublisher();
            if (ls != null && ls.Count() > 0)
            {
                sb.Append(@"<tr>
                            <th> Tên Nhà Phát Hành </th>
                            <th> Thông Tin </th>
                          </tr>");
                foreach (var pub in ls)
                {
                    sb.Append(@"<tr>");
                    sb.Append("<td>" + pub.name + @"</td >");
                    sb.Append("<td>" + pub.detail + @"</td >");
                    sb.Append(@"</ tr>");
                }
            }

            return sb.ToString();
        }

        public ActionResult Create()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListPublisher();
            return View();
        }

        public ActionResult UpdateDelete()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListPublisher();
            return View();
        }

        [HttpPost]
        public string GetListPublisher()
        {
            return JsonConvert.SerializeObject(sqler.GetListPublisher());
        }
    }
}