﻿namespace PhotoContest.App.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    
    using AutoMapper.QueryableExtensions;

    using Data.Contracts;
    using Models.Search;
    using PhotoContest.Models;

    public class SearchController : BaseController
    {
        public SearchController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Search
        [HttpGet]
        public virtual ActionResult Index(string keyword)
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
                .Where(p => p.IsDeleted == false && p.Title.IndexOf(keyword) >= 0)
                .ProjectTo<PictureSearchResultModel>()
                .ToList();
            results.Results.AddRange(picturesResults);

            this.ViewBag.Keyword = keyword;
            return this.View(results);
        }
    }
}