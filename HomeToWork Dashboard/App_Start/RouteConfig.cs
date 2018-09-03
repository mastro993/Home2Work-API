using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HomeToWork_Dashboard
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Action",
                url: "{controller}/{action}",
                defaults: new {controller = "Dashboard", action = "Index"}
            );

            routes.MapRoute(
                name: "Id",
                url: "{controller}/{id}",
                defaults: new {controller = "Dashboard", id = UrlParameter.Optional}
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Dashboard", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}