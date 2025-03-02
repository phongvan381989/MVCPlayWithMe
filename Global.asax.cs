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
using System.Threading;
using System.Threading.Tasks;
using MVCPlayWithMe.OpenPlatform.API.TikiAPI.Event;

namespace MVCPlayWithMe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static private Timer _timer;

        protected void Application_Start()
        {
            MyLogger.GetInstance().Info("Application is starting...");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Common.ProductMediaFolderPath = ConfigurationManager.AppSettings["ProductMediaFolderPath"];
            Common.absoluteProductMediaFolderPath =
                System.Web.HttpContext.Current.Server.MapPath(Common.ProductMediaFolderPath);

            Common.ItemMediaFolderPath = ConfigurationManager.AppSettings["ItemMediaFolderPath"];
            Common.absoluteItemMediaFolderPath =
                System.Web.HttpContext.Current.Server.MapPath(Common.ItemMediaFolderPath);

            Common.MediaFolderPath = ConfigurationManager.AppSettings["MediaFolderPath"];
            Common.TemporaryImageShopeeMediaFolderPath = 
                ConfigurationManager.AppSettings["TemporaryImageShopeeMediaFolderPath"];
            Common.TemporaryImageTikiMediaFolderPath = 
                ConfigurationManager.AppSettings["TemporaryImageTikiMediaFolderPath"];

            MyMySql.connStr = ConfigurationManager.AppSettings["AdminConectMysql"];
            MyMySql.customerConnStr = ConfigurationManager.AppSettings["CustomerVBNConectMysql"];

            TikiMySql tikiMySql = new TikiMySql();
            CommonTikiAPI.tikiConfigApp = tikiMySql.GetTikiConfigApp();

            ShopeeMySql shopeeMySql = new ShopeeMySql();
            CommonShopeeAPI.shopeeAuthen = shopeeMySql.ShopeeGetAuthen();

            _timer = new System.Threading.Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(20));
        }

        protected void Application_End()
        {
            // Hàm được gọi khi ứng dụng dừng
            MyLogger.GetInstance().Info("Application is stopping...");
            // Dừng và hủy Timer
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            MyLogger.GetInstance().Info($"Function of Timer executed at {DateTime.Now}");
            TikiPullEventService tikiPullEventService = new TikiPullEventService();
            tikiPullEventService.DoWork();
        }
    }
}
