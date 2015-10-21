namespace PhotoContest.Tests.UnitTests
{
    using System.Linq;
    using PhotoContest.Models;
    using System.Web.Mvc;
    using Moq;
    using PhotoContest.App.Controllers;
    using PhotoContest.Data.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContestControllerTests
    {
        private MockContainer mock;
        private IQueryable<Contest> fakeContests;
        private Mock<IPhotoContestData> mockContext;
        private ContestsController contestController;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMocks();

            this.fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            this.mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            this.contestController = new ContestsController(mockContext.Object);
        }

        [TestMethod]
        public void ContextCreateShouldReturnViewResult()
        {
            var result = contestController.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            //Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<SummaryContestViewModel>));
        }
    }
}
