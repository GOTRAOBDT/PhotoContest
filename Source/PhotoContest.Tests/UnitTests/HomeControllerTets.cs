namespace PhotoContest.Tests.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Contest;

    using Data.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HomeControllerTets
    {
        private MockContainer mock;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMocks();
        }

        [TestMethod]
        public void HomeIndexShouldReturnSummuryContestViewModel()
        {
            var fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            var mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            var homeController = new HomeController(mockContext.Object);

            var result = homeController.Index(null);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<SummaryContestViewModel>));
            
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = fakeContests.ToList();
            Assert.AreEqual(fakeContests.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }
    }
}
