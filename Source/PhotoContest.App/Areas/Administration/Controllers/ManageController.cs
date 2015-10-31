namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Web.Mvc;

    using Data.Contracts;

    public class ManageController : App.Controllers.ManageController
    {
        public ManageController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Administration/Account
        public ActionResult Index()
        {
            return this.View();
        }
    }
}