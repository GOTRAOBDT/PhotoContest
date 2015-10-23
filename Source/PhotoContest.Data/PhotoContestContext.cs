namespace PhotoContest.Data
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    using Contracts;

    using Microsoft.AspNet.Identity.EntityFramework;
    using Migrations;
    
    using Models;

    public class PhotoContestContext : IdentityDbContext<User>, IPhotoContestContext
    {
        public PhotoContestContext()
            : base("name=PhotoContestContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PhotoContestContext, Configuration>());
        }

        public virtual IDbSet<Contest> Contests { get; set; }

        public virtual IDbSet<Picture> Pictures { get; set; }

        public virtual IDbSet<Prize> Prizes { get; set; }

        public virtual IDbSet<Vote> Votes { get; set; }

        public virtual IDbSet<VotingCommittee> Commettees { get; set; }

        public virtual IDbSet<Notification> Notifications { get; set; }

        public static PhotoContestContext Create()
        {
            return new PhotoContestContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Pictures)
                .WithRequired(p => p.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.MyContests)
                .WithRequired(c => c.Owner)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.InvitedContests)
                .WithMany(c => c.Invitees)
                .Map(m =>
                {
                    m.MapLeftKey("ContestId");
                    m.MapRightKey("InviteeId");
                    m.ToTable("ContestsInvitees");
                });

            modelBuilder.Entity<User>()
                .HasMany(u => u.ParticipationContests)
                .WithMany(c => c.Participants)
                .Map(m =>
                {
                    m.MapLeftKey("ParticipantId");
                    m.MapRightKey("ContestId");
                    m.ToTable("ContestsParticipants");
                });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Votes)
                .WithRequired(v => v.Voter)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithRequired(n => n.Recipient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contest>()
                .HasMany(c => c.Pictures)
                .WithMany(p => p.Contests)
                .Map(m =>
                {
                    m.MapLeftKey("ContestId");
                    m.MapRightKey("PictureId");
                    m.ToTable("ContestPictures");
                });

            modelBuilder.Entity<Contest>()
                .HasMany(c => c.Prizes)
                .WithRequired(p => p.Contest)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contest>()
                .HasMany(c => c.Votes)
                .WithRequired(v => v.Contest)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contest>()
                .HasOptional(c => c.Jury)
                .WithRequired(cm => cm.Contest)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VotingCommittee>()
                .HasMany(vc => vc.Members)
                .WithMany(m => m.Commettees)
                .Map(m =>
                {
                    m.MapLeftKey("ContestId");
                    m.MapRightKey("MemberId");
                    m.ToTable("VotingCommettees");
                });
        }
    }
}