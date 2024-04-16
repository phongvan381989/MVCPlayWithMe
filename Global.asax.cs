using MVCPlayWithMe.General;
using MVCPlayWithMe.OpenPlatform.API.ShopeeAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Product;
using MVCPlayWithMe.OpenPlatform.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVCPlayWithMe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Common.ProductMediaFolderPath = ConfigurationManager.AppSettings["ProductMediaFolderPath"];
            Common.ItemMediaFolderPath = ConfigurationManager.AppSettings["ItemMediaFolderPath"];
            Common.MediaFolderPath = ConfigurationManager.AppSettings["MediaFolderPath"];
            Common.ThongTinBaoMatPath = ConfigurationManager.AppSettings["ThongTinBaoMatPath"];
            Common.TemporaryImageShopeeMediaFolderPath = 
                ConfigurationManager.AppSettings["TemporaryImageShopeeMediaFolderPath"];
            Common.TemporaryImageTikiMediaFolderPath = 
                ConfigurationManager.AppSettings["TemporaryImageTikiMediaFolderPath"];

            MyMySql.connStr = ConfigurationManager.AppSettings["ConectMysql"];
            TikiMySql tikiMySql = new TikiMySql();
            CommonTikiAPI.tikiConfigApp = tikiMySql.GetTikiConfigApp();

            ShopeeMySql shopeeMySql = new ShopeeMySql();
            CommonShopeeAPI.shopeeAuthen = shopeeMySql.ShopeeGetAuthen();
        }
    }
}
