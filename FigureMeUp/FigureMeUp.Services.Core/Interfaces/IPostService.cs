using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core.Interfaces
{
    public interface IPostService
    {
        public Task<IEnumerable<Post>> GetAllPostsAsync();
        public Task<Post?> GetPostByIdAsync(Guid id);
        public Task<bool> CreatePostAsync(PostViewModel post, string userId);
        public Task<bool> UpdatePostAsync(Post post);
        public Task<bool> DeletePostAsync(Guid id);
        public Task<bool> RestorePostAsync(Guid id);
        public Task<bool> AddLikeAsync(Guid postId, string userId);
        Task<bool> ToggleLikeAsync(Guid postId, string userId);
        public Task<bool> AddViewAsync(Guid postId);
        public Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId);
        public Task<IEnumerable<Post>> GetLikedPostsByUserIdAsync(string userId);


    }
}
