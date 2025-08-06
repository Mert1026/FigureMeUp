using FigureMeUp.Data;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Data.Repositories;
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
        private Mock<IHelperMetods> _mockHelperMethods;
        private ApplicationDbContext _context;
        private FigureService _figureService;

        [SetUp]
        public void Setup()
        {
            _mockFiguresRepository = new Mock<IFiguresRepository>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _mockHelperMethods = new Mock<IHelperMetods>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // New DB for isolation
                .Options;

            _context = new ApplicationDbContext(options);

            _figureService = new FigureService(
                _mockFiguresRepository.Object,
                _mockUserManager.Object,
                _context,
                _mockHelperMethods.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose(); // Dispose of the context
        }

        [Test]
        public async Task DeleteFigureAsync_NonExistingFigure_ReturnsFalse()
        {
            // Arrange
            var emptyQueryable = new List<Figure>().AsQueryable();
            _mockFiguresRepository.Setup(x => x.GetAllAttached())
                .Returns(emptyQueryable);

            // Act
            var result = await _figureService.DeleteFigureAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.False);
            _mockFiguresRepository.Verify(x => x.DeleteAsync(It.IsAny<Figure>()), Times.Never);
        }

        [Test]
        public async Task AddFigureAsync_InvalidRarity_ReturnsFalse()
        {
            // Arrange
            var user = TestsHelper.CreateTestUser();
            var figureViewModel = new FiguresViewModel
            {
                Name = "Test Figure",
                Description = "Test Description",
                ImageUrls = new List<string> { "https://test.com/image.jpg" },
                Hashtags = new List<string> { "test" },
                Rarity = "InvalidRarity"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockHelperMethods.Setup(x => x.RarityValidation("InvalidRarity"))
                .Returns((Rarity)null);

            // Act
            var result = await _figureService.AddFigureAsync(figureViewModel, "test-user-1");

            // Assert
            Assert.That(result, Is.False);
            _mockFiguresRepository.Verify(x => x.AddAsync(It.IsAny<Figure>()), Times.Never);
        }

        [Test]
        public async Task ToggleLikeAsync_InvalidFigureId_ReturnsFalse()
        {
            // Arrange
            var emptyQueryable = new List<Figure>().AsQueryable();
            _mockFiguresRepository.Setup(x => x.GetAllAttached())
                .Returns(emptyQueryable);

            // Act
            var result = await _figureService.ToggleLikeAsync(Guid.NewGuid(), "test-user-1");

            // Assert
            Assert.That(result, Is.False);
            _mockFiguresRepository.Verify(x => x.UpdateAsync(It.IsAny<Figure>()), Times.Never);
        }

        [Test]
        public async Task GetLikedFiguresByUserIdAsync_ValidUserId_ReturnsLikedFigures()
        {
            // Arrange
            var userId = "test-user-1";
            var figures = new List<Figure>
            {
                TestsHelper.CreateTestFigure(),
                TestsHelper.CreateTestFigure(),
                TestsHelper.CreateTestFigure()
            };

            // Only first two figures are liked by the user
            figures[0].LikedByUsersIds.Add(userId);
            figures[1].LikedByUsersIds.Add(userId);

            var queryable = figures.AsQueryable();
            _mockFiguresRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);

            // Act
            var result = await _figureService.GetLikedFiguresByUserIdAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(f => f.LikedByUsersIds.Contains(userId)), Is.True);
        }

        [Test]
        public async Task GetFiguresByOwnerIdAsync_ValidOwnerId_ReturnsOwnerFigures()
        {
            // Arrange
            var ownerId = "test-owner-1";
            var figures = new List<Figure>
            {
                TestsHelper.CreateTestFigure(ownerId),
                TestsHelper.CreateTestFigure(ownerId),
                TestsHelper.CreateTestFigure("different-owner")
            };

            var queryable = figures.AsQueryable();
            _mockFiguresRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);

            // Act
            var result = await _figureService.GetFiguresByUserIdAsync(ownerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(f => f.OwnerId == ownerId), Is.True);
        }

    }
}