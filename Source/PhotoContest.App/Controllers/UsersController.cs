namespace PhotoContest.App.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    using AutoMapper;

    using Data.Contracts;

    using Models.Users;
    using Common;

    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Users
        public ActionResult Index(string username)
        {
            if (username == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var dbUser = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            if (dbUser == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.UserNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var userModel = Mapper.Map<DetailedUserInfoModel>(dbUser);
            userModel.TotalContests = this.Data.Contests.All()
                .Where(c => c.Participants.Any(p => p.Id == dbUser.Id))
                .Count();
            userModel.TotalPictures = dbUser.Pictures.Count();
            userModel.TotalVotes = dbUser.Pictures
                .Select(p => p.Votes.Count)
                .Sum();

            return View(userModel);
        }
    }
}