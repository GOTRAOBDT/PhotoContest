namespace PhotoContest.App.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using Data.Contracts;
    using Microsoft.AspNet.Identity;

    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    public class BaseController : Controller
    {
        private IPhotoContestData data;

        public BaseController(IPhotoContestData data)
        {
            this.data = data;
            if (this.CheckMaintanceLog())
            {
                this.MaintainContests();
            }
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

        private bool CheckMaintanceLog()
        {
            var lastLog = this.Data.MaintanceLogs.All()
                .OrderByDescending(l => l.CreatedAt)
                .FirstOrDefault();
            if (lastLog == null)
            {
                return true;
            }

            if (DateTime.Now.Date > lastLog.CreatedAt.Date)
            {
                return true;
            }

            return false;
        }

        private void MaintainContests()
        {
            var contestsForClosing = this.Data.Contests.All()
                .Where(c => (c.Status == ContestStatus.Active || c.Status == ContestStatus.Inactive)
                && DbFunctions.TruncateTime(DateTime.Now) > DbFunctions.TruncateTime(c.EndDate))
                .ToList();

            foreach (var contest in contestsForClosing)
            {
                contest.Status = ContestStatus.Finished;
            }

            var newMaintanceLog = new MaintanceLog { CreatedAt = DateTime.Now };
            this.Data.MaintanceLogs.Add(newMaintanceLog);

            this.Data.SaveChanges();
        }

        private int GetUnreadNotificationsCount()
        {
            var userId = this.User.Identity.GetUserId();
            if (userId != null)
            {
                return this.data.Notifications.All()
                .Count(n => n.RecipientId == userId && n.IsRead == false);
            }

            return 0;
        }
    }
}