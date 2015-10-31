namespace PhotoContest.App.Models.Users
{
    using System;
    using AutoMapper;
    using Bookmarks.Common.Mappings;

    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    public class DetailedUserInfoModel : IMapFrom<User>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public UserGender Gender { get; set; }

        public string ProfilePicture { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}