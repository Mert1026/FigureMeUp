using FigureMeUp.Data;
using FigureMeUp.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Integration
{
    public class BaseIntegration : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                object value = services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });

                // Build service provider and seed data
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                SeedTestData(context, userManager).GetAwaiter().GetResult();
            });

            builder.UseEnvironment("Testing");
        }

        private static async Task SeedTestData(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            // Seed test users
            var testUser = new IdentityUser
            {
                Id = "test-user-1",
                UserName = "testuser",
                Email = "test@example.com",
                EmailConfirmed = true
            };

            var adminUser = new IdentityUser
            {
                Id = "admin-user-1",
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(testUser, "TestPassword123!");
            await userManager.CreateAsync(adminUser, "AdminPassword123!");

            // Seed rarities
            context.Rarities.AddRange(
                new Rarity { Id = 1, Name = "Common", IsDeleted = false },
                new Rarity { Id = 2, Name = "Rare", IsDeleted = false },
                new Rarity { Id = 3, Name = "Epic", IsDeleted = false }
            );

            // Seed hashtags
            context.Hashtags.AddRange(
                new Hashtag { Id = 1, Name = "anime", IsDeleted = false },
                new Hashtag { Id = 2, Name = "manga", IsDeleted = false },
                new Hashtag { Id = 3, Name = "collectible", IsDeleted = false }
            );

            await context.SaveChangesAsync();
        }
    }
}

