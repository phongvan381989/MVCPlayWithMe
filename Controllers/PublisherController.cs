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
    public class PublisherController : BasicController
    {
        public PublisherMySql sqler;
        public PublisherController() : base()
        {
            sqler = new PublisherMySql();
        }

        [HttpPost]
        public async Task<string> CreatePublisher(string name, float discount, string detail, string tikiCertificate)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.INVALID, "Tên không hợp lệ."));
            }

            MySqlResultState result = await sqler.CreateNewPublisherAsync(name, discount, detail);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> DeletePublisher(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await sqler.DeletePublisherAsync(id);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task<string> UpdatePublisher(int id, string name, float discount, string detail)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new MySqlResultState(EMySqlResultState.AUTHEN_FAIL, MySqlResultState.authenFailMessage));
            }

            MySqlResultState result = await sqler.UpdatePublisherAsync(id, name, discount, detail);
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

        public async Task<ActionResult> Create()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            //ViewDataGetListPublisher();
            return View();
        }

        public async Task<ActionResult> UpdateDelete(int id)
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return AuthenticationFail();
            }
            Publisher publisher = await sqler.GetPublisherAsync(id);
            if (publisher != null)
            {
                ViewData["publisherName"] = publisher.name;
                ViewData["publisherDiscount"] = publisher.discount;
                ViewData["publisherDetail"] = publisher.detail;
            }
            return View();
        }

        [HttpPost]
        public async Task<string> GetListPublisher()
        {
            if ((await AuthentAdministratorAsync()) == null)
            {
                return JsonConvert.SerializeObject(new List<Publisher>());
            }
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                await conn.OpenAsync();
                List<Publisher> ls = await sqler.GetListPublisherConnectOutAsync(conn);
                return JsonConvert.SerializeObject(ls);
            }
        }
    }
}