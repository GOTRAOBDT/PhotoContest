namespace PhotoContest.App.Models.Pictures
{
    using AutoMapper;

    using Bookmarks.Common.Mappings;

    using PhotoContest.Models;

    public class SummaryPictureViewModel : IMapFrom<Picture>, IHaveCustomMappings
    {   
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public bool IsAuthor { get; set; }
        
        public string ThumbnailImageData { get; set; }

        public int ContestsCount { get; set; }

        public int VotesCount { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Picture, SummaryPictureViewModel>()
                .ForMember(p => p.Author, cfg => cfg.MapFrom(p => p.Author.Name))
                .ForMember(p => p.ContestsCount, cfg => cfg.MapFrom(p => p.Contests.Count))
                .ForMember(p => p.VotesCount, cfg => cfg.MapFrom(p => p.Votes.Count));
        }
    }
}