namespace PhotoContest.App.Controllers
{
    using System;
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
    using Common;

    //using DropboxFileSystem;

    public class PicturesController : BaseController
    {
        public object PictureUtils { get; private set; }

        public PicturesController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Pictures/{pictureId}
        // Returned model type: DetailsPictureViewModel
        
        //dropbox editions
        //public async ActionResult Index(int id, int? contestId)
        public virtual ActionResult Index(int id, int? contestId)
        {
            var dbPicture = this.Data.Pictures.Find(id);

            ////get picture link from dropbox here:
            //var fs = new DropboxFileSystem();
            //string linkFormDrobbox = fs.Download("pictures", );

            var picture = Mapper.Map<DetailsPictureViewModel>(dbPicture);

            if (picture == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var user = this.Data.Users.Find(this.User.Identity.GetUserId());

            picture.CanBeDeleted = PictureUtills.IsAuthor(user, dbPicture) || this.User.IsInRole("Administrator");
            picture.CanBeRemoved = picture.CanBeDeleted && contestId != null;

            if (contestId == null)
            {
                picture.CanBeVoted = false;
                picture.CanBeUnvoted = false;
            }
            else
            {
                var dbContest = this.Data.Contests.Find(contestId);
                picture.ContestId = contestId;
                picture.CanBeVoted = PictureUtills.CanVoteForPicture(user, dbPicture, dbContest);
                picture.CanBeUnvoted = PictureUtills.CanUnvotePicture(user, dbPicture, dbContest);
                picture.VotesCount = dbPicture.Votes.Where(v => v.ContestId == contestId).Count();
            }

            return this.View(picture);
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

        public ActionResult Remove(int id, int contestId)
        {
            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var picture = contest.Pictures.FirstOrDefault(p => p.Id == id);
            if (picture == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var user = this.Data.Users.Find(this.User.Identity.GetUserId());

            if (picture.Author.Id != user.Id && !this.User.IsInRole("Administrator"))
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }

            var votesForPictureInContest = picture.Votes.Where(v => v.ContestId == contestId).ToList();
            foreach (var vote in votesForPictureInContest)
            {
                this.Data.Votes.Delete(vote);
            }
            this.Data.SaveChanges();
            if (contest.Pictures.Where(p => p.AuthorId == user.Id).Count() < 2)
            {
                contest.Participants.Remove(user);
            }

            contest.Pictures.Remove(picture);
            this.Data.SaveChanges();

            return this.RedirectToAction("Pictures", "Contests", new { id = contestId });
        }
    }
}