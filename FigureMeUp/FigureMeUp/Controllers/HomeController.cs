using FigureMeUp.Controllers.IControllers;
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
    public class HomeController : Controller, IHomeController
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
            return RedirectToAction("er500", "ErrorController");
            try
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
        public async Task<IActionResult> MyPosts()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var allPosts = await _postService.GetAllPostsAsync();
                var userPosts = allPosts.Where(p => p.PublisherId == userId && !p.IsDeleted).ToList();

                return View(userPosts);
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
        public async Task<IActionResult> MyFigures()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var allFigures = await _figureService.GetAllFiguresAsync();
                var userFigures = allFigures.Where(f => f.OwnerId == userId && !f.IsDeleted).ToList();

                return View(userFigures);
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
        public async Task<IActionResult> ViewProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Forbid();
                }
                var user = await _userManager.FindByIdAsync(userId);
                var figures = await _figureService.GetAllFiguresAsync();
                var posts = await _postService.GetAllPostsAsync();

                if (user == null)
                {
                    return View("Err404");
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
        public async Task<IActionResult> LikedContent()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Forbid();
                }
                var likedFigures = await _figureService.GetLikedFiguresByUserIdAsync(userId);
                var likedPosts = await _postService.GetLikedPostsByUserIdAsync(userId);

                var model = new LikedContentViewModel
                {
                    LikedFigures = likedFigures.ToList(),
                    LikedPosts = likedPosts.ToList()
                };

                return View(model);
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
                if (string.IsNullOrEmpty(userId))
                {
                    return Forbid();
                }
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
        public async Task<IActionResult> UnlikePost([FromForm] string postId)
        {
            try
            {
                if (string.IsNullOrEmpty(postId) || !Guid.TryParse(postId, out Guid parsedPostId))
                {
                    return Json(new { success = false, message = "Invalid post ID" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Forbid();
                }
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
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }
    }

}