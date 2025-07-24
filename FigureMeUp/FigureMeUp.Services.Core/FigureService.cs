using FigureMeUp.Data;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Data.Repositories;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core.Helpers;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly HelperMetods _helperMetods;

        public FigureService(IFiguresRepository figuresRepository,
            UserManager<IdentityUser> userManeger,
            ApplicationDbContext context,
            HelperMetods helperMetods)
        {
            this._figuresRepository = figuresRepository;
            this._userManager = userManeger;
            this._context = context;
            _helperMetods = helperMetods;
        }

        public async Task<bool> AddFigureAsync(FiguresViewModel figure, string userId)
        {
            bool OpResult = false;
            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            List<Hashtag> hashtags = _helperMetods.HashtagsConversion(figure.Hashtags ?? new List<string>());
            Rarity? rarityValidation = _helperMetods.RarityValidation(figure.Rarity);
            if (user != null
                && rarityValidation != null)
            {
                Figure toAdd = new Figure
                {
                    Name = figure.Name,
                    Description = figure.Description,
                    ImageUrls = figure.ImageUrls ?? new List<string>(), // Assuming ImageUrls is a collection of strings
                    RarityId = rarityValidation.id,
                    OwnerId = userId,
                    Owner = user,
                    LastChanged = DateTime.Now,
                    IsDeleted = false,
                    Hashtags = hashtags// Assuming Hashtags is a collection of Hashtag objects
                };


                await _figuresRepository.AddAsync(toAdd);
                OpResult = true;
            }

            return OpResult;

        }

        public async Task<bool> DeleteFigureAsync(int id)
        {
            bool OpResult = false;
            Figure? figureToDelete = await this.GetFigureByIdAsync(id);
            if (figureToDelete != null)
            {
                OpResult = await _figuresRepository.DeleteAsync(figureToDelete);
            }
            return OpResult;
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
                    LastChanged = p.LastChanged,
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

        public async Task<Figure?> GetFigureByIdAsync(int id)
        {
            Figure? figure = await this._figuresRepository
                .GetAllAttached()
                .Include(p => p.Rarity)
                .Include(p => p.Owner)
                .Include(p => p.UserFigures)
                .Include(p => p.Hashtags)
                .FirstOrDefaultAsync(p => p.Id == id);

            return figure;
        }

        public async Task<bool> UpdateFigureAsync(FiguresViewModel newFigure, string userId)
        {
            bool OpResult = false;
            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            List<Hashtag> hashtags = _helperMetods.HashtagsConversion(newFigure.Hashtags ?? new List<string>());
            Rarity? rarityValidation = _helperMetods.RarityValidation(newFigure.Rarity);
            Figure? FigureToEdit = await this.GetFigureByIdAsync(newFigure.Id);
            if (FigureToEdit != null
                && rarityValidation != null)
            {
                FigureToEdit.Name = newFigure.Name;
                FigureToEdit.Description = newFigure.Description;
                FigureToEdit.ImageUrls = newFigure.ImageUrls ?? new List<string>();
                FigureToEdit.RarityId = rarityValidation.id;
                FigureToEdit.Rarity = rarityValidation;
                FigureToEdit.Hashtags = hashtags;
                FigureToEdit.IsDeleted = false;
                FigureToEdit.LastChanged = DateTime.Now;
                FigureToEdit.OwnerId = FigureToEdit.OwnerId;
                FigureToEdit.Owner = FigureToEdit.Owner;

                OpResult = await _figuresRepository.UpdateAsync(FigureToEdit);
                return OpResult;
            }

            return OpResult;
        }
    }
}
