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

    using PhotoContest.App.Models.Account;
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

        [TestMethod]
        public void CallingIndexActionShouldReturnViewResultAndIEnumerableOfSummuryContestViewModel()
        {
            var result = this.meController.Index(null, null);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<SummaryContestViewModel>));
        }

        [TestMethod]
        public void CallingIndexActionWithNoSortByAndFilterByOptionsShouldReturnByDefaultActiveEntitiesOrderdByPicturesCountAndThenByVotesCount()
        {
            this.LoginMock(true);
            var result = this.meController.Index(null, null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active && c.OwnerId == this.user.Id)
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
        public void CallingIndexActionWithNoSortByAndFilterByComingSoonOptionsShouldReturnCorectEntities()
        {
            var result = this.meController.Index(null, "coming-soon");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Inactive && c.OwnerId == this.user.Id)
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
        public void CallingIndexActionWithNoSortByAndFilterByFinishedOptionsShouldReturnCorectEntities()
        {
            var result = this.meController.Index(null, "finished");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Finished && c.OwnerId == this.user.Id)
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
        public void CallingIndexActionWithNoSortByAndFilterByInvalidCriterionOptionsShouldReturnActiveEntitiesByDefault()
        {
            var result = this.meController.Index(null, "invalidFilter");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active && c.OwnerId == this.user.Id)
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
        public void CallingIndexActionWithSortByNewestAndNoFilterByOptionsShouldReturnActiveEntitiesOrderedByNewestOpened()
        {
            var result = this.meController.Index("newest", null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
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
        public void CallingIndexActionWithSortByInvalidSortCriterionAndNoFilterByOptionsShouldReturnByDefaultActiveEntitiesOrderedByPicturesCountAndThenByVotesCount()
        {
            var result = this.meController.Index("invalidSort", null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Active && c.OwnerId == this.user.Id)
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
        public void CallingIndexActionWithSortByNewestAndFilterByFinishedOptionsShouldReturnCorrectEntities()
        {
            var result = this.meController.Index("newest", "finished");
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as List<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.Status == ContestStatus.Finished && c.OwnerId == this.user.Id)
                .OrderBy(c => TestableDbFunctions.DiffMinutes(c.StartDate, DateTime.Now))
                .ToList();

            Assert.AreEqual(fakeContestsList.Count(), actualModelList.Count());

            for (int i = 0; i < fakeContestsList.Count; i++)
            {
                Assert.AreEqual(fakeContestsList[i].Id, actualModelList[i].Id);
            }
        }

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

        [TestMethod]
        public void CallingUploadPictureActionWithoutModelShouldReturnViewResultWithoutModel()
        {
            var result = this.meController.UploadPicture();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsNull(viewResult.Model);
        }

        //[TestMethod]
        //public void CallingUploadPictureActionWithCorrectUploadPictureBindingModelShouldShouldAddPicture()
        //{
        //    var result = this.meController.UploadPicture();
        //    Assert.IsInstanceOfType(result, typeof(ViewResult));

        //    var viewResult = result as ViewResult;
        //    Assert.IsNull(viewResult.Model);
        //}

        [TestMethod]
        public void CallingEditProfileActionWithoutModelShouldReturnViewResultWithEditProfileBindingModel()
        {
            var result = this.meController.EditProfile();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(EditProfileBindingModel));
        }

        [TestMethod]
        public void CallingEditProfileActionWithCorrectModelShouldSuccesfullyEditUserProfileAndRedirectsToHomeIndex()
        {
            this.LoginMock(true);
            var editProfileBindingModel = new EditProfileBindingModel
            {
                Name = "Edited Name",
                Email = "edited@mail.com",
                BirthDate = DateTime.Now
            };
            var result = this.meController.EditProfile(editProfileBindingModel);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            var routeResult = result as RedirectToRouteResult;
            Assert.AreEqual(routeResult.RouteValues["controller"], "Home");
            Assert.AreEqual(routeResult.RouteValues["action"], "Index");

            var fakeUser = this.fakeUsers.FirstOrDefault();
            Assert.AreEqual(editProfileBindingModel.Name, fakeUser.Name);
            Assert.AreEqual(editProfileBindingModel.Name, fakeUser.Email);
            Assert.AreEqual(editProfileBindingModel.Name, fakeUser.BirthDate);
        }

        [TestMethod]
        public void CallingEditProfileActionWithInCorrectModelShouldNotMakeChangesToDatabase()
        {
            this.LoginMock(true);
            var editProfileBindingModel = new EditProfileBindingModel
            {
                Name = null,
                Email = "edited@mail.com",
                BirthDate = DateTime.Now
            };

            this.meController.ModelState.AddModelError("Name", "Current value: " + editProfileBindingModel.Name);
            var result = this.meController.EditProfile(editProfileBindingModel);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(EditProfileBindingModel));

            var fakeUser = this.fakeUsers.FirstOrDefault();
            Assert.AreNotEqual(editProfileBindingModel.Name, fakeUser.Name);
            Assert.AreNotEqual(editProfileBindingModel.Name, fakeUser.Email);
            Assert.AreNotEqual(editProfileBindingModel.Name, fakeUser.BirthDate);
        }

        //TODO Add more tests

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
