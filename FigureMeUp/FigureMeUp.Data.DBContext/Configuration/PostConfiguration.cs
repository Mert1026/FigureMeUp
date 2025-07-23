using FigureMeUp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FigureMeUp.GCommon.ValidationConstraints.PostConstraints;

namespace FigureMeUp.Data.DBContext.Configuration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> entity)
        {
            entity
                .HasKey(p => p .Id);

            entity
                .Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(TitleMaxLength);

            entity 
                .Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(ContentMaxLength);

            entity
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            //entity
            //    .Property(p => p.ImageUrls)
            //    .HasDefaultValue(new string[ImageUrlsMaxCount]);

            //entity
            //    .Property(p => p.Hashtags)
            //    .HasDefaultValue(new Hashtag[HashtagsMaxCount]);

            entity
                .Property(p => p.PublisherId)
                .IsRequired();

            entity
                .HasOne(p => p.Publisher)
                .WithMany()
                .HasForeignKey(p => p.PublisherId);


        }
    }
}
