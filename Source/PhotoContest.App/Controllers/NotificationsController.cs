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

    [Authorize]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAsRead(int id)
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException("Invalid operation!");
            }

            var notification = this.Data.Notifications.Find(id);
            string loggedUserId = this.User.Identity.GetUserId();

            if (notification == null)
            {
                throw new ArgumentException("Notification not found!");
            }

            if (loggedUserId != notification.RecipientId)
            {
                throw new ArgumentException("You are not the owner of this notification!");
            }

            notification.IsRead = true;
            this.Data.SaveChanges();

            return Content(string.Empty);

        }
    }
}