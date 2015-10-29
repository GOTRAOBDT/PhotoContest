namespace PhotoContest.Data.Contracts
{
    using Models;

    public interface IPhotoContestData
    {
        IRepository<User> Users { get; }

        IRepository<Contest> Contests { get; }

        IRepository<Picture> Pictures { get; }

        IRepository<Prize> Prizes { get; }

        IRepository<Vote> Votes { get; }

        IRepository<VotingCommittee> Committees { get; }

        IRepository<Notification> Notifications { get; }


        int SaveChanges();
    }
}
