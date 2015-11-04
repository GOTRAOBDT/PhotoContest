namespace PhotoContest.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;

    using AutoMapper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using PagedList;

    using PhotoContest.App.Areas.Administration.Controllers;
    using PhotoContest.App.CommonFunctions;
    using PhotoContest.App.Models.Account;
    using PhotoContest.App.Models.Contest;
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

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

            Mapper.CreateMap<Contest, EditContestBindingModel>();
            Mapper.CreateMap<Contest, DetailsContestViewModel>();
            Mapper.CreateMap<Prize, PrizeViewModel>();
            Mapper.CreateMap<User, BasicUserInfoViewModel>();
            Mapper.CreateMap<IPagedList<User>, IPagedList<BasicUserInfoViewModel>>()
                .ConvertUsing<PagedListConverter>();
            Mapper.CreateMap<IEnumerable<User>, IEnumerable<BasicUserInfoViewModel>>();
            Mapper.CreateMap<User, BasicUserInfoViewModel>();
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

            var result = this.contestsController.GetContestById(0);

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewRezult = result as ViewResult;

            Assert.IsNotNull(viewRezult);

            var model = viewRezult.Model;
            Assert.IsNotNull(model);

            var fullModel = model as FullContestViewModel;
            Assert.IsNotNull(fullModel);
            Assert.AreEqual(fullModel.ContestSummary.Title, "Title 1");
            Assert.AreEqual(fullModel.ContestSummary.Description, "Description 1");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Web.Http.HttpResponseException))]
        public void ManageContest_WhichDoestNotExist_ShouldArgumentException()
        {
            LoginMock();

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            var result = this.contestsController.Manage(1);
        }

        [TestMethod]
        public void ManageContest_WithValidData_ShouldReturnView()
        {
            LoginMock();

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            AddContest();


            var result = this.contestsController.Manage(0);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(EditContestBindingModel));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ManageContest_WithNullModel_ShouldThrowArgumentException()
        {
            LoginMock();

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            AddContest();

            var result = this.contestsController.Manage(0, null);
        }

        [TestMethod]
        public void ManageContest_WithValidData_ShouldUpdate()
        {
            LoginMock();

            Assert.AreEqual(this.data.Contests.All().Count(), 0);

            AddContest();

            var result = this.contestsController.Manage(0);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(EditContestBindingModel));

            var model = viewResult.Model as EditContestBindingModel;

            Assert.IsNotNull(model);

            model.Title = "New Title";
            model.Description = "New Description";

            result = this.contestsController.Manage(0, model);
            Assert.IsNotNull(result);

            var updatedContestGetResult = this.contestsController.GetContestById(0);

            var viewRezult = updatedContestGetResult as ViewResult;

            Assert.IsNotNull(viewRezult);

            var updatedContestModel = viewRezult.Model;
            Assert.IsNotNull(model);

            var fullModel = updatedContestModel as FullContestViewModel;
            Assert.IsNotNull(fullModel);
            Assert.AreEqual(fullModel.ContestSummary.Title, "New Title");
            Assert.AreEqual(fullModel.ContestSummary.Description, "New Description");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void GetContestJury_WithNotExistingJury_ShouldHttpRequestException()
        {
            LoginMock();

            this.contestsController.Jury(321);
        }

        [TestMethod]
        [ExpectedException(typeof(AutoMapperMappingException))]
        public void GetContestJury_WithExistingJury_ShouldReturnView()
        {
            LoginMock();

            AddContestWithJury();

            Assert.AreEqual(this.data.Contests.All().Count(), 1);

            this.contestsController.Jury(0);
        }

        [TestMethod]
        public void AddJuryMember_WithValidContestId_ShouldReturnView()
        {
            LoginMock();

            AddContestWithJury();

            Assert.AreEqual(this.data.Contests.All().Count(), 1);

            var result = this.contestsController.AddJuryMember(0);

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(AddJuryMemberBindingModel));

            var model = viewResult.Model as AddJuryMemberBindingModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.ContestId, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void GetContestJury_WithNonExistingJury_ShouldThrowHttpRequestException()
        {
            LoginMock();

            var result = this.contestsController.AddJuryMember(0);
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

        private void AddContestWithJury()
        {
            var contest = new CreateContestBindingModel
            {
                Title = "Title 1",
                VotingType = VotingType.Closed,
                Description = "Description 1",
                Prizes = new HashSet<Prize>(),
                DeadlineType = DeadlineType.EndDate,
                ParticipationType = ParticipationType.Open,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ParticipationLimit = 1,
                Thumbnail = "Thumbnail",
            };

            this.contestsController.Create(contest);


            this.data.Contests.Find(0).Jury.Members.Add(
                    new User()
                    {
                        UserName = "Pesho",
                        Id = "123"
                    }
                );

            this.data.SaveChanges();
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
