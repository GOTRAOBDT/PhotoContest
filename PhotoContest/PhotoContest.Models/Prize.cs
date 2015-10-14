namespace PhotoContest.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Prize
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        public virtual Contest Contest { get; set; }
    }
}
