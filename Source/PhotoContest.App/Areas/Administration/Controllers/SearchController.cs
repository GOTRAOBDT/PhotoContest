namespace PhotoContest.App.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Data.Contracts;
    
    using PhotoContest.Models;

    using ContestSearchResultModel = Models.Search.ContestSearchResultModel;
    using PictureSearchResultModel = Models.Search.PictureSearchResultModel;
    using UserSearchResultModel = Models.Search.UserSearchResultModel;

    [Authorize(Roles = "Administrator")]
    public class SearchController : App.Controllers.SearchController
    {
        public SearchController(IPhotoContestData data)
            : base(data)
        {
            Mapper.CreateMap<Contest, ContestSearchResultModel>();
            Mapper.CreateMap<Picture, PictureSearchResultModel>();
            Mapper.CreateMap<User, UserSearchResultModel>();
        }

        [HttpGet]
        public override ActionResult Index(string keyword)
        {
            var results = new SearchResultsRepository();

            var usersResults = this.Data.Users.All()
                .Where(u => u.UserName.IndexOf(keyword) >= 0 || u.Name.IndexOf(keyword) >= 0)
                .ProjectTo<UserSearchResultModel>()
                .ToList();
            results.Results.AddRange(usersResults);

            var contestsResults = this.Data.Contests.All()
                .Where(c => c.Title.IndexOf(keyword) >= 0)
                .ProjectTo<ContestSearchResultModel>()
                .ToList();
            results.Results.AddRange(contestsResults);

            var picturesResults = this.Data.Pictures.All()
                .Where(p => p.Title.IndexOf(keyword) >= 0)
                .ProjectTo<PictureSearchResultModel>()
                .ToList();
            results.Results.AddRange(picturesResults);

            this.ViewBag.Keyword = keyword;
            return this.View(results);
        }
    }
}