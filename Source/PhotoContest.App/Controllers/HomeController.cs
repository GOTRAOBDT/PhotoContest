namespace PhotoContest.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Common;
    using Data.Contracts;

    using Models.Contest;

    using PagedList;

    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    public class HomeController : BaseController
    {
        public HomeController(IPhotoContestData data)
            : base(data)
        {
            Mapper.CreateMap<Contest, SummaryContestViewModel>();
        }

        // GET: Index{?sortBy=popularity&filterBy=active}
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Index(int? page, string sortBy)
        {
            IPagedList<SummaryContestViewModel> contests = null;
            if (sortBy == null)
            {
                contests = this.Data.Contests.All()
                    .Where(c => c.Status == ContestStatus.Active)
                    .OrderByDescending(c => c.Pictures.Count)
                    .ThenByDescending(c => c.Votes.Count)
                    .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                    .ProjectTo<SummaryContestViewModel>()
                    .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
            }

            if (sortBy != null)
            {
                switch (sortBy)
                {
                    case "Latest":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Active)
                            .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                    case "ComingSoon":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Inactive)
                            .OrderByDescending(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                    case "EndingSoon":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Active)
                            .OrderByDescending(c => TestableDbFunctions.DiffMinutes(c.EndDate, DateTime.Now))
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                    case "Archive":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Finished)
                            .OrderByDescending(c => c.Pictures.Count)
                            .ThenByDescending(c => c.Votes.Count)
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                    default:
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Active)
                            .OrderByDescending(c => c.Pictures.Count)
                            .ThenByDescending(c => c.Votes.Count)
                            .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                }
            }
            
            this.ViewBag.sortBy = sortBy;

            return this.View(contests);
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return this.View();
        }

        [HttpGet]
        public ActionResult Rules()
        {
            return this.View();
        }
    }
}