using FigureMeUp.Data.Models;
using FigureMeUp.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Repositories
{
    public class PostsRepository
                : BaseRepository<Post, Guid>, IPostsRepository
    {
        public PostsRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
