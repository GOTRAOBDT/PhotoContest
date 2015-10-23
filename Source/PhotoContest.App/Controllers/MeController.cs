namespace PhotoContest.App.Controllers
{
    using System.Web.Mvc;

    using Data.Contracts;

    using Models.Account;
    using Models.Pictures;

    [Authorize]
    public class MeController : BaseController
    {
        public MeController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Index{?sortBy=popularity&filterBy=active}
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Index(string sortBy, string filterBy)
        {
            return this.View();
        }

        // GET: Me/Contests
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Contests()
        {
            return View();
        }
        
        // GET: Me/Pictures
        // Returned model type: SummaryPictureViewModel
        [HttpGet]
        public ActionResult Pictures()
        {
            return View();
        }

        // GET: Me/UploadPicture
        [HttpGet]
        public ActionResult UploadPicture()
        {
            return View();
        }

        // Post: Me/UploadPicture
        [HttpPost]
        public ActionResult UploadPicture(UploadPictureBindingModel model)
        {
            return View();
        }

        // GET: Me/Profile
        [HttpGet]
        public ActionResult EditProfile()
        {
            return View();
        }

        // PUT: Me/Profile
        [HttpPut]
        public ActionResult EditProfile(EditProfileBindingModel model)
        {
            return View();
        }
    }
}