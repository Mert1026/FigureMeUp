using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    public class FiguresController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
