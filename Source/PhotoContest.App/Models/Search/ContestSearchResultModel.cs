namespace PhotoContest.App.Models.Search
{
    using System;

    using AutoMapper;

    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;
    using PhotoContest.Models.Contracts;


    public class ContestSearchResultModel : ISearchResult, IMapFrom<User>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string OwnerUsername { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


        public string ResultText()
        {
            return string.Format("Contest titled '{0}', moderator: {1}, starts on {2}, ends on: {3}.",
                this.Title, 
                this.OwnerUsername,
                this.StartDate,
                this.EndDate);
        }

        public virtual string ResultUrl()
        {
            return string.Format("/contests/{0}", this.Id);
        }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contest, ContestSearchResultModel>()
                .ForMember(c => c.OwnerUsername, cfg => cfg.MapFrom(c => c.Owner.UserName));
        }
    }
}