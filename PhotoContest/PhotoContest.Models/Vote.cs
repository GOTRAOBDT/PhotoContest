namespace PhotoContest.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Vote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual ApplicationUser Voter { get; set; }

        [Required]
        public virtual Contest Contest { get; set; }

        [Required]
        public virtual Picture Picture { get; set; }
    }
}
