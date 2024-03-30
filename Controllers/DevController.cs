using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class DevController : Controller
    {
        // GET: Dev
        public ActionResult Index()
        {
            return View();
        }
    }
}