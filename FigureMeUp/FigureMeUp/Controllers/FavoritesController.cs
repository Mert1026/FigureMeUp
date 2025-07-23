using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers
{
    public class FavoritesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
