using FigureMeUp.Controllers;
using FigureMeUp.Controllers.IControllers;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    [TestFixture]
    public class Tests_FiguresControlles
    {
        private Mock<IFigureService> _mockFigureService;
        private FiguresController _controller;

        [SetUp]
        public void Setup()
        {
            _mockFigureService = new Mock<IFigureService>();
            _controller = new FiguresController(_mockFigureService.Object);

            var user = TestsHelper.CreateTestClaimsPrincipal();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithActiveFigures()
        {
            // Arrange
            var figures = new List<Figure>
            {
                TestsHelper.CreateTestFigure(isDeleted: false),
                TestsHelper.CreateTestFigure(isDeleted: true),
                TestsHelper.CreateTestFigure(isDeleted: false)
            };

            _mockFigureService.Setup(x => x.GetAllFiguresAsync())
                .ReturnsAsync(figures);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<Figure>;
            Assert.That(model.Count, Is.EqualTo(2)); // Only non-deleted figures
            Assert.That(model.All(f => !f.IsDeleted), Is.True);
        }

        [Test]
        public async Task Details_ExistingId_ReturnsViewWithFigure()
        {
            // Arrange
            var figure = TestsHelper.CreateTestFigure();
            _mockFigureService.Setup(x => x.GetFigureByIdAsync(figure.Id))
                .ReturnsAsync(figure);

            // Act
            var result = await _controller.Details(figure.Id);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.EqualTo(figure));
        }

        [Test]
        public async Task Details_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockFigureService.Setup(x => x.GetFigureByIdAsync(id))
                .ReturnsAsync((Figure)null);

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var model = new FiguresViewModel
            {
                Name = "Test Figure",
                Description = "Test Description",
                ImageUrls = new List<string> { "https://test.com/image.jpg" },
                Rarity = "Common"
            };

            _mockFigureService.Setup(x => x.AddFigureAsync(It.IsAny<FiguresViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(FiguresController.Index)));
        }

        [Test]
        public async Task ToggleLike_ValidRequest_ReturnsJsonSuccess()
        {
            // Arrange
            var figure = TestsHelper.CreateTestFigure();
            figure.LikedByUsersIds.Add("test-user-1");
            figure.LikesCount = 1;

            _mockFigureService.Setup(x => x.GetFigureByIdAsync(figure.Id))
                .ReturnsAsync(figure);
            _mockFigureService.Setup(x => x.ToggleLikeAsync(figure.Id, "test-user-1"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ToggleLike(figure.Id.ToString());

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var value = jsonResult.Value;
            // Use reflection to check the anonymous object properties
            var successProperty = value.GetType().GetProperty("success");
            Assert.That(successProperty?.GetValue(value), Is.True);
        }

        [Test]
        public async Task Delete_AuthorizedUser_RedirectsToIndex()
        {
            // Arrange
            var figure = TestsHelper.CreateTestFigure("test-user-1");
            _mockFigureService.Setup(x => x.GetFigureByIdAsync(figure.Id))
                .ReturnsAsync(figure);
            _mockFigureService.Setup(x => x.DeleteFigureAsync(figure.Id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(figure.Id);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(FiguresController.Index)));
        }
    }
}
