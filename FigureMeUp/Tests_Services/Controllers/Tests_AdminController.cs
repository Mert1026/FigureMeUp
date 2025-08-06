using FigureMeUp.Controllers;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    [TestFixture]
    public class Tests_AdminController
    {
        private Mock<IPostService> _mockPostService;
        private Mock<IFigureService> _mockFigureService;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<ILogger<AdminController>> _mockLogger;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _mockPostService = new Mock<IPostService>();
            _mockFigureService = new Mock<IFigureService>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _mockLogger = new Mock<ILogger<AdminController>>();

            _controller = new AdminController(
                _mockPostService.Object,
                _mockFigureService.Object,
                _mockUserManager.Object,
                _mockLogger.Object);

            var user = TestsHelper.CreateTestClaimsPrincipal("admin-user", "Admin");
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
        public async Task ManagePosts_ReturnsViewWithAllPosts()
        {
            var posts = new List<Post>
            {
                TestsHelper.CreateTestPost(isDeleted: false),
                TestsHelper.CreateTestPost(isDeleted: true)
            };

            _mockPostService.Setup(x => x.GetAllPostsAsync())
                .ReturnsAsync(posts);

            var result = await _controller.ManagePosts();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<Post>;
            Assert.That(model.Count(), Is.EqualTo(2)); // Should include both deleted and non-deleted
        }

        [Test]
        public async Task DeletePost_ExistingPost_RedirectsToManagePosts()
        {
            var postId = Guid.NewGuid();
            _mockPostService.Setup(x => x.DeletePostAsync(postId))
                .ReturnsAsync(true);

            var result = await _controller.DeletePost(postId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AdminController.ManagePosts)));
            _mockPostService.Verify(x => x.DeletePostAsync(postId), Times.Once);
        }

        [Test]
        public async Task RestorePost_ExistingPost_RedirectsToManagePosts()
        {
            var postId = Guid.NewGuid();
            _mockPostService.Setup(x => x.RestorePostAsync(postId))
                .ReturnsAsync(true);

            var result = await _controller.RestorePost(postId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AdminController.ManagePosts)));
            _mockPostService.Verify(x => x.RestorePostAsync(postId), Times.Once);
        }

        [Test]
        public async Task ManageFigures_ReturnsViewWithAllFigures()
        {
            var figures = new List<Figure>
            {
                TestsHelper.CreateTestFigure(isDeleted: false),
                TestsHelper.CreateTestFigure(isDeleted: true)
            };

            _mockFigureService.Setup(x => x.GetAllFiguresAsync())
                .ReturnsAsync(figures);

            var result = await _controller.ManageFigures();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<Figure>;
            Assert.That(model.Count(), Is.EqualTo(2)); // Should include both deleted and non-deleted
        }

        [Test]
        public async Task ManageUsers_ReturnsViewWithUserViewModels()
        {
            var users = new List<IdentityUser>
            {
                TestsHelper.CreateTestUser("user1", "User1"),
                TestsHelper.CreateTestUser("user2", "User2")
            };

            var posts = new List<Post>
            {
                TestsHelper.CreateTestPost("user1"),
                TestsHelper.CreateTestPost("user2")
            };

            var figures = new List<Figure>
            {
                TestsHelper.CreateTestFigure("user1"),
                TestsHelper.CreateTestFigure("user2")
            };

            _mockUserManager.Setup(x => x.Users)
                .Returns(users.AsQueryable());
            _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<IdentityUser>(), "Admin"))
                .ReturnsAsync(false);
            _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<IdentityUser>(), "Banned"))
                .ReturnsAsync(false);
            _mockPostService.Setup(x => x.GetAllPostsAsync())
                .ReturnsAsync(posts);
            _mockFigureService.Setup(x => x.GetAllFiguresAsync())
                .ReturnsAsync(figures);

            var result = await _controller.ManageUsers();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<AdminUserViewModel>;
            Assert.That(model.Count, Is.EqualTo(2));
            Assert.That(model.All(u => u.PostsCount == 1), Is.True);
            Assert.That(model.All(u => u.FiguresCount == 1), Is.True);
        }
    }
}

