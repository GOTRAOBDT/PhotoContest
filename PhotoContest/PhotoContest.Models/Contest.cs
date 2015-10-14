namespace PhotoContest.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoContest.Models.Enumerations;

    public class Contest
    {
        private ICollection<Prize> prizes;
        private ICollection<Picture> pictures;

        private ICollection<ApplicationUser> invitees;
        private ICollection<ApplicationUser> participants;

        private ICollection<Vote> votes;

        public Contest()
        {
            this.prizes = new HashSet<Prize>();
            this.pictures = new HashSet<Picture>();
            this.participants = new HashSet<ApplicationUser>();
            this.invitees = new HashSet<ApplicationUser>();
            this.votes = new HashSet<Vote>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        [Required]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public virtual ApplicationUser Owner { get; set; }

        [Required]
        public ContestStatus Status { get; set; }

        [Required]
        public VotingType VotingType { get; set; }

        [Required]
        public ParticipationType ParticipationType { get; set; }

        [Required]
        public DeadlineType DeadlineType { get; set; }

        public virtual ICollection<Prize> Prizes
        {
            get
            {
                return this.prizes;
            }

            set
            {
                this.prizes = value;
            }
        }

        public virtual ICollection<Picture> Pictures
        {
            get
            {
                return this.pictures;
            }

            set
            {
                this.pictures = value;
            }
        }

        public virtual ICollection<ApplicationUser> Invitees
        {
            get
            {
                return this.invitees;
            }
            set
            {
                this.invitees = value;
            }
        }

        public ICollection<ApplicationUser> Participants
        {
            get
            {
                return this.participants;
            }

            set
            {
                this.participants = value;
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

        public virtual VotingCommittee Commettee { get; set; }
    }
}
