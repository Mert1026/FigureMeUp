using FigureMeUp.Controllers;
using FigureMeUp.Data.Models.View_models;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    [TestFixture]
    public class Tests_AccountController
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private AccountController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
                null, null, null, null);

            _controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);

            // Create a proper HttpContext with User identity
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Unauthenticated user

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            _controller.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>());

            // Mock Url property for RedirectToLocal method
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(false);
            _controller.Url = mockUrlHelper.Object;
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task Register_ValidModel_RedirectsToHome()
        {
            var model = new RegisterViewModel
            {
                Username = "TestUser",
                Email = "test@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockSignInManager.Setup(x => x.SignInAsync(It.IsAny<IdentityUser>(), false, null))
                .Returns(Task.CompletedTask);

            var result = await _controller.Register(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
        }

        [Test]
        public async Task Login_ValidCredentials_RedirectsToHome()
        {
            var model = new LoginViewModel
            {
                Username = "TestUser",
                Password = "Password123!"
            };

            var user = TestsHelper.CreateTestUser("test-id", "TestUser");

            _mockSignInManager.Setup(x => x.PasswordSignInAsync("TestUser", "Password123!", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockUserManager.Setup(x => x.FindByNameAsync("TestUser"))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Banned"))
                .ReturnsAsync(false);

            var result = await _controller.Login(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
        }

        [Test]
        public async Task Login_BannedUser_ReturnsViewWithError()
        {
            var model = new LoginViewModel
            {
                Username = "BannedUser",
                Password = "Password123!"
            };

            var user = TestsHelper.CreateTestUser("banned-id", "BannedUser");

            _mockSignInManager.Setup(x => x.PasswordSignInAsync("BannedUser", "Password123!", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockUserManager.Setup(x => x.FindByNameAsync("BannedUser"))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Banned"))
                .ReturnsAsync(true);

            var result = await _controller.Login(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task Logout_AuthenticatedUser_RedirectsToHome()
        {
            // Set up authenticated user
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;

            _mockSignInManager.Setup(x => x.SignOutAsync())
                .Returns(Task.CompletedTask);

            var result = await _controller.Logout();

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
        }
    }
}
