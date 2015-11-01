namespace PhotoContest.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;

    using Data.Contracts;

    public class BaseController : Controller
    {
        private IPhotoContestData data;

        public BaseController(IPhotoContestData data)
        {
            this.data = data;
            //this.ViewData["UnreadCount"] = this.GetUnreadNotificationsCount();
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

        private int GetUnreadNotificationsCount()
        {
            var userId = this.User.Identity.GetUserId();
            if (userId != null)
            {
                return this.data.Notifications.All()
                .Where(n => n.RecipientId == userId && n.IsRead == false)
                .Count();
            }

            return 0;
        }
    }
}