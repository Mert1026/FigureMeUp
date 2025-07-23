using FigureMeUp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Interfaces
{
    public interface IHashtagsService
    {
        public Task<bool> CreateHashtagAsync(Post post);

        public Task<bool> DeleteHashtagAsync(int id);
    }
}
