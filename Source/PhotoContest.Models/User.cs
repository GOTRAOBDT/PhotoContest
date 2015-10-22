namespace PhotoContest.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Enumerations;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    
    public class User : IdentityUser
    {
        private ICollection<Picture> pictures;

        private ICollection<Contest> myContests;

        private ICollection<Contest> invitedContests;
        private ICollection<Contest> participationContests;
        private ICollection<Vote> votes;

        private ICollection<Notification> notifications;

        private ICollection<VotingCommittee> commettees;

        public User()
        {
            this.pictures = new HashSet<Picture>();
            this.myContests = new HashSet<Contest>();
            this.invitedContests = new HashSet<Contest>();
            this.participationContests = new HashSet<Contest>();
            this.votes = new HashSet<Vote>();
            this.commettees = new HashSet<VotingCommittee>();
            this.notifications = new HashSet<Notification>();
        }

        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public UserGender Gender { get; set; }

        public string ProfilePicture { get; set; }

        public DateTime BirthDate { get; set; }

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

        public virtual ICollection<Contest> MyContests
        {
            get
            {
                return this.myContests;
            }

            set
            {
                this.myContests = value;
            }
        }

        public virtual ICollection<Contest> InvitedContests
        {
            get
            {
                return this.invitedContests;
            }

            set
            {
                this.invitedContests = value;
            }
        }

        public virtual ICollection<Contest> ParticipationContests
        {
            get
            {
                return this.participationContests;
            }

            set
            {
                this.participationContests = value;
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

        public virtual ICollection<VotingCommittee> Commettees
        {
            get
            {
                return this.commettees;
            }

            set
            {
                this.commettees = value;
            }
        }

        public virtual ICollection<Notification> Notifications
        {
            get
            {
                return this.notifications;
            }

            set
            {
                this.notifications = value;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
