namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Data.Contracts;

    public class NotificationsController : BaseController
    {
        public NotificationsController(IPhotoContestData data)
            : base(data)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}