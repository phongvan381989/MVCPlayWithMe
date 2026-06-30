using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCPlayWithMe
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //   name: "voibenho",
            //   url: "",
            //   defaults: new { controller = "Home", action = "Index" }
            //);

            // Route cho trang chi tiết sản phẩm: /item/{slug}-{id}
            routes.MapRoute(
                name: "ItemDetail",
                url: "Item/{slugId}",
                defaults: new { controller = "Home", action = "Item" }
            );

            //// Redirect URL cũ /Home/Item/{id} sang URL mới /item/{slug}-{id}
            //routes.MapRoute(
            //    name: "ItemDetailOld",
            //    url: "Home/Item/{id}",
            //    defaults: new { controller = "Home", action = "ItemRedirect" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{keyword}",
                defaults: new { controller = "Home", action = "Search", keyword = UrlParameter.Optional}
                //defaults: new { controller = "AllProducts", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
