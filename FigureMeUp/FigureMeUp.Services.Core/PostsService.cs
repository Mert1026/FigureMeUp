using FigureMeUp.Data;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Services.Core.Helpers;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureMeUp.Services.Core
{

    public class PostsService : IPostService
    {

        private readonly IPostsRepository _postsRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private HelperMetods _helperMetods;

        public PostsService(IPostsRepository postsRepository,
            UserManager<IdentityUser> userManager,
            HelperMetods helperMetods)
        {
            this._postsRepository = postsRepository;
            this._userManager = userManager;
            this._helperMetods = helperMetods;
        }
        public async Task<bool> CreatePostAsync(PostViewModel post, string userId)
        {
            bool OpResult = false;
            try
            {
                IdentityUser? user = await this._userManager.FindByIdAsync(userId);
                List<Hashtag> hashtags = _helperMetods.HashtagsConversion(post.Hashtags ?? new List<string>());

                if (user != null)
                {
                    Post toAdd = new Post
                    {
                        Title = post.Title,
                        Content = post.Content,
                        CreatedAt = DateTime.UtcNow,
                        ImageUrls = post.ImageUrls.ToList() ?? new List<string>(),
                        Hashtags = hashtags.ToList(),
                        PublisherId = userId,
                        IsDeleted = false
                    };

                    await _postsRepository.AddAsync(toAdd);
                    OpResult = true;
                }

                return OpResult;
            }
            catch(Exception ex)
            {
                //Redirection to error page
                return false;
            }

            


        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            bool OpResult = false;
            try
            {
                Post? postToDelete = await this.GetPostByIdAsync(id);
                if (postToDelete != null)
                {
                    OpResult = await _postsRepository.DeleteAsync(postToDelete);
                }
                return OpResult;
            }
            catch (Exception ex)
            {
                //Redirection to error page
                return false;
            }
              
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            try
            {
                IEnumerable<Post> allPosts = await _postsRepository
                .GetAllAttached()
                .Select(p => new Post
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    ImageUrls = p.ImageUrls,
                    Hashtags = p.Hashtags,
                    PublisherId = p.PublisherId,
                    Publisher = p.Publisher,
                    IsDeleted = p.IsDeleted
                })
                .ToArrayAsync();

                return allPosts;
            }
            catch(Exception ex)
            {
                //Redirection to error page
                return Enumerable.Empty<Post>();
            }
            
        }

        public Task<Post?> GetPostByIdAsync(Guid id)
        {
            try
            {
                Post? post = _postsRepository
                .GetAllAttached()
                .Include(p => p.Publisher)
                .FirstOrDefault(p => p.Id == id);

                return Task.FromResult(post);
            }
            catch (Exception ex)
            {
                //Redirection to error page
                return null;
            }

        }

        public async Task<bool> UpdatePostAsync(Post newPost)
        {
            bool OpResult = false;
            try
            {
                Post? PostToEdit = await this.GetPostByIdAsync(newPost.Id);
                if (PostToEdit != null)
                {
                    PostToEdit.Title = newPost.Title;
                    PostToEdit.Content = newPost.Content;
                    PostToEdit.ImageUrls = newPost.ImageUrls;
                    PostToEdit.Hashtags = newPost.Hashtags;
                    PostToEdit.IsDeleted = newPost.IsDeleted;

                    OpResult = await this._postsRepository.UpdateAsync(PostToEdit);

                }

                return OpResult;
            }
            catch (Exception ex)
            {
                //Redirection to error page -- naistina ne zabravyay
                return false;
            }

        }
    }
}
