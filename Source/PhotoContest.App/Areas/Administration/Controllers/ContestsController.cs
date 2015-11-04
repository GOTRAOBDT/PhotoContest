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
    }
}