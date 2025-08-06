using FigureMeUp.Data.Models.View_models;
using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IAccountController
    {
        IActionResult Register();
        Task<IActionResult> Register(RegisterViewModel model);
        IActionResult Login(string? returnUrl = null);
        Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null);
        Task<IActionResult> Logout();
    }
}
