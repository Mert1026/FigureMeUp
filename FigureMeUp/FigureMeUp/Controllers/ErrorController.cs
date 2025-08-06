using Microsoft.AspNetCore.Mvc;
using FigureMeUp.Controllers.IControllers;

namespace FigureMeUp.Controllers
{
    public class ErrorController : Controller, IErrorController
    {
        [Route("error/404")]
        public IActionResult er404()
        {
            return View();
        }

        [Route("error/500")]
        public IActionResult er500()
        {
            return View();
        }
    }
}
