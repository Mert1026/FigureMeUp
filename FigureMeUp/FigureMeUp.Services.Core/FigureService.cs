using FigureMeUp.Data.Models;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core
{
    public class FigureService : IFiguresService
    {

        private readonly IFiguresRepository _figuresRepository;

        public FigureService(IFiguresRepository figuresRepository)
        {
            this._figuresRepository = figuresRepository;
        }

        public Task<bool> AddFigureAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFigureAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Figure>> GetAllFiguresAsync()
        {
            IEnumerable<Figure> figures = await this._figuresRepository
                .GetAllAttached()
                .Include(p => p.Rarity)
                .Include(p => p.Owner)
                .Include(p => p.UserFigures)
                .Include(p => p.Hashtags)
                .Select(p => new Figure()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    IsDeleted = p.IsDeleted,
                    ImageUrls = p.ImageUrls,
                    RarityId = p.RarityId,
                    Rarity = p.Rarity,
                    Owner = p.Owner,
                    OwnerId = p.OwnerId,
                    CreatedAt = p.CreatedAt,
                    UserFigures = p.UserFigures,
                    Hashtags = p.Hashtags// Note: The ImageUrls property is assumed to be a collection of strings.
                })
                .ToArrayAsync();

            if (figures == null || !figures.Any())
            {             
                return Enumerable.Empty<Figure>();
            }
            else
            {
                return figures;
            }
        }

        public Task<Figure?> GetFigureByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateFigureAsync(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
