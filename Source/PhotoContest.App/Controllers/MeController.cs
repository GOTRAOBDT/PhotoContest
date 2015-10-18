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

        // Post: Me/Pictures
        [HttpPost]
        public ActionResult Pictures(UploadPictureBindingModel model)
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