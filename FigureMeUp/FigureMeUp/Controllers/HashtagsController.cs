using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    public class HashtagsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
