using Microsoft.AspNetCore.Mvc;

namespace FigureMeUp.Controllers.IControllers
{
    public interface IAdminController
    {
        IActionResult Index();
        Task<IActionResult> ManagePosts();
        Task<IActionResult> DeletePost(Guid id);
        Task<IActionResult> RestorePost(Guid id);
        Task<IActionResult> ManageFigures();
        Task<IActionResult> DeleteFigure(Guid id);
        Task<IActionResult> RestoreFigure(Guid id);
        Task<IActionResult> ManageUsers();
        Task<IActionResult> BanUser(string id);
        Task<IActionResult> UnbanUser(string id);
    }
}
