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
        private static Thread _backgroundThread; // Thread chạy nền
        private static bool _isRunning;          // Cờ kiểm soát vòng lặp của thread

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

            Common.client.Timeout = 120000; //120 giây

            MyMySql.connStr = ConfigurationManager.AppSettings["AdminConectMysql"];
            MyMySql.customerConnStr = ConfigurationManager.AppSettings["CustomerVBNConectMysql"];

            TikiMySql tikiMySql = new TikiMySql();
            CommonTikiAPI.tikiConfigApp = tikiMySql.GetTikiConfigApp();

            ShopeeMySql shopeeMySql = new ShopeeMySql();
            CommonShopeeAPI.shopeeAuthen = shopeeMySql.ShopeeGetAuthen();

            // Khởi tạo cờ kiểm soát
            _isRunning = true;

            // Tạo thread sống lâu dài
            _backgroundThread = new Thread(() =>
            {
                // Vòng lặp chính của thread
                while (_isRunning)
                {
                    try
                    {
                        // Công việc chạy nền
                        //MyLogger.GetInstance().Info($"Background thread running at {DateTime.Now}");
                        TikiPullEventService tikiPullEventService = new TikiPullEventService();
                        tikiPullEventService.DoWork();
                        Thread.Sleep(20 * 60 * 1000); // Tạm dừng 20 phút trước lần lặp tiếp theo
                    }
                    catch (Exception ex)
                    {
                        MyLogger.GetInstance().Info($"Error in background thread: {ex.Message}");
                    }
                }
            });

            // Đặt thread thành background thread (không chặn ứng dụng dừng)
            _backgroundThread.IsBackground = true;
            _backgroundThread.Start();
        }

        protected void Application_End()
        {
            // Hàm được gọi khi ứng dụng dừng
            MyLogger.GetInstance().Info("Application is stopping...");

            // Đặt cờ kiểm soát để dừng vòng lặp
            _isRunning = false;

            // Chờ thread hoàn tất công việc
            _backgroundThread?.Join();

            MyLogger.GetInstance().Info("Background thread stopped.");
        }

    }
}
