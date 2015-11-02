namespace PhotoContest.App.Models.Search
{
    using System;

    using AutoMapper;

    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;
    using PhotoContest.Models.Contracts;


    public class UserSearchResultModel : ISearchResult, IMapFrom<User>, IHaveCustomMappings
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string ResultText()
        {
            return string.Format("Username: {0}, Name: {1}", this.UserName, this.Name);
        }

        public string ResultUrl()
        {
            return string.Format("/users/{0}", this.UserName);
        }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}