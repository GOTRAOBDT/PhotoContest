namespace PhotoContest.App.Controllers
{
    using System.Linq;
    using System.Net.Http;
    using System.Net;
    using System.Web.Mvc;

    using AutoMapper;

    using Data.Contracts;
    using Models.Users;

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
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            var dbUser = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            if (dbUser == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
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