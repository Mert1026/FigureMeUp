using FigureMeUp.Data.Models;
using Microsoft.AspNetCore.Mvc;
using FigureMeUp.Services.Core;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;

namespace FigureMeUp.Controllers
{
    public class AdminController : Controller
    {
        // You'll need to inject your services here
        private readonly IPostService _postService;
        private readonly IFigureService _figureService;
        //private readonly IUserService _userService;

        public AdminController(IPostService postService, IFigureService figureService)
        {
            _postService = postService;
            _figureService = figureService;
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
            // var figures = await _figureService.GetAllFiguresWithDetailsAsync();
            // return View(figures);

            // Temporary - replace with your service call
            var figures = new List<Figure>();
            return View(figures);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFigure(Guid id)
        {
            // await _figureService.SoftDeleteFigureAsync(id);
            return RedirectToAction(nameof(ManageFigures));
        }

        [HttpPost]
        public async Task<IActionResult> RestoreFigure(Guid id)
        {
            // await _figureService.RestoreFigureAsync(id);
            return RedirectToAction(nameof(ManageFigures));
        }

        // USERS MANAGEMENT
        public async Task<IActionResult> ManageUsers()
        {
            // var users = await _userService.GetAllNonAdminUsersAsync();
            // return View(users);

            // Temporary - replace with your service call
            var users = new List<AdminUserViewModel>();
            return View(users);
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

