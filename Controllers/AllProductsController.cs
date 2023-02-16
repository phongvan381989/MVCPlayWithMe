using MVCPlayWithMe.General;
using MVCPlayWithMe.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPlayWithMe.Controllers
{
    public class AllProductsController : Controller
    {
        static IList<Models.ProductForSelector> listPro = new List<Models.ProductForSelector>
        {
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổiCombo 10 cuốn ehon cho bé từ 0 đến 6 tuổiCombo 10 cuốn ehon cho bé từ 0 đến 6 tuổiCombo 10 cuốn ehon cho bé từ 0 đến 6 tuổiCombo 10 cuốn ehon cho bé từ 0 đến 6 tuổiCombo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000),
            new Models.ProductForSelector("Combo 10 cuốn ehon cho bé từ 0 đến 6 tuổi",
                "Sach1.jpg", 100, 50000, 20000, 30000)
        };

        // GET: AllProduts
        public ActionResult Index()
        {
            return View(listPro);
        }
    }
}