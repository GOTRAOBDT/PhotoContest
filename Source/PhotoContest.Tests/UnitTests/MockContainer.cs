namespace PhotoContest.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Data.Contracts;
    using Models;
    using Models.Enumerations;
    using Moq;
    
    public class MockContainer
    {
        public Mock<IRepository<User>> UsersRepositoryMock { get; set; }

        public Mock<IRepository<Contest>> ContestsRepositoryMock { get; set; }

        public Mock<IRepository<Picture>> PictureRepositoryMock { get; set; }

        public Mock<IRepository<Prize>> PrizesRepositoryMock { get; set; }

        public Mock<IRepository<Vote>> VotesRepositoryMock { get; set; }

        public Mock<IRepository<VotingCommittee>> VotingCommitteesRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetupFakeUsers();
            this.SetupFakeContests();
            this.SetupFakePictures();
            this.SetupFakePrizes();
            this.SetupFakeVotes();
            this.SetupFakeVotingCommittees();
        }

        private void SetupFakeUsers()
        {
            var fakeUsers = new List<User>
            {
                new User { Id = "1", Name = "Peter Petrov", UserName  = "peter", Email = "peter@abv.bg", Gender = UserGender.Male },
                new User { Id = "2", Name = "Ivan Ivanov", UserName  = "ivan", Email = "ivan@abv.bg", Gender = UserGender.Male },
                new User { Id = "3", Name = "Maria Georgieva", UserName  = "maria", Email = "maria@abv.bg", Gender = UserGender.Female }
            };

            this.UsersRepositoryMock = new Mock<IRepository<User>>();
            this.UsersRepositoryMock.Setup(r => r.All())
                .Returns(fakeUsers.AsQueryable());
            this.UsersRepositoryMock.Setup(r => r.Find(It.IsAny<string>()))
                .Returns((string id) =>
                {
                    return fakeUsers.FirstOrDefault(n => n.Id == id);
                });
            this.UsersRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    fakeUsers.Add(user);
                });
            this.UsersRepositoryMock.Setup(r => r.Update(It.IsAny<User>()))
                .Callback((User users) =>
                {
                    var userToRemove = fakeUsers.FirstOrDefault(n => n.Id == users.Id);
                    fakeUsers.Remove(userToRemove);
                    fakeUsers.Add(users);
                });
            this.UsersRepositoryMock.Setup(r => r.Delete(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    fakeUsers.Remove(user);
                });
        }

        private void SetupFakePictures()
        {
            var fakePictures = new List<Picture>
            {
                new Picture
                {
                    Id = 1,
                    AuthorId = "3",
                    PictureData = "http://www.picture.com/1",
                    PostedOn = DateTime.Now.AddDays(-30)
                },
                new Picture
                {
                    Id = 2,
                    AuthorId = "1",
                    PictureData = "http://www.picture.com/2",
                    PostedOn = DateTime.Now.AddDays(-20)
                },
                new Picture
                {
                    Id = 3,
                    AuthorId = "2",
                    PictureData = "http://www.picture.com/3",
                    PostedOn = DateTime.Now.AddDays(-10)
                }
            };

            this.PictureRepositoryMock = new Mock<IRepository<Picture>>();
            this.PictureRepositoryMock.Setup(r => r.All())
                .Returns(fakePictures.AsQueryable());
            this.PictureRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakePictures.FirstOrDefault(n => n.Id == id);
                });
            this.PictureRepositoryMock.Setup(r => r.Add(It.IsAny<Picture>()))
                .Callback((Picture picture) =>
                {
                    fakePictures.Add(picture);
                });
            this.PictureRepositoryMock.Setup(r => r.Update(It.IsAny<Picture>()))
                .Callback((Picture picture) =>
                {
                    var pictureToRemove = fakePictures.FirstOrDefault(n => n.Id == picture.Id);
                    fakePictures.Remove(pictureToRemove);
                    fakePictures.Add(picture);
                });
            this.PictureRepositoryMock.Setup(r => r.Delete(It.IsAny<Picture>()))
                .Callback((Picture picture) =>
                {
                    fakePictures.Remove(picture);
                });
        }

        private void SetupFakeContests()
        {
            var fakeContests = new List<Contest>
            {
                new Contest
                {
                    Id = 1,
                    Title = "Nature",
                    Description = "Photos of nature",
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now,
                    OwnerId = "2",
                    Status = ContestStatus.Active,
                    VotingType = VotingType.Open,
                    ParticipationType = ParticipationType.Open,
                    DeadlineType = DeadlineType.ParticipationLimit,
                    Thumbnail = "www.thumbnail.com/1"
                },
                new Contest
                {
                    Id = 2,
                    Title = "Portrets",
                    Description = "Portrets photos",
                    StartDate = DateTime.Now.AddMonths(-1),
                    EndDate = DateTime.Now,
                    OwnerId = "1",
                    Status = ContestStatus.Finished,
                    VotingType = VotingType.Closed,
                    ParticipationType = ParticipationType.Open,
                    DeadlineType = DeadlineType.EndDate,
                    Thumbnail = "www.thumbnail.com/2"
                },
                new Contest
                {
                    Id = 3,
                    Title = "Street",
                    Description = "Street photography",
                    StartDate = DateTime.Now.AddMonths(-2),
                    EndDate = DateTime.Now,
                    OwnerId = "3",
                    Status = ContestStatus.Inactive,
                    VotingType = VotingType.Open,
                    ParticipationType = ParticipationType.Open,
                    DeadlineType = DeadlineType.EndDate,
                    Thumbnail = "www.thumbnail.com/2"
                },
            };

            this.ContestsRepositoryMock = new Mock<IRepository<Contest>>();
            this.ContestsRepositoryMock.Setup(r => r.All())
                .Returns(fakeContests.AsQueryable());
            this.ContestsRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeContests.FirstOrDefault(n => n.Id == id);
                });
            this.ContestsRepositoryMock.Setup(r => r.Add(It.IsAny<Contest>()))
                .Callback((Contest contest) =>
                {
                    fakeContests.Add(contest);
                });
            this.ContestsRepositoryMock.Setup(r => r.Update(It.IsAny<Contest>()))
                .Callback((Contest contest) =>
                {
                    var pictureToRemove = fakeContests.FirstOrDefault(n => n.Id == contest.Id);
                    fakeContests.Remove(pictureToRemove);
                    fakeContests.Add(contest);
                });
            this.ContestsRepositoryMock.Setup(r => r.Delete(It.IsAny<Contest>()))
                .Callback((Contest contest) =>
                {
                    fakeContests.Remove(contest);
                });
        }

        private void SetupFakeVotingCommittees()
        {
            var fakeVotingCommittees = new List<VotingCommittee>();

            this.VotingCommitteesRepositoryMock = new Mock<IRepository<VotingCommittee>>();
            this.VotingCommitteesRepositoryMock.Setup(r => r.All())
                .Returns(fakeVotingCommittees.AsQueryable());
            this.VotingCommitteesRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeVotingCommittees.FirstOrDefault(n => n.Id == id);
                });
            this.VotingCommitteesRepositoryMock.Setup(r => r.Add(It.IsAny<VotingCommittee>()))
                .Callback((VotingCommittee votingCommittee) =>
                {
                    fakeVotingCommittees.Add(votingCommittee);
                });
            this.VotingCommitteesRepositoryMock.Setup(r => r.Update(It.IsAny<VotingCommittee>()))
                .Callback((VotingCommittee votingCommittee) =>
                {
                    var pictureToRemove = fakeVotingCommittees.FirstOrDefault(n => n.Id == votingCommittee.Id);
                    fakeVotingCommittees.Remove(pictureToRemove);
                    fakeVotingCommittees.Add(votingCommittee);
                });
            this.VotingCommitteesRepositoryMock.Setup(r => r.Delete(It.IsAny<VotingCommittee>()))
                .Callback((VotingCommittee votingCommittee) =>
                {
                    fakeVotingCommittees.Remove(votingCommittee);
                });
        }

        private void SetupFakeVotes()
        {
            var fakeVotes = new List<Vote>();

            this.VotesRepositoryMock = new Mock<IRepository<Vote>>();
            this.VotesRepositoryMock.Setup(r => r.All())
                .Returns(fakeVotes.AsQueryable());
            this.VotesRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeVotes.FirstOrDefault(n => n.Id == id);
                });
            this.VotesRepositoryMock.Setup(r => r.Add(It.IsAny<Vote>()))
                .Callback((Vote vote) =>
                {
                    fakeVotes.Add(vote);
                });
            this.VotesRepositoryMock.Setup(r => r.Update(It.IsAny<Vote>()))
                .Callback((Vote vote) =>
                {
                    var pictureToRemove = fakeVotes.FirstOrDefault(n => n.Id == vote.Id);
                    fakeVotes.Remove(pictureToRemove);
                    fakeVotes.Add(vote);
                });
            this.VotesRepositoryMock.Setup(r => r.Delete(It.IsAny<Vote>()))
                .Callback((Vote vote) =>
                {
                    fakeVotes.Remove(vote);
                });
        }

        private void SetupFakePrizes()
        {
            var fakePrizes = new List<Prize>();

            this.PrizesRepositoryMock = new Mock<IRepository<Prize>>();
            this.PrizesRepositoryMock.Setup(r => r.All())
                .Returns(fakePrizes.AsQueryable());
            this.PrizesRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakePrizes.FirstOrDefault(n => n.Id == id);
                });
            this.PrizesRepositoryMock.Setup(r => r.Add(It.IsAny<Prize>()))
                .Callback((Prize prize) =>
                {
                    fakePrizes.Add(prize);
                });
            this.PrizesRepositoryMock.Setup(r => r.Update(It.IsAny<Prize>()))
                .Callback((Prize prize) =>
                {
                    var pictureToRemove = fakePrizes.FirstOrDefault(n => n.Id == prize.Id);
                    fakePrizes.Remove(pictureToRemove);
                    fakePrizes.Add(prize);
                });
            this.PrizesRepositoryMock.Setup(r => r.Delete(It.IsAny<Prize>()))
                .Callback((Prize prize) =>
                {
                    fakePrizes.Remove(prize);
                });
        }
    }
}
