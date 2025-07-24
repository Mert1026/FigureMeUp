using FigureMeUp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Interfaces
{
    public interface IHashtagService
    {
        Task<bool> CreateHashtagAsync(Hashtag hashtag);
        Task<bool> DeleteHashtagWithIdAsync(int id);
        Task<bool> DeleteHashtagWithNameAsync(string name);
        Task<Hashtag?> GetHashtagByIdAsync(int id);
        Task<Hashtag?> GetHashtagByNameAsync(string name);
    }
}
