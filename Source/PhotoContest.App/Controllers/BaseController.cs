namespace PhotoContest.App.Controllers
{
    using System;
    using System.Web.Mvc;

    using Data.Contracts;

    public class BaseController : Controller
    {
        private IPhotoContestData data;

        public BaseController(IPhotoContestData data)
        {
            this.data = data;
        }

        protected IPhotoContestData Data
        {
            get
            {
                return this.data;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            filterContext.ExceptionHandled = true;

            var result = this.View(
                "Error",
                new HandleErrorInfo(
                    exception,
                    filterContext.RouteData.Values["controller"].ToString(),
                    filterContext.RouteData.Values["action"].ToString()));

            filterContext.Result = result;
        }
    }
}