using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Repository;
using SnippetsLibraryWebApp.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace SnippetsLibraryWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserRepository _userRepository;
        private readonly SnippetRepository _snippetRepository;

        public HomeController(ILogger<HomeController> logger, UserRepository userRepository, SnippetRepository snippetRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _snippetRepository = snippetRepository;
        }

        public async Task<IActionResult> Index()
        {
            var snippets = await _snippetRepository.GetAllPublicSnippetsAsync();
            return View(snippets);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsername(int userId)
        {
            try
            {
                // ??????????? ???? ??????????? ? ??????
                string username = await _userRepository.GetUsernameByIdAsync(userId);

                if (!string.IsNullOrEmpty(username))
                {
                    return Json(new { success = true, username = username });
                }
                else
                {
                    return Json(new { success = false, message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model)
        {
            try
            {
                // ????????? ?? ??????? ????????
                if (string.IsNullOrEmpty(model.username))
                {
                    return Json(new { success = false, message = "Username cannot be empty" });
                }

                if (string.IsNullOrEmpty(model.email))
                {
                    return Json(new { success = false, message = "Email cannot be empty" });
                }

                if (string.IsNullOrEmpty(model.password))
                {
                    return Json(new { success = false, message = "Password cannot be empty" });
                }

                // ?????? ????????????? ???????????
                int? userId = await _userRepository.AddUserAsync(model.username, model.email, model.password);

                if (userId.HasValue && Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "User registered successfully", userId = userId.Value });
                }
                else
                {
                    return Json(new { success = false, message = "Error registering user" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}"});
            }
        }

        [HttpPost]
        [Route("/Account/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // ????????? ?? ??????? ????????
                    if (string.IsNullOrEmpty(model.email))
                    {
                        return Json(new { success = false, message = "Email cannot be empty" });
                    }

                    if (string.IsNullOrEmpty(model.password))
                    {
                        return Json(new { success = false, message = "Password cannot be empty" });
                    }

                    // ?????? ??????????? ???????????
                    var user = await _userRepository.GetUserAsync(model.email, model.password);

                    if (user == null)
                    {
                        return Json(new { success = false, message = "Невірний email або пароль" });
                    }

                    // Створюємо Claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                    var authProperties = new AuthenticationProperties
                    {
                        // Встановіть властивості аутентифікації, якщо потрібно
                    };

                    await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Login successful", Id = user.Id });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Невірний email або пароль" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error: {ex.Message}" });
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Невірний email або пароль" });
            }

            return View(model);
        }

        [HttpPost]
        [Route("/Account/Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("AllSnippets", "Snippets");
        }
    }
}
