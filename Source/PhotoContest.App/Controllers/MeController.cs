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

        // GET: Me/{sortBy}
        [HttpGet]
        public ActionResult Index(string sortBy)
        {
            return View();
        }

        // GET: Me/Contest
        [HttpGet]
        public ActionResult Contest()
        {
            return View();
        }

        // GET: Me/Pictures
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
        public ActionResult Profile()
        {
            return View();
        }

        // POST: Me/Profile
        [HttpPost]
        public ActionResult Profile(EditProfileBindingModel model)
        {
            return View();
        }
    }
}