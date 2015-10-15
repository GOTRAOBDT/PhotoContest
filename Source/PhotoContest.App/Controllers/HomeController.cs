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

        public ActionResult Index()
        {
            return this.View();
        }
    }
}