using System.Collections.Generic;
using PhotoContest.Models.Enumerations;

namespace PhotoContest.App.Controllers
{
    using System.Web.Mvc;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNet.Identity;
    using Data.Contracts;
    using PhotoContest.App.Models.Account;
    using PhotoContest.Models;
    using Models.Contest;
    using AutoMapper;

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
            var contest = Mapper.Map<Contest>(model);
            contest.OwnerId = this.User.Identity.GetUserId();
            contest.Status = ContestStatus.Active;

            this.Data.Contests.Add(contest);
            this.Data.SaveChanges();

            contest.Jury = new VotingCommittee();
            contest.Jury.ContestId = contest.Id;
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

        // POST: Contests/{contestId}/Candidates/ApproveCandidate/{username}
        [HttpPost]
        public ActionResult ApproveCandidate(int contestId, string username)
        {
            return View();
        }

        // POST: Contests/{contestId}/Candidates/RejectCandidate/{username}
        [HttpPost]
        public ActionResult RejectCandidate(int contestId, string username)
        {
            return View();
        }

        // GET: Contests/{contestId}/Participants
        [HttpGet]
        public ActionResult Participants(int contestId)
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