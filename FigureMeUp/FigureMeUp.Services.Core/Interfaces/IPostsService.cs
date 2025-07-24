using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Interfaces
{
    public interface IPostsService
    {
        public Task<IEnumerable<Post>> GetAllPostsAsync();
        public Task<Post?> GetPostByIdAsync(int id);
        public Task<bool> CreatePostAsync(PostViewModel post, string userId);
        public Task<bool> UpdatePostAsync(Post post);
        public Task<bool> DeletePostAsync(int id);
    }
}
