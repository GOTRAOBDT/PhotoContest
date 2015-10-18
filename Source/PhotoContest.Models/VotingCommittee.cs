namespace PhotoContest.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class VotingCommittee
    {
        private ICollection<User> members;

        public VotingCommittee()
        {
            this.members = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public virtual Contest Contest { get; set; }

        public virtual ICollection<User> Members
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
