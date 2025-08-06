using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IErrorController
    {
        IActionResult er404();
        IActionResult er500();
    }
}
