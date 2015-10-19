namespace PhotoContest.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Vote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VoterId { get; set; }
        
        public virtual User Voter { get; set; }

        [Required]
        public int ContestId { get; set; }
        
        public virtual Contest Contest { get; set; }

        [Required]
        public int PictureId { get; set; }
        
        public virtual Picture Picture { get; set; }
    }
}
