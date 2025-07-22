using FigureMeUp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FigureMeUp.GCommon.ValidationConstraints.HashtagConstraints;

namespace FigureMeUp.Data.DBContext.Configuration
{
    public class HashtagConfiguration : IEntityTypeConfiguration<Hashtag>
    {
        public void Configure(EntityTypeBuilder<Hashtag> entity)
        {
            entity
                .HasKey(h => h.Id); 

            entity
                .Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);
        }
    }
}
