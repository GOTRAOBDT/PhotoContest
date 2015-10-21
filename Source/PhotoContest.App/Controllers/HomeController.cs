namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using Data.Contracts;

    using Models.Contest;

    using PhotoContest.App.Common;
    using PhotoContest.Models.Enumerations;

    public class HomeController : BaseController
    {
        public HomeController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Index{?sortBy=popularity&filterBy=active}
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Index(string sortBy, string filterBy)
        {
            var contests = new List<SummaryContestViewModel>();
            if (sortBy == null && filterBy == null)
            {
                contests = this.Data.Contests.All()
                    .Where(c => c.Status == ContestStatus.Active)
                    .OrderByDescending(c => c.Pictures.Count)
                    .ThenByDescending(c => c.Votes.Count)
                    .Select(SummaryContestViewModel.Create)
                    .ToList();
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
                            .Select(SummaryContestViewModel.Create)
                            .ToList();
                        break;
                    case "coming-soon":
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Inactive)
                            .OrderByDescending(c => c.Pictures.Count)
                            .ThenByDescending(c => c.Votes.Count)
                            .Select(SummaryContestViewModel.Create)
                            .ToList();
                        break;
                    default:
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Active)
                            .OrderByDescending(c => c.Pictures.Count)
                            .ThenByDescending(c => c.Votes.Count)
                            .Select(SummaryContestViewModel.Create)
                            .ToList();
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
                            .Select(SummaryContestViewModel.Create)
                            .ToList();
                        break;
                    default:
                        contests = this.Data.Contests.All()
                            .Where(c => c.Status == ContestStatus.Active)
                            .OrderByDescending(c => c.Pictures.Count)
                            .ThenByDescending(c => c.Votes.Count)
                            .Select(SummaryContestViewModel.Create)
                            .ToList();
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
                                .Select(SummaryContestViewModel.Create)
                                .ToList();
                            break;
                        default:
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Active)
                                .OrderByDescending(c => c.Pictures.Count)
                                .ThenByDescending(c => c.Votes.Count)
                                .Select(SummaryContestViewModel.Create)
                                .ToList();
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
                                .Select(SummaryContestViewModel.Create)
                                .ToList();
                            break;
                        default:
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Finished)
                                .OrderByDescending(c => c.Pictures.Count)
                                .ThenByDescending(c => c.Votes.Count)
                                .Select(SummaryContestViewModel.Create)
                                .ToList();
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
                                .Select(SummaryContestViewModel.Create)
                                .ToList();
                            break;
                        default:
                            contests = this.Data.Contests.All()
                                .Where(c => c.Status == ContestStatus.Inactive)
                                .OrderByDescending(c => c.Pictures.Count)
                                .ThenByDescending(c => c.Votes.Count)
                                .Select(SummaryContestViewModel.Create)
                                .ToList();
                            break;
                    }
                }
            }

            return this.View(contests);
        }
    }
}