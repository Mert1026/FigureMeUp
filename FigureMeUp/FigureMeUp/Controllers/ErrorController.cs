using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/404")]
        public IActionResult Err404()
        {
            return View();
        }

        [Route("error/500")]
        public IActionResult Err500()
        {
            return View();
        }
    }
}
