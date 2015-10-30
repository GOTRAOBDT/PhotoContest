namespace PhotoContest.App.Models.Notification
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;

    using PhotoContest.Models.Enumerations;
    using PhotoContest.Models;

    public class NotificationViewModel
    {
        public int Id { get; set; }

        [Required]
        public string RecipientUsername { get; set; }

        [Required]
        public string SenderUsername { get; set; }

        [Required]
        public string ContestTitle { get; set; }

        public int ContestId { get; set; }

        [Required]
        public string PictureTitle { get; set; }

        public int PictureId { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public NotificationType NotificationType{ get; set; }

        public static Expression<Func<Notification, NotificationViewModel>> Create
        {
            get
            {
                return n => new NotificationViewModel()
                {
                    Id = n.Id,
                    RecipientUsername = n.Recipient.UserName,
                    SenderUsername = n.Sender.UserName,
                    ContestTitle = n.Contest.Title,
                    ContestId = n.Contest.Id,
                    PictureId = n.PictureId,
                    PictureTitle = n.Picture.Title,
                    IsRead = n.IsRead,
                    CreatedOn = n.CreatedOn,
                    NotificationType = n.NotificationType
                };
            }
        }
    }
}