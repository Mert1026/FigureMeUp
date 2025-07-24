using FigureMeUp.Data.Models;
using FigureMeUp.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Repositories
{
    public class HashtagsRepository
        : BaseRepository<Hashtag, Guid>, IHashtagsRepository
    {
        public HashtagsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
