using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    public class AdminController : Controller
    {
        // You'll need to inject your services here
        private readonly IPostService _postService;
        private readonly IFigureService _figureService;
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly IUserService _userService;

        public AdminController(IPostService postService, IFigureService figureService, UserManager<IdentityUser> userManager)
        {
            _postService = postService;
            _figureService = figureService;
            _userManager = userManager;
        }

        // Dashboard - Overview of all admin functions
        public IActionResult Index()
        {
            // You can add statistics here later
            return View();
        }

        // POSTS MANAGEMENT
        public async Task<IActionResult> ManagePosts()
        {
            var posts = await _postService.GetAllPostsAsync();
            return View(posts);

            // Temporary - replace with your service call
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            await _postService.DeletePostAsync(id);
            return RedirectToAction(nameof(ManagePosts));
        }

        [HttpPost]
        public async Task<IActionResult> RestorePost(Guid id)
        {
            await _postService.RestorePostAsync(id);
            return RedirectToAction(nameof(ManagePosts));
        }

        // FIGURES MANAGEMENT
        public async Task<IActionResult> ManageFigures()
        {
            var figures = await _figureService.GetAllFiguresAsync();
            // return View(figures);

            // Temporary - replace with your service call
            return View(figures);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFigure(Guid id)
        {
            await _figureService.DeleteFigureAsync(id);
            return RedirectToAction(nameof(ManageFigures));
        }

        [HttpPost]
        public async Task<IActionResult> RestoreFigure(Guid id)
        {
            await _figureService.RestoreFigureAsync(id);
            return RedirectToAction(nameof(ManageFigures));
        }

        // USERS MANAGEMENT

        public async Task<bool> FromUserToBan(string id)
        {
            IdentityUser? user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }
            if (!(await _userManager.IsInRoleAsync(user, "User")))
            {
                return false;
            }
            var result1 = await _userManager.RemoveFromRoleAsync(user, "User");
            if (!result1.Succeeded)
            {
                return false;
            }
            var result2 = await _userManager.AddToRoleAsync(user, "Banned");
            if (!result2.Succeeded)
            {
                return false;
            }
            return true;

        }

        public async Task<bool> FromBanToUser(string id)
        {
            IdentityUser? user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }
            if (!(await _userManager.IsInRoleAsync(user, "Banned")))
            {
                return false;
            }
            var result1 = await _userManager.RemoveFromRoleAsync(user, "Banned");
            if (!result1.Succeeded)
            {
                return false;
            }
            var result2 = await _userManager.AddToRoleAsync(user, "User");
            if (!result2.Succeeded)
            {
                return false;
            }
            return true;

        }

        public async Task<IActionResult> ManageUsers()
        {
            // var users = await _userService.GetAllNonAdminUsersAsync();
            // return View(users);
            var posts = await _postService.GetAllPostsAsync();
            var figures = await _figureService.GetAllFiguresAsync();
            List<IdentityUser> users = _userManager.Users.ToList();

            var usersView = await Task.WhenAll(users.Select(async u => new AdminUserViewModel
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                CreatedAt = DateTime.Now, // Replace with actual created date if needed
                Email = u.Email ?? string.Empty,
                IsBanned = await _userManager.IsInRoleAsync(u, "Banned"),
                PostsCount = posts.Count(p => p.PublisherId == u.Id),      // Adjust if needed
                FiguresCount = figures.Count(f => f.OwnerId == u.Id)   // Adjust if needed
            }));

            // Now usersView is List<AdminUserViewModel>

            // Temporary - replace with your service call
            return View(usersView);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            // await _userService.SoftDeleteUserAsync(id);
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        public async Task<IActionResult> RestoreUser(string id)
        {
            // await _userService.RestoreUserAsync(id);
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(string id)
        {
            // await _userService.BanUserAsync(id);
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        public async Task<IActionResult> UnbanUser(string id)
        {
            // await _userService.UnbanUserAsync(id);
            return RedirectToAction(nameof(ManageUsers));
        }
    }

    // ViewModel for user management

}

