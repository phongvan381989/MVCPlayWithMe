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
    public class PublisherController : BasicController
    {
        public PublisherMySql sqler;
        public PublisherController() : base()
        {
            sqler = new PublisherMySql();
        }

        [HttpPost]
        public string CreatePublisher(string name, float discount, string detail)
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

            result = sqler.CreateNewPublisher(name, discount, detail);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string DeletePublisher(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;

            result = sqler.DeletePublisher(id);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string UpdatePublisher(int id, string name, float discount, string detail)
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = null;

            result = sqler.UpdatePublisher(id, name, discount, detail);
            return JsonConvert.SerializeObject(result);
        }

        //public string LoadPublisher()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    List<Publisher> ls = sqler.GetListPublisher();
        //    if (ls != null && ls.Count() > 0)
        //    {
        //        sb.Append(@"<tr>
        //                    <th> Tên Nhà Phát Hành </th>
        //                    <th> Thông Tin </th>
        //                  </tr>");
        //        foreach (var pub in ls)
        //        {
        //            sb.Append(@"<tr>");
        //            sb.Append("<td>" + pub.name + @"</td >");
        //            sb.Append("<td>" + pub.detail + @"</td >");
        //            sb.Append(@"</ tr>");
        //        }
        //    }

        //    return sb.ToString();
        //}

        public ActionResult Create()
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListPublisher();
            return View();
        }

        public ActionResult UpdateDelete(int id)
        {
            if (AuthentAdministrator() == null)
            {
                return AuthenticationFail();
            }
            Publisher publisher = sqler.GetPublisher(id);
            if (publisher != null)
            {
                ViewData["publisherName"] = publisher.name;
                ViewData["publisherDiscount"] = publisher.discount;
                ViewData["publisherDetail"] = publisher.detail;
            } 
            return View();
        }

        [HttpPost]
        public string GetListPublisher()
        {
            if (AuthentAdministrator() == null)
            {
                return JsonConvert.SerializeObject(new List<Publisher>());
            }
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            conn.Open();
            List<Publisher> ls = sqler.GetListPublisherConnectOut(conn);
            conn.Close();
            return JsonConvert.SerializeObject(ls);
        }
    }
}