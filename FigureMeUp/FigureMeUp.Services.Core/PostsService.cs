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

        //public async Task<bool> AddLikeAsync(Guid postId, string userId)
        //{
        //    Post post = _postsRepository.GetAllAttached().FirstOrDefault(f => f.Id == postId);
        //    if (post == null 
        //        || post.IsDeleted 
        //        || !post.LikedByUsersIds.Contains(userId))
        //    {
        //        return false;
        //    }

        //    post.LikedByUsersIds.Add(userId);
        //    post.LikesCount++;

        //    await _postsRepository.UpdateAsync(post);
        //    return true;

        //}


        // Method to add a like (only if not already liked)
        public async Task<bool> AddLikeAsync(Guid postId, string userId)
        {
            Post post = _postsRepository.GetAllAttached().FirstOrDefault(f => f.Id == postId);
            if (post == null || post.IsDeleted)
            {
                return false;
            }

            // Check if user hasn't liked the post yet
            if (!post.LikedByUsersIds.Contains(userId))
            {
                post.LikedByUsersIds.Add(userId);
                post.LikesCount++;
                await _postsRepository.UpdateAsync(post);
                return true;
            }

            // User has already liked the post
            return false;
        }

        // Method to toggle like/unlike (better for your ToggleLike controller action)
        public async Task<bool> ToggleLikeAsync(Guid postId, string userId)
        {
            Post post = _postsRepository.GetAllAttached().FirstOrDefault(f => f.Id == postId);
            if (post == null || post.IsDeleted)
            {
                return false;
            }

            if (post.LikedByUsersIds.Contains(userId))
            {
                // User has liked it, so unlike it
                post.LikedByUsersIds.Remove(userId);
                post.LikesCount = Math.Max(0, post.LikesCount - 1); // Prevent negative counts
            }
            else
            {
                // User hasn't liked it, so like it
                post.LikedByUsersIds.Add(userId);
                post.LikesCount++;
            }

            await _postsRepository.UpdateAsync(post);
            return true;
        }

        public async Task<bool> AddViewAsync(Guid postId)
        {
            Post post = _postsRepository.GetAllAttached().FirstOrDefault(f => f.Id == postId);
            if (post != null)
            {
                post.ViewsCount++;
                await _postsRepository.UpdateAsync(post);
                return true;
            }
            return false;
        }

        public async Task<bool> CreatePostAsync(PostViewModel post, string userId)
        {
            bool OpResult = false;
            try
            {
                IdentityUser? user = await this._userManager.FindByIdAsync(userId);
                List<Hashtag> hashtags = _helperMetods.HashtagsConversion(post.Hashtags ?? new List<string>());
                bool isValidImageUrl = await HelperMetods.IsValidImageUrlAsync(post.ImageUrls[0]);
                if (post.ImageUrls[0] == null
                    || !isValidImageUrl)
                {
                    post.ImageUrls[0] = "https://media.istockphoto.com/id/1290743328/vector/faceless-man-abstract-silhouette-of-person-the-figure-of-man-without-a-face-front-view.jpg?s=612x612&w=0&k=20&c=Ys-4Co9NaWFFBDjmvDJABB2BPePxJwHugC8_G5u0rOk=";
                }
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
                    IsDeleted = p.IsDeleted,
                    LikedByUsersIds = p.LikedByUsersIds,
                    LikesCount = p.LikesCount,
                    ViewsCount = p.ViewsCount
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
                bool isValidImageUrl = await HelperMetods.IsValidImageUrlAsync(newPost.ImageUrls[0]);
                Post? PostToEdit = await this.GetPostByIdAsync(newPost.Id);
                if (newPost.ImageUrls[0] == null
                    || isValidImageUrl == false)
                {
                    newPost.ImageUrls[0] = "https://media.istockphoto.com/id/1290743328/vector/faceless-man-abstract-silhouette-of-person-the-figure-of-man-without-a-face-front-view.jpg?s=612x612&w=0&k=20&c=Ys-4Co9NaWFFBDjmvDJABB2BPePxJwHugC8_G5u0rOk=";
                }
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

        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
        {
            try
            {
                List<Post> posts = _postsRepository
                    .GetAllAttached()
                    .Where(p => p.PublisherId == userId && !p.IsDeleted)
                    .ToList();

                return posts;
            }
            catch (Exception ex)
            {
                //Redirection to error page
                return Enumerable.Empty<Post>();
            }
        }

        public async Task<IEnumerable<Post>> GetLikedPostsByUserIdAsync(string userId)
        {
            try
            {
                List<Post> posts = _postsRepository
                    .GetAllAttached()
                    .Where(p => p.LikedByUsersIds.Contains(userId) 
                    && !p.IsDeleted)
                    .ToList();

                return posts;
            }
            catch (Exception ex)
            {
                //Redirection to error page
                return Enumerable.Empty<Post>();
            }
        }
    }
}
