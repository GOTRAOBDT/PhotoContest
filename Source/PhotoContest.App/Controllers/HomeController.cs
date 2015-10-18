namespace PhotoContest.App.Controllers
{
    using System.Web.Mvc;
    using Data.Contracts;

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
            return this.View();
        }
    }
}