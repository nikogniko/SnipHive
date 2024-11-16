using Microsoft.AspNetCore.Mvc;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Repository;
using System.Diagnostics;

namespace SnippetsLibraryWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, UserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
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
                // Витягування ніку користувача з моделі
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
        public async Task<IActionResult> RegisterUser(string username, string email, string password)
        {
            try
            {
                // Перевірка на порожні значення
                if (string.IsNullOrEmpty(username))
                {
                    return Json(new { success = false, message = "Username cannot be empty" });
                }

                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "Email cannot be empty" });
                }

                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Password cannot be empty" });
                }

                // Спроба зареєструвати користувача
                int? userId = await _userRepository.AddUserAsync(username, email, password);

                if (userId.HasValue)
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

        [HttpGet]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            try
            {
                // Перевірка на порожні значення
                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "Email cannot be empty" });
                }

                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Password cannot be empty" });
                }

                // Спроба авторизації користувача
                int? userId = await _userRepository.GetUserIdAsync(email, password);

                if (userId.HasValue)
                {
                    // Успішна авторизація - передаємо ID користувача
                    return Json(new { success = true, message = "Login successful", userId = userId.Value });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid email or password" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
