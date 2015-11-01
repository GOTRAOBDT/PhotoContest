namespace PhotoContest.App.Models.Contest
{
    using AutoMapper;

    using Bookmarks.Common.Mappings;

    using PhotoContest.Models;

    public class ContestsHubNotificationViewModel : IMapFrom<Contest>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Owner { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contest, ContestsHubNotificationViewModel>()
                .ForMember(c => c.Owner, cfg => cfg.MapFrom(c => c.Owner.UserName));
        }
    }
}