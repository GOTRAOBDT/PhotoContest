namespace PhotoContest.Tests.Mocks
{
    using PhotoContest.Models;
    using Data.Contracts;

    public class PhotoContestDataMock : IPhotoContestData
    {
        private IRepository<User> usersRepositoryMock = new GenericRepositoryMock<User>();
        private IRepository<Contest> contestsRepositoryMock = new GenericRepositoryMock<Contest>();
        private IRepository<Picture> pictureRepositoryMock = new GenericRepositoryMock<Picture>();
        private IRepository<Prize> prizesRepositoryMock = new GenericRepositoryMock<Prize>();
        private IRepository<Vote> votesRepositoryMock = new GenericRepositoryMock<Vote>();
        private IRepository<Notification> notificationsRepositoryMock = new GenericRepositoryMock<Notification>();
        private IRepository<VotingCommittee> votingCommitteesRepositoryMock = new GenericRepositoryMock<VotingCommittee>();

        public bool ChangesSaved { get; set; }

        public IRepository<User> Users
        {
            get { return this.usersRepositoryMock; }
        }

        public IRepository<Contest> Contests
        {
            get { return this.contestsRepositoryMock; }
        }

        public IRepository<Picture> Pictures
        {
            get { return this.pictureRepositoryMock; }
        }

        public IRepository<Prize> Prizes
        {
            get { return this.prizesRepositoryMock; }
        }

        public IRepository<Vote> Votes
        {
            get { return this.votesRepositoryMock; }
        }

        public IRepository<VotingCommittee> Committees
        {
            get { return this.votingCommitteesRepositoryMock; }
        }

        public IRepository<Notification> Notifications
        {
            get { return this.notificationsRepositoryMock; }
        }

        public int SaveChanges()
        {
            this.ChangesSaved = true;
            return 1;
        }
    }
}
