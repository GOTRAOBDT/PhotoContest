
namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Web.Mvc;
    
    using Data.Contracts;

    using Models.Contest;

    [Authorize(Roles = ("Administrator"))]
    public class ContestsController : App.Controllers.ContestsController
    {
        public ContestsController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Administration/Contests/{id}/Details
        [HttpGet]
        public override ActionResult GetContestById(int id)
        {
            return base.GetContestById(id);
        }

        // GET: Administration/Contests/{id}/Manage
        [HttpGet]
        public override ActionResult Manage(int id)
        {
            return base.Manage(id);
        }

        // POST: Administration/Contests/{id}/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Manage(int id, EditContestBindingModel model)
        {
            return base.Manage(id, model);
        }
    }
}