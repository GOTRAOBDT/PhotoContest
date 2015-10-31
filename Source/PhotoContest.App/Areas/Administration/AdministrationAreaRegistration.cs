namespace PhotoContest.App.Areas.Administration
{
    using System.Web.Mvc;

    public class AdministrationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Administration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AdminManage",
                "Administration/{controller}/{id}/{action}",
                new { id = @"\d+" }, 
                namespaces: new[] { "PhotoContest.App.Areas.Administration.Controllers" });

            context.MapRoute(
                "Administration_default",
                "Administration/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PhotoContest.App.Areas.Administration.Controllers" });
        }
    }
}