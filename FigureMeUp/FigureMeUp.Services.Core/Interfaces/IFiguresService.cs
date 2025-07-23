using FigureMeUp.Data.Models;
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
        public Task<bool> AddFigureAsync(Post post);
        public Task<bool> UpdateFigureAsync(Post post);
        public Task<bool> DeleteFigureAsync(int id);
    }
}
