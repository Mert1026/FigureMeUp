using FigureMeUp.Data.Models;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FigureMeUp.Controllers
{
    public class FiguresController : Controller
    {
        private readonly IFigureService _figureService;

        public FiguresController(IFigureService figureService)
        {
            _figureService = figureService;
        }

        // GET: Figures
        public async Task<IActionResult> Index()
        {
            var figures = await _figureService.GetAllFiguresAsync();
            var activeFigures = figures.Where(f => !f.IsDeleted).ToList();
            return View(activeFigures);
        }

        // GET: Figures/Details/5(vece e GUID)
        public async Task<IActionResult> Details(Guid id)
        {
            var figure = await _figureService.GetFigureByIdAsync(id);
            if (figure == null || figure.IsDeleted)
            {
                return NotFound();
            }

            return View(figure);
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
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

        // GET: Figures/Edit/5(vece e GUID)
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var figure = await _figureService.GetFigureByIdAsync(id);
            if (figure == null || figure.IsDeleted)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (figure.OwnerId != userId)
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

        // POST: Figures/Edit/5(vece e GUID)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FiguresViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

        // POST: Figures/Delete/5(vece e GUID)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var figure = await _figureService.GetFigureByIdAsync(id);
            if (figure == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (figure.OwnerId != userId)
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
    }
}
