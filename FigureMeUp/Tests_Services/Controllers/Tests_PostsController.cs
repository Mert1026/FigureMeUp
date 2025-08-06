using FigureMeUp.Controllers;
using FigureMeUp.Data.Models;
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
    internal class Tests_PostsController
    {
        private Mock<IPostService> _mockPostService;
        private PostsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockPostService = new Mock<IPostService>();
            _controller = new PostsController(_mockPostService.Object);

            // Setup controller context
            var user = TestsHelper.CreateTestClaimsPrincipal();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Setup TempData
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
        public async Task Index_ReturnsViewWithActivePosts()
        {
            var posts = new List<Post>
            {
                TestsHelper.CreateTestPost(isDeleted: false),
                TestsHelper.CreateTestPost(isDeleted: true),
                TestsHelper.CreateTestPost(isDeleted: false)
            };

            _mockPostService.Setup(x => x.GetAllPostsAsync())
                .ReturnsAsync(posts);

            var result = await _controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<Post>;
            Assert.That(model.Count, Is.EqualTo(2));
            Assert.That(model.All(p => !p.IsDeleted), Is.True);
        }

        [Test]
        public async Task Details_ExistingId_ReturnsViewWithPost()
        {
            var post = TestsHelper.CreateTestPost();
            _mockPostService.Setup(x => x.GetPostByIdAsync(post.Id))
                .ReturnsAsync(post);
            _mockPostService.Setup(x => x.ToggleLikeAsync(post.Id, "test-user-1"))
                .ReturnsAsync(true);

            var result = await _controller.ToggleLike(post.Id.ToString());

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var value = jsonResult.Value;
            var successProperty = value.GetType().GetProperty("success");
            Assert.That(successProperty?.GetValue(value), Is.True);
        }

        [Test]
        public async Task Delete_AuthorizedUser_RedirectsToIndex()
        {
            var post = TestsHelper.CreateTestPost("test-user-1");
            _mockPostService.Setup(x => x.GetPostByIdAsync(post.Id))
                .ReturnsAsync(post);
            _mockPostService.Setup(x => x.DeletePostAsync(post.Id))
                .ReturnsAsync(true);

            var result = await _controller.Delete(post.Id);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(PostsController.Index)));
        }
    }
}

