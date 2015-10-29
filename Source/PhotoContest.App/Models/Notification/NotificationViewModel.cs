namespace PhotoContest.App.Models.Notification
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;

    using PhotoContest.Models;

    public class NotificationViewModel
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

        public static Expression<Func<Notification, NotificationViewModel>> Create
        {
            get
            {
                return n => new NotificationViewModel()
                {
                    Id = n.Id,
                    RecipientUsername = n.Recipient.UserName,
                    Content = n.Content,
                    IsRead = n.IsRead,
                    CreatedOn = n.CreatedOn
                };
            }
        }
    }
}