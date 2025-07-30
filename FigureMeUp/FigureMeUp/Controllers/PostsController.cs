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

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllPostsAsync();
            var activePosts = posts.Where(p => !p.IsDeleted).OrderByDescending(p => p.CreatedAt).ToList();
            return View(activePosts);
        }

        // GET: Posts/Details/5(vece e GUID)!
        public async Task<IActionResult> Details(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null || post.IsDeleted)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (ModelState.IsValid)
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

        // GET: Posts/Edit/5(vece e GUID)!!
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null || post.IsDeleted)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.PublisherId != userId)
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

        // POST: Posts/Edit/5(vece e GUID)!
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PostViewModel model)
        {
            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingPost.PublisherId != userId)
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

        // POST: Posts/Delete/5(vece e GUID)
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
            if (post.PublisherId != userId)
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
    }
}
