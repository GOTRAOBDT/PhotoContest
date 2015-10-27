namespace PhotoContest.App.Models.Pictures
{
    using AutoMapper;

    using Bookmarks.Common.Mappings;

    using PhotoContest.Models;

    public class SummaryPictureViewModel : IMapFrom<Picture>, IHaveCustomMappings
    {   
        public int Id { get; set; }
        
        public string Author { get; set; }

        public bool IsAuthor { get; set; }
        
        public string ThumbnailImageData { get; set; }

        public int ContestsCount { get; set; }

        public int VotesCount { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}