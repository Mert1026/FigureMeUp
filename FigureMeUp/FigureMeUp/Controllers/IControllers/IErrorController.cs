using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IErrorController
    {
        IActionResult Err404();
        IActionResult Err500();
    }
}
