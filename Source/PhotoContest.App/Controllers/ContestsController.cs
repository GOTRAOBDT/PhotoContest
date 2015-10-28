namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;
    using Models.Contest;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Data.Contracts;
    using PhotoContest.App.Models.Account;
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;
    


    [Authorize]
    public class ContestsController : BaseController
    {
        public ContestsController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Contests/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateContestBindingModel model)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var contest = new Contest()
            {
                Title = model.Title,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                OwnerId = loggedUserId,
                VotingType = model.VotingType,
                ParticipationType = model.ParticipationType,
                DeadlineType = model.DeadlineType,
                Thumbnail = model.Thumbnail,
                Status = model.StartDate < DateTime.Now ? ContestStatus.Active : ContestStatus.Inactive,
            };

            this.Data.Contests.Add(contest);

            foreach (var prize in model.Prizes)
            {
                var dbPrize = new Prize()
                {
                    Name = prize.Name,
                    Description = prize.Description,
                    ContestId = contest.Id,
                };
                contest.Prizes.Add(dbPrize);
            }

            this.Data.SaveChanges();

            return RedirectToAction("Contests", "Me");
        }

        // GET: Contests/{contestId}
        // Returned model type: DetailsContestViewModel
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetContestById(int id)
        {
            var contest = Mapper.Map<DetailsContestViewModel>(this.Data.Contests.Find(id));

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            return View(contest);
        }

        // GET: Contests/{contestId}/Manage
        [HttpGet]
        public ActionResult Manage(int id)
        {
            var contest = this.Data.Contests.All()
                .Where(c => c.Id == id).ProjectTo<EditContestBindingModel>().FirstOrDefault();

            return View(contest);
        }

        // POST: Contests/{contestId}/Manage
        [HttpPost]
        public ActionResult Manage(int id, EditContestBindingModel model)
        {
            if (model == null)
            {
                return this.HttpNotFound();
            }

            var contest = this.Data.Contests.Find(id);
            contest.VotingType = model.VotingType;
            contest.DeadlineType = model.DeadlineType;
            contest.Title = model.Title;
            contest.Description = model.Description;
            contest.EndDate = model.EndDate;
            contest.StartDate = model.StartDate;
            contest.Thumbnail = model.Thumbnail;
            this.Data.SaveChanges();
            return RedirectToAction("contests", "Me");
        }

        // GET: Contests/{contestId}/Jury
        // Returned model type: BasicUserInfoViewModel
        [HttpGet]
        public ActionResult Jury(int? id)
        {
            if (id == null)
            {
                return this.HttpNotFound();
            }

            Mapper.CreateMap<User, BasicUserInfoViewModel>();

            var juryMembers = this.Data.Contests.All()
                .Where(c => c.Id == id).Select(c => c.Jury.Members).FirstOrDefault();

            var juryMembersView = Mapper.Map<IEnumerable<User>, IEnumerable<BasicUserInfoViewModel>>(juryMembers);

            if (juryMembersView == null)
            {
                return this.HttpNotFound();
            }

            return View(juryMembersView);
        }

        // GET: Contests/{contestId}/AddJuryMember
        [HttpGet]
        public ActionResult AddJuryMember(int id)
        {
            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                return this.HttpNotFound();
            }

            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId != contest.OwnerId)
            {
                return this.RedirectToAction("Contests", "Me");
            }

            return View(contest);
        }

        // POST: Contests/{contestId}/AddJuryMember/{username}
        [HttpPost]
        public ActionResult AddJuryMember(int id, string username, Contest model)
        {
            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return this.HttpNotFound();
            }

            var contest = this.Data.Contests.Find(id);

            if (contest.Jury.Members.Any(u => u.Id == user.Id))
            {
                return this.HttpNotFound(); // TODO already added!
            }

            contest.Jury.Members.Add(user);
            this.Data.SaveChanges();

            return this.RedirectToAction("Contests", "Me");
        }

        // GET: Contests/{contestId}/Candidates
        // Returned model type: BasicUserInfoViewModel
        [HttpGet]
        public ActionResult Candidates(int? id)
        {
            if (id == null)
            {
                return this.HttpNotFound();
            }

            Mapper.CreateMap<User, BasicUserInfoViewModel>();

            var contest = this.Data.Contests.All().FirstOrDefault(c => c.Id == id);

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            if (contest.OwnerId != this.User.Identity.GetUserId())
            {
                return this.HttpNotFound(); // TODO unauthorized!
            }

            var candidates = contest.Candidates;

            var candidatesView = Mapper.Map<IEnumerable<User>, IEnumerable<BasicUserInfoViewModel>>(candidates);

            if (candidatesView == null)
            {
                return this.HttpNotFound();
            }

            return View(candidatesView);
        }

        public ActionResult ManageCandidate(string operation, int id, string username)
        {
            if (operation == "approve")
            {
                return RedirectToAction("ApproveCandidate" , new {id, username});
            }

            return RedirectToAction("RejectCandidate", new { id, username });

        }

        // POST: Contests/{contestId}/Candidates/ApproveCandidate/{username}
        public ActionResult ApproveCandidate(int id, string username)
        {
            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();

            if (user == null)
            {
                return this.HttpNotFound();
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            if (contest.OwnerId != loggedUserId)
            {
                return this.HttpNotFound(); // TODO unauthorized;
            }

            contest.Participants.Add(user);
            contest.Candidates.Remove(user);
            this.Data.SaveChanges();

            return RedirectToAction("Contests", "Me");
        }

        // POST: Contests/{contestId}/Candidates/RejectCandidate/{username}
        public ActionResult RejectCandidate(int id, string username)
        {
            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();
            if (user == null)
            {
                return this.HttpNotFound();
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            if (contest.OwnerId != loggedUserId)
            {
                return this.HttpNotFound(); // TODO unauthorized;
            }
            contest.Candidates.Remove(user);
            this.Data.SaveChanges();
            return RedirectToAction("Contests", "Me");
        }

        // GET: Contests/{contestId}/Participants
        [HttpGet]
        public ActionResult Participants(int id)
        {
            return View();
        }

        // POST: Contests/{contestId}/Participants/RemoveParticipant/{username}
        [HttpPost]
        public ActionResult RemoveParticipant(int contestId, string username)
        {
            return View();
        }

        // GET: Contests/{contestId}/Gallery/{pictureId}
        // Returned model type: DetailsPictureViewModel
        [HttpGet]
        public ActionResult Gallery(int contestId, int pictureId)
        {
            return View();
        }

        // POST: Contests/{contestId}/Vote/{pictureId}
        [HttpPost]
        public ActionResult Vote(int contestId, int pictureId)
        {
            return View();
        }
    }
}