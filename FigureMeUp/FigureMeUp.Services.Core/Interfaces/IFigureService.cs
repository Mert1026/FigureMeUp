using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Interfaces
{
    public interface IFigureService
    {
        public Task<IEnumerable<Figure>> GetAllFiguresAsync();
        public Task<Figure?> GetFigureByIdAsync(Guid id);
        public Task<bool> AddFigureAsync(FiguresViewModel figure, string userId);
        public Task<bool> UpdateFigureAsync(FiguresViewModel figure, string userId);
        public Task<bool> DeleteFigureAsync(Guid id);
        public Task<bool> RestoreFigureAsync(Guid id);
        public Task<bool> AddLikeAsync(Guid postId, string userId);
        public Task<bool> ToggleLikeAsync(Guid postId, string userId);
        public Task<IEnumerable<Figure>> GetFiguresByUserIdAsync(string userId);
        public Task<IEnumerable<Figure>> GetLikedFiguresByUserIdAsync(string userId);
        public Task<bool> HardDeleteFigureByIdAsync(Guid figureId);

    }
}
