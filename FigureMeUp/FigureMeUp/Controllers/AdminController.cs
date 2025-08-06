using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class AdminController : Controller
    {
        private readonly IPostService _postService;
        private readonly IFigureService _figureService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IPostService postService,
            IFigureService figureService,
            UserManager<IdentityUser> userManager,
            ILogger<AdminController> logger)
        {
            _postService = postService;
            _figureService = figureService;
            _userManager = userManager;
            _logger = logger;
        }

        // Dashboard - Overview of all admin functions
        public IActionResult Index()
        {
            return View();
        }

        // POSTS MANAGEMENT
        public async Task<IActionResult> ManagePosts()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();
                return View(posts);
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
        public async Task<IActionResult> DeletePost(Guid id)
        {
            try
            {
                await _postService.DeletePostAsync(id);
                TempData["Success"] = "Post deleted successfully.";
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
            return RedirectToAction(nameof(ManagePosts));
        }

        [HttpPost]
        public async Task<IActionResult> RestorePost(Guid id)
        {
            try
            {
                await _postService.RestorePostAsync(id);
                TempData["Success"] = "Post restored successfully.";
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
            return RedirectToAction(nameof(ManagePosts));
        }

        // FIGURES MANAGEMENT
        public async Task<IActionResult> ManageFigures()
        {
            try
            {
                var figures = await _figureService.GetAllFiguresAsync();
                return View(figures);
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
        public async Task<IActionResult> DeleteFigure(Guid id)
        {
            try
            {
                await _figureService.DeleteFigureAsync(id);
                TempData["Success"] = "Figure deleted successfully.";
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
            return RedirectToAction(nameof(ManageFigures));
        }

        [HttpPost]
        public async Task<IActionResult> RestoreFigure(Guid id)
        {
            try
            {
                await _figureService.RestoreFigureAsync(id);
                TempData["Success"] = "Figure restored successfully.";
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
            return RedirectToAction(nameof(ManageFigures));
        }

        // USERS MANAGEMENT
        private async Task<bool> FromUserToBan(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Attempted to ban user with null or empty ID");
                return false;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", id);
                    return false;
                }

                // Check if user is already banned
                var isAlreadyBanned = await _userManager.IsInRoleAsync(user, "Banned");
                if (isAlreadyBanned)
                {
                    _logger.LogInformation("User {UserId} is already banned", id);
                    return true; // Already banned, consider it successful
                }

                // Check if user is in User role before removing
                var isInUserRole = await _userManager.IsInRoleAsync(user, "User");

                if (isInUserRole)
                {
                    var removeResult = await _userManager.RemoveFromRoleAsync(user, "User");
                    if (!removeResult.Succeeded)
                    {
                        _logger.LogError("Failed to remove user {UserId} from User role. Errors: {Errors}",
                            id, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                        return false;
                    }
                }

                // Add to Banned role
                var addResult = await _userManager.AddToRoleAsync(user, "Banned");
                if (!addResult.Succeeded)
                {
                    _logger.LogError("Failed to add user {UserId} to Banned role. Errors: {Errors}",
                        id, string.Join(", ", addResult.Errors.Select(e => e.Description)));

                    // Try to rollback - add back to User role if we removed them
                    if (isInUserRole)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        _logger.LogInformation("Rolled back user {UserId} to User role after failed ban attempt", id);
                    }
                    return false;
                }

                _logger.LogInformation("Successfully banned user {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while banning user {UserId}", id);
                return false;
            }
        }

        private async Task<bool> FromBanToUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Attempted to unban user with null or empty ID");
                return false;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", id);
                    return false;
                }

                // Check if user is actually banned
                var isBanned = await _userManager.IsInRoleAsync(user, "Banned");
                if (!isBanned)
                {
                    _logger.LogInformation("User {UserId} is not banned", id);
                    return false;
                }

                // Remove from Banned role
                var removeResult = await _userManager.RemoveFromRoleAsync(user, "Banned");
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Failed to remove user {UserId} from Banned role. Errors: {Errors}",
                        id, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                    return false;
                }

                // Add back to User role
                var addResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addResult.Succeeded)
                {
                    _logger.LogError("Failed to add user {UserId} back to User role. Errors: {Errors}",
                        id, string.Join(", ", addResult.Errors.Select(e => e.Description)));

                    // Try to rollback - add back to Banned role
                    await _userManager.AddToRoleAsync(user, "Banned");
                    _logger.LogInformation("Rolled back user {UserId} to Banned role after failed unban attempt", id);
                    return false;
                }

                _logger.LogInformation("Successfully unbanned user {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while unbanning user {UserId}", id);
                return false;
            }
        }

        public async Task<IActionResult> ManageUsers()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();
                var figures = await _figureService.GetAllFiguresAsync();
                var users = _userManager.Users.ToList();

                var usersViewList = new List<AdminUserViewModel>();

                foreach (var user in users)
                {
                    // Skip admin users if needed
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    if (isAdmin) continue; // Optional: skip admin users

                    int postsCount = posts.Count(p => p.PublisherId == user.Id);
                    int figuresCount = figures.Count(f => f.OwnerId == user.Id);

                    usersViewList.Add(new AdminUserViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        CreatedAt = DateTime.Now, // You should get this from your user entity if available
                        IsBanned = await _userManager.IsInRoleAsync(user, "Banned"),
                        PostsCount = postsCount,
                        FiguresCount = figuresCount
                    });
                }

                return View(usersViewList);
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
        public async Task<IActionResult> BanUser(string id)
        {
            try
            {
                var result = await FromUserToBan(id); // Fixed: Added await

                if (result)
                {
                    TempData["Success"] = "User has been banned successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to ban user. Please try again.";
                }
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        public async Task<IActionResult> UnbanUser(string id)
        {
            try
            {
                var result = await FromBanToUser(id); // Fixed: Added await

                if (result)
                {
                    TempData["Success"] = "User has been unbanned successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to unban user. Please try again.";
                }
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }

            return RedirectToAction(nameof(ManageUsers));
        }
    }
}