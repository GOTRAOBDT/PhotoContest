namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;

    using Data.Contracts;
    using Models.Notification;

    public class NotificationsController : BaseController
    {
        public NotificationsController(IPhotoContestData data)
            : base(data)
        {
        }

        public ActionResult Index()
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            var notifications = user.Notifications
                .AsQueryable()
                .OrderByDescending(n => n.CreatedOn)
                .Select(NotificationViewModel.Create)
                .ToList();

            return View(notifications);
        }
    }
}