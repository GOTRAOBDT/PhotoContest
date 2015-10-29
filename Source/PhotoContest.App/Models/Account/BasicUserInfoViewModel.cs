namespace PhotoContest.App.Models.Account
{
    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;

    public class BasicUserInfoViewModel : IMapFrom<User>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string ProfilePicture { get; set; }
    }
}