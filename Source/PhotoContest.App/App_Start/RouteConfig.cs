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
                "Manage",
                "{controller}/{id}/{action}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "ContestPagesSortedByFilteredBy",
                url: "Home/Index/{page}/{sortBy}/{filterBy}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    page = UrlParameter.Optional,
                    sortBy = UrlParameter.Optional,
                    filterBy = UrlParameter.Optional
                });
        }
    }
}
