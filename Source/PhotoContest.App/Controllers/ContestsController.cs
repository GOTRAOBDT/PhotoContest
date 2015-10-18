namespace PhotoContest.App.Controllers
{
    using System.Web.Mvc;

    using Data.Contracts;

    using Models.Contest;
    
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
        public ActionResult Create(CreateContestBindingModel model)
        {
            return View();
        }

        // GET: Contests/{contestId}
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetContestById(int? contestId)
        {
            return View();
        }

        // GET: Contests/{contestId}/Manage
        [HttpGet]
        public ActionResult Manage(int contestId)
        {
            return View();
        }

        // POST: Contests/{contestId}/Manage
        [HttpPost]
        public ActionResult Manage(int contestId, EditContestBindingModel model)
        {
            return View();
        }

        // GET: Contests/{contestId}/Jury
        [HttpGet]
        public ActionResult Jury(int contestId)
        {
            return View();
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