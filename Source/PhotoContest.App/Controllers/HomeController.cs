namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Data.Contracts;

    using PhotoContest.App.Models.Contest;
    using PhotoContest.Models.Enumerations;

    public class HomeController : BaseController
    {
        public HomeController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Index/{sortBy}
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Index(string sortBy)
        {
            var model = this.Data.Contests.All()
                .Select(c => new SummaryContestViewModel
                {
                    Id = c.Id,
                    IsOwner = false,
                    ParticipantsCount = c.Participants.Count,
                    PicturesCount = c.Pictures.Count,
                    VotesCount = c.Votes.Count,
                    DeadlineType = c.DeadlineType.ToString(),
                    ParticipationType = c.ParticipationType.ToString(),
                    StartDate = c.StartDate,
                    Title = c.Title,
                    VotingType = c.VotingType.ToString(),
                    Status = c.Status.ToString(),
                    Owner = c.Owner.UserName,
                    EndDate = c.EndDate
                }).ToList();

            return this.View(model);
        }
    }
}