namespace PhotoContest.App.Models.Contest
{
    using AutoMapper;

    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;

    public class PrizeViewModel : IMapFrom<Prize>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}