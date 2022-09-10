using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{resource}.info/{*pathInfo}");
            routes.IgnoreRoute("{resource}.sample/{*pathInfo}");
            routes.IgnoreRoute("{resource}.sampleasync");
            routes.IgnoreRoute("abc.test");
            routes.IgnoreRoute("def.test");
            routes.IgnoreRoute("xyz.test");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
