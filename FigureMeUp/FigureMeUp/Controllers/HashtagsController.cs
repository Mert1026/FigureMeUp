using FigureMeUp.Data.Models;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    [Authorize]
    public class HashtagController : Controller
    {
        private readonly IHashtagService _hashtagService;

        public HashtagController(IHashtagService hashtagService)
        {
            _hashtagService = hashtagService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hashtag model)
        {
            if (ModelState.IsValid)
            {
                var existingHashtag = await _hashtagService.GetHashtagByNameAsync(model.Name);
                if (existingHashtag != null)
                {
                    ModelState.AddModelError("Name", "This hashtag already exists.");
                    return View(model);
                }

                var result = await _hashtagService.CreateHashtagAsync(model);

                if (result)
                {
                    TempData["Success"] = "Hashtag created successfully!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create hashtag. Please try again.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _hashtagService.DeleteHashtagWithIdAsync(id);

            if (result)
            {
                TempData["Success"] = "Hashtag deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete hashtag.";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Hashtag name is required.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _hashtagService.DeleteHashtagWithNameAsync(name);

            if (result)
            {
                TempData["Success"] = "Hashtag deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete hashtag or hashtag not found.";
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int id)
        {
            var hashtag = await _hashtagService.GetHashtagByIdAsync(id);
            if (hashtag == null || hashtag.IsDeleted)
            {
                return NotFound();
            }

            return View(hashtag);
        }

        public async Task<IActionResult> DetailsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return NotFound();
            }

            var hashtag = await _hashtagService.GetHashtagByNameAsync(name);
            if (hashtag == null || hashtag.IsDeleted)
            {
                return NotFound();
            }

            return View("Details", hashtag);
        }
    }
}