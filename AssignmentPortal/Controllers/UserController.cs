using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AssignmentPortal.Models;
using AssignmentPortal.Repository.Interfaces;
using AssignmentPortal.Services;
using System.Data;

namespace AssignmentPortal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user != null && PasswordHelper.VerifyPassword(model.Password, user.PasswordHash) && model.Role.ToLower() == user.Role.ToLower())
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role) 
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
                TempData["success"] = "Login Successfully.";
                if (user.Role == "Faculty")
                    return RedirectToAction("Index", "Dashboard");
                else
                    return RedirectToAction("Index", "Dashboard");
            }

            TempData["error"] = "Invalid Credentials.";
            ViewData["SelectedRole"] = model.Role;
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["success"] = "Logout successfully.";
            return RedirectToAction("Login");
        }
    }
}
