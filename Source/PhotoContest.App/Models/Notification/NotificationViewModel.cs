namespace PhotoContest.App.Models.Notification
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NotificationViewModel
    {
        [Required]
        public string RecipientUsername { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}