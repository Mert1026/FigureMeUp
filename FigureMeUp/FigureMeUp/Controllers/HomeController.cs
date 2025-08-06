using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FigureMeUp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class HomeController : Controller
    {
        private readonly IFigureService _figureService;
        private readonly IPostService _postService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IFigureService figureService, IPostService postService, UserManager<IdentityUser> userManager)
        {
            _figureService = figureService;
            _postService = postService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var recentFigures = await _figureService.GetAllFiguresAsync();
            var recentPosts = await _postService.GetAllPostsAsync();

            var model = new HomeViewModel
            {
                RecentFigures = recentFigures.Where(f => !f.IsDeleted).Take(6).ToList(),
                RecentPosts = recentPosts.Where(p => !p.IsDeleted).Take(5).ToList()
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> MyPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allPosts = await _postService.GetAllPostsAsync();
            var userPosts = allPosts.Where(p => p.PublisherId == userId && !p.IsDeleted).ToList();

            return View(userPosts);
        }

        [Authorize]
        public async Task<IActionResult> MyFigures()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allFigures = await _figureService.GetAllFiguresAsync();
            var userFigures = allFigures.Where(f => f.OwnerId == userId && !f.IsDeleted).ToList();

            return View(userFigures);
        }

        [Authorize]
        public async Task<IActionResult> ViewProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var figures = await _figureService.GetAllFiguresAsync();
            var posts = await _postService.GetAllPostsAsync();

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileDetailsViewModel
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FiguresCount = figures.Count(f => f.OwnerId == userId && !f.IsDeleted),
                PostsCount = posts.Count(p => p.PublisherId == userId && !p.IsDeleted),
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ViewFavorites()
        {
            //TODO
            var favorites = new List<Figure>();
            return View(favorites);
        }

        [Authorize]
        public async Task<IActionResult> LikedContent()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // TODO: Replace these with your actual service methods
                var likedFigures = await _figureService.GetLikedFiguresByUserIdAsync(userId);
                var likedPosts = await _postService.GetLikedPostsByUserIdAsync(userId);

                var model = new LikedContentViewModel
                {
                    LikedFigures = likedFigures.ToList(),
                    LikedPosts = likedPosts.ToList()
                };

                return View(model);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "An error occurred while fetching liked content." });
            }
            
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlikeFigure([FromForm] string figureId)
        {
            try
            {
                if (string.IsNullOrEmpty(figureId) || !Guid.TryParse(figureId, out Guid parsedFigureId))
                {
                    return Json(new { success = false, message = "Invalid figure ID" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _figureService.ToggleLikeAsync(parsedFigureId, userId);

                if (result)
                {
                    return Json(new { success = true, message = "Figure unliked successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to unlike figure" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while unliking the figure" });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlikePost([FromForm] string postId)
        {
            try
            {
                if (string.IsNullOrEmpty(postId) || !Guid.TryParse(postId, out Guid parsedPostId))
                {
                    return Json(new { success = false, message = "Invalid post ID" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _postService.ToggleLikeAsync(parsedPostId, userId);

                if (result)
                {
                    return Json(new { success = true, message = "Post unliked successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to unlike post" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while unliking the post" });
            }
        }
    }

    // Helper view model for Home/Index
    public class HomeViewModel
    {
        public IEnumerable<Figure> RecentFigures { get; set; } = new List<Figure>();
        public IEnumerable<Post> RecentPosts { get; set; } = new List<Post>();
    }
}