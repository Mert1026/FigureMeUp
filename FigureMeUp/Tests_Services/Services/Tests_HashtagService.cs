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

        

        private Mock<IQueryable<Hashtag>> CreateMockHashtagDbSet(List<Hashtag> data)
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<IQueryable<Hashtag>>();

            mockSet.As<IAsyncEnumerable<Hashtag>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Hashtag>(queryable.GetEnumerator()));

            mockSet.As<IQueryable<Hashtag>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Hashtag>(queryable.Provider));

            mockSet.As<IQueryable<Hashtag>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Hashtag>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Hashtag>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }
    }
}
