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

            return RedirectToAction("Contests", "Me");
        }

        // GET: Contests/{contestId}
        // Returned model type: DetailsContestViewModel
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetContestById(int contestId)
        {
            var contest = Mapper.Map<DetailsContestViewModel>(this.Data.Contests.Find(contestId));

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            return View(contest);
        }

        // GET: Contests/{contestId}/Manage
        [HttpGet]
        public ActionResult Manage(int contestId)
        {
            var contest = this.Data.Contests.All()
                .Where(c => c.Id == contestId).ProjectTo<EditContestBindingModel>().FirstOrDefault();

            return View(contest);
        }

        // POST: Contests/{contestId}/Manage
        [HttpPost]
        public ActionResult Manage(int contestId, EditContestBindingModel model)
        {
            if (model == null)
            {
                return this.HttpNotFound();
            }
            return View();
        }

        // GET: Contests/{contestId}/Jury
        // Returned model type: BasicUserInfoViewModel
        [HttpGet]
        public ActionResult Jury(int contestId)
        {
            var juryMembers = this.Data.Contests.All()
                .Where(c => c.Id == contestId).Select(c => c.Jury).ProjectTo<BasicUserInfoViewModel>();
            if (juryMembers == null)
            {
                return this.HttpNotFound();
            }

            return View(juryMembers);
        }

        // GET: Contests/{contestId}/Jury/AddJuryMember
        [HttpGet]
        public ActionResult AddJuryMember(int contestId)
        {
            return View();
        }

        // POST: Contests/{contestId}/Jury/AddJuryMember/{username}
        [HttpPost]
        public ActionResult AddJuryMember(int contestId, string username)
        {
            return View();
        }

        // GET: Contests/{contestId}/Candidates
        // Returned model type: BasicUserInfoViewModel
        [HttpGet]
        public ActionResult Candidates(int contestId)
        {
            return View();
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