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
        public ActionResult Index(int? page, string sortBy, string filterBy)
        {
            IPagedList<SummaryContestViewModel> contests = null;
            if (sortBy == null && filterBy == null)
            {
                contests = this.Data.Contests.All()
                    .Where(c => c.Status == ContestStatus.Active)
                    .OrderByDescending(c => c.Pictures.Count)
                    .ThenByDescending(c => c.Votes.Count)
                    .ProjectTo<SummaryContestViewModel>()
                    .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
            }

            if (sortBy == null && filterBy != null)
            {
                switch (filterBy)
                {
                    case "finished":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Finished)
                            .OrderByDescending(c => c.Pictures.Count)
                            .ThenByDescending(c => c.Votes.Count)
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                    case "coming-soon":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Inactive)
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
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                }
            }

            if (sortBy != null && filterBy == null)
            {
                switch (sortBy)
                {
                    case "newest":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Active)
                            .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                    default:
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Active)
                            .OrderByDescending(c => c.Pictures.Count)
                            .ThenByDescending(c => c.Votes.Count)
                            .ProjectTo<SummaryContestViewModel>()
                            .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                        break;
                }
            }

            if (sortBy != null && filterBy != null)
            {
                if (filterBy == "active")
                {
                    switch (sortBy)
                    {
                        case "newest":
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Active)
                                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                                .ProjectTo<SummaryContestViewModel>()
                                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                            break;
                        default:
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Active)
                                .OrderByDescending(c => c.Pictures.Count)
                                .ThenByDescending(c => c.Votes.Count)
                                .ProjectTo<SummaryContestViewModel>()
                                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                            break;
                    }
                }

                if (filterBy == "finished")
                {
                    switch (sortBy)
                    {
                        case "newest":
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Finished)
                                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                                .ProjectTo<SummaryContestViewModel>()
                                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                            break;
                        default:
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Finished)
                                .OrderByDescending(c => c.Pictures.Count)
                                .ThenByDescending(c => c.Votes.Count)
                                .ProjectTo<SummaryContestViewModel>()
                                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                            break;
                    }
                }

                if (filterBy == "coming-soon")
                {
                    switch (sortBy)
                    {
                        case "newest":
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Inactive)
                                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                                .ProjectTo<SummaryContestViewModel>()
                                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                            break;
                        default:
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Inactive)
                                .OrderByDescending(c => c.Pictures.Count)
                                .ThenByDescending(c => c.Votes.Count)
                                .ProjectTo<SummaryContestViewModel>()
                                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
                            break;
                    }
                }
            }

            return this.View(contests);
        }
    }
}