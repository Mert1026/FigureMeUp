using FigureMeUp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FigureMeUp.GCommon.ValidationConstraints.RarityConstraints;

namespace FigureMeUp.Data.DBContext.Configuration
{
    public class RarityConfigure : IEntityTypeConfiguration<Rarity>
    {
        public void Configure(EntityTypeBuilder<Rarity> entity)
        {
            entity
                .HasKey(r => r.Id);

            entity
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            entity
                .Property(r => r.IsDeleted)
                .HasDefaultValue(false);
        }
    }

}
