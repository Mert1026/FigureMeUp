using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FigureMeUp.Data.Models.View_models;
using FigureMeUp.Controllers.IControllers;

namespace FigureMeUp.Controllers
{
    public class AccountController : Controller, IAccountController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser
                    {
                        UserName = model.Username,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        TempData["Success"] = "Registration successful! Welcome to FigureMeUp!";
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }

        }
        // GET: Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;

                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        model.Username,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        IdentityUser? user = await _userManager.FindByNameAsync(model.Username);
                        if (user == null)
                        {
                            ModelState.AddModelError(string.Empty, "User not found.");
                            return View(model);
                        }
                        else
                        {
                            // Check if the user is banned
                            if (await _userManager.IsInRoleAsync(user, "Banned"))
                            {
                                ModelState.AddModelError(string.Empty, "Your account is banned. Please contact support.");
                                return View(model);
                            }

                        }
                        TempData["Success"] = "Welcome back!";
                        return RedirectToLocal(returnUrl);
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

                return View(model);

            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }

        // POST: Account/Logout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "You have been logged out successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }
        }

       

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                CustomErrorViewModel err = new CustomErrorViewModel()
                {
                    ErrorMessage = ex.Message
                };
                return View("CustomError", err);
            }   
        }

    }
}