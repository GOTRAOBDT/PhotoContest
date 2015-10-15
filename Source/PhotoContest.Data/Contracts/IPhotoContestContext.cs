namespace PhotoContest.Data.Contracts
{
    using System.Data.Entity;

    using PhotoContest.Models;

    public interface IPhotoContestContext
    {
        IDbSet<Contest> Contests { get; set; }

        IDbSet<Picture> Pictures { get; set; }

        IDbSet<Prize> Prizes { get; set; }

        IDbSet<Vote> Votes { get; set; }

        IDbSet<VotingCommittee> Commettees { get; set; }

        int SaveChanges();
    }
}
