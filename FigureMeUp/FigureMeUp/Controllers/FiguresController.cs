using FigureMeUp.Controllers.IControllers;
using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FigureMeUp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class FiguresController : Controller, IFiguresController
    {
        private readonly IFigureService _figureService;

        public FiguresController(IFigureService figureService)
        {
            _figureService = figureService;
        }

        // GET: Figures
        public async Task<IActionResult> Index()
        {
            try
            {
                var figures = await _figureService.GetAllFiguresAsync();
                var activeFigures = figures.Where(f => !f.IsDeleted).ToList();
                return View(activeFigures);
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

        // GET: Figures/Details/5(vece e GUID)
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var figure = await _figureService.GetFigureByIdAsync(id);
                if (figure == null || figure.IsDeleted)
                {
                    return NotFound();
                }

                return View(figure);
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

        // GET: Figures/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Figures/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FiguresViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Forbid();
                    }
                    var result = await _figureService.AddFigureAsync(model, userId);

                    if (result)
                    {
                        TempData["Success"] = "Figure created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to create figure. Please try again.");
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

        // GET: Figures/Edit/5(vece e GUID)
        [Authorize]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var figure = await _figureService.GetFigureByIdAsync(id);
                if (figure == null || figure.IsDeleted)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (figure.OwnerId != userId
                    && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var model = new FiguresViewModel
                {
                    Id = figure.Id,
                    Name = figure.Name,
                    Description = figure.Description,
                    ImageUrls = figure.ImageUrls.ToList(),
                    Hashtags = figure.Hashtags.Select(h => h.Name).ToList(),
                    Rarity = figure.Rarity.Name,
                    OwnerName = figure.Owner.UserName ?? string.Empty
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

        // POST: Figures/Edit/5(vece e GUID)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(Guid id, FiguresViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Forbid();
                    }
                    var result = await _figureService.UpdateFigureAsync(model, userId);

                    if (result)
                    {
                        TempData["Success"] = "Figure updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update figure. Please try again.");
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

        // POST: Figures/Delete/5(vece e GUID)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var figure = await _figureService.GetFigureByIdAsync(id);
                if (figure == null)
                {
                    return NotFound();
                }

                bool a = User.IsInRole("Admin");
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (figure.OwnerId != userId
                    && !a)
                {
                    return Forbid();
                }

                var result = await _figureService.DeleteFigureAsync(id);
                if (result)
                {
                    TempData["Success"] = "Figure deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete figure.";
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

                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Figure ID is required" });
                }
                if (!Guid.TryParse(id, out Guid figureId))
                {
                    return Json(new { success = false, message = "Invalid figure ID format" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }


                var figure = await _figureService.GetFigureByIdAsync(figureId);
                if (figure == null || figure.IsDeleted)
                {
                    return Json(new { success = false, message = "Figure not found" });
                }


                var result = await _figureService.ToggleLikeAsync(figureId, userId);
                if (result)
                {

                    var updatedFigure = await _figureService.GetFigureByIdAsync(figureId);
                    if (updatedFigure != null)
                    {
                        var isLiked = updatedFigure.LikedByUsersIds.Contains(userId);
                        var message = isLiked ? "Figure liked successfully!" : "Figure unliked successfully!";
                        return Json(new
                        {
                            success = true,
                            message = message,
                            isLiked = isLiked,
                            likesCount = updatedFigure.LikesCount
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
    }

}
