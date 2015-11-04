﻿namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    using Data.Contracts;

    [Authorize(Roles = ("Administrator"))]
    public class PicturesController : App.Controllers.PicturesController
    {
        public PicturesController(IPhotoContestData data)
            : base(data)
        {
        }

        [HttpGet]
        public ActionResult DeletePicture(int id, int? contestId)
        {
          var picture = this.Data.Pictures.Find(id);

            if (picture == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            picture.Votes.Clear();
            this.Data.SaveChanges();

            var participationContests = this.Data.Contests.All()
                .Where(c => c.Pictures.Any(p => p.Id == picture.Id))
                .ToList();
            for (int i = 0; i < participationContests.Count(); i++)
            {
                participationContests[i].Pictures.Remove(picture);
            }
            this.Data.SaveChanges();

            this.Data.Pictures.Delete(picture);
            this.Data.SaveChanges();

            return this.RedirectToAction("Pictures", "Contests", new { id = contestId });
        }
    }
}