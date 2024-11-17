using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Repository;
using System.Security.Claims;

namespace SnippetsLibraryWebApp.Controllers
{
    public class SnippetsController : Controller
    {
        private readonly SnippetRepository _snippetsRepository;

        public SnippetsController(SnippetRepository snippetsRepository)
        {
            _snippetsRepository = snippetsRepository;
        }

        // Сторінка "Усі сніпети"
        [AllowAnonymous]
        public async Task<IActionResult> AllSnippets()
        {
            var snippets = await _snippetsRepository.GetAllPublicSnippetsAsync();
            return View(snippets);
        }

        // Сторінка "Обрані сніпети"
        [Authorize]
        public async Task<IActionResult> FavoriteSnippets()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Перенаправлення на сторінку входу
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var savedSnippets = await _snippetsRepository.GetSavedSnippetsByUserAsync(userId);
            return View(savedSnippets);
        }

        // Сторінка "Мої сніпети"
        [Authorize]
        public async Task<IActionResult> MySnippets()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Перенаправлення на сторінку входу
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userSnippets = await _snippetsRepository.GetUserSnippetsAsync(userId);
            return View(userSnippets);
        }

        // Детальна сторінка сніпета
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var snippet = await _snippetsRepository.GetSnippetByIdAsync(id);
            if (snippet == null)
            {
                return NotFound();
            }
            return View(snippet);
        }
    }
}
