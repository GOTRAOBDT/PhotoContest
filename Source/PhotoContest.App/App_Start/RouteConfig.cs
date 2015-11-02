namespace PhotoContest.App
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                "ContestDetails",
                "Contests/{id}",
                new { controller = "Contests", action = "GetContestById" },
                new { id = @"\d+" },
                namespaces: new[] { "PhotoContest.App.Controllers" });

            routes.MapRoute(
                "Manage",
                "{controller}/{id}/{action}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { id = @"\d+" },
                namespaces: new[] { "PhotoContest.App.Controllers" });

            routes.MapRoute(
                "Users",
                "Users/{username}",
                new { controller = "Users", action = "Index" },
                null,
                namespaces: new[] { "PhotoContest.App.Controllers" });

            routes.MapRoute(
                "Search",
                "Search/{keyword}",
                new { controller = "Search", action = "Index" },
                null,
                namespaces: new[] { "PhotoContest.App.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PhotoContest.App.Controllers" });
        }
    }
}
