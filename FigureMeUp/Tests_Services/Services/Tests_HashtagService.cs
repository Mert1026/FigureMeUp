using FigureMeUp.Data.Models;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core;
using Helper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
    [TestFixture]
    public class Tests_HashtagService
    {
        private Mock<IHashtagsRepository> _mockHashtagsRepository;
        private HashtagsService _hashtagsService;

        [SetUp]
        public void Setup()
        {
            _mockHashtagsRepository = new Mock<IHashtagsRepository>();
            _hashtagsService = new HashtagsService(_mockHashtagsRepository.Object);
        }

        [Test]
        public async Task CreateHashtagAsync_ValidHashtag_ReturnsTrue()
        {
            
            var hashtag = TestsHelper.CreateTestHashtag();
            _mockHashtagsRepository.Setup(x => x.AddAsync(It.IsAny<Hashtag>()))
                .Returns(Task.CompletedTask);
   
            var result = await _hashtagsService.CreateHashtagAsync(hashtag);

            Assert.That(result, Is.True);
            _mockHashtagsRepository.Verify(x => x.AddAsync(hashtag), Times.Once);
        }

        [Test]
        public async Task GetHashtagByNameAsync_ExistingName_ReturnsHashtag()
        {
            var hashtag = TestsHelper.CreateTestHashtag(1, "TestTag");
            var queryable = new List<Hashtag> { hashtag }.AsQueryable();

            _mockHashtagsRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);

            var result = await _hashtagsService.GetHashtagByNameAsync("TestTag");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("TestTag"));
        }

        [Test]
        public async Task DeleteHashtagWithIdAsync_ExistingHashtag_ReturnsTrue()
        {
            var hashtag = TestsHelper.CreateTestHashtag();
            var queryable = new List<Hashtag> { hashtag }.AsQueryable();

            _mockHashtagsRepository.Setup(x => x.GetAllAttached())
                .Returns(queryable);
            _mockHashtagsRepository.Setup(x => x.DeleteAsync(It.IsAny<Hashtag>()))
                .ReturnsAsync(true);

            var result = await _hashtagsService.DeleteHashtagWithIdAsync(hashtag.Id);

            Assert.That(result, Is.True);
            _mockHashtagsRepository.Verify(x => x.DeleteAsync(hashtag), Times.Once);
        }
    }
}
