using FigureMeUp.Controllers;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class Tests_HomeController
    {
        private Mock<IFigureService> _mockFigureService;
        private Mock<IPostService> _mockPostService;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _mockFigureService = new Mock<IFigureService>();
            _mockPostService = new Mock<IPostService>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            _controller = new HomeController(
                _mockFigureService.Object,
                _mockPostService.Object,
                _mockUserManager.Object);

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
        public async Task Index_ReturnsViewWithHomeViewModel()
        {
            var figures = new List<Figure>
            {
                TestsHelper.CreateTestFigure(isDeleted: false),
                TestsHelper.CreateTestFigure(isDeleted: false),
                TestsHelper.CreateTestFigure(isDeleted: true) // This should be filtered out
            };

            var posts = new List<Post>
            {
                TestsHelper.CreateTestPost(isDeleted: false),
                TestsHelper.CreateTestPost(isDeleted: false),
                TestsHelper.CreateTestPost(isDeleted: true) // This should be filtered out
            };

            _mockFigureService.Setup(x => x.GetAllFiguresAsync())
                .ReturnsAsync(figures);
            _mockPostService.Setup(x => x.GetAllPostsAsync())
                .ReturnsAsync(posts);

            var result = await _controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as HomeViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.RecentFigures.Count(), Is.EqualTo(2)); // Only non-deleted figures
            Assert.That(model.RecentPosts.Count(), Is.EqualTo(2)); // Only non-deleted posts
        }

        [Test]
        public async Task ViewProfile_AuthenticatedUser_ReturnsProfileView()
        {
            var user = TestsHelper.CreateTestUser();
            var figures = new List<Figure> { TestsHelper.CreateTestFigure(user.Id, false) };
            var posts = new List<Post> { TestsHelper.CreateTestPost(user.Id, false) };

            _mockUserManager.Setup(x => x.FindByIdAsync("test-user-1"))
                .ReturnsAsync(user);
            _mockFigureService.Setup(x => x.GetAllFiguresAsync())
                .ReturnsAsync(figures);
            _mockPostService.Setup(x => x.GetAllPostsAsync())
                .ReturnsAsync(posts);

            var result = await _controller.ViewProfile();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as ProfileDetailsViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.UserName, Is.EqualTo(user.UserName));
            Assert.That(model.Email, Is.EqualTo(user.Email));
            Assert.That(model.FiguresCount, Is.EqualTo(1));
            Assert.That(model.PostsCount, Is.EqualTo(1));
        }

        [Test]
        public async Task LikedContent_AuthenticatedUser_ReturnsLikedContentView()
        {
            var likedFigures = new List<Figure> { TestsHelper.CreateTestFigure() };
            var likedPosts = new List<Post> { TestsHelper.CreateTestPost() };

            _mockFigureService.Setup(x => x.GetLikedFiguresByUserIdAsync("test-user-1"))
                .ReturnsAsync(likedFigures);
            _mockPostService.Setup(x => x.GetLikedPostsByUserIdAsync("test-user-1"))
                .ReturnsAsync(likedPosts);

            var result = await _controller.LikedContent();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as LikedContentViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.LikedFigures.Count, Is.EqualTo(1));
            Assert.That(model.LikedPosts.Count, Is.EqualTo(1));
        }
    }
}

