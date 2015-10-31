namespace PhotoContest.App.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;

    using AutoMapper.QueryableExtensions;

    using Data.Contracts;
    using Models.Pictures;
    using AutoMapper;
    using PhotoContest.Models;
    using System;

    public class PicturesController : BaseController
    {
        public PicturesController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Pictures/{pictureId}
        // Returned model type: DetailsPictureViewModel
        public virtual ActionResult Index(int id, int? contestId)
        {
            var dbPicture = this.Data.Pictures.Find(id);
            var picture = Mapper.Map<DetailsPictureViewModel>(dbPicture);

            if (picture == null)
            {
                return this.HttpNotFound();
            }

            var user = this.Data.Users.Find(this.User.Identity.GetUserId());

            if (dbPicture.AuthorId == user.Id)
            {
                picture.IsAuthor = true;
            }

            if (contestId == null)
            {
                picture.CanVote = false;
                picture.HasVoted = true;
            }
            else
            {
                var dbContest = this.Data.Contests.Find(contestId);
                picture.ContestId = contestId;
                picture.CanVote = this.CanVote(user, dbPicture, dbContest);
                picture.HasVoted = this.HasVoted(user, dbPicture, dbContest);
            }

            return this.View(picture);
        }

        private bool HasVoted(User user, Picture dbPicture, Contest dbContest)
        {
            if (dbPicture.Votes.Any(v => v.VoterId == user.Id && v.ContestId == dbContest.Id))
            {
                return true;
            }

            return false;
        }

        private bool CanVote(User user, Picture dbPicture, Contest dbContest)
        {
            if (dbContest.Status != PhotoContest.Models.Enumerations.ContestStatus.Active)
            {
                return false;
            }

            if (dbPicture.Author == user)
            {
                return false;
            }

            if (dbContest.OwnerId == user.Id)
            {
                return false;
            }

            if (dbPicture.Votes.Any(v => v.VoterId == user.Id))
            {
                return false;
            }

            if (dbContest.VotingType == PhotoContest.Models.Enumerations.VotingType.Closed &&
                !dbContest.Jury.Members.Any(m => m.Id == user.Id))
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var picture = this.Data.Pictures.Find(id);

            if (picture == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var user = this.Data.Users.Find(this.User.Identity.GetUserId());

            if (picture.Author.Id != user.Id && !this.User.IsInRole("Administrator"))
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
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

            return this.RedirectToAction("Pictures", "Me");
        }
    }
}