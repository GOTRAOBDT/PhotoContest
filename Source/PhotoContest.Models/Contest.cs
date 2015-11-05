namespace PhotoContest.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    using Enumerations;

    public class Contest
    {
        private List<Prize> prizes;
        private ICollection<Picture> pictures;

        private ICollection<User> invitees;
        private ICollection<User> participants;
        private ICollection<User> candidates;

        private ICollection<Vote> votes;

        public Contest()
        {
            this.prizes = new List<Prize>();
            this.pictures = new HashSet<Picture>();
            this.participants = new HashSet<User>();
            this.invitees = new HashSet<User>();
            this.votes = new HashSet<Vote>();
            this.candidates = new HashSet<User>();
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
        public string OwnerId { get; set; }
        
        public virtual User Owner { get; set; }

        [Required]
        public ContestStatus Status { get; set; }

        [Required]
        public VotingType VotingType { get; set; }

        [Required]
        public ParticipationType ParticipationType { get; set; }

        [Required]
        public DeadlineType DeadlineType { get; set; }

        public int? ParticipationLimit { get; set; }

        public string Picture { get; set; }

        public string Thumbnail { get; set; }

        public virtual List<Prize> Prizes
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

        public virtual ICollection<User> Invitees
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

        public virtual ICollection<User> Participants
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

        public virtual ICollection<User> Candidates
        {
            get { return this.candidates; }
            set { this.candidates = value; }
        }

        public virtual VotingCommittee Jury { get; set; }
    }
}
