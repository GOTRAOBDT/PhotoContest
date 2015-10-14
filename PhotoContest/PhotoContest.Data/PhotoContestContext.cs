namespace PhotoContest.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using PhotoContest.Data.Migrations;
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    public class PhotoContestContext : IdentityDbContext<ApplicationUser>
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Pictures)
                .WithRequired(p => p.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.MyContests)
                .WithRequired(c => c.Owner)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.InvitedContests)
                .WithMany(c => c.Invitees)
                .Map(m =>
                {
                    m.MapLeftKey("ContestId");
                    m.MapRightKey("InviteeId");
                    m.ToTable("ContestsInvitees");
                });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ParticipationContests)
                .WithMany(c => c.Participants)
                .Map(m =>
                {
                    m.MapLeftKey("ParticipantId");
                    m.MapRightKey("ContestId");
                    m.ToTable("ContestsParticipants");
                });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Votes)
                .WithRequired(v => v.Voter)
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
                .HasOptional(c => c.Commettee)
                .WithRequired(cm => cm.Contest)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<VotingCommittee>()
            //    .HasMany(vc => vc.Members)
            //    .WithMany(m => m.Commettees)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("ContestId");
            //        m.MapRightKey("MemberId");
            //        m.ToTable("VotingCommettees");
            //    });

            base.OnModelCreating(modelBuilder);
        }
    }
}