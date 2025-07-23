using FigureMeUp.Data.Models;
using FigureMeUp.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core
{
    public class PostsService : IPostsService
    {
        public Task<bool> CreatePostAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePostAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Post?> GetPostByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePostAsync(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
