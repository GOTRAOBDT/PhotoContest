namespace PhotoContest.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class VotingCommittee
    {
        private ICollection<ApplicationUser> members;

        public VotingCommittee()
        {
            this.members = new HashSet<ApplicationUser>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public Contest Contest { get; set; }

        public virtual ICollection<ApplicationUser> Members
        {
            get
            {
                return this.members;
            }

            set
            {
                this.members = value;
            }
        }
    }
}
