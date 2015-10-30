namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using App.Controllers;

    using AutoMapper.QueryableExtensions;

    using Common;
    using Data.Contracts;
    
    using Models.Contest;

    using PagedList;

    [Authorize(Roles = ("Administrator"))]
    public class ManageController : BaseController
    {
        private const int DefaultPageSize = 7;

        public ManageController(IPhotoContestData data)
            : base(data)
        {
            
        }

        // GET: Administration/Manage
        public ActionResult Index()
        {
            return this.View();
        }

        // GET: Administration/Manage/Users
        public ActionResult Users()
        {
            this.ViewBag.Manage = "Users";
            return this.View();
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