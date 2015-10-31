namespace PhotoContest.App.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net;
    using System.Web.Http;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;

    using AutoMapper.QueryableExtensions;

    using PagedList;

    using Data.Contracts;

    using Common;

    using Models.Account;
    using Models.Contest;
    using Models.Pictures;


    using PhotoContest.Models;

    [System.Web.Mvc.Authorize]
    public class MeController : BaseController
    {
        public MeController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Me/Contests
        // Returned model type: SummaryContestViewModel
        [System.Web.Mvc.HttpGet]
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
        [System.Web.Mvc.HttpGet]
        public ActionResult Pictures(int? page)
        {
            IPagedList<SummaryPictureViewModel> pictures = null;
            var userId = this.User.Identity.GetUserId();

                pictures = this.Data.Pictures.All()
                    .Where(p => p.Author.Id == userId)
                    .OrderByDescending(p => p.PostedOn)
                    .ThenByDescending(c => c.Contests.Count())
                    .ProjectTo<SummaryPictureViewModel>()
                    .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);

            return View(pictures);
        }

        // GET: Me/UploadPicture
        [System.Web.Mvc.HttpGet]
        public ActionResult UploadPicture()
        {
            return View(new UploadPictureBindingModel());
        }

        // Post: Me/UploadPicture
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPicture(UploadPictureBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                RedirectToAction("Index", "Home");
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

            return RedirectToAction("Pictures");
        }

        // GET: Me/Profile
        [System.Web.Mvc.HttpGet]
        public ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                return this.HttpNotFound();
            }

            var editProfileModel = this.Data.Users.All()
                .Where(u => u.Id == userId)
                .ProjectTo<EditProfileBindingModel>()
                .FirstOrDefault();

            return View(editProfileModel);
        }

        // POST: Me/Profile
        [System.Web.Mvc.HttpPost]
        public virtual ActionResult Profile(EditProfileBindingModel model)
        {
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

            return RedirectToAction("Index", "Home");
        }
    }
}