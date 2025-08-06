using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IHomeController
    {
        Task<IActionResult> Index();
        Task<IActionResult> MyPosts();
        Task<IActionResult> MyFigures();
        Task<IActionResult> ViewProfile();
        Task<IActionResult> LikedContent();
        Task<IActionResult> UnlikeFigure(string figureId);
        Task<IActionResult> UnlikePost(string postId);
    }
}
