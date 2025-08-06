using FigureMeUp.Data.Models.View_models;
using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IFiguresController
    {
        Task<IActionResult> Index();
        Task<IActionResult> Details(Guid id);
        IActionResult Create();
        Task<IActionResult> Create(FiguresViewModel model);
        Task<IActionResult> Edit(Guid id);
        Task<IActionResult> Edit(Guid id, FiguresViewModel model);
        Task<IActionResult> Delete(Guid id);
        Task<IActionResult> ToggleLike(string id);
    }
}
