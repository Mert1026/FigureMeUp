using FigureMeUp.Data;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Data.Repositories;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core.Helpers;
using FigureMeUp.Services.Core.Helpers.Interfaces;
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
    public class FigureService : IFigureService
    {

        private readonly IFiguresRepository _figuresRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHelperMetods _helperMetods;

        public FigureService(IFiguresRepository figuresRepository,
            UserManager<IdentityUser> userManeger,
            ApplicationDbContext context,
            IHelperMetods helperMetods)
        {
            this._figuresRepository = figuresRepository;
            this._userManager = userManeger;
            this._context = context;
            _helperMetods = helperMetods;
        }

        public async Task<bool> AddFigureAsync(FiguresViewModel figure, string userId)
        {
            bool OpResult = false;
            try
            {
                IdentityUser? user = await this._userManager.FindByIdAsync(userId);
                List<Hashtag> hashtags = _helperMetods.HashtagsConversion(figure.Hashtags ?? new List<string>());
                Rarity? rarityValidation = _helperMetods.RarityValidation(figure.Rarity);
                bool isValidImageUrl = await HelperMetods.IsValidImageUrlAsync(figure.ImageUrls[0]);
                if (user != null
                    && rarityValidation != null)
                {
                    if (figure.ImageUrls[0] == null
                        || !isValidImageUrl)
                    {
                        figure
                            .ImageUrls[0] = "https://media.istockphoto.com/id/1290743328/vector/faceless-man-abstract-silhouette-of-person-the-figure-of-man-without-a-face-front-view.jpg?s=612x612&w=0&k=20&c=Ys-4Co9NaWFFBDjmvDJABB2BPePxJwHugC8_G5u0rOk=";
                    }

                    Figure toAdd = new Figure
                    {
                        Name = figure.Name,
                        Description = figure.Description,
                        ImageUrls = figure.ImageUrls.ToList() ?? new List<string>(),
                        RarityId = rarityValidation.Id,
                        OwnerId = userId,
                        Owner = user,
                        LastChanged = DateTime.Now,
                        IsDeleted = false,
                        Hashtags = hashtags.ToList()
                    };


                    await _figuresRepository.AddAsync(toAdd);
                    OpResult = true;
                }

                return OpResult;
            }
            catch
            {
                return false;
            }
            

        }

        public async Task<bool> DeleteFigureAsync(Guid id)
        {
            bool OpResult = false;
            try
            {
                Figure? figureToDelete = await this.GetFigureByIdAsync(id);
                if (figureToDelete != null)
                {
                    OpResult = await _figuresRepository.DeleteAsync(figureToDelete);
                }
                return OpResult;
            }
            catch
            {
                return false;
            }

        }

        public async Task<IEnumerable<Figure>> GetAllFiguresAsync()
        {
            try
            {
                IEnumerable<Figure> figures = await this._figuresRepository
                .GetAllAttached()
                .Include(f => f.Rarity)
                .Include(f => f.Owner)
                .Include(f => f.UserFigures)
                .Include(f => f.Hashtags)
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
                    UserFigures = p.UserFigures.ToList(),
                    Hashtags = p.Hashtags.ToList(),
                    LikedByUsersIds = p.LikedByUsersIds,
                    LikesCount = p.LikesCount
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
            catch
            {
                return Enumerable.Empty<Figure>();
            }
            
        }

        public async Task<Figure?> GetFigureByIdAsync(Guid id)
        {
            try
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
            catch
            {
                return null;
            }

        }

        public async Task<bool> UpdateFigureAsync(FiguresViewModel newFigure, string userId)
        {
            bool OpResult = false;
            try
            {
                IdentityUser? user = await this._userManager.FindByIdAsync(userId);
                List<Hashtag> hashtags = _helperMetods.HashtagsConversion(newFigure.Hashtags ?? new List<string>());
                Rarity? rarityValidation = _helperMetods.RarityValidation(newFigure.Rarity);
                Figure? FigureToEdit = await this.GetFigureByIdAsync(newFigure.Id);
                bool isValidImageUrl = await HelperMetods.IsValidImageUrlAsync(newFigure.ImageUrls[0]);

                if (newFigure.ImageUrls[0] == null
                   || isValidImageUrl == false)
                {
                    newFigure.ImageUrls[0] = "https://media.istockphoto.com/id/1290743328/vector/faceless-man-abstract-silhouette-of-person-the-figure-of-man-without-a-face-front-view.jpg?s=612x612&w=0&k=20&c=Ys-4Co9NaWFFBDjmvDJABB2BPePxJwHugC8_G5u0rOk=";
                }

                if (FigureToEdit != null
                    && rarityValidation != null)
                {
                    FigureToEdit.Name = newFigure.Name;
                    FigureToEdit.Description = newFigure.Description;
                    FigureToEdit.ImageUrls = newFigure.ImageUrls.ToList() ?? new List<string>();
                    FigureToEdit.RarityId = rarityValidation.Id;
                    FigureToEdit.Rarity = rarityValidation;
                    FigureToEdit.Hashtags = hashtags.ToList();
                    FigureToEdit.IsDeleted = false;
                    FigureToEdit.LastChanged = DateTime.Now;
                    FigureToEdit.OwnerId = FigureToEdit.OwnerId;
                    FigureToEdit.Owner = FigureToEdit.Owner;

                    OpResult = await _figuresRepository.UpdateAsync(FigureToEdit);
                    return OpResult;
                }

                return OpResult;
            }
            catch
            {
                return false;
            }
           
        }

        public async Task<bool> AddLikeAsync(Guid figureId, string userId)
        {
            Figure? figure = _figuresRepository.GetAllAttached().FirstOrDefault(f => f.Id == figureId);
            if (figure == null || figure.IsDeleted)
            {
                return false;
            }

            if (!figure.LikedByUsersIds.Contains(userId))
            {
                figure.LikedByUsersIds.Add(userId);
                    figure.LikesCount++;
                await _figuresRepository.UpdateAsync(figure);
                return true;
            }

            return false;
        }

        public async Task<bool> ToggleLikeAsync(Guid figureId, string userId)
        {
            try
            {
                if(figureId != Guid.Empty
                    || userId != String.Empty)
                {
                    Figure? figure = _figuresRepository.GetAllAttached()
                    .FirstOrDefault(f => f.Id == figureId);
                    if (figure == null || figure.IsDeleted)
                    {
                        return false;
                    }

                    if (figure.LikedByUsersIds.Contains(userId))
                    {
                        // User has liked it, so unlike it
                        figure.LikedByUsersIds.Remove(userId);
                        figure.LikesCount = Math.Max(0, figure.LikesCount - 1); // Prevent negative counts
                    }
                    else
                    {
                        // User hasn't liked it, so like it
                        figure.LikedByUsersIds.Add(userId);
                        figure.LikesCount++;
                    }

                    await _figuresRepository.UpdateAsync(figure);
                    return true;
                }
                return false;
                
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<IEnumerable<Figure>> GetFiguresByUserIdAsync(string userId)
        {
            try
            {
                List<Figure> figures = _figuresRepository
                .GetAllAttached()
                .Include(f => f.Rarity)
                .Include(f => f.Owner)
                .Include(f => f.UserFigures)
                .Include(f => f.Hashtags)
                .Where(f => f.OwnerId == userId && !f.IsDeleted)
                .ToList();

                return figures;
            }
            catch
            {
                return Enumerable.Empty<Figure>();
            }
            
        }

        public async Task<IEnumerable<Figure>> GetLikedFiguresByUserIdAsync(string userId)
        {
            try
            {
                List<Figure> figures = _figuresRepository
                .GetAllAttached()
                .Include(f => f.Rarity)
                .Include(f => f.Owner)
                .Include(f => f.UserFigures)
                .Include(f => f.Hashtags)
                .Where(f => f.LikedByUsersIds.Contains(userId)
                && !f.IsDeleted)
                .ToList();

                return figures;
            }
            catch
            {
                return Enumerable.Empty<Figure>();
            }
        }

        public async Task<bool> RestoreFigureAsync(Guid id)
        {
            Figure? figure = await this.GetFigureByIdAsync(id);
            if (figure != null)
            {
                figure.IsDeleted = false;
                return await _figuresRepository.UpdateAsync(figure);
            }
            return false;
        }
    }
}
