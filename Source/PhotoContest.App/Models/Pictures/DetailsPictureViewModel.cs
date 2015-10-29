namespace PhotoContest.App.Models.Pictures
{
    using System;

    using AutoMapper;

    using Bookmarks.Common.Mappings;

    using PhotoContest.Models;

    public class DetailsPictureViewModel : IMapFrom<Picture>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public bool IsAuthor { get; set; }

        public bool CanVote { get; set; }

        public bool HasVoted { get; set; }

        public int? ContestId { get; set; }

        public string PictureData { get; set; }

        public int VotesCount { get; set; }

        public int ContestsCount { get; set; }

        public DateTime PostedOn { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Picture, DetailsPictureViewModel>()
                .ForMember(p => p.Author, cfg => cfg.MapFrom(p => p.Author.Name))
                .ForMember(p => p.ContestsCount, cfg => cfg.MapFrom(p => p.Contests.Count))
                .ForMember(p => p.VotesCount, cfg => cfg.MapFrom(p => p.Votes.Count));
        }
    }
}