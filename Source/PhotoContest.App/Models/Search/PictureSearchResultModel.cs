namespace PhotoContest.App.Models.Search
{
    using System;

    using AutoMapper;

    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;
    using PhotoContest.Models.Contracts;


    public class PictureSearchResultModel : ISearchResult, IMapFrom<User>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string AuthorUsername { get; set; }

        public DateTime PostedOn { get; set; }

        public string ResultText()
        {
            return string.Format("Picture titled '{0}' by {1}, posted on {2}.", this.Title, this.AuthorUsername, this.PostedOn);
        }

        public virtual string ResultUrl()
        {
            return string.Format("/pictures/{0}", this.Id);
        }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Picture, PictureSearchResultModel>()
                .ForMember(p => p.AuthorUsername, cfg => cfg.MapFrom(p => p.Author.UserName));
        }
    }
}