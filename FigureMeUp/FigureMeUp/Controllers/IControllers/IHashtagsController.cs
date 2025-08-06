using FigureMeUp.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IHashtagsController
    {
        IActionResult Create();
        Task<IActionResult> Create(Hashtag model);
        Task<IActionResult> Delete(int id);
        Task<IActionResult> DeleteByName(string name);
        Task<IActionResult> Details(int id);
        Task<IActionResult> DetailsByName(string name);
    }
}
