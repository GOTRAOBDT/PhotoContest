namespace PhotoContest.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RecipientId { get; set; }

        public virtual User Recipient { get; set; }

        [Required]
        [MinLength(3)]
        public string Content { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
