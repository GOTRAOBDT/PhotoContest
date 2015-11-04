namespace PhotoContest.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Contest;

    using AutoMapper.QueryableExtensions;

    using Common;
    using Data.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Models;
    using Models.Enumerations;

    using Moq;

    using PagedList;

    [TestClass]
    public class HomeControllerTests
    {
        private MockContainer mock;
        private IQueryable<Contest> fakeContests;
        private IQueryable<MaintanceLog> fakeMaintanceLogs;
        private Mock<IPhotoContestData> mockContext;
        private HomeController homeController;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMocks();

            this.fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            this.fakeMaintanceLogs = this.mock.MaintanceLogRepositoryMock.Object.All();
            this.mockContext = new Mock<IPhotoContestData>();
            this.mockContext.Setup(c => c.Contests.All())
                .Returns(this.fakeContests);

            this.mockContext.Setup(c => c.MaintanceLogs.All())
                .Returns(this.fakeMaintanceLogs);

            this.homeController = new HomeController(this.mockContext.Object);
        }

        [TestMethod]
        public void HomeIndexShouldReturnViewResultAndIEnumerableOfSummuryContestViewModel()
        {
            var result = this.homeController.Index(null, null);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IPagedList<SummaryContestViewModel>));
        }

        [TestMethod]
        public void HomeIndexWithNoSortByOptionShouldReturnByDefaultActiveEntitiesOrderdByPicturesCountAndThenByVotesCount()
        {
            var result = this.homeController.Index(null, null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as IPagedList<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ProjectTo<SummaryContestViewModel>()
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithSortByMostPopularShouldReturnCorectEntities()
        {
            var result = this.homeController.Index(null, "MostPopular");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as IPagedList<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ProjectTo<SummaryContestViewModel>()
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithSortByLatestShouldReturnCorectEntities()
        {
            var result = this.homeController.Index(null, "Latest");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as IPagedList<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ProjectTo<SummaryContestViewModel>()
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithSortByComingSoonShouldReturnCorectEntities()
        {
            var result = this.homeController.Index(null, "ComingSoon");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as IPagedList<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Inactive && TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now) < 0)
                .OrderByDescending(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ProjectTo<SummaryContestViewModel>()
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }


        [TestMethod]
        public void HomeIndexWithSortByEndingSoonShouldReturnCorectEntities()
        {
            var result = this.homeController.Index(null, "EndingSoon");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as IPagedList<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active)
                .OrderByDescending(c => TestableDbFunctions.DiffMinutes(c.EndDate, DateTime.Now))
                .ProjectTo<SummaryContestViewModel>()
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeIndexWithSortByArchiveShouldReturnCorectEntities()
        {
            var result = this.homeController.Index(null, "Archive");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as IPagedList<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Finished)
                .OrderByDescending(c => c.Pictures.Count)
                .ThenByDescending(c => c.Votes.Count)
                .ProjectTo<SummaryContestViewModel>()
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void HomeContactShouldReturnViewResult()
        {
            var result = this.homeController.Contact();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void HomeRulesShouldReturnViewResult()
        {
            var result = this.homeController.Rules();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        //TODO Add more test for other orderBy and filterBy cases.
    }
}
