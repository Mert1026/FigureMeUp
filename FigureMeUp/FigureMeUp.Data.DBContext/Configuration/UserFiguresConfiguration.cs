using FigureMeUp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.DBContext.Configuration
{
    internal class UserFiguresConfiguration : IEntityTypeConfiguration<UserFigures>
    {
        public void Configure(EntityTypeBuilder<UserFigures> entity)
        {
            entity
                .HasKey(uf => new { uf.UserId, uf.FigureId });

            entity
                .HasOne(uf => uf.User)
                .WithMany()
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(uf => uf.Figure)
                .WithMany(f => f.UserFigures)
                .HasForeignKey(uf => uf.FigureId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
