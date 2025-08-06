using FigureMeUp.Controllers.IControllers;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FigureMeUp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class PostsController : Controller, IPostsController
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();
                var activePosts = posts.Where(p => !p.IsDeleted).OrderByDescending(p => p.CreatedAt).ToList();
                return View(activePosts);
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }

        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(id);
                var addView = await _postService.AddViewAsync(id);
                if ((post == null || post.IsDeleted) && !User.IsInRole("Admin"))
                {
                    return NotFound();
                }

                return View(post);
            }
            catch(Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
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
            try
            {
                if (ModelState.IsValid
               || model != null)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Forbid();
                    }
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
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }


        [Authorize]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id, PostViewModel model)
        {
          
            try
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
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
           
        }

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
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }

        //test
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLikeAjax(Guid id)
        {
           
            try
            {
                var post = await _postService.GetPostByIdAsync(id);
                if (post == null || post.IsDeleted)
                {
                    return Json(new { success = false, message = "Post not found" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return Json(new { success = false, message = "Like functionality not implemented yet" });

            }
            catch(Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }
    }
}