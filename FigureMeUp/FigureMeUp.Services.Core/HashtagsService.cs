using FigureMeUp.Data.Models;
using FigureMeUp.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core
{
    public class HashtagsService : IHashtagsService
    {
        public Task<bool> CreateHashtagAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteHashtagAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
