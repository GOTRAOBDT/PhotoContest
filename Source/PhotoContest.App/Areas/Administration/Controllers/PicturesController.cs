namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    using Data.Contracts;
    using Common;

    [Authorize(Roles = ("Administrator"))]
    public class PicturesController : App.Controllers.PicturesController
    {
        public PicturesController(IPhotoContestData data)
            : base(data)
        {
        }
    }
}