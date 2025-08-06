using FigureMeUp.Data.Models.View_models;
using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IPostsController
    {
        Task<IActionResult> Index();
        Task<IActionResult> Details(Guid id);
        IActionResult Create();
        Task<IActionResult> Create(PostViewModel model);
        Task<IActionResult> Edit(Guid id);
        Task<IActionResult> Edit(Guid id, PostViewModel model);
        Task<IActionResult> Delete(Guid id);
        Task<IActionResult> ToggleLike(string id);
        Task<IActionResult> ToggleLikeAjax(Guid id);
    }
}
