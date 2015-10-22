namespace PhotoContest.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Contest;
    using Data.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Models;

    using Moq;

    using PhotoContest.App.Models.Pictures;
    using PhotoContest.Common;
    using PhotoContest.Models.Enumerations;

    [TestClass]
    public class MeControllerTests
    {
        private MockContainer mock;
        private IQueryable<Contest> fakeContests;
        private IQueryable<Picture> fakePictures;
        private IQueryable<User> fakeUsers;
        private Mock<IPhotoContestData> mockContext;
        private MeController meController;
        private User user;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMocks();
            this.fakeContests = this.mock.ContestsRepositoryMock.Object.All();
            this.fakePictures = this.mock.PictureRepositoryMock.Object.All();
            this.fakeUsers = this.mock.UsersRepositoryMock.Object.All();
            this.user = this.fakeUsers.FirstOrDefault();
            this.mockContext = new Mock<IPhotoContestData>();
            this.mockContext.Setup(c => c.Contests.All())
                .Returns(this.fakeContests);
            this.mockContext.Setup(c => c.Pictures.All())
                .Returns(this.fakePictures);

            this.meController = new MeController(this.mockContext.Object);
        }

        //[TestMethod]
        //public void CallingSomeActionUnauthorizedShouldReturnNotAuthorized()
        //{
        //    this.LoginMock(true);
        //    var result = this.meController.Contests();
        //    Assert.IsInstanceOfType(result, typeof(HttpUnauthorizedResult));
        //}

        [TestMethod]
        public void CallingContestsActionShouldReturnViewResultAndIEnumerableOfSummaryContestViewModel()
        {
            var result = this.meController.Contests();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<SummaryContestViewModel>));
        }

        [TestMethod]
        public void CallingContestsActionShouldReturnLoggedUsersActiveSummaryContestViewModelEntitiesOrderedByNewestFirst()
        {
            this.LoginMock(true);
            var result = this.meController.Contests() as ViewResult;
            var actualModelList = result.Model as List<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active && c.OwnerId == this.user.Id)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ToList();


            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void CallingPicturesActionShouldReturnViewResultAndIEnumerableOfSummaryPictureViewModel()
        {
            var result = this.meController.Pictures();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<SummaryPictureViewModel>));
        }

        [TestMethod]
        public void CallingPicturesActionShouldReturnLoggedUsersUploadedPicturesOrderedByNewestFirst()
        {
            this.LoginMock(true);
            var result = this.meController.Pictures() as ViewResult;
            var actualModelList = result.Model as List<SummaryPictureViewModel>;
            var fakePicturesList = this.fakePictures
                .Where(c => c.AuthorId == this.user.Id)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.PostedOn, DateTime.Now))
                .ToList();
            
            Assert.AreEqual(fakePicturesList.Count(), actualModelList.Count());

            for (int i = 0; i < fakePicturesList.Count; i++)
            {
                Assert.AreEqual(fakePicturesList[i].Id, actualModelList[i].Id);
            }
        }

        private void LoginMock(bool isAuthenticated)
        {
            var claim = new Claim("Id", this.user.Id);
            var mockIdentity =
                Mock.Of<ClaimsIdentity>(ci => ci.FindFirst(It.IsAny<string>()) == claim);
            var principal = new ClaimsPrincipal(mockIdentity);

            var controllerContext = new Mock<ControllerContext>();

            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            controllerContext.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
            controllerContext.SetupGet(x => x.HttpContext.Request).Returns(request.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal);
            controllerContext.SetupGet(x => x.HttpContext.Request.IsAuthenticated)
                .Returns(isAuthenticated);

            this.meController.ControllerContext = controllerContext.Object;
        }
    }
}
