using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using PhotoContest.App.Models.Contest;
using PhotoContest.Models.Enumerations;
using PhotoContest.Tests.Mocks;

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
        private User user;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMocks();
            this.user = new User
            {
                Id = "1111",
                UserName = "test",
                Email = "test@test.com"
            };
            this.fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            this.mockContext = new Mock<IPhotoContestData>();
            mockContext.Setup(c => c.Contests.All())
                .Returns(fakeContests);

            this.contestController = new ContestsController(mockContext.Object);
        }

        [TestMethod]
        public void ContestCreate_ShouldAddNewContest()
        {
            var contest = new CreateContestBindingModel
            {
                DeadlineType = DeadlineType.EndDate,
                EndDate = DateTime.MaxValue,
                Description = "Krasivi zalezi",
                ParticipationType = ParticipationType.Open,
                Prizes = new HashSet<Prize>()
                {
                    new Prize()
                    {
                        Description = "$1000",
                        Id = 1,
                        Contest = this.mock.ContestsRepositoryMock.Object.All().First()
                    }
                },
                StartDate = DateTime.Today,
                Title = "Fast",
                VotingType = VotingType.Open,
                Thumbnail = "helloworld.png"
            };

            LoginMock();
            Assert.AreEqual(contestController.User.Identity.IsAuthenticated, true);

            Assert.AreEqual(this.mockContext.Object.Contests.All().Count(), 4);

            var result = this.contestController.Create(contest);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(CreateContestBindingModel));

            Assert.AreEqual(this.mockContext.Object.Contests.All().Count(), 5);
        }

        private void LoginMock()
        {
            var claim = new Claim(user.UserName, user.Id);
            var mockIdentity =
                Mock.Of<ClaimsIdentity>(ci => ci.FindFirst(It.IsAny<string>()) == claim);
            var principal = new ClaimsPrincipal(mockIdentity);

            var controllerContext = new Mock<ControllerContext>();

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal);

            contestController.ControllerContext = controllerContext.Object;
            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            controllerContext.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
            controllerContext.SetupGet(x => x.HttpContext.Request).Returns(request.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal);
            controllerContext.SetupGet(x => x.HttpContext.User.Identity.IsAuthenticated)
                .Returns(true);

            contestController.ControllerContext = controllerContext.Object;
        }
    }
}
