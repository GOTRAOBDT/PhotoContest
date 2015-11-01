namespace PhotoContest.App.Models.Notification
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;
    

    public class NotificationViewModel : IMapFrom<Notification>, IHaveCustomMappings
    {
        public int Id { get; set; }

        [Required]
        public string RecipientUsername { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Notification, NotificationViewModel>()
                .ForMember(n => n.RecipientUsername, n => n.MapFrom(m => m.Recipient.UserName));
        }
    }
}