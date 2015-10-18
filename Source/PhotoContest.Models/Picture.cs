namespace PhotoContest.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Picture
    {
        private ICollection<Contest> contests;

        public Picture()
        {
            this.contests = new HashSet<Contest>();
        }

        [Key]
        public int Id { get; set; }
        
        [Required]
        public virtual User Author { get; set; }

        [Required]
        public string PictureData { get; set; }

        [Required]
        public DateTime PostedOn { get; set; }

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
    }
}
