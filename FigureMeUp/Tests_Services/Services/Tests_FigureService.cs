using FigureMeUp.Data;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core;
using FigureMeUp.Services.Core.Helpers;
using FigureMeUp.Services.Core.Helpers.Interfaces;
using Helper;
using Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.EntityFrameworkCore;
using Moq;

namespace Tests.Services
{
    [TestFixture]
    public class Tests_FigureService
    {
        private Mock<IFiguresRepository> _mockFiguresRepository;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<IHelperMetods> _mockHelperMethods;
        private FigureService _figureService;

        [SetUp]
        public void Setup()
        {
            _mockFiguresRepository = new Mock<IFiguresRepository>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _mockHelperMethods = new Mock<IHelperMetods>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var realContext = new ApplicationDbContext(options);

            _figureService = new FigureService(
                _mockFiguresRepository.Object,
                _mockUserManager.Object,
                realContext,
                _mockHelperMethods.Object);
        }

        [Test]
        public async Task AddFigureAsync_ValidInput_ReturnsTrue()
        {
            var user = TestsHelper.CreateTestUser();
            var figureViewModel = new FiguresViewModel
            {
                Name = "Test Figure",
                Description = "Test Description",
                ImageUrls = new List<string> { "https://test.com/image.jpg" },
                Hashtags = new List<string> { "test" },
                Rarity = "Common"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockHelperMethods.Setup(x => x.HashtagsConversion(It.IsAny<List<string>>()))
                .Returns(new List<Hashtag>());
            _mockHelperMethods.Setup(x => x.RarityValidation(It.IsAny<string>()))
                .Returns(new Rarity { Id = 1, Name = "Common" });
            _mockFiguresRepository.Setup(x => x.AddAsync(It.IsAny<Figure>()))
                .Returns(Task.CompletedTask);

            var result = await _figureService.AddFigureAsync(figureViewModel, "test-user-1");

            Assert.That(result, Is.True);
            _mockFiguresRepository.Verify(x => x.AddAsync(It.IsAny<Figure>()), Times.Once);
        }

        [Test]
        public async Task AddFigureAsync_InvalidUser_ReturnsFalse()
        {
            var figureViewModel = new FiguresViewModel
            {
                Name = "Test Figure",
                Description = "Test Description",
                ImageUrls = new List<string> { "https://test.com/image.jpg" },
                Hashtags = new List<string> { "test" },
                Rarity = "Common"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null);

            var result = await _figureService.AddFigureAsync(figureViewModel, "invalid-user");

            Assert.That(result, Is.False);
            _mockFiguresRepository.Verify(x => x.AddAsync(It.IsAny<Figure>()), Times.Never);
        }

       

        [Test]
        public async Task ToggleLikeAsync_UserNotLiked_AddsLike()
        {
            var figure = TestsHelper.CreateTestFigure();
            var userId = "test-user-2";
            var queryable = new List<Figure> { figure }.AsQueryable();

            _mockFiguresRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);
            _mockFiguresRepository.Setup(x => x.UpdateAsync(It.IsAny<Figure>()))
                .ReturnsAsync(true);

            var result = await _figureService.ToggleLikeAsync(figure.Id, userId);

            Assert.That(result, Is.True);
            Assert.That(figure.LikedByUsersIds.Contains(userId), Is.True);
            Assert.That(figure.LikesCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ToggleLikeAsync_UserAlreadyLiked_RemovesLike()
        {
            var userId = "test-user-2";
            var figure = TestsHelper.CreateTestFigure();
            figure.LikedByUsersIds.Add(userId);
            figure.LikesCount = 1;
            var queryable = new List<Figure> { figure }.AsQueryable();

            _mockFiguresRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);
            _mockFiguresRepository.Setup(x => x.UpdateAsync(It.IsAny<Figure>()))
                .ReturnsAsync(true);

            var result = await _figureService.ToggleLikeAsync(figure.Id, userId);

            Assert.That(result, Is.True);
            Assert.That(figure.LikedByUsersIds.Contains(userId), Is.False);
            Assert.That(figure.LikesCount, Is.EqualTo(0));
        }

       

        private Mock<IQueryable<Figure>> CreateMockDbSet(List<Figure> data)
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<IQueryable<Figure>>();

            mockSet.As<IAsyncEnumerable<Figure>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new Helper.TestAsyncEnumerator<Figure>(queryable.GetEnumerator()));

            mockSet.As<IQueryable<Figure>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Figure>(queryable.Provider));

            mockSet.As<IQueryable<Figure>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Figure>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Figure>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }
    }
}