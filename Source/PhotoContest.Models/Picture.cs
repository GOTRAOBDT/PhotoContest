namespace PhotoContest.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Picture
    {
        private ICollection<Contest> contests;
        private ICollection<Vote> votes;

        public Picture()
        {
            this.contests = new HashSet<Contest>();
            this.votes = new HashSet<Vote>();
        }

        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [Required]
        public string AuthorId { get; set; }
        
        public virtual User Author { get; set; }

        [Required]
        public string PictureData { get; set; }
        
        public string ThumbnailImageData { get; set; }

        [Required]
        public DateTime PostedOn { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Contest> Contests
        {
            get
            {
                return this.contests;
            }

            set
            {
                this.contests = value;
            }
        }

        public virtual ICollection<Vote> Votes
        {
            get
            {
                return this.votes;
            }

            set
            {
                this.votes = value;
            }
        }
    }
}
