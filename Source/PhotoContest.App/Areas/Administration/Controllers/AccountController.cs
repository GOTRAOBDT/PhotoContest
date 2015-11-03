namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;

    [Authorize(Roles = "Administrator")]
    public class AccountController : App.Controllers.AccountController
    {
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult LogOff()
        {
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }
    }
}