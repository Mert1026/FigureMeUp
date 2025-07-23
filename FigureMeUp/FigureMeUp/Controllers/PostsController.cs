using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    public class PostsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
