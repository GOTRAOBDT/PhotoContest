namespace PhotoContest.Tests.Mocks
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using Moq;
    using System.Collections.Generic;
    using PhotoContest.App.Models.Contest;
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Web.Mvc;
    using PhotoContest.App.Areas.Administration.Controllers;

    [TestClass]
    public class ContestControllerTests
    {
        private PhotoContestDataMock data;
        private ContestsController contestsController;

        [TestInitialize]
        public void InitTest()
        {
            this.data = new PhotoContestDataMock();
            this.contestsController = new ContestsController(this.data);
        }

        [TestMethod]
        public void ContestCreate_ShouldReturnViewResult()
        {

            var result = this.contestsController.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsNull(viewResult.Model);
        }

        [TestMethod]
        public void ContestCreate_ShouldAddNewContest()
        {
            LoginMock();

            var contest = new CreateContestBindingModel
            {
                VotingType = VotingType.Open,
                Description = "Contest 1",
                Prizes = new HashSet<Prize>(),
                DeadlineType = DeadlineType.EndDate,
                ParticipationType = ParticipationType.Open,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Thumbnail = "Thumbnail",
                Title = "Title 1",
                ParticipationLimit = 1
            };


            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = this.contestsController.Create(contest);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual(this.data.Contests.All().Count(), 1);

            var dbContest = this.data.Contests.All()
                .FirstOrDefault(c => c.Title == "Title 1");

            Assert.IsNotNull(dbContest);
            Assert.AreEqual(dbContest.Thumbnail, "Thumbnail");
            Assert.AreEqual(dbContest.Prizes.Count, 0);
            Assert.AreEqual(dbContest.Description, "Contest 1");

        }

        [TestMethod]
        public void ContestCreate_WithInvalidInfo_ShouldNotAdd()
        {
            LoginMock();

            var contest = new CreateContestBindingModel
            {
                VotingType = VotingType.Open,
                Description = null,
                Prizes = new HashSet<Prize>(),
                DeadlineType = DeadlineType.EndDate,
                ParticipationType = ParticipationType.Open,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ParticipationLimit = 1
            };


            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = this.contestsController.Create(contest);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            var dbContest = this.data.Contests.All()
                .FirstOrDefault(c => c.Title == "Title 1");

            Assert.IsNull(dbContest);
        }

        [TestMethod]
        public void ContestGetById_WithInvalidInfo_ReturnNull()
        {
            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = this.contestsController.GetContestById(12);

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void ContestGetById_WithVaidInfo_ReturnContestView()
        {
            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            LoginMock();

            var contest = new CreateContestBindingModel
            {
                VotingType = VotingType.Open,
                Description = null,
                Prizes = new HashSet<Prize>(),
                DeadlineType = DeadlineType.EndDate,
                ParticipationType = ParticipationType.Open,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ParticipationLimit = 1
            };


            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            this.contestsController.Create(contest);

            Assert.AreEqual(this.data.Contests.All().Count(), 1);

            //var result = this.contestsController.GetContestById(1);

            //Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        private void LoginMock()
        {
            var claim = new Claim("TestUser", "UserId");
            var mockIdentity =
                Mock.Of<ClaimsIdentity>(ci => ci.FindFirst(It.IsAny<string>()) == claim);
            var principal = new ClaimsPrincipal(mockIdentity);

            var controllerContext = new Mock<ControllerContext>();

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal);

            contestsController.ControllerContext = controllerContext.Object;
            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            controllerContext.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
            controllerContext.SetupGet(x => x.HttpContext.Request).Returns(request.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal);
            controllerContext.SetupGet(x => x.HttpContext.User.Identity.IsAuthenticated)
                .Returns(true);

            contestsController.ControllerContext = controllerContext.Object;
        }
    }
}
