namespace PhotoContest.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper.QueryableExtensions;

    using Common;

    using Data.Contracts;

    using Microsoft.AspNet.Identity;

    using Models.Notification;

    using PagedList;
    using System.Net.Http;
    using System.Net;

    [Authorize]
    public class NotificationsController : BaseController
    {
        public NotificationsController(IPhotoContestData data)
            : base(data)
        {
        }

        public ActionResult Index(int? page)
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            var notifications = user.Notifications
                .AsQueryable()
                .OrderByDescending(n => n.CreatedOn)
                .ProjectTo<NotificationViewModel>()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);


            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAsRead(int id)
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var notification = this.Data.Notifications.Find(id);
            string loggedUserId = this.User.Identity.GetUserId();

            if (notification == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.NotificationNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (loggedUserId != notification.RecipientId)
            {
                throw new ArgumentException(Messages.NotOwnerOfNotification);
            }

            notification.IsRead = true;
            this.Data.SaveChanges();

            return Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAllAsRead()
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var loggedUserId = this.User.Identity.GetUserId();

            this.Data.Notifications.All()
                .Where(n => n.RecipientId == loggedUserId && n.IsRead == false)
                .ToList().ForEach(n => n.IsRead = true);

            this.Data.SaveChanges();

            return Content(string.Empty);
        }
    }
}