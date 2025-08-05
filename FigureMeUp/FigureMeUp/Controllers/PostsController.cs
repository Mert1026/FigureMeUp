using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FigureMeUp.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllPostsAsync();
            var activePosts = posts.Where(p => !p.IsDeleted).OrderByDescending(p => p.CreatedAt).ToList();
            return View(activePosts);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            var addView = await _postService.AddViewAsync(id);
            if ((post == null|| post.IsDeleted) && !User.IsInRole("Admin"))
            {
                return NotFound();
            }

            return View(post);
        }


        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (ModelState.IsValid
                || model != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _postService.CreatePostAsync(model, userId);

                if (result)
                {
                    TempData["Success"] = "Post created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create post. Please try again.");
                }
            }

            return View(model);
        }


        [Authorize]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null || post.IsDeleted)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.PublisherId != userId
                && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var model = new PostViewModel
            {
                Title = post.Title,
                Content = post.Content,
                ImageUrls = post.ImageUrls.ToList(),
                Hashtags = post.Hashtags.Select(h => h.Name).ToList(),
                AuthorName = post.Publisher?.UserName ?? string.Empty,
                PostedOn = post.CreatedAt.ToString("yyyy-MM-dd")
            };

            return View(model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id, PostViewModel model)
        {
            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingPost.PublisherId != userId
                && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                existingPost.Title = model.Title;
                existingPost.Content = model.Content;
                existingPost.ImageUrls = model.ImageUrls ?? new List<string>();

                var result = await _postService.UpdatePostAsync(existingPost);

                if (result)
                {
                    TempData["Success"] = "Post updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update post. Please try again.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.PublisherId != userId
                && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _postService.DeletePostAsync(id);
            if (result)
            {
                TempData["Success"] = "Post deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete post.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Posts/ToggleLike/5
        //[HttpPost]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ToggleLike(Guid id)
        //{
        //    var post = await _postService.GetPostByIdAsync(id);
        //    if (post == null || post.IsDeleted)
        //    {
        //        return NotFound();
        //    }

        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // TODO: Implement like functionality in PostService
        //    // 1. Create a PostLikes table with PostId, UserId, CreatedAt columns
        //    // 2. Add methods to PostService:
        //    //    - Task<bool> TogglePostLikeAsync(Guid postId, string userId)
        //    //    - Task<int> GetPostLikesCountAsync(Guid postId)
        //    //    - Task<bool> IsPostLikedByUserAsync(Guid postId, string userId)
        //    // 3. Update Post model to include navigation property for likes

        //    var result = await _postService.ToggleLikeAsync(id, userId);
        //    if (result)
        //    {
        //        TempData["Success"] = "Like status updated!";
        //    }

        //    //// For now, just redirect back
        //    //TempData["Info"] = "Like functionality will be implemented soon!";
        //    return RedirectToAction(nameof(Index));
        //    //return Json(new { success = true, message = "Like functionality will be implemented soon!" });
        //}

        // Add this method to your PostsController.cs

        // Add this method to your PostsController.cs

        // Add this method to your PostsController.cs
        // Replace the existing ToggleLike method with this simpler version

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLike(string id)
        {
            try
            {
                // Validate the ID
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Post ID is required" });
                }

                if (!Guid.TryParse(id, out Guid postId))
                {
                    return Json(new { success = false, message = "Invalid post ID format" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Check if post exists first
                var post = await _postService.GetPostByIdAsync(postId);
                if (post == null || post.IsDeleted)
                {
                    return Json(new { success = false, message = "Post not found" });
                }

                // Toggle the like
                var result = await _postService.ToggleLikeAsync(postId, userId);

                if (result)
                {
                    // Get updated post data
                    var updatedPost = await _postService.GetPostByIdAsync(postId);
                    if (updatedPost != null)
                    {
                        var isLiked = updatedPost.LikedByUsersIds.Contains(userId);
                        var message = isLiked ? "Post liked successfully!" : "Post unliked successfully!";

                        return Json(new
                        {
                            success = true,
                            message = message,
                            isLiked = isLiked,
                            likesCount = updatedPost.LikesCount
                        });
                    }
                }

                return Json(new { success = false, message = "Failed to toggle like. Please try again." });
            }
            catch (Exception ex)
            {
                // Log the actual exception for debugging
                System.Diagnostics.Debug.WriteLine($"ToggleLike error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        //test
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLikeAjax(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null || post.IsDeleted)
            {
                return Json(new { success = false, message = "Post not found" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Json(new { success = false, message = "Like functionality not implemented yet" });
        }
    }
}