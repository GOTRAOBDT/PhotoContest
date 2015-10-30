
namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Web.Mvc;
    
    using Data.Contracts;

    [Authorize(Roles = ("Administrator"))]
    public class ContestsController : App.Controllers.ContestsController
    {
        public ContestsController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Administration/Contests/{id}/Details
        public ActionResult Details(int id)
        {
            return this.GetContestById(id);
        }
    }
}