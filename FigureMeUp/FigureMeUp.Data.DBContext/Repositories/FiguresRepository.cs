using FigureMeUp.Data.Models;
using FigureMeUp.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Repositories
{
    public class FiguresRepository
        : BaseRepository<Figure, Guid>, IFiguresRepository
    {
        public FiguresRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }
    }
}
