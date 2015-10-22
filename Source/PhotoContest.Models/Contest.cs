using System.Data.Entity.Infrastructure.Design;
using System.Net.Mime;

namespace PhotoContest.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    using Enumerations;

    public class Contest
    {
        private ICollection<Prize> prizes;
        private ICollection<Picture> pictures;

        private ICollection<User> invitees;
        private ICollection<User> participants;

        private ICollection<Vote> votes;

        public Contest()
        {
            this.prizes = new HashSet<Prize>();
            this.pictures = new HashSet<Picture>();
            this.participants = new HashSet<User>();
            this.invitees = new HashSet<User>();
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

        [Required]
        public string Thumbnail { get; set; }

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

        public ICollection<User> Participants
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

        public virtual VotingCommittee Jury { get; set; }
    }
}
