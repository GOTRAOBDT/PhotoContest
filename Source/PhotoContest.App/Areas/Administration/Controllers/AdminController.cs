namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Contest;
    using AutoMapper.QueryableExtensions;

    using Common;
    using Data.Contracts;

    using Models;
    using PagedList;

    [Authorize(Roles = ("Administrator"))]
    public class AdminController : BaseController
    {
        private const int DefaultPageSize = 7;

        public AdminController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Administration/Manage
        public ActionResult Index()
        {
            return this.View();
        }

        // GET: Administration/Manage/Users
        public ActionResult Users(int? page)
        {
            var usersViewModel = this.Data.Users.All()
                .OrderBy(u => u.UserName)
                .ProjectTo<UserViewModel>()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, DefaultPageSize);

            this.ViewBag.Manage = "Users";
            return this.View(usersViewModel);
        }

        // GET: Administration/Manage/Contests
        public ActionResult Contests(int? page)
        {
            var contestsViewModel = this.Data.Contests.All()
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ProjectTo<SummaryContestViewModel>()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, DefaultPageSize);

            this.ViewBag.Manage = "Contests";
            return this.View(contestsViewModel);
        }
    }
}