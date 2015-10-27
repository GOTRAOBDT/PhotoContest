namespace PhotoContest.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;

    using AutoMapper.QueryableExtensions;

    using Data.Contracts;

    using Models.Account;
    using Models.Pictures;
    using PhotoContest.Models;

    [Authorize]
    public class MeController : BaseController
    {
        public MeController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Index{?sortBy=popularity&filterBy=active}
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Index(string sortBy, string filterBy)
        {
            return this.View();
        }

        // GET: Me/Contests
        // Returned model type: SummaryContestViewModel
        [HttpGet]
        public ActionResult Contests()
        {
            return View();
        }
        
        // GET: Me/Pictures
        // Returned model type: SummaryPictureViewModel
        [HttpGet]
        public ActionResult Pictures()
        {
            return View();
        }

        // GET: Me/UploadPicture
        [HttpGet]
        public ActionResult UploadPicture()
        {
            return View(new UploadPictureBindingModel());
        }

        // Post: Me/UploadPicture
        [HttpPost]
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
                AuthorId = userId,
                PostedOn = DateTime.Now,
            };
            this.Data.Pictures.Add(picture);
            this.Data.SaveChanges();

            return RedirectToAction("Pictures");
        }


        // GET: Me/Profile
        [HttpGet]
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
        [HttpPost]
        public ActionResult Profile(EditProfileBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (user == null)
            {
                RedirectToAction("Index", "Home");
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