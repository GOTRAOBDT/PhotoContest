namespace PhotoContest.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Common;

    using Data.Contracts;

    using Microsoft.AspNet.Identity;

    using Models.Account;
    using Models.Contest;
    using Models.Pictures;

    using PagedList;

    using PhotoContest.Models;

    [Authorize]
    public class MeController : BaseController
    {
        public MeController(IPhotoContestData data)
            : base(data)
        {
            Mapper.CreateMap<Contest, SummaryContestViewModel>();
            Mapper.CreateMap<Picture, SummaryPictureViewModel>();
            Mapper.CreateMap<User, EditProfileBindingModel>();
        }

        // GET: Me/Contests
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Contests(int? page)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            IPagedList<SummaryContestViewModel> contests = this.Data.Contests.All()
                .Where(c => c.OwnerId == loggedUserId)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ProjectTo<SummaryContestViewModel>()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);

            return this.View(contests);
        }
        
        // GET: Me/Pictures
        // Returned model type: SummaryPictureViewModel
        [HttpGet]
        public ActionResult Pictures(int? page, int? contestId)
        {
            IPagedList<SummaryPictureViewModel> pictures = null;
            var userId = this.User.Identity.GetUserId();

                pictures = this.Data.Pictures.All()
                    .Where(p => p.Author.Id == userId)
                    .OrderByDescending(p => p.PostedOn)
                    .ThenByDescending(c => c.Contests.Count())
                    .ProjectTo<SummaryPictureViewModel>()
                    .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);
            if (contestId != null)
            {
                for (int i = 0; i < pictures.Count; i++)
                {
                    pictures[i].ContestId = contestId;
                }
            }

            return this.View(pictures);
        }

        // GET: Me/UploadPicture
        [HttpGet]
        public ActionResult UploadPicture()
        {
            return this.View(new UploadPictureBindingModel());
        }

        // Post: Me/UploadPicture
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPicture(UploadPictureBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                this.RedirectToAction("Index", "Home");
            }

            var picture = new Picture()
            {
                PictureData = model.PictureData,
                ThumbnailImageData = model.PictureData,
                Title = model.Title,
                AuthorId = userId,
                PostedOn = DateTime.Now,
            };
            this.Data.Pictures.Add(picture);
            this.Data.SaveChanges();

            return this.RedirectToAction("Pictures");
        }

        // GET: Me/Profile
        [HttpGet]
        public ActionResult Profile()
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                return this.HttpNotFound();
            }

            var editProfileModel = this.Data.Users.All()
                .Where(u => u.Id == userId)
                .ProjectTo<EditProfileBindingModel>()
                .FirstOrDefault();

            return this.View(editProfileModel);
        }

        // POST: Me/Profile
        [HttpPost]
        public virtual ActionResult Profile(EditProfileBindingModel model)
        {
            if (this.User.IsInRole("Administrator"))
            {
                this.TempData["message"] = "Admin profile can not be edited!";
                return this.RedirectToAction("Index", "Home");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var userId = User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                this.RedirectToAction("Index", "Home");
            }

            user.Name = model.Name;
            if (model.BirthDate != null)
            {
                user.BirthDate = model.BirthDate;
            }
            if (model.ProfilePicture != null)
            {
                user.ProfilePicture = model.ProfilePicture;
            }
            user.Gender = model.Gender;
            this.Data.SaveChanges();

            return this.RedirectToAction("Index", "Home");
        }
    }
}