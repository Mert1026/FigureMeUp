using FigureMeUp.Data.DBContext.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FigureMeUp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public virtual DbSet<Models.Rarity> Rarities { get; set; } = null!;
        public virtual DbSet<Models.Figure> Figures { get; set; } = null!;
        public virtual DbSet<Models.UserFigures> UserFigures { get; set; } = null!;
        public virtual DbSet<Models.Post> Posts { get; set; } = null!;
        public virtual DbSet<Models.Hashtag> Hashtags { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(FigureConfiguration).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(HashtagConfiguration).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(PostConfiguration).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(RarityConfigure).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(UserFiguresConfiguration).Assembly);

        }
    }
}
