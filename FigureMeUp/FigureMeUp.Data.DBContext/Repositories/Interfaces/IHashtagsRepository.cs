using FigureMeUp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Data.Repositories.Interfaces
{
    public interface IHashtagsRepository
        : IRepository<Hashtag, Guid>, IAsyncRepository<Hashtag, Guid>
    {
    }
}
