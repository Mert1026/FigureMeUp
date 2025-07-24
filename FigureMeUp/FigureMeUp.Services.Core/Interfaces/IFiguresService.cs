using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Interfaces
{
    public interface IFiguresService
    {
        public Task<IEnumerable<Figure>> GetAllFiguresAsync();
        public Task<Figure?> GetFigureByIdAsync(int id);
        public Task<bool> AddFigureAsync(FiguresViewModel figure, string userId);
        public Task<bool> UpdateFigureAsync(FiguresViewModel figure, string userId);
        public Task<bool> DeleteFigureAsync(int id);
    }
}
