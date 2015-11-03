namespace PhotoContest.Tests.UnitTests
{
    using System.Linq;
    using System.Web.Mvc;

    using App.Controllers;
    using App.Models.Pictures;
    using Data.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Models;

    using Moq;

    [TestClass]
    public class PictureControllerTests
    {
        private MockContainer mock;
        private IQueryable<Picture> fakePictures;
        private Mock<IPhotoContestData> mockContext;
        private PicturesController pictureController;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMocks();
            this.fakePictures = this.mock.PictureRepositoryMock.Object.All();
            this.mockContext = new Mock<IPhotoContestData>();
            this.mockContext.Setup(c => c.Pictures.All())
                .Returns(this.fakePictures);

            this.pictureController = new PicturesController(this.mockContext.Object);
        }

        //[TestMethod]
        //public void CallingIndexActionWithExistingPictureIdShouldReturnViewResultAndDetailsPictureViewModel()
        //{
        //    var result = this.pictureController.Index(1, null);
        //    Assert.IsInstanceOfType(result, typeof(ViewResult));

        //    var viewResult = result as ViewResult;
        //    Assert.IsInstanceOfType(viewResult.Model, typeof(DetailsPictureViewModel));
        //}

        //[TestMethod]
        //public void CallingIndexActionWithExistingPictureIdShouldReturnCorrectPicture()
        //{
        //    var returnedPicture = (this.pictureController.Index(1, null) as ViewResult).Model as DetailsPictureViewModel;
        //    var fakePicture = this.fakePictures.FirstOrDefault(p => p.Id == 1);

        //    Assert.AreEqual(fakePicture.Id, returnedPicture.Id);
        //}

        //[TestMethod]
        //public void CallingIndexActionWithNotExistingPictureIdShouldReturnNotFound()
        //{
        //    var result = this.pictureController.Index(-1, null);
        //    Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        //}
    }
}
