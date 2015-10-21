namespace PhotoContest.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Contest;

    using Data.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Models.Enumerations;

    using Moq;

    using PhotoContest.App.Common;

    [TestClass]
    public class HomeControllerTests
    {
        private MockContainer mock;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMocks();
        }

        [TestMethod]
        public void HomeIndexShouldReturnViewResultAndIEnumerableOfSummuryContestViewModel()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index(null, null);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<SummaryContestViewModel>));
        }

        [TestMethod]
        public void HomeIndexWithNoSortByAndFilterByOptionsShouldReturnByDefaultActiveEntitiesOrderdByPicturesCountAndThenByVotesCount()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index(null, null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithNoSortByAndFilterByComingSoonOptionsShouldReturnCorectEntities()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index(null, "coming-soon");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests
                .Where(c => c.Status == ContestStatus.Inactive)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithNoSortByAndFilterByFinishedOptionsShouldReturnCorectEntities()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index(null, "finished");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests
                .Where(c => c.Status == ContestStatus.Finished)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithNoSortByAndFilterByInvalidCriterionOptionsShouldReturnActiveEntitiesByDefault()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index(null, "invalidFilter");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }


        [TestMethod]
        public void HomeIndexWithSortByNewestAndNoFilterByOptionsShouldReturnActiveEntitiesOrderedByNewestOpened()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index("newest", null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithSortByInvalidSortCriterionAndNoFilterByOptionsShouldReturnByDefaultActiveEntitiesOrderedByPicturesCountAndThenByVotesCount()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index("invalidSort", null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithSortByNewestAndFilterByFinishedOptionsShouldReturnCorrectEntities()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index("newest", "finished");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests
                .Where(c => c.Status == ContestStatus.Finished)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        //TODO Add more test for other orderBy and filterBy cases.
    }
}
