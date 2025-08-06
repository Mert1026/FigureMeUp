using FigureMeUp.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace Helper
{
    public static class TestsHelper
    {
        public static IdentityUser CreateTestUser(string id = "test-user-1", string username = "TestUser", string email = "test@test.com")
        {
            return new IdentityUser
            {
                Id = id,
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };
        }

        public static Figure CreateTestFigure(string ownerId = "test-user-1", bool isDeleted = false)
        {
            return new Figure
            {
                Id = Guid.NewGuid(),
                Name = "Test Figure",
                Description = "Test Description",
                ImageUrls = new List<string> { "https://test.com/image.jpg" },
                RarityId = 1,
                Rarity = new Rarity { Id = 1, Name = "Common" },
                OwnerId = ownerId,
                Owner = CreateTestUser(ownerId),
                LastChanged = DateTime.UtcNow,
                IsDeleted = isDeleted,
                LikesCount = 0,
                LikedByUsersIds = new List<string>(),
                Hashtags = new List<Hashtag>()
            };
        }

        public static Post CreateTestPost(string publisherId = "test-user-1", bool isDeleted = false)
        {
            return new Post
            {
                Id = Guid.NewGuid(),
                Title = "Test Post",
                Content = "Test Content",
                CreatedAt = DateTime.UtcNow,
                ImageUrls = new List<string> { "https://test.com/image.jpg" },
                PublisherId = publisherId,
                Publisher = CreateTestUser(publisherId),
                IsDeleted = isDeleted,
                LikesCount = 0,
                LikedByUsersIds = new List<string>(),
                ViewsCount = 0,
                Hashtags = new List<Hashtag>()
            };
        }

        public static Hashtag CreateTestHashtag(int id = 1, string name = "TestTag")
        {
            return new Hashtag
            {
                Id = id,
                Name = name,
                IsDeleted = false
            };
        }

        public static ClaimsPrincipal CreateTestClaimsPrincipal(string userId = "test-user-1", string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }
    }
}
