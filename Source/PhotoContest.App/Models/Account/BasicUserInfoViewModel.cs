namespace PhotoContest.App.Models.Account
{
    using AutoMapper;

    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;

    public class BasicUserInfoViewModel : IMapTo<User>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string ProfilePicture { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}