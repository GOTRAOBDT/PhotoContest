namespace PhotoContest.App
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Microsoft.AspNet.Identity;

    using Bookmarks.Common.Mappings;
    using Data;
    using System.Linq;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            var autoMapperConfig = new AutoMapperConfig(new List<Assembly> { Assembly.GetExecutingAssembly() });
            autoMapperConfig.Execute();
        }

        public static string GetUnreadNotificationsCount()
        {
            var userId = Thread.CurrentPrincipal.Identity.GetUserId();
            if (userId == null)
            {
                return "";
            }
            var dbContext = new PhotoContestContext();
            var count = dbContext.Notifications.Where(n => n.RecipientId == userId && n.IsRead == false).Count();
            return count.ToString();
        }
    }
}