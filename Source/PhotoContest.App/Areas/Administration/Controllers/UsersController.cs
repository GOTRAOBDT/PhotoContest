namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Account;
    using AutoMapper.QueryableExtensions;

    using Data.Contracts;

    using Microsoft.AspNet.Identity;

    [Authorize(Roles = ("Administrator"))]
    public class UsersController : MeController
    {
        public UsersController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Administration/Users/GetUserByUsername/username
        [HttpGet]
        public ActionResult GetUserById(string id)
        {
            var userModel = this.Data.Users.All()
                .Where(u => u.Id == id)
                .ProjectTo<EditProfileBindingModel>()
                .FirstOrDefault();

            return this.View(userModel);
        }

        public override ActionResult Profile(EditProfileBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("GetUserById", model);
            }

            var user = this.Data.Users.All()
                .FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                return this.RedirectToAction("Users", "Admin");
            }

            if (user.Id == this.User.Identity.GetUserId())
            {
                this.TempData["message"] = "Admin profile can not be edited!";
                return this.RedirectToAction("Users", "Admin");
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

            this.TempData["message"] = "User profile of [" + model.Name + "] edited successfully";
            return this.RedirectToAction("Users", "Admin");
        }
    }
}