using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core;
using FigureMeUp.Services.Core.Helpers;
using FigureMeUp.Services.Core.Helpers.Interfaces;
using Helper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Services
{
    [TestFixture]
    public class Test_PostService
    {
        private Mock<IPostsRepository> _mockPostsRepository;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IHelperMetods> _mockHelperMethods;
        private PostsService _postsService;

        [SetUp]
        public void Setup()
        {
            _mockPostsRepository = new Mock<IPostsRepository>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            _mockHelperMethods = new Mock<IHelperMetods>();

            _postsService = new PostsService(
                _mockPostsRepository.Object,
                _mockUserManager.Object,
                _mockHelperMethods.Object);
        }

        [Test]
        public async Task CreatePostAsync_ValidInput_ReturnsTrue()
        {
            var user = TestsHelper.CreateTestUser();
            var postViewModel = new PostViewModel
            {
                Title = "Test Post",
                Content = "Test Content",
                ImageUrls = new List<string> { "https://test.com/image.jpg" },
                Hashtags = new List<string> { "test" }
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockHelperMethods.Setup(x => x.HashtagsConversion(It.IsAny<List<string>>()))
                .Returns(new List<Hashtag>());
            _mockPostsRepository.Setup(x => x.AddAsync(It.IsAny<Post>()))
                .Returns(Task.CompletedTask);

            var result = await _postsService.CreatePostAsync(postViewModel, "test-user-1");

            Assert.That(result, Is.True);
            _mockPostsRepository.Verify(x => x.AddAsync(It.IsAny<Post>()), Times.Once);
        }

        [Test]
        public async Task ToggleLikeAsync_UserNotLiked_AddsLike()
        {

            var post = TestsHelper.CreateTestPost();
            var userId = "test-user-2";
            var queryable = new List<Post> { post }.AsQueryable();

            _mockPostsRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);
            _mockPostsRepository.Setup(x => x.UpdateAsync(It.IsAny<Post>()))
                .ReturnsAsync(true);

            var result = await _postsService.ToggleLikeAsync(post.Id, userId);

            Assert.That(result, Is.True);
            Assert.That(post.LikedByUsersIds.Contains(userId), Is.True);
            Assert.That(post.LikesCount, Is.EqualTo(1));
        }

        [Test]
        public async Task AddViewAsync_ExistingPost_IncreasesViewCount()
        {
            var post = TestsHelper.CreateTestPost();
            var initialViewCount = post.ViewsCount;
            var queryable = new List<Post> { post }.AsQueryable();

            _mockPostsRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);
            _mockPostsRepository.Setup(x => x.UpdateAsync(It.IsAny<Post>()))
                .ReturnsAsync(true);

            var result = await _postsService.AddViewAsync(post.Id);

            Assert.That(result, Is.True);
            Assert.That(post.ViewsCount, Is.EqualTo(initialViewCount + 1));
        }

        [Test]
        public async Task DeletePostAsync_ExistingPost_ReturnsTrue()
        {
            var post = TestsHelper.CreateTestPost();
            var queryable = new List<Post> { post }.AsQueryable();

            _mockPostsRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);
            _mockPostsRepository.Setup(x => x.DeleteAsync(It.IsAny<Post>()))
                .ReturnsAsync(true);

            var result = await _postsService.DeletePostAsync(post.Id);

            Assert.That(result, Is.True);
            _mockPostsRepository.Verify(x => x.DeleteAsync(It.IsAny<Post>()), Times.Once);
        }
    }
}
