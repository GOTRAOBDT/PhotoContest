namespace PhotoContest.Tests.UnitTests
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Account;
    using App.Models.Contest;
    using App.Models.Pictures;

    using AutoMapper.QueryableExtensions;

    using Common;

    using Data.Contracts;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Models;

    using Moq;

    using PagedList;

    [TestClass]
    public class MeControllerTests
    {
        private MockContainer mock;
        private IQueryable<Contest> fakeContests;
        private IQueryable<Picture> fakePictures;
        private IQueryable<MaintanceLog> fakeMaintanceLogs;
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
            this.fakeMaintanceLogs = this.mock.MaintanceLogRepositoryMock.Object.All();
            this.fakeUsers = this.mock.UsersRepositoryMock.Object.All();
            this.user = this.fakeUsers.FirstOrDefault();
            this.mockContext = new Mock<IPhotoContestData>();
            this.mockContext.Setup(c => c.Contests.All())
                .Returns(this.fakeContests);
            this.mockContext.Setup(c => c.Pictures.All())
                .Returns(this.fakePictures);
            this.mockContext.Setup(c => c.MaintanceLogs.All())
                .Returns(this.fakeMaintanceLogs);
            this.mockContext.Setup(c => c.Users.All())
                .Returns(this.fakeUsers);
            this.mockContext.Setup(c => c.Users.Find(It.IsAny<string>()))
                .Returns((string id) =>
                {
                    return this.fakeUsers.FirstOrDefault(n => n.Id == id);
                });
            this.mockContext.Setup(c => c.Pictures.Add(It.IsAny<Picture>()))
                .Callback((Picture picture) =>
                {
                    this.mock.PictureRepositoryMock.Object.Add(picture);
                });

            this.meController = new MeController(this.mockContext.Object);
        }

        [TestMethod]
        public void CallingContestsActionShouldReturnViewResultAndIPagedListOfSummuryContestViewModel()
        {
            this.LoginMock(true);
            var result = this.meController.Contests(null);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IPagedList<SummaryContestViewModel>));
        }

        [TestMethod]
        public void CallingContestsActionShouldReturnByDefaultActiveEntitiesOrderdByPicturesCountAndThenByVotesCount()
        {
            this.LoginMock(true);
            var result = this.meController.Contests(null);
            var viewResult = result as ViewResult;
            var actualModelList = viewResult.Model as IPagedList<SummaryContestViewModel>;
            var fakeContestsList = this.fakeContests
                .Where(c => c.OwnerId == this.user.Id)
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
        public void CallingPicturesActionShouldReturnViewResultAndIPagedListOfSummaryPictureViewModel()
        {
            this.LoginMock(true);
            var result = this.meController.Pictures(null, null);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IPagedList<SummaryPictureViewModel>));
        }

        [TestMethod]
        public void CallingPicturesActionShouldReturnLoggedUsersUploadedPicturesOrderedByNewestFirst()
        {
            this.LoginMock(true);
            var result = this.meController.Pictures(null, null) as ViewResult;
            var actualModelList = result.Model as IPagedList<SummaryPictureViewModel>;
            var fakePicturesList = this.fakePictures
                .Where(p => p.Author.Id == this.user.Id)
                .OrderByDescending(p => p.PostedOn)
                .ThenByDescending(c => c.Contests.Count())
                .ProjectTo<SummaryPictureViewModel>()
                .ToList();
            
            Assert.AreEqual(fakePicturesList.Count(), actualModelList.Count());

            for (int i = 0; i < fakePicturesList.Count; i++)
            {
                Assert.AreEqual(fakePicturesList[i].Id, actualModelList[i].Id);
            }
        }

        [TestMethod]
        public void CallingUploadPictureActionWithInocrrectModelShouldReturnViewResultWithModel()
        {
            this.LoginMock(true);
            var incorrectNewPicture = new UploadPictureBindingModel
            {
                PictureData = null,
                Title = "Title"
            };

            this.meController.ModelState.AddModelError("PictureData", "Current value: null");

            var result = this.meController.UploadPicture(incorrectNewPicture);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(UploadPictureBindingModel));
        }

        [TestMethod]
        public void CallingUploadPictureActionWithCorrectUploadPictureBindingModelShouldShouldAddPicture()
        {
            this.LoginMock(true);
            var fakePicturesCountBeforeAdding = this.fakePictures.Count();
            var newPicture = new UploadPictureBindingModel { PictureData = "base64", Title = "newPicture" };
            var result = this.meController.UploadPicture(newPicture);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            var routeResult = result as RedirectToRouteResult;
            Assert.AreEqual(routeResult.RouteValues["action"], "Pictures");

            var fakePicturesCountAfterAdding = this.mock.PictureRepositoryMock.Object.All().Count();
            Assert.AreEqual(fakePicturesCountBeforeAdding + 1, fakePicturesCountAfterAdding);
        }

        [TestMethod]
        public void CallingEditProfileActionWithoutModelShouldReturnViewResultWithEditProfileBindingModel()
        {
            this.LoginMock(true);
            var result = this.meController.Profile();
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
                BirthDate = DateTime.Now
            };
            var result = this.meController.Profile(editProfileBindingModel);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            var routeResult = result as RedirectToRouteResult;
            Assert.AreEqual(routeResult.RouteValues["controller"], "Home");
            Assert.AreEqual(routeResult.RouteValues["action"], "Index");

            var fakeUser = this.fakeUsers.FirstOrDefault();
            Assert.AreEqual(editProfileBindingModel.Name, fakeUser.Name);
            Assert.AreEqual(editProfileBindingModel.BirthDate, fakeUser.BirthDate);
        }

        [TestMethod]
        public void CallingEditProfileActionWithCorrectModelAndProfilePictureShouldSuccesfullyEditUserProfileAndRedirectsToHomeIndex()
        {
            this.LoginMock(true);
            var editProfileBindingModel = new EditProfileBindingModel
            {
                Name = "Edited Name",
                BirthDate = DateTime.Now,
                ProfilePicture = "someBase64string"
            };
            var result = this.meController.Profile(editProfileBindingModel);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            var routeResult = result as RedirectToRouteResult;
            Assert.AreEqual(routeResult.RouteValues["controller"], "Home");
            Assert.AreEqual(routeResult.RouteValues["action"], "Index");

            var fakeUser = this.fakeUsers.FirstOrDefault();
            Assert.AreEqual(editProfileBindingModel.Name, fakeUser.Name);
            Assert.AreEqual(editProfileBindingModel.BirthDate, fakeUser.BirthDate);
            Assert.AreEqual(editProfileBindingModel.ProfilePicture, fakeUser.ProfilePicture);
        }

        [TestMethod]
        public void CallingEditProfileActionWithInCorrectModelShouldNotMakeChangesToDatabase()
        {
            this.LoginMock(true);
            var editProfileBindingModel = new EditProfileBindingModel
            {
                Name = null,
                BirthDate = DateTime.Now
            };

            this.meController.ModelState.AddModelError("Name", "Current value: " + editProfileBindingModel.Name);
            var result = this.meController.Profile(editProfileBindingModel);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(EditProfileBindingModel));

            var fakeUser = this.fakeUsers.FirstOrDefault();
            Assert.AreNotEqual(editProfileBindingModel.Name, fakeUser.Name);
            Assert.AreNotEqual(editProfileBindingModel.BirthDate, fakeUser.BirthDate);
        }

        private void LoginMock(bool isAuthenticated)
        {
            var identity = new GenericIdentity(this.user.UserName);
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", this.user.Id));
            var principal = new GenericPrincipal(identity, new[] { "user" });

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

        //TODO Add more tests
    }
}
