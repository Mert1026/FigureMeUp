using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FigureMeUp.Controllers
{
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

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileDetailsViewModel
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ViewFavorites()
        {
            // This would require implementing a favorites system in your models and services
            // For now, returning empty list as placeholder
            var favorites = new List<Figure>();
            return View(favorites);
        }
    }

    // Helper view model for Home/Index
    public class HomeViewModel
    {
        public IEnumerable<Figure> RecentFigures { get; set; } = new List<Figure>();
        public IEnumerable<Post> RecentPosts { get; set; } = new List<Post>();
    }
}