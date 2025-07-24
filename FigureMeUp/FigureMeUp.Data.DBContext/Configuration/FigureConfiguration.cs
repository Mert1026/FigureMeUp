using Microsoft.EntityFrameworkCore;
using System;
using FigureMeUp.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FigureMeUp.GCommon.ValidationConstraints.FigureConstraints;

namespace FigureMeUp.Data.DBContext.Configuration
{
    public class FigureConfiguration : IEntityTypeConfiguration<Figure>
    {
        public void Configure(EntityTypeBuilder<Figure> entity)
        {
            entity
                .HasKey(f => f.Id);

            entity
                .Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            entity
                .Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            entity
                .Property(f => f.LastChanged)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .Property(f => f.IsDeleted)
                .HasDefaultValue(false);

            //entity
            //    .Property(f => f.ImageUrls)
            //    .HasDefaultValue(new string[ImageUrlsMaxCount]);

            //entity
            //    .Property(f => f.Hashtags)
            //    .HasDefaultValue(new Hashtag[HashtagsMaxCount]);

            entity
                .Property(f => f.OwnerId)
                .IsRequired();

            entity
                .HasOne(f => f.Owner)
                .WithMany()
                .HasForeignKey(f => f.OwnerId);

            entity
                .Property(f => f.RarityId)
                .IsRequired();

            entity
                .HasOne(f => f.Rarity)
                .WithMany()
                .HasForeignKey(f => f.RarityId);



        }
    }
}
