namespace PhotoContest.App.Areas.Administration.Models
{
    using System;

    using Bookmarks.Common.Mappings;
    
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    public class UserViewModel : IMapFrom<User>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public UserGender Gender { get; set; }
    }
}