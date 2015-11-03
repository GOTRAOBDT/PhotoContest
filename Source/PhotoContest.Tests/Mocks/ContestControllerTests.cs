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

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = AddContest();
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual(this.data.Contests.All().Count(), 1);

            var dbContest = this.data.Contests.All()
                .FirstOrDefault(c => c.Title == "Title 1");

            Assert.IsNotNull(dbContest);
            Assert.AreEqual(dbContest.Thumbnail, "Thumbnail");
            Assert.AreEqual(dbContest.Prizes.Count, 0);
            Assert.AreEqual(dbContest.Description, "Description 1");

        }

        [TestMethod]
        public void ContestCreate_WithInvalidInfo_ShouldNotAdd()
        {
            LoginMock();

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = AddInvalidContest();
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            var dbContest = this.data.Contests.All()
                .FirstOrDefault(c => c.Title == "Title 1");

            Assert.IsNull(dbContest);
        }

        [TestMethod]
        public void ContestGetById_WithInvalidInfo_ReturnNotFound()
        {
            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = this.contestsController.GetContestById(12);

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void ContestGetById_WithVaidInfo_ReturnContestView()
        {
            LoginMock();

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            AddContest();

            Assert.AreEqual(this.data.Contests.All().Count(), 1);

            //var result = this.contestsController.GetContestById(1);

            //Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ManageContest_WhichDoestNotExist_ShouldThrowInvalidOperationException()
        {
            LoginMock();

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = this.contestsController.Manage(1);
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

        private ActionResult AddContest()
        {
            var contest = new CreateContestBindingModel
            {
                Title = "Title 1",
                VotingType = VotingType.Open,
                Description = "Description 1",
                Prizes = new HashSet<Prize>(),
                DeadlineType = DeadlineType.EndDate,
                ParticipationType = ParticipationType.Open,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ParticipationLimit = 1,
                Thumbnail = "Thumbnail",
            };
            return this.contestsController.Create(contest);
        }

        private ActionResult AddInvalidContest()
        {
            var contest = new CreateContestBindingModel
            {
                VotingType = VotingType.Open,
                Description = "Description 1",
                Prizes = new HashSet<Prize>(),
                DeadlineType = DeadlineType.EndDate,
                ParticipationType = ParticipationType.Open,
                StartDate = DateTime.Now,
            };
            return this.contestsController.Create(contest);
        }
    }
}
